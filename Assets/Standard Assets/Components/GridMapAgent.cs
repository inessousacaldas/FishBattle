using System;
using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(Seeker))]
public class GridMapAgent : MonoBehaviour
{
    public void SetOnPosChangeEvent(Action<Vector3, Vector3, bool> action)
    {
        onPosChange = action;
    }

    public void SetOnTargetReached(Action action)
    {
        onTargetReached = action;
    }

    public void Clear()
    {
        onPosChange = null;
        onTargetReached = null;
    }
    private event System.Action<Vector3, Vector3, bool> onPosChange;    //<当前的位置，前一个位置, 到达目标>
    private event System.Action onTargetReached;

    public bool HasPath
    {
        get { return path != null && !targetReached; }
    }

    public bool isMoving = false;
    private FunnelModifier _funnelModifier;
    private RadiusModifier _radiusModifier;
    /** Determines how often it will search for new paths.
	 * If you have fast moving targets or AIs, you might want to set it to a lower value.
	 * The value is in seconds between path requests.
	 */
    public float repathRate = 0.5F;

    /** Target to move towards.
	 * The AI will try to follow/move towards this target.
	 * It can be a point on the ground where the player has clicked in an RTS for example, or it can be the player object in a zombie game.
	 */
    //public Transform target;
    private Vector3 _targetPos;

    /** Enables or disables searching for paths.
	 * Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
	 * \see #canMove
	 */
    public bool canSearch = true;

    /** Enables or disables movement.
	 * \see #canSearch */
    public bool canMove = true;

    /** Speed in world units */
    public float speed = 3;

    /** How quickly to rotate */
    public float rotationSpeed = 10;

    /** If true, some interpolation will be done when a new path has been calculated.
	 * This is used to avoid short distance teleportation.
	 */
    public bool interpolatePathSwitches = true;

    /** How quickly to interpolate to the new path */
    public float switchPathInterpolationSpeed = 5;

    public Vector3 gridPos
    {
        get
        {
            if (tr == null) return Vector3.zero;
            return tr.position;
        }
    }

    /** Cached Seeker component */
    protected Seeker seeker;

    /** Cached Transform component */
    protected Transform tr;

    /** Time when the last path request was sent */
    protected float lastRepath = -9999;

    /** Current path which is followed */
    protected ABPath path;

    /** Current index in the path which is current target */
    protected int currentWaypointIndex = 0;

    /** How far the AI has moved along the current segment */
    protected float distanceAlongSegment = 0;

    /** True if the end-of-path is reached.
	 * \see TargetReached */
    public bool targetReached { get; private set; }

    /** Only when the previous path has been returned should be search for a new path */
    protected bool canSearchAgain = true;

    /** When a new path was returned, the AI was moving along this ray.
	 * Used to smoothly interpolate between the previous movement and the movement along the new path.
	 * The speed is equal to movement direction.
	 */
    protected Vector3 previousMovementOrigin;
    protected Vector3 previousMovementDirection;
    protected float previousMovementStartTime = -9999;

    /** Holds if the Start function has been run.
	 * Used to test if coroutines should be started in OnEnable to prevent calculating paths
	 * in the awake stage (or rather before start on frame 0).
	 */
    private bool startHasRun = false;

    /** Initializes reference variables.
	 * If you override this function you should in most cases call base.Awake () at the start of it.
	 * */
    void Awake()
    {
        //This is a simple optimization, cache the transform component lookup
        tr = transform;

        seeker = GetComponent<Seeker>();
        if (seeker == null)
        {
            seeker = gameObject.AddComponent<Seeker>();
        }

        // Tell the StartEndModifier to ask for our exact position
        // when post processing the path
        // This is important if we are using prediction and
        // requesting a path from some point slightly ahead of us
        // since then the start point in the path request may be far
        // from our position when the path has been calculated.
        // This is also good because if a long path is requested, it may
        // take a few frames for it to be calculated so we could have
        // moved some distance during that time
        seeker.startEndModifier.adjustStartPoint = () =>
        {
            return gridPos;
        };

        _funnelModifier = gameObject.AddMissingComponent<FunnelModifier>();
        _radiusModifier = gameObject.AddMissingComponent<RadiusModifier>();
        _radiusModifier.radius = 0.5f;
        _radiusModifier.detail = 10f;

        if (_needInitMode)
        {
            Debug.LogError("寻路调试信息,Call SetModeHandle 延迟设置是否飞行状态");
            SetModeHandle();
            _needInitMode = false;
        }
    }

    /** Starts searching for paths.
	 * If you override this function you should in most cases call base.Start () at the start of it.
	 * \see OnEnable
	 * \see RepeatTrySearchPath
	 */
    void Start()
    {
        startHasRun = true;
        OnEnable();
    }

    /** Run at start and when reenabled.
	 * Starts RepeatTrySearchPath.
	 *
	 * \see Start
	 */
    void OnEnable()
    {
        lastRepath = -9999;
        canSearchAgain = true;

        if (startHasRun)
        {
            // Make sure we receive callbacks when paths complete
            seeker.pathCallback += OnPathComplete;

            //StartCoroutine(RepeatTrySearchPath());
        }
    }

    public void OnDisable()
    {
        // Abort calculation of path
        if (seeker != null && !seeker.IsDone()) seeker.GetCurrentPath().Error();

        // Release current path
        if (path != null) path.Release(this);
        path = null;

        // Make sure we receive callbacks when paths complete
        seeker.pathCallback -= OnPathComplete;
    }

    /** Tries to search for a path every #repathRate seconds.
	 * \see TrySearchPath
	 */
    protected IEnumerator RepeatTrySearchPath()
    {
        while (true)
        {
            float v = TrySearchPath();
            yield return new WaitForSeconds(v);
        }
    }

    /** Tries to search for a path.
	 * Will search for a new path if there was a sufficient time since the last repath and both
	 * #canSearchAgain and #canSearch are true and there is a target.
	 *
	 * \returns The time to wait until calling this function again (based on #repathRate)
	 */
    public float TrySearchPath()
    {
        if (Time.time - lastRepath >= repathRate && canSearchAgain && canSearch)
        {
            SearchPath(_targetPos);
            return repathRate;
        }
        else
        {
            float v = repathRate - (Time.time - lastRepath);
            return v < 0 ? 0 : v;
        }
    }

    /** Requests a path to the target.
	 * Some inheriting classes will prevent the path from being requested immediately when
	 * this function is called, for example when the AI is currently traversing a special path segment
	 * in which case it is usually a bad idea to search for a new path.
	 */
    public void SearchPath(Vector3 targetPosition)
    {
        ForceSearchPath(targetPosition);
    }

    /** Requests a path to the target.
	 * Bypasses 'is-it-a-good-time-to-request-a-path' checks.
	 */
    public void ForceSearchPath(Vector3 targetPosition)
    {
        lastRepath = Time.time;
        // This is where we should search to
        //Vector3 targetPosition = Vector3.zero;
        Vector3 currentPosition = gridPos;
        _targetPos = targetPosition;

        canSearchAgain = false;

        //Alternative way of requesting the path
        //ABPath p = ABPath.Construct (currentPosition,targetPoint,null);
        //seeker.StartPath (p);

        // We should search from the current position
        seeker.StartPath(currentPosition, targetPosition);
    }

    /** The end of the path has been reached.
	 * If you want custom logic for when the AI has reached it's destination
	 * add it here
	 * You can also create a new script which inherits from this one
	 * and override the function in that script.
	 */
    public void OnTargetReached()
    {
        if (onTargetReached != null)
            onTargetReached();
    }

    /** Called when a requested path has finished calculation.
	 * A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
	 * Finally it is returned to the seeker which forwards it to this function.\n
	 */
    public void OnPathComplete(Path _p)
    {
        ABPath p = _p as ABPath;

        if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

        canSearchAgain = true;

        // Claim the new path
        // This is used for path pooling
        p.Claim(this);

        // Path couldn't be calculated of some reason.
        // More info in p.errorLog (debug string)
        if (p.error)
        {
            p.Release(this);
            return;
        }

        if (interpolatePathSwitches)
        {
            ConfigurePathSwitchInterpolation();
        }

        // Release the previous path
        // This is used for path pooling
        if (path != null) path.Release(this);

        // Replace the old path
        path = p;
        curNodeIndex = 0;
        // Just for the rest of the code to work, if there is only one waypoint in the path
        // add another one
        if (path.vectorPath != null && path.vectorPath.Count == 1)
        {
            path.vectorPath.Insert(0, gridPos);
        }

        targetReached = false;

        // Reset some variables
        ConfigureNewPath();
    }

    protected void ConfigurePathSwitchInterpolation()
    {
        bool previousPathWasValid = path != null && path.vectorPath != null && path.vectorPath.Count > 1;

        bool reachedEndOfPreviousPath = false;

        if (previousPathWasValid)
        {
            reachedEndOfPreviousPath = currentWaypointIndex == path.vectorPath.Count - 1 && distanceAlongSegment >= (path.vectorPath[path.vectorPath.Count - 1] - path.vectorPath[path.vectorPath.Count - 2]).magnitude;
        }

        if (previousPathWasValid && !reachedEndOfPreviousPath)
        {
            List<Vector3> vPath = path.vectorPath;

            // Make sure we stay inside valid ranges
            currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 1, vPath.Count - 1);

            // Current segment vector
            Vector3 segment = vPath[currentWaypointIndex] - vPath[currentWaypointIndex - 1];
            float segmentLength = segment.magnitude;

            // Find the approximate length of the path that is left on the current path
            float approximateLengthLeft = segmentLength * Mathf.Clamp01(1 - distanceAlongSegment);
            for (int i = currentWaypointIndex; i < vPath.Count - 1; i++)
            {
                approximateLengthLeft += (vPath[i + 1] - vPath[i]).magnitude;
            }

            previousMovementOrigin = gridPos;
            previousMovementDirection = segment.normalized * approximateLengthLeft;
            previousMovementStartTime = Time.time;
        }
        else
        {
            previousMovementOrigin = Vector3.zero;
            previousMovementDirection = Vector3.zero;
            previousMovementStartTime = -9999;
        }
    }

    /** Finds the closest point on the current path.
	 * Sets #currentWaypointIndex and #lerpTime to the appropriate values.
	 */
    protected void ConfigureNewPath()
    {
        var points = path.vectorPath;

        var currentPosition = gridPos;

        // Find the closest point on the new path
        // to our current position
        // and initialize the path following variables
        // to start following the path from that point
        float bestDistanceAlongSegment = 0;
        float bestDist = float.PositiveInfinity;
        Vector3 bestDirection = Vector3.zero;
        int bestIndex = 1;

        for (int i = 0; i < points.Count - 1; i++)
        {
            float factor = VectorMath.ClosestPointOnLineFactor(points[i], points[i + 1], currentPosition);
            factor = Mathf.Clamp01(factor);
            Vector3 point = Vector3.Lerp(points[i], points[i + 1], factor);
            float dist = (currentPosition - point).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                bestDirection = points[i + 1] - points[i];
                bestDistanceAlongSegment = factor * bestDirection.magnitude;
                bestIndex = i + 1;
            }
        }

        currentWaypointIndex = bestIndex;
        distanceAlongSegment = bestDistanceAlongSegment;

        if (interpolatePathSwitches && switchPathInterpolationSpeed > 0.01f)
        {
            var correctionFactor = Mathf.Max(-Vector3.Dot(previousMovementDirection.normalized, bestDirection.normalized), 0);
            distanceAlongSegment -= speed * correctionFactor * (1f / switchPathInterpolationSpeed);
        }
    }

    void Update()
    {
        if (canMove)
        {
            Vector3 direction;
            Vector3 nextPos = CalculateNextPosition(out direction);

            MoveTo(nextPos, direction);
        }
    }
    private Vector3 lastNextPos;

    private void MoveTo(Vector3 nextPos, Vector3 direction)
    {
        if (lastNextPos == nextPos)
            return;

        if (direction != Vector3.zero)
        {
            Vector3 rotationDirection = direction;
            rotationDirection.y = 0;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(rotationDirection), rotationSpeed * Time.deltaTime);
        }
        Vector3 tOrginPos = tr.transform.position;
        if (SetFinalPos(nextPos, _currMoveType))
        {
            isMoving = true;
            lastNextPos = Vector3.zero;
            if (onPosChange != null)
            {
                onPosChange(nextPos, tOrginPos, targetReached);
            }
        }
        else
        {
            isMoving = false;
            lastNextPos = nextPos;
        }
    }

    private bool SetFinalPos(Vector3 nextPos, MoveType moveType)
    {
        if (tr == null) return false;

        if (tr.position != nextPos)
        {
            //ProfileHelper.SystimeBegin("AStart");
            float y;
            if (GetNearYAxix(nextPos, out y))
            {
                nextPos.y = y;
            }
            //ProfileHelper.SystimeEnd("AStart");
            tr.position = nextPos;
            return true;
        }
        return false;
    }

    /** Calculate the AI's next position (one frame in the future).
	 * \param direction The direction of the segment the AI is currently traversing. Not normalized.
	 */
    protected Vector3 CalculateNextPosition(out Vector3 direction)
    {
        if (path == null || path.vectorPath == null || path.vectorPath.Count == 0)
        {
            direction = Vector3.zero;
            return gridPos;
        }

        List<Vector3> vPath = path.vectorPath;

        // Make sure we stay inside valid ranges
        currentWaypointIndex = Mathf.Clamp(currentWaypointIndex, 1, vPath.Count - 1);

        // Current segment vector
        Vector3 segment = vPath[currentWaypointIndex] - vPath[currentWaypointIndex - 1];
        float segmentLength = segment.magnitude;
        //Debug.Log("vPath[currentWaypointIndex] = " + vPath[currentWaypointIndex] + ",vPath[currentWaypointIndex - 1] = " + vPath[currentWaypointIndex - 1] + ",segment = " + segment);

        // Move forwards
        distanceAlongSegment += Time.deltaTime * speed;

        // Pick the next segment if we have traversed the current one completely
        if (distanceAlongSegment >= segmentLength && currentWaypointIndex < vPath.Count - 1)
        {
            float overshootDistance = distanceAlongSegment - segmentLength;

            while (true)
            {
                currentWaypointIndex++;

                // Next segment vector
                Vector3 nextSegment = vPath[currentWaypointIndex] - vPath[currentWaypointIndex - 1];
                float nextSegmentLength = nextSegment.magnitude;

                if (overshootDistance <= nextSegmentLength || currentWaypointIndex == vPath.Count - 1)
                {
                    segment = nextSegment;
                    segmentLength = nextSegmentLength;
                    distanceAlongSegment = overshootDistance;
                    break;
                }
                else
                {
                    overshootDistance -= nextSegmentLength;
                }
            }
        }

        if (distanceAlongSegment >= segmentLength && currentWaypointIndex == vPath.Count - 1)
        {
            if (!targetReached)
            {
                OnTargetReached();
            }
            targetReached = true;
        }

        // Find our position along the path using a simple linear interpolation
        Vector3 positionAlongCurrentPath = segment * Mathf.Clamp01(segmentLength > 0 ? distanceAlongSegment / segmentLength : 1) + vPath[currentWaypointIndex - 1];

        direction = segment;

        if (interpolatePathSwitches)
        {
            // Find the approximate position we would be at if we
            // would have continued to follow the previous path
            Vector3 positionAlongPreviousPath = previousMovementOrigin + Vector3.ClampMagnitude(previousMovementDirection, speed * (Time.time - previousMovementStartTime));

            // Use this to debug
            //Debug.DrawLine (previousMovementOrigin, positionAlongPreviousPath, Color.yellow);

            return Vector3.Lerp(positionAlongPreviousPath, positionAlongCurrentPath, switchPathInterpolationSpeed * (Time.time - previousMovementStartTime));
        }
        else
        {
            return positionAlongCurrentPath;
        }
    }

    public List<Vector3> GetVectorPath()
    {
        if (path != null)
            return path.vectorPath;
        return new List<Vector3>();
    }

    public void ReleasePath()
    {
        // Release current path
        if (path != null) path.Release(this);
        path = null;
    }

    public Vector3 GetTeamMemberFollowPos(float space)
    {
        if (path != null)
        {
            List<Vector3> vPath = path.vectorPath;
            Vector3 pos2 = vPath[currentWaypointIndex];
            Vector3 pos1 = gridPos;

            if (pos1.x != pos2.x)
            {
                float k = (pos1.y - pos2.y) / (pos1.x - pos2.x);
                float value = Mathf.Sqrt(space * space / (1 + k * k));

                if (pos1.x - pos2.x >= 0)
                    return new Vector3(value + pos1.x, k * value + pos1.y, 0f);
                else
                    return new Vector3(pos1.x - value, pos1.y - k * value, 0f);
            }
            else if (pos1.y != pos2.y)
            {
                if (pos1.y - pos2.y >= 0)
                    return new Vector3(pos1.x, pos1.y - space, 0f);
                else
                    return new Vector3(pos1.x, pos1.y + space, 0f);
            }
        }

        var dest = gridPos - tr.forward * space;
        dest.z = 0;
        return dest;
    }

    #region 行走 or 飞行

    public enum MoveType
    {
        Land = 0,
        Fly = 1,
    }

    private MoveType _currMoveType = MoveType.Land;

    private static NNConstraint _landConstraint;
    public static NNConstraint LandConstraint
    {
        get
        {
            if (_landConstraint == null)
            {
                _landConstraint = new NNConstraint();
                _landConstraint.tags = 1;
            }
            return _landConstraint;
        }
    }

    private MoveType _newMoveType;
    private bool _needInitMode = false;

    public void SetMode(MoveType type)
    {
        _newMoveType = type;

        if (seeker != null && tr != null)
        {
            SetModeHandle();
        }
        else
        {
            _needInitMode = true;
            Debug.LogError("寻路调试信息,SetMode: seeker != null || tr != null,延迟设置是否飞行状态");
        }
    }

    private void SetModeHandle()
    {
        ReleasePath();
        if (_newMoveType == MoveType.Land)
        {
            seeker.traversableTags = -3;

            if (_currMoveType == MoveType.Fly)
            {
                Vector3 pos = new Vector3(tr.position.x, tr.position.y, 0);
                // 飞行转地面,落地点转移到最近的可行走陆地点
                NNInfo info = AstarPath.active.GetNearest(pos, LandConstraint);
                info.UpdateInfo();
                SetFinalPos(info.clampedPosition, _newMoveType);
            }
            else
            {
                SetFinalPos(tr.position, _newMoveType);
            }
        }
        else if (_newMoveType == MoveType.Fly)
        {
            seeker.traversableTags = -1;
            SetFinalPos(tr.position, _newMoveType);
        }
        _currMoveType = _newMoveType;
    }
    #endregion

    void OnDestroy()
    {
        Clear();
    }

    private int curNodeIndex = 0;
    private bool GetNearYAxix(Vector3 position,out float y)
    {
        if (path == null || path.path == null || !(AstarPath.active != null && AstarPath.active.astarData != null && AstarPath.active.astarData.navmesh != null))
        {
            y = 0;
            return false;
        }
        Int3[] vertisces = AstarPath.active.astarData.navmesh.vertices;
        GraphNode curNode = path.path[curNodeIndex];
        for (int index = curNodeIndex; index < path.path.Count; index++)
        {
            TriangleMeshNode node = (TriangleMeshNode)path.path[index];
            if (Polygon.ContainsPointXZ((Vector3) vertisces[node.v0], (Vector3) vertisces[node.v1], (Vector3) vertisces[node.v2], position))
            {
                curNode = node;
                curNodeIndex = index;
                break;
            }
        }
        Vector3 pos = ClosestPointOnNode((TriangleMeshNode)curNode, AstarPath.active.astarData.navmesh.vertices, position);
        y = pos.y;
        return true;
    }
    static Vector3 ClosestPointOnNode(TriangleMeshNode node, Int3[] vertices, Vector3 pos)
    {
        return Polygon.ClosestPointOnTriangle((Vector3)vertices[node.v0], (Vector3)vertices[node.v1], (Vector3)vertices[node.v2], pos);
    }

}

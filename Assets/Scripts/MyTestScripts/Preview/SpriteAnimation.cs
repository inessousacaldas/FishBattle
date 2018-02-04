using UnityEngine;

namespace Game.Preview
{
    /// <summary>
    /// 描述：人、怪、NPC动作动画控制类。
    /// 作者：helei
    /// 时间：2014-07-25 15:25:10
    /// </summary>
    public class SpriteAnimation : MonoBehaviour
    {
        private enum MoveState
        {
            Simple,
            Navigate,
            Raw,
            Stop
        }

        private Animator animator;
        private UnityEngine.AI.NavMeshAgent agent;
        private CharacterController cc;

        private string lastTrigger;
        private string state;

        private MoveState moveState = MoveState.Stop;
        private float moveAngle = 0f;
        private float moveSpeed = 0f;
        private Vector3 rawMoveSpeed;
        private UnityEngine.AI.NavMeshPath path;
        private long navPointIndex;

        void Awake()
        {
            animator = GetComponent<Animator>();
            cc = GetComponent<CharacterController>();
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = false;
            }
        }

        void Update()
        {
            switch (moveState)
            {
                case MoveState.Simple:
                    if (moveSpeed <= 0.1f || Camera.main == null)
                    {
                        break;
                    }
                    transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y + moveAngle, 0);
                    cc.SimpleMove(transform.TransformDirection(Vector3.forward) * moveSpeed);
                    moveState = MoveState.Stop;
                    break;
                case MoveState.Navigate:
                    if (moveSpeed <= 0.1f)
                    {
                        break;
                    }
                    Vector3 nextPoint = path.corners[navPointIndex];
                    if (Vector3.Distance(transform.position, new Vector3(nextPoint.x, transform.position.y, nextPoint.z)) <= moveSpeed * Time.deltaTime)
                    {
                        if (navPointIndex == path.corners.Length - 1)
                        {
                            transform.position = nextPoint;
                            StopMove();
                        }
                        else
                        {
                            navPointIndex++;
                        }
                    }
                    if (moveState == MoveState.Navigate)
                    {
                        Vector3 direction = nextPoint - transform.position;
                        direction.y = 0;
                        transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
                        cc.SimpleMove(transform.TransformDirection(Vector3.forward) * moveSpeed);
                    }
                    break;
                case MoveState.Raw:
                    cc.SimpleMove(rawMoveSpeed);
                    break;
            }
        }

        public void Action(string state, float speed = 1f)
        {
            if (animator != null)
            {
                animator.speed = speed;
                if (this.state != state)
                {
                    this.state = state;
//                    if (lastTrigger != null)
//                    {
//                        animator.ResetTrigger(lastTrigger);
//                    }
//                    lastTrigger = state.ToString();
//                    animator.SetTrigger(lastTrigger);
//                    animator.speed = speed;

					animator.Play(state);
                }
            }
        }

        void OnActionBegin(string name)
        {
           // state = (ActionState)System.Enum.Parse(typeof(ActionState), name);
        }

        void OnActionEnd(string name)
        {
            if (name == lastTrigger)
            {
                animator.speed = 1f;
            }
        }

        /// <summary>
        /// 以主摄像机为0度方向，-180~180度范围，以指定速率移动。
        /// </summary>
        /// <param name="angle">角度</param>
        /// <param name="speed">速率</param>
        public void Move(float angle, float speed)
        {
            moveState = MoveState.Simple;
            moveAngle = angle;
            moveSpeed = speed;
        }

        /// <summary>
        /// 在本物体坐标系中，移动指定矢量。
        /// </summary>
        /// <param name="speed">速度</param>
        public void SimpleMove(Vector3 speed)
        {
            moveState = MoveState.Raw;
            rawMoveSpeed = speed;
        }

        /// <summary>
        /// 移动到指定坐标。
        /// </summary>
        /// <param name="point">目标点</param>
        /// <param name="speed">速率</param>
        /// <param name="near">是否在目标点不可达时尽可能移动到接近目标点的位置处，比如：目标点在悬崖外，near=true则会移到悬崖边，否则原地不动</param>
        public void MoveTo(Vector3 point, float speed, bool near = false)
        {
            if (Vector3.Equals(point, transform.position))
            {
                return;
            }
            UnityEngine.AI.NavMeshPath tmpPath = new UnityEngine.AI.NavMeshPath();
            agent.enabled = true;
            agent.CalculatePath(point, tmpPath);
            if (tmpPath.status == UnityEngine.AI.NavMeshPathStatus.PathComplete || (tmpPath.status == UnityEngine.AI.NavMeshPathStatus.PathPartial && near))
            {
                moveState = MoveState.Navigate;
                moveSpeed = speed;
                path = tmpPath;
                navPointIndex = 1;
            }
            agent.enabled = false;
        }

        /// <summary>
        /// 停止移动。
        /// </summary>
        public void StopMove()
        {
            moveState = MoveState.Stop;
            path = null;
        }
    }

    /// <summary>
    /// 动作名。
    /// </summary>
    public enum ActionState
    {
        idle,           //休闲
        run,           //移动
        attack1,        //普攻1
        attack2,        //普攻2
        attack3,        //普攻3
        skill,         //技能
    }
}
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

namespace SceneUtility
{

    public class SceneQuadTree<T> where T : class, ISceneQuadObject<T>
    {
        private const int MaxShowSqrDistance = 20 * 20;
        private const float ProjectionScale = 1.2f;
        private const float ProjectionDisScale = 0.8f;
        private float _tempFov;
        private float _tempNear;
        private float _tempFar;
        private Matrix4x4 _tempMatrix;

        private readonly bool sort = false;
        private readonly Vector2 minLeafSize;
        private readonly int maxObjectsPerLeaf;
        private QuadNode root = null;
        private Dictionary<T, QuadNode> objectToNodeLookup = new Dictionary<T, QuadNode>();
        private Dictionary<T, int> objectSortOrder = new Dictionary<T, int>();
        private List<T> results = new List<T>();
        public QuadNode Root
        {
            get { return root; }
        }

        private int objectSortId = 0;

        public SceneQuadTree(Vector2 minLeafSize, int maxObjectsPerLeaf)
        {
            this.minLeafSize = minLeafSize;
            this.maxObjectsPerLeaf = maxObjectsPerLeaf;
        }

        public int GetSortOrder(T quadObject)
        {
            lock (objectSortOrder)
            {
                if (!objectSortOrder.ContainsKey(quadObject))
                    return -1;
                else
                {
                    return objectSortOrder[quadObject];
                }
            }
        }

        public SceneQuadTree(Vector2 minLeafSize, int maxObjectsPerLeaf, bool sort)
            : this(minLeafSize, maxObjectsPerLeaf)
        {
            this.sort = sort;
        }

        public void Insert(T[] quadObjectArray)
        {
            for (int i = 0; i < quadObjectArray.Length; i++)
            {
                Insert(quadObjectArray[i]);
            }
        }
        public void Insert(T quadObject)
        {

            if (sort && !objectSortOrder.ContainsKey(quadObject))
            {
                objectSortOrder.Add(quadObject, objectSortId++);
            }
            Bounds bounds = quadObject.Bounds;
            if (root == null)
            {
                var rootSize = new Vector2(Mathf.Ceil(bounds.width2D() / minLeafSize.x),
                    Mathf.Ceil(bounds.height2D() / minLeafSize.y));
                float multiplier = Mathf.Max(rootSize.x, rootSize.y);
                //                multiplier = Mathf.Max(1f, multiplier);
                rootSize = new Vector2(minLeafSize.x * multiplier, minLeafSize.y * multiplier);
                var center = bounds.center;
                root = new QuadNode(SceneQuadHelper.Bounds2D(center, rootSize));
            }

            while (!root.Bounds.Contains2D(bounds))
            {
                ExpandRoot(bounds);
            }

            InsertNodeObject(root, quadObject);

        }

        public T[] QueryByCamera(Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            results.Clear();
            if (root != null)
                QueryByCamera(planes, root, results);
            if (sort)
                results.Sort((a, b) => { return objectSortOrder[a].CompareTo(objectSortOrder[b]); });
            return results.ToArray();
        }

        private void QueryByCamera(Plane[] planes, QuadNode node, List<T> results)
        {
            if (node == null) return;
            if (GeometryUtility.TestPlanesAABB(planes, node.Bounds))
            {
                for (int i = 0; i < node.Objects.Count; i++)
                {
                    T quadObject = node.Objects[i];
                    if (GeometryUtility.TestPlanesAABB(planes, quadObject.Bounds))
                        results.Add(quadObject);
                }
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    QuadNode childNode = node.Nodes[i];
                    QueryByCamera(planes, childNode, results);

                }
            }
        }

        public T[] Query(Camera SceneCamera, Vector3 heroPos)
        {
            results.Clear();
            var planes = CalculateFrustumPlanes(SceneCamera);
            if (root != null)
                Query(planes, heroPos, root, results);
            if (sort)
                results.Sort((a, b) => { return objectSortOrder[a].CompareTo(objectSortOrder[b]); });
            return results.ToArray();
        }

        private void Query(Plane[] planes, Vector3 heroPos, QuadNode node, List<T> results)
        {
            if (node == null) return;
            if (node.Bounds.SqrDistance(heroPos) <= MaxShowSqrDistance)
            {
                for (int i = 0; i < node.Objects.Count; i++)
                {
                    T quadObject = node.Objects[i];
                    var added = false;
                    switch (quadObject.GoType)
                    {
                        case SceneGoType.Small:
                            {
                                added = GeometryUtility.TestPlanesAABB(planes, quadObject.Bounds) && quadObject.Bounds.SqrDistance(heroPos) <= MaxShowSqrDistance;
                                break;
                            }
                        case SceneGoType.Normal:
                            {
                                added = quadObject.Bounds.SqrDistance(heroPos) <= MaxShowSqrDistance;
                                break;
                            }
                        case SceneGoType.Big:
                            {
                                added = quadObject.Bounds.SqrDistance(heroPos) <= MaxShowSqrDistance || GeometryUtility.TestPlanesAABB(planes, quadObject.Bounds);
                                break;
                            }
                    }
                    if (added)
                    {
                        results.Add(quadObject);
                    }
                }
                for (int i = 0; i < node.Nodes.Count; i++)
                {
                    QuadNode childNode = node.Nodes[i];
                    Query(planes, heroPos, childNode, results);
                }
            }
        }

        private Plane[] CalculateFrustumPlanes(Camera camera)
        {
            if (_tempFov != camera.fieldOfView || _tempNear != camera.nearClipPlane || _tempFar != camera.farClipPlane)
            {
                _tempFov = camera.fieldOfView;
                _tempNear = camera.nearClipPlane;
                _tempFar = camera.farClipPlane;

                var top = _tempNear * Mathf.Tan(_tempFov * 0.5f * Mathf.Deg2Rad);
                top = top * ProjectionScale;
                var bottom = -top;
                var right = top * camera.aspect;
                var left = -right;
                _tempMatrix = SceneQuadHelper.PerspectiveOffCenter(left, right, bottom, top, _tempNear,
                    _tempFar * ProjectionDisScale);
            }

            // 考虑优化 GC ？
            return GeometryUtility.CalculateFrustumPlanes(_tempMatrix * camera.worldToCameraMatrix);
        }


        private void ExpandRoot(Bounds newChildBounds)
        {

            bool isNorth = root.Bounds.z() < newChildBounds.z();
            bool isWest = root.Bounds.x() < newChildBounds.x();

            Direction rootDirection;
            if (isNorth)
            {
                rootDirection = isWest ? Direction.NW : Direction.NE;
            }
            else
            {
                rootDirection = isWest ? Direction.SW : Direction.SE;
            }

            float newX = isWest
                ? root.Bounds.x() + root.Bounds.width2D() / 2
                : root.Bounds.x() - root.Bounds.width2D() / 2;
            float newY = isNorth
                ? root.Bounds.z() + root.Bounds.height2D() / 2
                : root.Bounds.z() - root.Bounds.height2D() / 2;
            Bounds newRootBounds = SceneQuadHelper.Bounds2D(newX, newY, root.Bounds.width2D() * 2, root.Bounds.height2D() * 2);
            QuadNode newRoot = new QuadNode(newRootBounds);
            SetupChildNodes(newRoot);
            newRoot[rootDirection] = root;
            root = newRoot;

        }

        private void InsertNodeObject(QuadNode node, T quadObject)
        {

            if (!node.Bounds.Contains2D(quadObject.Bounds))
            {

                CSGameDebuger.LogError("This should not happen, child does not fit within node bounds");
                return;
            }
            node.EncapsulateY(quadObject.Bounds);
            if (!node.HasChildNodes() && node.Objects.Count + 1 > maxObjectsPerLeaf)
            {
                SetupChildNodes(node);

                List<T> childObjects = new List<T>(node.Objects);
                List<T> childrenToRelocate = new List<T>();
                for (int i = 0; i < childObjects.Count; i++)
                {
                    T childObject = childObjects[i];
                    for (int j = 0; j < node.Nodes.Count; j++)
                    {
                        QuadNode childNode = node.Nodes[j];
                        if (childNode == null)
                            continue;

                        if (childNode.Bounds.Contains2D(childObject.Bounds))
                        {
                            childrenToRelocate.Add(childObject);
                        }
                    }
                }
                for (int i = 0; i < childrenToRelocate.Count; i++)
                {
                    T childObject = childrenToRelocate[i];
                    RemoveQuadObjectFromNode(childObject);
                    InsertNodeObject(node, childObject);
                }
            }
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                QuadNode childNode = node.Nodes[i];
                if (childNode != null)
                {
                    if (childNode.Bounds.Contains2D(quadObject.Bounds))
                    {
                        InsertNodeObject(childNode, quadObject);
                        return;
                    }
                }
            }

            AddQuadObjectToNode(node, quadObject);

        }

        private void ClearQuadObjectsFromNode(QuadNode node)
        {

            List<T> quadObjects = new List<T>(node.Objects);
            for (int i = 0; i < quadObjects.Count; i++)
            {
                T quadObject = quadObjects[i];

                RemoveQuadObjectFromNode(quadObject);

            }

        }

        private void RemoveQuadObjectFromNode(T quadObject)
        {

            QuadNode node = objectToNodeLookup[quadObject];
            node.quadObjects.Remove(quadObject);
            objectToNodeLookup.Remove(quadObject);
            //quadObject.BoundsChanged -= quadObject_BoundsChanged;

        }

        private void AddQuadObjectToNode(QuadNode node, T quadObject)
        {

            node.quadObjects.Add(quadObject);
            objectToNodeLookup.Add(quadObject, node);
            //quadObject.BoundsChanged += quadObject_BoundsChanged;

        }

        void quadObject_BoundsChanged(T quadObject)
        {

            if (quadObject != null)
            {
                QuadNode node = objectToNodeLookup[quadObject];
                if (!node.Bounds.Contains2D(quadObject.Bounds) || node.HasChildNodes())
                {
                    RemoveQuadObjectFromNode(quadObject);
                    Insert(quadObject);
                    if (node.Parent != null)
                    {
                        CheckChildNodes(node.Parent);
                    }
                }
            }

        }

        private void SetupChildNodes(QuadNode node)
        {
            float childWidth = node.Bounds.width2D() / 2;
            float childHeight = node.Bounds.height2D() / 2;
            if (minLeafSize.x <= childWidth && minLeafSize.y <= childHeight)
            {
                node[Direction.NW] = new QuadNode(node.Bounds.x() - childWidth / 2, node.Bounds.z() + childWidth / 2,
                    childWidth, childHeight);

                node[Direction.NE] = new QuadNode(node.Bounds.x() + childWidth / 2, node.Bounds.z() + childHeight / 2,
                    childWidth, childHeight);

                node[Direction.SW] = new QuadNode(node.Bounds.x() - childWidth / 2, node.Bounds.z() - childHeight / 2,
                    childWidth, childHeight);

                node[Direction.SE] = new QuadNode(node.Bounds.x() + childWidth / 2, node.Bounds.z() - childHeight / 2,
                    childWidth, childHeight);
            }

        }

        public void Remove(T quadObject)
        {

            if (sort && objectSortOrder.ContainsKey(quadObject))
            {
                objectSortOrder.Remove(quadObject);
            }

            if (!objectToNodeLookup.ContainsKey(quadObject))
            {
                CSGameDebuger.LogError("quadObject not found");
                return;
            }

            QuadNode containingNode = objectToNodeLookup[quadObject];
            RemoveQuadObjectFromNode(quadObject);

            if (containingNode.Parent != null)
                CheckChildNodes(containingNode.Parent);

        }



        private void CheckChildNodes(QuadNode node)
        {

            if (GetQuadObjectCount(node) <= maxObjectsPerLeaf)
            {
                List<T> subChildObjects = GetChildObjects(node);
                for (int i = 0; i < subChildObjects.Count; i++)
                {
                    T childObject = subChildObjects[i];

                    if (!node.Objects.Contains(childObject))
                    {
                        RemoveQuadObjectFromNode(childObject);
                        AddQuadObjectToNode(node, childObject);

                    }
                }
                if (node[Direction.NW] != null)
                {
                    node[Direction.NW].Parent = null;
                    node[Direction.NW] = null;
                }
                if (node[Direction.NE] != null)
                {
                    node[Direction.NE].Parent = null;
                    node[Direction.NE] = null;
                }
                if (node[Direction.SW] != null)
                {
                    node[Direction.SW].Parent = null;
                    node[Direction.SW] = null;
                }
                if (node[Direction.SE] != null)
                {
                    node[Direction.SE].Parent = null;
                    node[Direction.SE] = null;
                }

                if (node.Parent != null)
                    CheckChildNodes(node.Parent);
                else
                {
                    int numQuadrantsWithObjects = 0;
                    QuadNode nodeWithObjects = null;
                    for (int i = 0; i < node.Nodes.Count; i++)
                    {
                        QuadNode childNode = node.Nodes[i];

                        if (childNode != null && GetQuadObjectCount(childNode) > 0)
                        {
                            numQuadrantsWithObjects++;
                            nodeWithObjects = childNode;
                            if (numQuadrantsWithObjects > 1) break;

                        }
                    }
                    if (numQuadrantsWithObjects == 1)
                    {
                        for (int i = 0; i < node.Nodes.Count; i++)
                        {
                            QuadNode childNode = node.Nodes[i];

                            if (childNode != nodeWithObjects)
                                childNode.Parent = null;

                        }
                        root = nodeWithObjects;
                    }
                }
            }
        }


        private List<T> GetChildObjects(QuadNode node)
        {

            List<T> results = new List<T>();
            results.AddRange(node.quadObjects);
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                QuadNode childNode = node.Nodes[i];

                if (childNode != null)
                    results.AddRange(GetChildObjects(childNode));

            }
            return results;
        }

        public int GetQuadObjectCount()
        {

            if (root == null)
                return 0;
            int count = GetQuadObjectCount(root);
            return count;

        }

        private int GetQuadObjectCount(QuadNode node)
        {

            int count = node.Objects.Count;
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                QuadNode childNode = node.Nodes[i];

                if (childNode != null)
                {
                    count += GetQuadObjectCount(childNode);

                }
            }
            return count;

        }

        public int GetQuadNodeCount()
        {

            if (root == null)
                return 0;
            int count = GetQuadNodeCount(root, 1);
            return count;

        }

        private int GetQuadNodeCount(QuadNode node, int count)
        {
            if (node == null) return count;

            for (int i = 0; i < node.Nodes.Count; i++)
            {
                QuadNode childNode = node.Nodes[i];

                if (childNode != null)
                    count++;

            }
            return count;

        }

        public List<QuadNode> GetAllNodes()
        {

            List<QuadNode> results = new List<QuadNode>();
            if (root != null)
            {
                results.Add(root);
                GetChildNodes(root, results);
            }
            return results;

        }

        private void GetChildNodes(QuadNode node, ICollection<QuadNode> results)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                QuadNode childNode = node.Nodes[i];

                if (childNode != null)
                {
                    results.Add(childNode);
                    GetChildNodes(childNode, results);

                }
            }

        }

        public void DrawTree(float duration)
        {
            if (root != null)
            {
                DrawNode(root, Color.green, duration, 0.1f);
                DrawChild(root, duration, 0.1f);
            }
        }

        Color[] colors = new Color[4] { Color.red, Color.yellow, Color.blue, Color.white };

        private void DrawChild(QuadNode node, float duration, float y)
        {
            if (node.HasChildNodes() == false)
                return;
            int i = 0;
            y += 0.1f;
            for (int j = 0; j < node.Nodes.Count; j++)
            {
                var item = node.Nodes[j];

                DrawNode(item, colors[i++], duration, y);
                DrawChild(item, duration, y);

            }
        }

        private void DrawNode(QuadNode node, Color color, float duration, float y)
        {
            DrawBounds(node.Bounds, color, duration, y);
            for (int i = 0; i < node.Objects.Count; i++)
            {
                var item = node.Objects[i];

                DrawBounds(item.Bounds, color, duration, y);

            }
        }

        public static void DrawBounds(Bounds bounds, Color color, float duration, float y)
        {
            //DrawBounds2D(bounds, color, duration, y);
            DrawBounds3D(bounds, color, duration);
        }

        private static void DrawBounds2D(Bounds bounds, Color color, float duration, float y)
        {
            Vector3 SW = bounds.min;
            Vector3 NE = bounds.max;
            SW.y = y;
            NE.y = y;
            Vector3 SE = SW + new Vector3(bounds.width2D(), 0f, 0f);
            Vector3 NW = SW + new Vector3(0f, 0f, bounds.height2D());
            DrawLine(SW, SE, color, duration);
            DrawLine(SE, NE, color, duration);
            DrawLine(NE, NW, color, duration);
            DrawLine(NW, SW, color, duration);
        }

        private static void DrawBounds3D(Bounds bounds, Color color, float duration)
        {
            CaculatePoin(bounds, vector3s);
            for (int i = 0; i < intArray.Length - 1; i += 2)
            {
                DrawLine(vector3s[intArray[i]], vector3s[intArray[i + 1]], color, duration);
            }
        }

        private static void CaculatePoin(Bounds bounds, Vector3[] vector3s)
        {
            for (int i = 0; i < vector3s.Length; i++)
            {
                vector3s[i] = bounds.center;
            }
            vector3s[0] = CaculatePoinHeler(vector3s[0], bounds.extents, -1, -1, -1);
            vector3s[1] = CaculatePoinHeler(vector3s[1], bounds.extents, -1, +1, -1);
            vector3s[2] = CaculatePoinHeler(vector3s[2], bounds.extents, +1, +1, -1);
            vector3s[3] = CaculatePoinHeler(vector3s[3], bounds.extents, +1, -1, -1);

            vector3s[4] = CaculatePoinHeler(vector3s[4], bounds.extents, -1, -1, +1);
            vector3s[5] = CaculatePoinHeler(vector3s[5], bounds.extents, -1, +1, +1);
            vector3s[6] = CaculatePoinHeler(vector3s[6], bounds.extents, +1, +1, +1);
            vector3s[7] = CaculatePoinHeler(vector3s[7], bounds.extents, +1, -1, +1);

        }
        private static Vector3 CaculatePoinHeler(Vector3 vector3, Vector3 extents, float _x, float _y, float _z)
        {
            vector3.x += _x * extents.x;
            vector3.y += _y * extents.y;
            vector3.z += _z * extents.z;
            return vector3;
        }
        static readonly Vector3[] vector3s = new Vector3[8];
        static readonly int[] intArray = new[]
            {
            0, 1, 1, 2, 2, 3, 3, 0,
            4, 5, 5, 6, 6, 7, 7, 4,
            0, 0 + 4,
            1, 1 + 4,
            2, 2 + 4,
            3, 3 + 4
        };
        private static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
        {
            Debug.DrawLine(start, end, color, duration);
        }
        public class QuadNode
        {
            private static int _id = 0;
            public readonly int ID = _id++;

            public QuadNode Parent { get; internal set; }

            private QuadNode[] _nodes = new QuadNode[4];

            public QuadNode this[Direction direction]
            {
                get
                {
                    switch (direction)
                    {
                        case Direction.NW:
                            return _nodes[0];
                        case Direction.NE:
                            return _nodes[1];
                        case Direction.SW:
                            return _nodes[2];
                        case Direction.SE:
                            return _nodes[3];
                        default:
                            return null;
                    }
                }
                set
                {
                    switch (direction)
                    {
                        case Direction.NW:
                            _nodes[0] = value;
                            break;
                        case Direction.NE:
                            _nodes[1] = value;
                            break;
                        case Direction.SW:
                            _nodes[2] = value;
                            break;
                        case Direction.SE:
                            _nodes[3] = value;
                            break;
                    }
                    if (value != null)
                        value.Parent = this;
                }
            }

            public ReadOnlyCollection<QuadNode> Nodes;

            internal List<T> quadObjects = new List<T>();
            public ReadOnlyCollection<T> Objects;

            public Bounds Bounds { get { return _bounds; } internal set { _bounds = value; } }
            private Bounds _bounds;
            internal void EncapsulateY(Bounds bounds)
            {
                SceneQuadHelper.EncapsulateY(ref _bounds, bounds);
            }
            public bool HasChildNodes()
            {
                return _nodes[0] != null;
            }

            public QuadNode(Bounds bounds)
            {
                Bounds = bounds;
                Nodes = new ReadOnlyCollection<QuadNode>(_nodes);
                Objects = new ReadOnlyCollection<T>(quadObjects);
            }

            public QuadNode(float x, float y, float width, float height)
                : this(SceneQuadHelper.Bounds2D(x, y, width, height))
            {

            }
        }

        public enum Direction : int
        {
            NW = 0,
            NE = 1,
            SW = 2,
            SE = 3
        }
    }
}

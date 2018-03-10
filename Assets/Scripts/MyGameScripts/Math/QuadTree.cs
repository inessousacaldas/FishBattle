using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

public class QuadTree<T> where T : class, IQuadObject<T>
{
    private readonly bool sort = false;
    private readonly Vector2 minLeafSize;
    private readonly int maxObjectsPerLeaf;
    private QuadNode root = null;
    private Dictionary<T, QuadNode> objectToNodeLookup = new Dictionary<T, QuadNode>();
    private Dictionary<T, int> objectSortOrder = new Dictionary<T, int>();
    public QuadNode Root { get { return root; } }
    private int objectSortId = 0;

    public QuadTree(Vector2 minLeafSize, int maxObjectsPerLeaf)
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
    public QuadTree(Vector2 minLeafSize, int maxObjectsPerLeaf, bool sort)
        : this(minLeafSize, maxObjectsPerLeaf)
    {
        this.sort = sort;
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
            multiplier = Mathf.Max(1f, multiplier);
            rootSize = new Vector2(minLeafSize.x * multiplier, minLeafSize.y * multiplier);
            var center = bounds.center;
            root = new QuadNode(MathHelper.Bounds2D(center, rootSize));
        }

        while (!root.Bounds.Contains(bounds))
        {
            ExpandRoot(bounds);
        }

        InsertNodeObject(root, quadObject);

    }

    public List<T> Query(Bounds bounds)
    {

        List<T> results = new List<T>();
        if (root != null)
            Query(bounds, root, results);
        if (sort)
            results.Sort((a, b) => { return objectSortOrder[a].CompareTo(objectSortOrder[b]); });
        return results;

    }

    private void Query(Bounds bounds, QuadNode node, List<T> results)
    {

        if (node == null) return;

        if (bounds.Intersects(node.Bounds))
        {
            for (int i = 0; i < node.Objects.Count; i++)
            {
                T quadObject = node.Objects[i];
                if (bounds.Intersects(quadObject.Bounds))
                    results.Add(quadObject);
            }
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                QuadNode childNode = node.Nodes[i];
                Query(bounds, childNode, results);

            }
        }

    }

    private void ExpandRoot(Bounds newChildBounds)
    {

        bool isNorth = root.Bounds.y2D() < newChildBounds.y2D();
        bool isWest = root.Bounds.x2D() < newChildBounds.x2D();

        Direction rootDirection;
        if (isNorth)
        {
            rootDirection = isWest ? Direction.NW : Direction.NE;
        }
        else
        {
            rootDirection = isWest ? Direction.SW : Direction.SE;
        }

        float newX = (rootDirection == Direction.NW || rootDirection == Direction.SW)
                          ? root.Bounds.x2D() + root.Bounds.width2D() / 2
                          : root.Bounds.x2D() - root.Bounds.width2D() / 2;
        float newY = (rootDirection == Direction.NW || rootDirection == Direction.NE)
                          ? root.Bounds.y2D() + root.Bounds.height2D() / 2
                          : root.Bounds.y2D() - root.Bounds.height2D() / 2;
        Bounds newRootBounds = MathHelper.Bounds2D(newX, newY, root.Bounds.width2D() * 2, root.Bounds.height2D() * 2);
        QuadNode newRoot = new QuadNode(newRootBounds);
        SetupChildNodes(newRoot);
        newRoot[rootDirection] = root;
        root = newRoot;

    }

    private void InsertNodeObject(QuadNode node, T quadObject)
    {

        if (!node.Bounds.Contains(quadObject.Bounds))
        {

            GameDebuger.LogError("This should not happen, child does not fit within node bounds");
            return;
        }
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

                    if (childNode.Bounds.Contains(childObject.Bounds))
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
                if (childNode.Bounds.Contains(quadObject.Bounds))
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
        quadObject.BoundsChanged -= quadObject_BoundsChanged;

    }

    private void AddQuadObjectToNode(QuadNode node, T quadObject)
    {

        node.quadObjects.Add(quadObject);
        objectToNodeLookup.Add(quadObject, node);
        quadObject.BoundsChanged += quadObject_BoundsChanged;

    }

    void quadObject_BoundsChanged(T quadObject)
    {

        if (quadObject != null)
        {
            QuadNode node = objectToNodeLookup[quadObject];
            if (!node.Bounds.Contains(quadObject.Bounds) || node.HasChildNodes())
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
            node[Direction.NW] = new QuadNode(node.Bounds.x2D() - childWidth / 2, node.Bounds.y2D() + childWidth / 2,
                                              childWidth, childHeight);

            node[Direction.NE] = new QuadNode(node.Bounds.x2D() + childWidth / 2, node.Bounds.y2D() + childHeight / 2,
                                              childWidth, childHeight);

            node[Direction.SW] = new QuadNode(node.Bounds.x2D() - childWidth / 2, node.Bounds.y2D() - childHeight / 2,
                                              childWidth, childHeight);

            node[Direction.SE] = new QuadNode(node.Bounds.x2D() + childWidth / 2, node.Bounds.y2D() - childHeight / 2,
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
            GameDebuger.LogError("quadObject not found");
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
        Vector3 SW = bounds.min;
        Vector3 NE = bounds.max;
        SW.y = y;
        NE.y = y;
        Vector3 SE = SW + new Vector3(bounds.width2D(), 0f, 0f);
        Vector3 NW = SW + new Vector3(0f, 0f, bounds.height2D());
        Debug.DrawLine(SW, SE, color, duration);
        Debug.DrawLine(SE, NE, color, duration);
        Debug.DrawLine(NE, NW, color, duration);
        Debug.DrawLine(NW, SW, color, duration);
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

        public Bounds Bounds { get; internal set; }

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
            : this(MathHelper.Bounds2D(x, y, width, height))
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


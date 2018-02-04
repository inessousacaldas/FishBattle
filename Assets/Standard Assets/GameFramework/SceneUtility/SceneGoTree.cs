using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SceneGoTreeNode
{
	public SceneGoTreeNode parentNode;
	public List<SceneGoTreeNode> subNodeList = new List<SceneGoTreeNode> ();

	SceneGoTreeNode[] subNodeArray = null;
	public SceneGoTreeNode[] SubNodeArray 
	{
		get {
			if (subNodeArray == null)
				subNodeArray = subNodeList.ToArray ();
			return subNodeArray;
		}
	}

	public Vector3 pos;
	public float size;
	public int level = 0;
	public const int maxLevel = 3;
	public List <int> sceneGoIdList = new List<int> ();

	public Bounds boundBox;
	int[] sceneGoIdArray = null;
	public int[] SceneGoIdArray {
		get {
			if (sceneGoIdArray == null)
				sceneGoIdArray = sceneGoIdList.ToArray ();
			return sceneGoIdArray;
		}
	}
}
	
public class SceneGoTree
{
	public SceneGoInfo[] sceneGoArray;
	public Vector3 minXYZ = new Vector3 (99999, 99999, 99999);
	public Vector3 maxXYZ = new Vector3 (-99999, -99999, -99999);
	public HashSet<int> addedSceneGo = new HashSet<int> ();
	public float maxSize = 0f;
    public List<int> visibleList;
	void InitRange (SceneGoInfo[] sceneGoInfoArray)
	{
		for (int i = 0; i < sceneGoInfoArray.Length; i++)
		{
			SceneGoInfo sceneGoInfo = sceneGoInfoArray[i];
			Vector3 pos = sceneGoInfo.bounds.center;
			if (minXYZ.x > pos.x - sceneGoInfo.bounds.extents.x)
				minXYZ.x = pos.x - sceneGoInfo.bounds.extents.x;
			if (maxXYZ.x < pos.x + sceneGoInfo.bounds.extents.x)
				maxXYZ.x = pos.x + sceneGoInfo.bounds.extents.x;

			if (minXYZ.y > pos.y - sceneGoInfo.bounds.extents.y)
				minXYZ.y = pos.y - sceneGoInfo.bounds.extents.y;
			if (maxXYZ.y < pos.y + sceneGoInfo.bounds.extents.y)
				maxXYZ.y = pos.y + sceneGoInfo.bounds.extents.y;

			if (minXYZ.z > pos.z - sceneGoInfo.bounds.extents.z)
				minXYZ.z = pos.z - sceneGoInfo.bounds.extents.z;
			if (maxXYZ.z < pos.z + sceneGoInfo.bounds.extents.z)
				maxXYZ.z = pos.z + sceneGoInfo.bounds.extents.z;
		}
	}

	void InitEmptyTree ()
	{
		this.rootNode = new SceneGoTreeNode ();
		rootNode.level = 0;
		rootNode.parentNode = null;
		rootNode.size = (this.maxXYZ - this.minXYZ).x;
		if ((this.maxXYZ - this.minXYZ).y > rootNode.size)
			rootNode.size = (this.maxXYZ - this.minXYZ).y;
		if ((this.maxXYZ - this.minXYZ).z > rootNode.size)
			rootNode.size = (this.maxXYZ - this.minXYZ).z;

		rootNode.size = (rootNode.size) / 2f;
		rootNode.pos = this.minXYZ + new Vector3 (rootNode.size, rootNode.size, rootNode.size);
		rootNode.boundBox = new Bounds (rootNode.pos, new Vector3 (rootNode.size * 2f, rootNode.size *2f, rootNode.size * 2f));

		SplitNode (rootNode);
	}

	void SplitNode (SceneGoTreeNode treeNode)
	{
		if (treeNode.level >= 3)
			return;
		for (int x = -1; x <= 1; x++)
			for (int y = -1; y <= 1; y++)
				for (int z = -1; z <= 1; z++)
				{
					if (x == 0 || y == 0 || z == 0)
						continue;
					SceneGoTreeNode subNode = new SceneGoTreeNode ();
					subNode.parentNode = treeNode;
					subNode.level = treeNode.level + 1;
					subNode.size = treeNode.size / 2f;
					subNode.pos = new Vector3 (x, y, z) * subNode.size + treeNode.pos;
					subNode.boundBox = new Bounds (subNode.pos, new Vector3 (subNode.size * 2f, subNode.size * 2f, subNode.size * 2f));
					treeNode.subNodeList.Add (subNode);
				}
		foreach (SceneGoTreeNode subNode in treeNode.subNodeList)
			SplitNode (subNode);
	}

	void FillTree (SceneGoInfo[] sceneGoInfoArray)
	{
		for (int i = 0; i < sceneGoInfoArray.Length; i++)
		{
			this.AddSceneGo (sceneGoInfoArray[i]);
		}

	}

	void AddSceneGo (SceneGoInfo sceneGoInfo)
	{
		if (sceneGoInfo.treeLevel == 0)
		{
			rootNode.sceneGoIdList.Add (sceneGoInfo.id);
			return;
		}
		foreach (SceneGoTreeNode treeNode in rootNode.subNodeList)
		{
            this.AddSceneGo(treeNode, sceneGoInfo);
		}
	}		

	bool InNodeRange (SceneGoTreeNode treeNode, SceneGoInfo sceneGoInfo)
	{
		
		bool res =  treeNode.boundBox.Intersects (sceneGoInfo.bounds);
		return res;
	}

	bool AddSceneGo (SceneGoTreeNode treeNode,  SceneGoInfo sceneGoInfo)
	{
		if (!this.InNodeRange (treeNode, sceneGoInfo))
			return false;
		if (treeNode.level == sceneGoInfo.treeLevel)
		{
			treeNode.sceneGoIdList.Add (sceneGoInfo.id);
			return true;
		}

		if (treeNode.level > sceneGoInfo.treeLevel)
			return false;

		bool res = false;
		foreach (SceneGoTreeNode subTreeNode in treeNode.subNodeList)
		{
            if (AddSceneGo(subTreeNode, sceneGoInfo))
            {
                res = true;
            }
		}
		return res;
	}


	public SceneGoTree (SceneGoInfo[] sceneGoInfoArray)
	{
		this.sceneGoArray = sceneGoInfoArray;
		this.InitRange (sceneGoInfoArray);
		this.InitEmptyTree ();
		this.FillTree (sceneGoInfoArray);
        this.visibleList = new List<int>(sceneGoInfoArray.Length / 16);
	}
	Plane[] camPlanes;
    Camera mainCamera;
    public List<int> GetVisibleList (Camera cam)
    {
        this.visibleList.Clear();
        this.VisitVisible(cam, sceneGoId => {
            if (!this.visibleList.Contains (sceneGoId))
                this.visibleList.Add(sceneGoId); });
        return this.visibleList;
    }

	public void VisitVisible (Camera cam, System.Action<int> visitFunc)
	{
		camPlanes = GeometryUtility.CalculateFrustumPlanes (cam);
		if (rootNode.SceneGoIdArray != null)
		{
			for (int i = 0; i < rootNode.SceneGoIdArray.Length; i++)
			{
				visitFunc (rootNode.SceneGoIdArray[i]);
			}
		}

		for (int i = 0; i < rootNode.SubNodeArray.Length;i++)
		{
			SceneGoTreeNode node = rootNode.SubNodeArray[i]; 
			if (!this.IsNodeInCamera (node))
				continue;
			this.VisitVisible (cam, node, visitFunc);
		}
	}

	bool IsBoundBoxInCamera (Bounds bound)
	{
		return GeometryUtility.TestPlanesAABB (camPlanes, bound);
	}


	bool IsNodeInCamera (SceneGoTreeNode node)
	{
		return GeometryUtility.TestPlanesAABB (camPlanes, node.boundBox);
	}

	void VisitVisible (Camera cam, SceneGoTreeNode node, System.Action <int> visitFunc)
	{
		if (node.SceneGoIdArray != null && node.SceneGoIdArray.Length > 0)
		{
			for (int i = 0; i < node.SceneGoIdArray.Length; i++) 
			{
				int sceneGoId = node.SceneGoIdArray[i];
				SceneGoInfo sceneGoInfo = this.sceneGoArray[sceneGoId];
				if (!IsBoundBoxInCamera (sceneGoInfo.bounds))
					continue;
				visitFunc(sceneGoId);
			}
		}

		if (node.SubNodeArray == null || node.SubNodeArray.Length == 0)
			return;

		for (int i = 0; i < node.SubNodeArray.Length;i++)
		{
			SceneGoTreeNode subNode = node.SubNodeArray[i];
			if (!this.IsNodeInCamera (subNode))
				continue;
			this.VisitVisible (cam, subNode, visitFunc);
		}
	}

	private SceneGoTreeNode rootNode;
    public SceneGoTreeNode RootNode
    {
        get
        {
            return rootNode;
        }
    }
}

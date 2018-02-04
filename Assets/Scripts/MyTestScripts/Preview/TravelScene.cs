using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using SceneUtility;
#if UNITY_EDITOR
using UnityEditor;

public class TravelScene : MonoBehaviour {
    public string idleAniName = null;
    public string moveAniName = null;
	Animator animator;
	public float visibleRange = 50f;
    Camera mainCamera;
    bool pathFinderReady = false;
    private GameObject astarPathGO;
	void Start () {
		animator = gameObject.GetComponentInChildren<Animator> ();
        this.mainCamera = this.gameObject.GetComponentInChildren <Camera> ();
		//camGo.transform.parent = ;
		//camGo.transform.localPosition = new Vector3 (0f, 7.4f, -6.8f);
		//camGo.transform.localEulerAngles = new Vector3 (45, 0, 0);
		string sceneName = SceneManager.GetActiveScene ().name.ToLower ();
		string navmeshPath = ("Assets/GameResources/ArtResources/SceneConfig/Navmesh/navmesh_" + sceneName + ".asset");
	    TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(navmeshPath);
        astarPathGO = new GameObject("astarPath");
	    astarPathGO.GetMissingComponent<AstarPath>();
        AstarPath.active.astarData.SetData(textAsset.bytes);
        InitSceneGoTree();
        pathFinderReady = true;

	}

	 void CollectPrefabGo (Transform t, ref List<GameObject> prefabGoList)
	{
        GameObject prefab = PrefabUtility.GetPrefabParent (t.gameObject) as GameObject;
         if (prefab != null)
		{
			if (!t.gameObject.activeInHierarchy)
			{
				return;
			}
			prefabGoList.Add (t.gameObject);
			return;
		}

		foreach (Transform _t in t)
		{
			CollectPrefabGo (_t, ref prefabGoList);
		}
    }

	static int GetLevel (string layerName)
	{
		if (layerName.Equals ("Always"))
			return 0;
		if (layerName.Equals ("High"))
			return 1;
		if (layerName.Equals ("Middle"))
			return 2;
		if (layerName.Equals ("Low"))
			return 3;
		return 3;
	}


	public static SceneGoInfo[] CreateSceneGoInfoArray (List<GameObject> prefabGoList)
	{
		SceneGoInfo[] res = new SceneGoInfo[prefabGoList.Count];
		for (int i = 0; i < prefabGoList.Count; i++)
		{
			GameObject prefabGo = prefabGoList[i];
			Transform t = prefabGo.transform;
			SceneGoInfo sceneGoInfo = new SceneGoInfo ();
			sceneGoInfo.id = i;
			sceneGoInfo.pos = t.position;
			sceneGoInfo.rotation = t.rotation;
			sceneGoInfo.scale = t.localScale;
			sceneGoInfo.prefabName = "";
			sceneGoInfo.gameobjectName = t.name;
			res[i] = sceneGoInfo;
			sceneGoInfo.treeLevel = GetLevel (prefabGo.tag);
		    var sceneGoInfoComponents = prefabGo.GetComponentsInChildren<SceneGoInfoComponent>();
			if(sceneGoInfoComponents.Length == 0)
                continue;
		    Bounds bounds = sceneGoInfoComponents[0].bounds;
		    foreach (var sceneGoInfoComponent in sceneGoInfoComponents)
		    {
		        bounds.Encapsulate(sceneGoInfoComponent.bounds);
		    }
            sceneGoInfo.bounds = bounds;
		}
		return res;
	}

	List <GameObject> prefabGoList;
	HashSet <int> toShowList;
    SceneQuadTree<SceneGoInfo> sceneGoTree;
	void InitSceneGoTree ()
	{
		GameObject root = GameObject.Find ("WorldStage");
		prefabGoList = new List<GameObject> ();
		CollectPrefabGo (root.transform, ref prefabGoList);
		SceneGoInfo[] sceneGoInfoArray = CreateSceneGoInfoArray (prefabGoList);
        sceneGoTree = new SceneQuadTree<SceneGoInfo>(new Vector2(10, 10), 5);
        sceneGoTree.Insert(sceneGoInfoArray);
        toShowList = new HashSet<int> ();
	}

    void SetVisible(int sceneGoId, bool visible)
    {
        prefabGoList[sceneGoId].SetActive(visible);
    }

    public float speed = 5f;
    // Update is called once per frame
    bool lastFrameIdle = true;
	void Update () {
        if (!pathFinderReady)
            return;
		toShowList.Clear ();
        var list = sceneGoTree.QueryByCamera(this.mainCamera);
	    foreach (var info in list)
	    {
	        toShowList.Add(info.id);
	    }
		for (int i = 0; i < this.prefabGoList.Count; i++)
		{
            SetVisible(i, toShowList.Contains(i));
		}
        Vector2 dir = Vector2.zero;

        if (Input.GetKey(KeyCode.A))
        {
            dir += Vector2.left; 
        }

        if (Input.GetKey(KeyCode.W))
            dir += Vector2.up;

        if (Input.GetKey(KeyCode.D))
            dir += Vector2.right;

        if (Input.GetKey(KeyCode.S))
            dir += Vector2.down;


        this.transform.position += new Vector3(dir.x, 0, dir.y).normalized * Time.deltaTime * speed;
        this.transform.position = SceneHelper.GetSceneStandPosition(this.transform.position, Vector3.zero);
	}

    private Coroutine coroutine;
    void ShowNodes()
    {
        coroutine = StartCoroutine(DrawTreeCoroutine());
    }

    void HideNodes()
    {
        StopCoroutine(coroutine);
        coroutine = null;
    }

    IEnumerator DrawTreeCoroutine()
    {
        while (true)
        {
            sceneGoTree.DrawTree(0);
            yield return null;
        }
    }
    bool hasShowAllNode = false;
    void OnGUI()
    {
        if (!hasShowAllNode) {
            if (GUI.Button (new Rect (0, 0, 100, 50), "ShowNodes"))
            {
                this.ShowNodes();
                hasShowAllNode = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect(0, 0, 100, 50), "HideNodes"))
            {
                this.HideNodes();
                hasShowAllNode = false;
            }
        }
    }
}
#endif

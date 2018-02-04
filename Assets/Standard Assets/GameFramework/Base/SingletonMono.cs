//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
//{
//	private static T _instance;
//	private static bool _isQuit = false;

//	public static T Instance {
//		get {
//			if (_instance == null && !_isQuit) {
//				var type = typeof(T);
//				var objects = FindObjectsOfType<T> ();

//				if (objects.Length > 0) {
//					_instance = objects [0];
//					if (objects.Length > 1) {
//						Debug.LogWarning ("There is more than one instance of Singleton of type \"" + type + "\". Keeping the first. Destroying the others.");
//						for (var i = 1; i < objects.Length; i++)
//							DestroyImmediate (objects [i].gameObject);
//					}
//					return _instance;
//				}

//				GameObject go = new GameObject (type.Name);
//				_instance = go.AddComponent<T> ();
//				DontDestroyOnLoad (go);
//			}
//			return _instance;
//		}
//	}

//	void Awake ()
//	{
//		Init ();
//	}

//	//destory
//	protected virtual void OnDestroy ()
//	{
//	}

//	//init
//	protected virtual void Init ()
//	{
//	}

//	public virtual void Dispose ()
//	{

//	}

//	void OnApplicationQuit ()
//	{
//		_isQuit = true;
//	}
//}

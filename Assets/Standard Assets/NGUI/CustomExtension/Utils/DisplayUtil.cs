using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Standard_Assets.NGUI.CustomExtension.Utils
{
    public static class DisplayUtil
    {
        public static Transform[] GetChildList(Transform tar)
        {
            return tar.GetComponentsInChildren<Transform>(true);
        }

        public static Transform GetChildByName(Transform tar,string name,bool inActive = true)
        {
            if(tar != null)
            {
                Transform[] tarList = tar.GetComponentsInChildren<Transform>(inActive);
                if(tarList != null)
                {
                    foreach(Transform t in tarList)
                    {
                        if(t.name == name)
                        {
                            return t;
                        }
                    }
                }
            }
            return null;
        }

        public static GameObject GetChildObjByName(Transform tar,string name,bool inActive = true)
        {
            Transform t = GetChildByName(tar, name,inActive);
            if(t != null)
            {
                return t.gameObject;
            }
            return null;
        }

        public static Component GetComponentByName(GameObject tar,string name)
        {
            Transform[] tarList = tar.transform.GetComponentsInChildren<Transform>();
            foreach(Transform t in tarList)
            {
                Component comp = t.gameObject.GetComponent(name);
                if(comp != null)
                {
                    return comp;
                }
            }
            return null;
        }

        public static List<Component> GetComponentsByName(GameObject tar,string name)
        {
            Transform[] tarList = tar.transform.GetComponentsInChildren<Transform>();
            List<Component> list = null;
            foreach(Transform t in tarList)
            {
                Component comp = t.gameObject.GetComponent(name);
                if(comp != null)
                {
                    if(list == null)
                    {
                        list = new List<Component>();
                    }
                    list.Add(comp);
                }
            }
            return list;
        }

        public static List<Component> GetComponentByType(GameObject tar,Type type,bool inActive = true)
        {
            List<Component> list = null;
            Component comp = tar.GetComponent(type);
            Transform[] tarList = tar.GetComponentsInChildren<Transform>(inActive);
            for(int i = 0, len = tarList.Length;i < len;i++)
            {
                Transform t = tarList[i];
                comp = t.gameObject.GetComponent(type);
                if(comp != null)
                {
                    if(list == null)
                    {
                        list = new List<Component>();
                    }
                    list.Add(comp);
                }
            }
            return list;
        }

        public static List<T> GetComponentByType<T>(GameObject tar,bool inActive = true) where T : Component
        {
            List<T> list = null;
            T comp = tar.GetComponent<T>();
            Transform[] tarList = tar.GetComponentsInChildren<Transform>(inActive);
            for(int i = 0, len = tarList.Length;i < len;i++)
            {
                Transform t = tarList[i];
                comp = t.gameObject.GetComponent<T>();
                if(comp != null)
                {
                    if(list == null)
                    {
                        list = new List<T>();
                    }
                    list.Add(comp);
                }
            }
            return list;
        }

        public static int getTrisAllByObj(GameObject tar)
        {
            int tris = 0;
            Transform[] tarList = tar.GetComponentsInChildren<Transform>();
            foreach(Transform t in tarList)
            {
                MeshFilter mF = t.gameObject.GetComponent("MeshFilter") as MeshFilter;
                if(mF != null)
                {
                    tris += mF.mesh.vertexCount;
                }
            }
            return tris;
        }

        public static MeshFilter GetMeshFilterByObj(GameObject tar)
        {
            MeshFilter[] tarList = tar.GetComponentsInChildren<MeshFilter>();
            return tarList[0];
        }

        //可以把gameobject上面的脚本暂停的方法
        public static void SetBehaviourEnable(GameObject tar,Boolean isEnable)
        {
            Component[] list = tar.GetComponents(typeof(MonoBehaviour));
            for(int i = 0;i < list.Length;i++)
            {
                MonoBehaviour behaviour = list[i] as MonoBehaviour;
                if(behaviour != null) behaviour.enabled = isEnable;
            }
        }



        public static void AddParentDepth(int parentDepth,GameObject go)
        {
            UIWidget[] subWidgets = go.GetComponentsInChildren<UIWidget>(true);
            for(int i = 0;i < subWidgets.Length;i++)
            {
                subWidgets[i].depth += parentDepth;
            }
        }
    }
}
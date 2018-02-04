using System;
using System.Collections.Generic;
using UnityEngine;
public class UpdateManager : MonoBehaviour
{
    private static UpdateManager _instance;

    public static UpdateManager Instarnce
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("UpdateManager");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<UpdateManager>();
            }
            return _instance;
        }
    }

    public static void Add(ILateUpdateObj nodeValue)
    {
        Instarnce.AddToList(nodeValue);  
    }

    public static void Remove(ILateUpdateObj nodeValue)
    {
        if (_instance)
        {
            _instance.RemoveToList(nodeValue);
        }
    }
    public interface ILateUpdateObj
    {
        void CustomLateUpdate();
        LinkedListNode<ILateUpdateObj> node { get; set; }
    }

    private readonly LinkedList<ILateUpdateObj> lateUpdateList = new LinkedList<ILateUpdateObj>();
    private readonly LinkedList<ILateUpdateObj> unUseList = new LinkedList<ILateUpdateObj>(); 
    private LinkedListNode<ILateUpdateObj> curLinkedNode = null; 
    void LateUpdate()
    {
        curLinkedNode = lateUpdateList.First;
        while (curLinkedNode != null)
        {
            try
            {
                LinkedListNode<ILateUpdateObj> node = curLinkedNode; 
                curLinkedNode = curLinkedNode.Next;
                node.Value.CustomLateUpdate();
            }
            catch (Exception e)
            {
                CSGameDebuger.LogException(e);
            }
        }
    }

    private void AddToList(ILateUpdateObj nodeValue)
    {
        LinkedListNode<ILateUpdateObj> node = unUseList.Last;
        if (node != null)
        {
            node.Value = nodeValue;
            unUseList.Remove(node);
        }
        else
        {
            node = new LinkedListNode<ILateUpdateObj>(nodeValue);
        }
        lateUpdateList.AddLast(node);
        nodeValue.node = node;
    }

    private void RemoveToList(ILateUpdateObj nodeValue)
    {
        if (nodeValue.node != null && nodeValue.node.List == lateUpdateList)
        {
            if (curLinkedNode == nodeValue.node)
            {
                curLinkedNode = nodeValue.node.Previous;
            }
            nodeValue.node.List.Remove(nodeValue);
            nodeValue.node.Value = null;
            unUseList.AddLast(nodeValue.node);
            nodeValue.node = null;
        }
    }
     
}

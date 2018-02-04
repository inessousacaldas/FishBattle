// **********************************************************************
// Copyright (c) 2015 cilugame. All rights reserved.
// File     : AudioclipDestroyHandler.cs
// Author   : senkay <senkay@126.com>
// Created  : 10/14/2015 
// Porpuse  : 
// **********************************************************************
//
using UnityEngine;
using System.Collections;

public class AudioclipDestroyHandler : MonoBehaviour
{

	public AudioClip audioClip;

	// Update is called once per frame
	void OnDestroy ()
	{
		ResourcePoolManager.Instance.Despawn(audioClip, ResourcePoolManager.PoolType.DESTROY_NO_REFERENCE);
	}
}


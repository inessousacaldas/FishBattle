using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IPathFinder {
	Vector3 FindNearestPos (Vector3 pos);
	Queue <Vector3> FindPath (Vector3 start, Vector3 end);
	Vector3 FindRandomPos();
}

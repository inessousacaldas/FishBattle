using UnityEngine;
using System;
using System.Collections.Generic;
public interface IQuadObject<T>: IEqualityComparer<T>
{
    Bounds Bounds { get; }
    event System.Action<T> BoundsChanged;
}


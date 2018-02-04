using System.Collections.Generic;
using UnityEngine;

namespace SceneUtility
{
    public interface ISceneQuadObject<T> : IEqualityComparer<T>
    {
        Bounds Bounds { get; }
        //event System.Action<T> BoundsChanged;
        SceneGoType GoType { get; set; }
    }

    public enum SceneGoType
    {
        Null,
        Small,
        Normal,
        Big,
    }
}
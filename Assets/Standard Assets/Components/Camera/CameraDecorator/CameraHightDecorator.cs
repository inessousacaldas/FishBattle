using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CameraHightDecorator : CameraDecoratorBase
{
    private readonly float addHeight;
    private readonly float addRadius;
    public CameraHightDecorator(float addHeight, float addRadius)
    {
        this.addHeight = addHeight;
        this.addRadius = addRadius;
    }

    public override Vector2 AddDistance(Vector2 rawHegiht)
    {
        return new Vector2(addRadius, addHeight);

    }
}
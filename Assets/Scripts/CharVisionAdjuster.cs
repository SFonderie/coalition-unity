using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharVisionAdjuster : MonoBehaviour
{
    /// <summary>
    /// Whether or not to stretch or sqeeze the cone's length.
    /// </summary>
    [SerializeField]
    public bool ModulateLength = false;

    /// <summary>
    /// Whether or not to stretch or sqeeze the cone's height.
    /// </summary>
    [SerializeField]
    public bool ModulateWidth = true;

    /// <summary>
    /// The factor by which the height of the cone should be stetched whenever the cone rotation reaches an apex.
    /// </summary>
    [SerializeField]
    public float LengthScaleFactor = 1.0f;

    /// <summary>
    /// The factor by which the width of the cone should be stetched whenever the cone rotation reaches an apex.
    /// </summary>
    [SerializeField]
    public float WidthScaleFactor = 2.0f;

    /// <summary>
    /// The angle increment at which rotations will reach an apex.
    /// </summary>
    [SerializeField]
    public float ApexAngle = 90.0f;
    
    void Start() { }
    
    void Update()
    {
        float zRot = transform.localEulerAngles.z;
        float mod = Mathf.Sin(zRot * (ApexAngle * 90.0f));

        Vector3 currentScale = transform.localScale;

        if (ModulateLength) currentScale.x *= LengthScaleFactor * mod;
        if (ModulateWidth) currentScale.y *= WidthScaleFactor * mod;

        transform.localScale = currentScale;
    }
}

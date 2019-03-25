using UnityEngine;
using System;

[Serializable]
public class NoiseSettings
{
    //scale of x,y
    public float noiseScale = 1f;

    //scale of the noise so 0-1f is normal * this
    public float noiseAmplitude = 1f;

    //noise offset
    public Vector2 noiseOffset = Vector2.zero;
}
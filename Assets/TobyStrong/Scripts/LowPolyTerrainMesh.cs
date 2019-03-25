using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class LowPolyTerrainMesh : MonoBehaviour 
{
    MeshFilter mf;
    MeshRenderer mr;
    MeshCollider mc;

    public float meshWidth = 100f;
    public int steps = 5;
    float[,] heights;

    public Color meshColor = Color.white;

    public NoiseSettings noiseSettings;

    void OnValidate()
    {
        mf = GetComponent<MeshFilter>() ? GetComponent<MeshFilter>() : gameObject.AddComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>() ? GetComponent<MeshRenderer>() : gameObject.AddComponent<MeshRenderer>();
        mc = GetComponent<MeshCollider>() ? GetComponent<MeshCollider>() : gameObject.AddComponent<MeshCollider>();

        heights = new float[steps, steps];

        for(int x = 0; x < steps; x++)
        {
            for(int y = 0; y < steps; y++)
            {
                Vector3 pos = new Vector3((meshWidth / (steps - 1)) * x, 0f, (meshWidth / (steps - 1)) * y);
                pos = transform.TransformPoint(pos);

                Vector2 vPos = new Vector2(pos.x, pos.z);
                Vector2 noisePos = (vPos + noiseSettings.noiseOffset) * noiseSettings.noiseScale;

                heights[x, y] = Mathf.PerlinNoise(noisePos.x, noisePos.y) * noiseSettings.noiseAmplitude;
            }
        }

        Mesh m = new Mesh();

        Vector3[] vertices = new Vector3[steps * steps * 6];
        Color[] colors = new Color[vertices.Length];

        int[] triangles = new int[((steps - 1) * (steps - 1)) * 6];

        int index = 0;
        int triIndex = 0;

        for (int y = 0; y < steps; y++)
        {
            for (int x = 0; x < steps; x++)
            {
                if (x < steps - 1 && y < steps - 1)
                {
                    vertices[index] = new Vector3((meshWidth / (steps - 1)) * x, heights[x, y], (meshWidth / (steps - 1)) * y);
                    vertices[index+1] = new Vector3((meshWidth / (steps - 1)) * x, heights[x, y + 1], (meshWidth / (steps - 1)) * (y + 1));
                    vertices[index+2] = new Vector3((meshWidth / (steps - 1)) * (x + 1), heights[x + 1, y + 1], (meshWidth / (steps - 1)) * (y + 1));

                    colors[index] = meshColor;
                    colors[index+1] = meshColor;
                    colors[index+2] = meshColor;

                    vertices[index+3] = new Vector3((meshWidth / (steps - 1)) * x, heights[x, y], (meshWidth / (steps - 1)) * y);
                    vertices[index+4] = new Vector3((meshWidth / (steps - 1)) * (x + 1), heights[x + 1, y + 1], (meshWidth / (steps - 1)) * (y + 1));
                    vertices[index+5] = new Vector3((meshWidth / (steps - 1)) * (x + 1), heights[x + 1, y], (meshWidth / (steps - 1)) * y);

                    colors[index+3] = meshColor;
                    colors[index+4] = meshColor;
                    colors[index+5] = meshColor;

                    triangles[triIndex++] = index;
                    triangles[triIndex++] = index + 1;
                    triangles[triIndex++] = index + 2;

                    triangles[triIndex++] = index + 3;
                    triangles[triIndex++] = index + 4;
                    triangles[triIndex++] = index + 5;
                }

                index += 6;
            }
        }

        m.vertices = vertices;
        m.triangles = triangles;
        m.colors = colors;

        m.RecalculateNormals();

        mf.mesh = m;
        mc.sharedMesh = m;
    }
}

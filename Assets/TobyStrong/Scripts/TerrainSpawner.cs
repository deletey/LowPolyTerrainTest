using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public bool drawGizmos = true;

    public float poissonRadius = 1f;
    public int samplesBeforeRejection = 30;

    List<TerrainPoint> points;

    struct TerrainPoint
    {
        public Vector3 point;
        public Vector3 normal;
        public bool valid;
    }

    void OnValidate()
    {
        points = GetPoints();
    }
    
    void OnDrawGizmos()
    {
        if(!drawGizmos)
            return;

        Gizmos.color = Color.red;

        if(points == null)
            return;

        foreach(TerrainPoint point in points)
        {
            Gizmos.color = point.valid ? Color.green : Color.red;
            Gizmos.DrawSphere(point.point, 0.1f);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.TransformPoint(point.point), transform.TransformPoint(point.point + point.normal));
        }
    }

    void Start()
    {
        List<TerrainPoint> points = GetPoints();

        foreach(TerrainPoint point in points)
        {
            GameObject go = Instantiate(objectToSpawn, point.point, Quaternion.identity);
            go.tag = "Spawned";

            go.transform.parent = transform;
            go.transform.localPosition = point.point;
            go.transform.localEulerAngles = new Vector3(Random.Range(-2f, 2f), Random.Range(0f, 360f), Random.Range(-2f, 2f));
        }
    }

    TerrainPoint GetPointOnTerrain(Vector2 point)
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.TransformPoint(new Vector3(point.x, 100f, point.y)), Vector3.down, out hit, Mathf.Infinity, 1 << 9))
        {
            TerrainPoint terrainPoint = new TerrainPoint();
            terrainPoint.point = transform.InverseTransformPoint(hit.point);
            terrainPoint.normal = hit.normal;
            terrainPoint.valid = true;

            return terrainPoint;
        }
        else
        {
            TerrainPoint terrainPoint = new TerrainPoint();
            terrainPoint.valid = false;
            return terrainPoint;
        }
    }

    List<TerrainPoint> GetPoints()
    {
        List<Vector2> points = PoissonDiscSampling.GeneratePoints(poissonRadius, new Vector2(100f, 100f), samplesBeforeRejection);
        List<TerrainPoint> terrainPoints = new List<TerrainPoint>();

        foreach(Vector2 p in points)
        {
            TerrainPoint tp = GetPointOnTerrain(p);

            if(tp.valid)
                terrainPoints.Add(tp);
        }

        return terrainPoints;
    }
}

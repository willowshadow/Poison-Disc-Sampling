using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDisc : MonoBehaviour
{
    [Range(0.4f,5)]
    [SerializeField] public float radius;
    [Range(0f, 1f)]
    [SerializeField] float gizmoRadius;
    [SerializeField] public Vector2 regionSize=new Vector2(10,10);
    [SerializeField] List<Vector2> points;


    int[,] grid;

    private void OnValidate()
    {
        Generator();
    }
    [ContextMenu("Generate")]
    public void Generator()
    {
        points = GeneratePoints(radius, regionSize);
    }
    public static List<Vector2> GeneratePoints(float radius,Vector2 sampleRegionSize, int samplesBeforeRejection=30)
    {
        float cellSize = radius / Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize),Mathf.CeilToInt(sampleRegionSize.y / cellSize)];

        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>
        {
            sampleRegionSize / 2
        };

        while (spawnPoints.Count>0)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);

            Vector2 spawnCenter = spawnPoints[spawnIndex];

            bool isCandidateAccepted = false;
            for (int i = 0; i < samplesBeforeRejection; i++)
            {
                float angle = Random.value*Mathf.PI*2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCenter + dir * Random.Range(radius, radius * 2);

                if(IsValid(candidate,radius,cellSize,points,grid,sampleRegionSize))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    isCandidateAccepted = true;
                    //radius = Random.Range(0.4f, radius*1.2f);
                    break;
                }
            }
            if(!isCandidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }

        }
        return points;

    }
    static bool IsValid(Vector2 candidate,float radius, float cellSize, List<Vector2> points, int[,] grid, Vector2 sampleRegionSize)
    {
        if (candidate.x >= 0 && candidate.x < sampleRegionSize.x && candidate.y >= 0 && candidate.y < sampleRegionSize.y)
        {
            int CellX = (int)(candidate.x / cellSize);
            int CellY = (int)(candidate.y / cellSize);

            int searchStartX = Mathf.Max(0, CellX - 2);
            int searchStartY = Mathf.Max(0, CellY - 2);
            int searchEndX = Mathf.Min(CellX+2, grid.GetLength(0) - 1);
            int searchEndY = Mathf.Min(CellY+2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointInd = grid[x, y] - 1;
                    if(pointInd!=-1)
                    {
                        float sqrDist = (candidate - points[pointInd]).sqrMagnitude;
                        if (sqrDist < radius * radius)
                            return false;
                    }
                }
            }
            return true;
        }
        else return false;

    }
    private void OnDrawGizmos()
    {
        if (points == null)
            return;
        Gizmos.DrawWireCube(regionSize / 2, regionSize);
        foreach(var i in points)
        {
            Gizmos.DrawSphere(i, gizmoRadius);
        }
    }
}

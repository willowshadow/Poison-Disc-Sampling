using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public GameObject[] prefabs;
    public PoissonDisc sampler;
    [ContextMenu("Spawn Trees")]// Start is called before the first frame update
    public void Spawn()
    {
        Destroy(transform.GetChild(0).gameObject);
        var obj = new GameObject();
        obj.transform.SetParent(transform,false);
        StartCoroutine(nameof(SpawnCo));
    }
    public IEnumerator SpawnCo()
    {
        var points = PoissonDisc.GeneratePoints(sampler.radius,sampler.regionSize);
        for (int i = 0; i < points.Count; i++)
        {
            yield return new WaitForSecondsRealtime(0.02f);
            Vector2 p = points[i];
            Instantiate(prefabs[Random.Range(0, prefabs.Length)], new Vector3(p.x,p.y,0), Quaternion.Euler(Vector3.up*180),transform.GetChild(0));
        }
    }

    [ContextMenu("Delete")]// Update is called once per frame
    void Delete()
    {
        for (int i = 0;i < transform.GetChild(0).childCount;i++)
        {
            DestroyImmediate(transform.GetChild(0).GetChild(i).gameObject);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Spawn();
        }
    }
}

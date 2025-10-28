using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform centerTarget;

    int wave = 1;
    bool running;

    public void Begin()
    {
        if (!enemyPrefab || !centerTarget) { Debug.LogError("Spawner missing refs"); return; }
        running = true;
        wave = 1;
        StopAllCoroutines();
        StartCoroutine(SpawnLoop());
    }

    public void BumpWave()
    {
        wave++;
    }

    IEnumerator SpawnLoop()
    {
        while (running && GameManager.I.state == GameState.Playing)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(GameManager.I.CurrentSpawnInterval());
        }
    }

    void SpawnEnemy()
    {
        var pos = RandomEdgeWorldPoint();
        var go = Instantiate(enemyPrefab, pos, Quaternion.identity);
        var e = go.GetComponent<Enemy>();
        e.Init(centerTarget);
    }

    Vector3 RandomEdgeWorldPoint()
    {
        int edge = Random.Range(0, 4);
        float x=0,y=0;
        switch (edge)
        {
            case 0: x = 0f; y = Random.value; break;     // left
            case 1: x = 1f; y = Random.value; break;     // right
            case 2: x = Random.value; y = 0f; break;     // bottom
            case 3: x = Random.value; y = 1f; break;     // top
        }
        var cam = Camera.main;
        var w = cam.ViewportToWorldPoint(new Vector3(x, y, -cam.transform.position.z));
        w.z = 0f;
        return w;
    }
}


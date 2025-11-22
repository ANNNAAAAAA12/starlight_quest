using UnityEngine;

public class DiamondSpawner : MonoBehaviour
{
    public GameObject diamondPrefab;
    public Transform spawnPoint;
    bool spawned;

    public void Spawn()
    {
        if (spawned) return;
        if (diamondPrefab == null) return;
        var p = spawnPoint != null ? spawnPoint.position : transform.position;
        var r = spawnPoint != null ? spawnPoint.rotation : transform.rotation;
        Instantiate(diamondPrefab, p, r);
        spawned = true;
    }
}
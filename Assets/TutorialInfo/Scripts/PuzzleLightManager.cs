using UnityEngine;
using System.Collections.Generic;

public class PuzzleLightManager : MonoBehaviour
{
    public List<PuzzleLight> lights = new List<PuzzleLight>();
    public DiamondSpawner spawner;
    bool done;

    void Update()
    {
        if (done) return;
        if (lights.Count == 0) return;
        for (int i = 0; i < lights.Count; i++)
        {
            if (!lights[i].covered) return;
        }
        done = true;
        if (spawner != null) spawner.Spawn();
    }
}
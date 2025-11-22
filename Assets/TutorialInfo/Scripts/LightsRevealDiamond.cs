using UnityEngine;
using System.Collections.Generic;

public class LightsRevealDiamond : MonoBehaviour
{
    public List<LightCover> lights = new List<LightCover>();
    public GameObject diamondToReveal;
    public bool oneTime = true;
    bool revealed;

    void Update()
    {
        if (oneTime && revealed) return;
        if (lights.Count == 0) return;
        for (int i = 0; i < lights.Count; i++)
        {
            if (lights[i] == null || !lights[i].covered) return;
        }
        revealed = true;
        if (diamondToReveal != null)
            diamondToReveal.SetActive(true);
    }
}
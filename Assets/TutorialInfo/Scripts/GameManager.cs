using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int diamondsCollected;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AddDiamond()
    {
        diamondsCollected++;
    }

    public bool HasAllDiamonds(int required)
    {
        return diamondsCollected >= required;
    }
}
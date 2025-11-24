using UnityEngine;
using UnityEngine.UI;

public class DiamondsUI : MonoBehaviour
{
    public Text diamondsText;
    public int maxDiamonds = 4;
    GameManager gm;

    void Awake()
    {
        gm = GameManager.Instance;
        UpdateText();
    }

    void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        if (diamondsText == null) return;
        int cur = gm != null ? gm.diamondsCollected : 0;
        int max = Mathf.Max(cur, maxDiamonds);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < cur; i++) sb.Append('♦');
        for (int i = cur; i < max; i++) sb.Append('◇');
        diamondsText.text = sb.ToString();
    }
}


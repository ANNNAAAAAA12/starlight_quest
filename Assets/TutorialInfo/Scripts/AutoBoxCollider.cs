using UnityEngine;

[ExecuteAlways]
public class AutoBoxCollider : MonoBehaviour
{
    public bool includeChildren = true;
    public bool isTrigger = false;
    public string setLayerTo = "";

    BoxCollider collider;

    void EnsureCollider()
    {
        collider = GetComponent<BoxCollider>();
        if (collider == null) collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = isTrigger;
        if (!string.IsNullOrEmpty(setLayerTo))
        {
            int layer = LayerMask.NameToLayer(setLayerTo);
            if (layer != -1) gameObject.layer = layer;
        }
    }

    Bounds GetLocalBounds()
    {
        var renderers = includeChildren ? GetComponentsInChildren<Renderer>() : GetComponents<Renderer>();
        bool hasAny = false;
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;
        foreach (var r in renderers)
        {
            if (!r.enabled) continue;
            var b = r.bounds;
            var c = b.center;
            var e = b.extents;
            Vector3[] corners = new Vector3[8];
            corners[0] = transform.InverseTransformPoint(c + new Vector3( e.x,  e.y,  e.z));
            corners[1] = transform.InverseTransformPoint(c + new Vector3( e.x,  e.y, -e.z));
            corners[2] = transform.InverseTransformPoint(c + new Vector3( e.x, -e.y,  e.z));
            corners[3] = transform.InverseTransformPoint(c + new Vector3( e.x, -e.y, -e.z));
            corners[4] = transform.InverseTransformPoint(c + new Vector3(-e.x,  e.y,  e.z));
            corners[5] = transform.InverseTransformPoint(c + new Vector3(-e.x,  e.y, -e.z));
            corners[6] = transform.InverseTransformPoint(c + new Vector3(-e.x, -e.y,  e.z));
            corners[7] = transform.InverseTransformPoint(c + new Vector3(-e.x, -e.y, -e.z));
            for (int i = 0; i < 8; i++)
            {
                Vector3 p = corners[i];
                if (!hasAny)
                {
                    min = p;
                    max = p;
                    hasAny = true;
                }
                else
                {
                    min = Vector3.Min(min, p);
                    max = Vector3.Max(max, p);
                }
            }
        }
        Bounds localBounds = new Bounds((min + max) * 0.5f, max - min);
        return localBounds;
    }

    public void Fit()
    {
        EnsureCollider();
        var b = GetLocalBounds();
        collider.center = b.center;
        collider.size = b.size;
    }

    void OnEnable()
    {
        Fit();
    }

    void OnValidate()
    {
        Fit();
    }

    [ContextMenu("Fit BoxCollider")]
    void FitMenu()
    {
        Fit();
    }
}
using UnityEditor;
using UnityEngine;

public static class AutoColliderEditor
{
    [MenuItem("Tools/Colliders/Fit BoxCollider To Selected")]
    public static void FitSelected()
    {
        foreach (var t in Selection.transforms)
        {
            var comp = t.GetComponent<AutoBoxCollider>();
            if (comp == null) comp = t.gameObject.AddComponent<AutoBoxCollider>();
            comp.includeChildren = true;
            comp.Fit();
        }
    }
}
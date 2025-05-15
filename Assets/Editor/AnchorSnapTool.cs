using UnityEditor;
using UnityEngine;

public class AnchorSnapTool
{
    [MenuItem("UI Tools/Anchors Match RectTransform %#a")] // Ctrl + Shift + A
    private static void AnchorsToCorners()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            RectTransform t = obj.GetComponent<RectTransform>();
            if (t == null || t.parent == null) continue;

            RectTransform parent = t.parent as RectTransform;
            if (parent == null) continue;

            Vector2 newAnchorMin = new Vector2(
                t.anchorMin.x + t.offsetMin.x / parent.rect.width,
                t.anchorMin.y + t.offsetMin.y / parent.rect.height
            );

            Vector2 newAnchorMax = new Vector2(
                t.anchorMax.x + t.offsetMax.x / parent.rect.width,
                t.anchorMax.y + t.offsetMax.y / parent.rect.height
            );

            Undo.RecordObject(t, "Anchor Match");
            t.anchorMin = newAnchorMin;
            t.anchorMax = newAnchorMax;
            t.offsetMin = Vector2.zero;
            t.offsetMax = Vector2.zero;
        }
    }
}
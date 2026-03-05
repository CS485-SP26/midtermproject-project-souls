using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SDCurveAttribute))]
public class SDCurveDrawer : PropertyDrawer
{
    private const float GraphHeight = 150f;
    private int draggingPoint = -1;
    private int draggingSD = -1;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return GraphHeight + EditorGUIUtility.singleLineHeight + 10f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SDCurveAttribute attr = (SDCurveAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        // Label
        Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);

        // Graph area
        Rect graphRect = new Rect(position.x + 30f, position.y + EditorGUIUtility.singleLineHeight + 5f, position.width - 35f, GraphHeight);
        EditorGUI.DrawRect(graphRect, new Color(0.15f, 0.15f, 0.15f));

        // Get points array
        SerializedProperty pointsProp = property.FindPropertyRelative("points");
        if (pointsProp == null || pointsProp.arraySize == 0)
        {
            EditorGUI.LabelField(graphRect, "No points defined", EditorStyles.centeredGreyMiniLabel);
            EditorGUI.EndProperty();
            return;
        }

        DrawGrid(graphRect, attr);
        DrawCurve(graphRect, pointsProp, attr);
        DrawHandles(graphRect, pointsProp, attr);
        HandleInput(graphRect, pointsProp, attr);

        EditorGUI.EndProperty();
    }

    void DrawGrid(Rect graphRect, SDCurveAttribute attr)
    {
        Handles.color = new Color(0.3f, 0.3f, 0.3f);
        int steps = 4;
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            float y = Mathf.Lerp(graphRect.yMax, graphRect.yMin, t);
            float value = Mathf.Lerp(attr.minValue, attr.maxValue, t);
            Handles.DrawLine(new Vector3(graphRect.xMin, y), new Vector3(graphRect.xMax, y));
            EditorGUI.LabelField(new Rect(graphRect.xMin - 28f, y - 8f, 26f, 16f),
                value.ToString("F0"), EditorStyles.miniLabel);
        }

        // Season dividers
        Handles.color = new Color(0.4f, 0.4f, 0.4f);
        string[] labels = { attr.xLabel };
        for (int i = 1; i < 4; i++)
        {
            float x = graphRect.x + (i / 4f) * graphRect.width;
            Handles.DrawLine(new Vector3(x, graphRect.yMin), new Vector3(x, graphRect.yMax));
        }
    }

    void DrawCurve(Rect graphRect, SerializedProperty pointsProp, SDCurveAttribute attr)
    {
        int count = pointsProp.arraySize;
        int steps = 40;

        for (int i = 0; i < count; i++)
        {
            var p0 = pointsProp.GetArrayElementAtIndex(i);
            var p1 = pointsProp.GetArrayElementAtIndex((i + 1) % count);

            float t0start = p0.FindPropertyRelative("time").floatValue;
            float t1end   = p1.FindPropertyRelative("time").floatValue;
            if (t1end <= t0start) t1end = 1f;

            for (int s = 0; s < steps; s++)
            {
                float ta = s / (float)steps;
                float tb = (s + 1) / (float)steps;

                float mean0 = Mathf.Lerp(p0.FindPropertyRelative("mean").floatValue,   p1.FindPropertyRelative("mean").floatValue,   ta);
                float mean1 = Mathf.Lerp(p0.FindPropertyRelative("mean").floatValue,   p1.FindPropertyRelative("mean").floatValue,   tb);
                float sd0   = Mathf.Lerp(p0.FindPropertyRelative("stdDev").floatValue, p1.FindPropertyRelative("stdDev").floatValue, ta);
                float sd1   = Mathf.Lerp(p0.FindPropertyRelative("stdDev").floatValue, p1.FindPropertyRelative("stdDev").floatValue, tb);

                float xA = graphRect.x + Mathf.Lerp(t0start, t1end, ta) * graphRect.width;
                float xB = graphRect.x + Mathf.Lerp(t0start, t1end, tb) * graphRect.width;

                Vector3[] quad = {
                    new Vector3(xA, ValueToY(graphRect, attr, mean0 - sd0)),
                    new Vector3(xA, ValueToY(graphRect, attr, mean0 + sd0)),
                    new Vector3(xB, ValueToY(graphRect, attr, mean1 + sd1)),
                    new Vector3(xB, ValueToY(graphRect, attr, mean1 - sd1))
                };
                Handles.DrawSolidRectangleWithOutline(quad, new Color(0.2f, 0.5f, 1f, 0.15f), Color.clear);

                Handles.color = new Color(0.2f, 0.8f, 1f);
                Handles.DrawLine(
                    new Vector3(xA, ValueToY(graphRect, attr, mean0)),
                    new Vector3(xB, ValueToY(graphRect, attr, mean1))
                );
            }
        }
    }

    void DrawHandles(Rect graphRect, SerializedProperty pointsProp, SDCurveAttribute attr)
    {
        for (int i = 0; i < pointsProp.arraySize; i++)
        {
            var point = pointsProp.GetArrayElementAtIndex(i);
            float mean   = point.FindPropertyRelative("mean").floatValue;
            float stdDev = point.FindPropertyRelative("stdDev").floatValue;
            float time   = point.FindPropertyRelative("time").floatValue;

            Vector2 center = new Vector2(graphRect.x + time * graphRect.width, ValueToY(graphRect, attr, mean));
            Vector2 sdUp   = new Vector2(center.x, ValueToY(graphRect, attr, mean + stdDev));
            Vector2 sdDown = new Vector2(center.x, ValueToY(graphRect, attr, mean - stdDev));

            Handles.color = Color.white;
            Handles.DrawSolidDisc(center, Vector3.forward, 5f);
            Handles.color = new Color(0.2f, 0.5f, 1f);
            Handles.DrawSolidDisc(sdUp,   Vector3.forward, 4f);
            Handles.DrawSolidDisc(sdDown, Vector3.forward, 4f);
            Handles.DrawDottedLine(sdUp, sdDown, 4f);
        }
    }

    void HandleInput(Rect graphRect, SerializedProperty pointsProp, SDCurveAttribute attr)
    {
        Event e = Event.current;

        for (int i = 0; i < pointsProp.arraySize; i++)
        {
            var point  = pointsProp.GetArrayElementAtIndex(i);
            float mean   = point.FindPropertyRelative("mean").floatValue;
            float stdDev = point.FindPropertyRelative("stdDev").floatValue;
            float time   = point.FindPropertyRelative("time").floatValue;

            Vector2 center = new Vector2(graphRect.x + time * graphRect.width, ValueToY(graphRect, attr, mean));
            Vector2 sdUp   = new Vector2(center.x, ValueToY(graphRect, attr, mean + stdDev));
            Vector2 sdDown = new Vector2(center.x, ValueToY(graphRect, attr, mean - stdDev));

            if (e.type == EventType.MouseDown)
            {
                if (Vector2.Distance(e.mousePosition, center) < 8f) { draggingPoint = i; e.Use(); }
                else if (Vector2.Distance(e.mousePosition, sdUp)   < 6f) { draggingSD = i * 2;     e.Use(); }
                else if (Vector2.Distance(e.mousePosition, sdDown) < 6f) { draggingSD = i * 2 + 1; e.Use(); }
            }
        }

        if (e.type == EventType.MouseDrag)
        {
            if (draggingPoint >= 0)
            {
                var point = pointsProp.GetArrayElementAtIndex(draggingPoint);
                point.FindPropertyRelative("mean").floatValue = YToValue(graphRect, attr, e.mousePosition.y);
                pointsProp.serializedObject.ApplyModifiedProperties();
            }
            if (draggingSD >= 0)
            {
                int idx = draggingSD / 2;
                var point = pointsProp.GetArrayElementAtIndex(idx);
                float mean = point.FindPropertyRelative("mean").floatValue;
                float draggedValue = YToValue(graphRect, attr, e.mousePosition.y);
                point.FindPropertyRelative("stdDev").floatValue = Mathf.Abs(draggedValue - mean);
                pointsProp.serializedObject.ApplyModifiedProperties();
            }
        }

        if (e.type == EventType.MouseUp) { draggingPoint = -1; draggingSD = -1; }
    }

    float ValueToY(Rect r, SDCurveAttribute attr, float value) =>
        r.y + (1f - Mathf.InverseLerp(attr.minValue, attr.maxValue, value)) * r.height;

    float YToValue(Rect r, SDCurveAttribute attr, float y) =>
        Mathf.Lerp(attr.minValue, attr.maxValue, 1f - (y - r.y) / r.height);
}
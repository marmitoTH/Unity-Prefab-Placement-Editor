using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlacementSystem))]
[CanEditMultipleObjects]
public class PrefabPlacementEditor : Editor {

    private SerializedProperty prefab;
    private SerializedProperty layerMask;
    private SerializedProperty prefabTag;
    private SerializedProperty minRage, maxRage;
    private SerializedProperty randomizePrefab;
    private SerializedProperty toInstantiate;

    private SerializedProperty spread;
    private SerializedProperty radius;
    private SerializedProperty amount;
    private SerializedProperty size;
    private SerializedProperty positionOffset;

    private SerializedProperty canPlaceOver;
    private SerializedProperty canAling;
    private SerializedProperty isRandomS;
    private SerializedProperty isRandomR;
    private SerializedProperty hideInHierarchy;

    private Vector3 lastPos;
    private Vector3 mousePos;
    private Quaternion mouseRot;


    private void OnEnable()
    {
        prefab = serializedObject.FindProperty("prefab");
        layerMask = serializedObject.FindProperty("layerMask");
        prefabTag = serializedObject.FindProperty("prefabTag");
        minRage = serializedObject.FindProperty("minRage");
        maxRage = serializedObject.FindProperty("maxRage");
        randomizePrefab = serializedObject.FindProperty("randomizePrefab");
        toInstantiate = serializedObject.FindProperty ("toInstantiate");
        spread = serializedObject.FindProperty("spread");
        radius = serializedObject.FindProperty("radius");
        amount = serializedObject.FindProperty("amount");
        size = serializedObject.FindProperty("size");
        positionOffset = serializedObject.FindProperty("positionOffset");
        canPlaceOver = serializedObject.FindProperty("canPlaceOver");
        canAling = serializedObject.FindProperty("canAling");
        isRandomS = serializedObject.FindProperty("isRandomS");
        isRandomR = serializedObject.FindProperty("isRandomR");
        hideInHierarchy = serializedObject.FindProperty("hideInHierarchy");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
   
        EditorGUILayout.PropertyField(prefab, new GUIContent ("Prefab"), true);
        layerMask.intValue = EditorGUILayout.LayerField(new GUIContent("Layer Mask"), layerMask.intValue);
        prefabTag.stringValue = EditorGUILayout.TagField(new GUIContent("Prefab Tag"), prefabTag.stringValue);
        EditorGUILayout.PropertyField(randomizePrefab, new GUIContent ("Randomize Prefab"));

        if (!randomizePrefab.boolValue)
            EditorGUILayout.DelayedIntField(toInstantiate, new GUIContent("Prefab Index"));
        else
        {
            EditorGUILayout.DelayedIntField(minRage, new GUIContent ("minRage"));
            EditorGUILayout.DelayedIntField(maxRage, new GUIContent("maxRage"));
        }

        EditorGUILayout.PropertyField(canPlaceOver, new GUIContent ("Override"));
        EditorGUILayout.Slider(spread, 1, 10, new GUIContent("Spread"));
        EditorGUILayout.Slider(radius, 1, 10, new GUIContent ("Radius"));
        EditorGUILayout.IntSlider(amount, 1, 50, new GUIContent ("Amount"));
        EditorGUILayout.Slider(size, 0.1f, 10, new GUIContent ("Size"));
        EditorGUILayout.PropertyField(canAling, new GUIContent("Aling with normal"));
        EditorGUILayout.DelayedFloatField(positionOffset, new GUIContent("yOffset"));
        EditorGUILayout.PropertyField(isRandomS, new GUIContent("Randomize Size"));
        EditorGUILayout.PropertyField (isRandomR, new GUIContent ("Randomize Rotation"));
        EditorGUILayout.PropertyField(hideInHierarchy, new GUIContent ("Hide in Hierarchy"));

        serializedObject.ApplyModifiedProperties();
    }
    
    private void OnSceneGUI()
    {
        Event current = Event.current;
        int controlId = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);

        MousePosition();

        if ((current.type == EventType.MouseDrag || current.type == EventType.MouseDown) && !current.alt && prefab != null)
        {
            if (current.button == 0 && (lastPos == Vector3.zero || CanDraw()) && !current.shift)
            {
                lastPos = mousePos;
                for (int i = 0; i < amount.intValue; i++)
                {
                    if (randomizePrefab.boolValue)
                        PrefabInstantiate (Random.Range(minRage.intValue, maxRage.intValue + 1));
                    else
                        PrefabInstantiate (toInstantiate.intValue);
                }
            } else if (current.button == 0 && current.shift)
            {
                lastPos = mousePos;
                PrefabRemove();
            }
        }

        if (current.type == EventType.MouseUp)
            lastPos = Vector3.zero;

        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(controlId);
        }

        SceneView.RepaintAll();
    }

    public bool CanDraw ()
    { 
        float dist = Vector3.Distance(mousePos, lastPos);

        if (dist >= spread.floatValue / 2)
            return true;
        else
            return false;
    }

    public void MousePosition ()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, 1 << layerMask.intValue))
        {
            mousePos = hit.point;
            mouseRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Handles.color = Color.blue;
            Handles.DrawWireDisc(mousePos, hit.normal, radius.floatValue / 2);
        }
    }

    public void PrefabInstantiate (int index)
    {
        RaycastHit hit;
        GameObject instanceOf = PrefabUtility.InstantiatePrefab(prefab.GetArrayElementAtIndex(index).objectReferenceValue) as GameObject;
        Vector3 radiusAdjust = Random.insideUnitSphere * radius.floatValue / 2;
        float prefabSize = size.floatValue;
        
        if (hideInHierarchy.boolValue)
            instanceOf.hideFlags = HideFlags.HideInHierarchy;

        instanceOf.transform.localScale = new Vector3 (prefabSize, prefabSize, prefabSize);
        instanceOf.transform.position = new Vector3(mousePos.x, mousePos.y, mousePos.z);
        instanceOf.transform.rotation = new Quaternion (0, 0, 0, 0);

        if (canAling.boolValue)
            instanceOf.transform.rotation = mouseRot;
        else
            instanceOf.transform.rotation = new Quaternion(0, 0, 0, 0);

        if (amount.intValue > 1)
        {
            instanceOf.transform.Translate(radiusAdjust.x, positionOffset.floatValue, radiusAdjust.y);

            if (Physics.Raycast(instanceOf.transform.position, -instanceOf.transform.up, out hit))
            {
                if (!canPlaceOver.boolValue && hit.collider.tag == instanceOf.tag)
                {
                    DestroyImmediate(instanceOf);
                    return;
                }

                instanceOf.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);

                if (canAling.boolValue)
                    instanceOf.transform.up = hit.normal;
            }
            else
            {
                DestroyImmediate(instanceOf);
                return;
            }
        }

        if (isRandomR.boolValue)
            instanceOf.transform.Rotate(0, Random.Range(0, 180) * 45, 0);

        Undo.RegisterCreatedObjectUndo(instanceOf, "Instantiate");
    }

    private void PrefabRemove ()
    {
        GameObject[] prefabsInRadius;

        prefabsInRadius = GameObject.FindGameObjectsWithTag (prefabTag.stringValue);

        foreach (GameObject p in prefabsInRadius)
        {
            float dist = Vector3.Distance(mousePos, p.transform.position);

            if (dist <= radius.floatValue / 2)
                if (p != null)
                    Undo.DestroyObjectImmediate(p);
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EncircleMenu))]
public class EncircleMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EncircleMenu menuScript = (EncircleMenu)target;
        EditorGUILayout.TextArea("This button will update UI on all the submenus, everything will be rearanged based on Hierarchy. Only the objects with EncircleSubMenu components will be affected. It will add EncircleMenu component to EncircleSubmenus that have EncircleSubMenu in their children."
            , GUI.skin.GetStyle("HelpBox"));
        if (GUILayout.Button("Update UI Recursively"))
        {
            menuScript.UpdateUI();
        }

        EditorGUILayout.TextArea("This will add UI Dummy Objects to All SubMenus That their UI value is not set. It does not position them to the right place. Press Update UI Recursively to rearrange everything automatically."
    , GUI.skin.GetStyle("HelpBox"));
        if (GUILayout.Button("Generate UI Containers Recursively"))
        {
            menuScript.GenerateUIContainers();
        }

    }
}

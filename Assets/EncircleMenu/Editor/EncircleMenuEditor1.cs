using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EncircleSubMenu))]
public class EncircleSubMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EncircleSubMenu menuScript = (EncircleSubMenu)target;
        EditorGUILayout.TextArea("This Script should be attached to all of your sub menus. each sub menu can be a menu itself. You just have to worry about the hierarchy and the messages and their targets."+
                "You don't have to have a UI Object but it's encouraged.(don't move UI objects they should be placed automatically. Use them as a container.)"
            , GUI.skin.GetStyle("HelpBox"));
        DrawDefaultInspector();
    }
}

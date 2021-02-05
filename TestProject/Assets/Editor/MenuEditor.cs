using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MenuEditor : Editor {
    [MenuItem("Custom/Game Config")]
    static void DoSomething() {
        Selection.activeObject = GameConfig.Instance;
        EditorGUIUtility.PingObject(GameConfig.Instance);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine.Rendering;

public class SpriteFixerEditorWindow : EditorWindow
{
    // simple unity editor window
    [MenuItem("Tools/Sprite Fixer")]
    public static void ShowWindow()
    {
        GetWindow<SpriteFixerEditorWindow>("Sprite Fixer");
    }

    private void OnGUI()
    {
        // button that calls the sprite fixer method
        if (GUILayout.Button("Enable Spriterenderer Shadows"))
        {
            FindObjectsOfType<SpriteRenderer>().ForEach(spriteRenderer =>
            {
                spriteRenderer.shadowCastingMode = ShadowCastingMode.On;
                spriteRenderer.receiveShadows = true;
            });
        }
    }
}

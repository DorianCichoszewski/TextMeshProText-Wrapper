using TMPro;
using UnityEditor;
using UnityEngine;

namespace QDC.Text.Editor
{
    /// <summary>
    /// Creates inaccessible in hierarchy game object with TextMeshProUGUI component based on data in config.
    /// Uses created object's inspector GUI as its own. You can toggle between inspector and direct view using context menu.
    /// </summary>
    [CustomEditor(typeof(TextConfig))]
    public class TextConfigEditor : UnityEditor.Editor
    {
        private static bool showInspector = true;
        private TextConfig config;
        private TextMeshProUGUI previewComponent;
        private UnityEditor.Editor rbEditor;

        private void OnEnable()
        {
            config = (TextConfig)target;
            CreatePreview();
        }

        private void OnDisable()
        {
            Cleanup();
        }

        private new void Cleanup()
        {
            if (rbEditor != null)
                DestroyImmediate(rbEditor);
            if (previewComponent != null)
                DestroyImmediate(previewComponent.gameObject);
        }

        private void CreatePreview()
        {
            Cleanup();
            var previewInstance = new GameObject($"TMPro component preview for TextConfig: {name}", typeof(RectTransform), typeof(TextMeshProUGUI))
            {
                hideFlags = HideFlags.DontSave
            };
            previewComponent = previewInstance.GetComponent<TextMeshProUGUI>();
            JsonUtility.FromJsonOverwrite(config.JsonData, previewComponent);
            if (previewComponent != null)
                rbEditor = CreateEditor(previewComponent);
        }

        [MenuItem("CONTEXT/" + nameof(TextConfig) + "/Toggle Inspector View")]
        private static void ResetValue(MenuCommand menuCommand)
        {
            showInspector = !showInspector;
        }

        public override void OnInspectorGUI()
        {
            TextMeshProUGUI component = null;

            // Check if Buttons should work
            if (!Selection.activeGameObject)
            {
                GUI.enabled = false;
                EditorGUILayout.HelpBox("No GameObject selected", MessageType.None);
            }
            else if (!Selection.activeGameObject.TryGetComponent(out component))
            {
                GUI.enabled = false;
                EditorGUILayout.HelpBox("Selected GameObject has no TMPro-Text component.", MessageType.Info);
            }
            else
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button("Load from selected", EditorStyles.miniButton))
            {
                Undo.RecordObject(config, "Load Text Data");
                config.LoadFrom(component);
                CreatePreview();
            }

            if (GUILayout.Button("Apply to selected"))
            {
                Undo.RecordObject(component, "Apply Text Data");
                config.ApplyTo(component);
                EditorUtility.SetDirty(component);
            }

            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();

            if (showInspector)
            {
                if (rbEditor == null)
                {
                    EditorGUILayout.HelpBox("No Text data loaded", MessageType.Info);
                    return;
                }

                rbEditor.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    config.JsonData = EditorJsonUtility.ToJson(previewComponent, true);
                    EditorUtility.SetDirty(config);
                }
            }
            else
            {
                var text = GUILayout.TextArea(config.JsonData);
                if (EditorGUI.EndChangeCheck())
                {
                    config.JsonData = text;
                    EditorUtility.SetDirty(config);
                }
            }
        }
    }
}
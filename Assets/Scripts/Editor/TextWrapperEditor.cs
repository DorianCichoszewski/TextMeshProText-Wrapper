using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace QDC.Text.Editor
{
    [CustomEditor(typeof(TextWrapper), true)]
    public class TextWrapperEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var defaultInspector = new VisualElement();
            InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);
            root.Add(defaultInspector);

            var overrideSection = CreateOverrideSection();
            root.Add(overrideSection);

            return root;
        }

        private VisualElement CreateOverrideSection()
        {
            var container = new VisualElement();
            container.style.marginTop = 4;

            var textWrapper = (TextWrapper)target;
            var text = textWrapper.TextMeshPro;
            
            if (!text)
            {
                if (!textWrapper.Prefab)
                {
                    container.Add(new Label("Wrapper requires TMPro text (UI) to work"));
                    return container;
                }

                textWrapper.ResetText();
                text = textWrapper.TextMeshPro;
                if (!text)
                {
                    container.Add(new Label("Wrapper can't generate underlying text"));
                    return container;
                }
            }

            var modifications = PrefabUtility.GetPropertyModifications(text);
            List<PropertyModification> textModifications = new();

            foreach (var modification in modifications)
            {
                // Prefab refers to TextMeshProUGUI directly
                if (modification.target != textWrapper.Prefab)
                    continue;
                
                if (modification.propertyPath == "m_text")
                    continue;

                textModifications.Add(modification);
            }

            if (textModifications.Count == 0)
            {
                container.Add(new Label("No relevant overrides detected"));
                return container;
            }
            
            // Show overrides in Foldout
            var foldout = new Foldout
            {
                text = "Overrides",
                value = false
            };

            foreach (var modification in textModifications)
            {
                foldout.Add(CreateOverrideRow(modification));
            }
            
            container.Add(foldout);
            return container;
        }
        
        private VisualElement CreateOverrideRow(PropertyModification mod)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;
            row.style.paddingLeft = 8;
            row.style.paddingRight = 8;
            row.style.marginBottom = 2;

            var name = new Label(mod.propertyPath);
            name.style.unityTextAlign = TextAnchor.MiddleLeft;
            name.style.flexGrow = 1;

            var value = new Label(mod.value);
            value.style.unityTextAlign = TextAnchor.MiddleRight;

            row.Add(name);
            row.Add(value);

            return row;
        }
    }
}
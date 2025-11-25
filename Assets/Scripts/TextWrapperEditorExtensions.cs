#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;

namespace QDC.Text
{
    public static class TextWrapperEditorExtensions
    {
        public static void SetTextOverrides(this TextWrapper textWrapper)
        {
            if (!textWrapper || !textWrapper.Prefab || !textWrapper.TextMeshPro)
                return;

            var modifications = PrefabUtility.GetPropertyModifications(textWrapper.TextMeshPro);
            List<PropertyModification> textModifications = new();

            foreach (var modification in modifications)
            {
                // Prefab refers to TextMeshProUGUI directly
                if (modification.target != textWrapper.Prefab)
                    continue;

                if (modification.propertyPath == "m_Enabled")
                    continue;

                if (modification.propertyPath == "m_text")
                    continue;

                textModifications.Add(modification);
            }
            
            textWrapper.TextOverrides = textModifications.ToArray();
        }
    }
}
#endif
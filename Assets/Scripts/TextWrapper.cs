using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace QDC.Text
{
    [ExecuteAlways]
    [AddComponentMenu("UI/Text Wrapper")]
    public class TextWrapper : MonoBehaviour
    {
        [SerializeField] private string textTest;
        
        private TextMeshProUGUI tmpText;

        public TextMeshProUGUI TMPText => tmpText;

        private void Awake()
        {
            if (tmpText != null) return;
            
            var tmpGameObject = new GameObject(gameObject.name + "_TMP", typeof(RectTransform));
            var rectTransform = tmpGameObject.GetComponent<RectTransform>();
            GameObjectUtility.SetParentAndAlign(tmpGameObject, gameObject);
            rectTransform.SetAsFirstSibling();
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            tmpGameObject.hideFlags = HideFlags.DontSave;
            tmpText = tmpGameObject.AddComponent<TextMeshProUGUI>();
        }

        private void OnValidate()
        {
            if (tmpText == null) return;
            if (textTest != tmpText.text)
            {
                tmpText.text = textTest;
            }
        }

        private void OnDestroy()
        {
            // Destroy text if it exists
            try
            {
#if UNITY_EDITOR
                DestroyImmediate(tmpText.gameObject);
#else
                Destroy(tmpText.gameObject);
#endif
            }
            catch
            {
                // ignored
            }
        }
    }
}

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

        [SerializeField] private TextConfig textConfig;
        
        private TextMeshProUGUI tmpText;

        public TextMeshProUGUI TMPText => tmpText;
        
        public bool IsDataReady => textConfig != null;

        private void Awake()
        {
            CreateUnderlyingText();
        }

        private void OnEnable()
        {
            tmpText.enabled = true;
        }

        private void OnDisable()
        {
            tmpText.enabled = false;
        }

        private void OnValidate()
        {
            // If data is wrong, disable (hide) underlying Text
            if (tmpText && !IsDataReady)
                tmpText.enabled = false;
            
            // Do nothing if Data isn't ready
            if (!IsDataReady)
                return;
            
            if (!tmpText)
            {
                Debug.LogWarning($"Restoring text for wrapper {name}.\nUnderlying text shouldn't be removed individually.", gameObject);
                CreateUnderlyingText();
            }

            tmpText.enabled = enabled;
            
            textConfig.ApplyTo(tmpText);
            
            if (textTest != tmpText.text)
            {
                tmpText.text = textTest;
            }
        }
        
        private void OnDestroy()
        {
            DestroyText();
        }

        private void CreateUnderlyingText()
        {
            // Don't have to do anything
            if (tmpText) return;

            // Can't initialize if data isn't ready
            if (!IsDataReady)
            {
                Debug.LogError("Trying to show text without correct data", gameObject);
                return;
            }
            
            var tmpGameObject = new GameObject(gameObject.name + "_TMP", typeof(RectTransform), typeof(TextMeshProUGUI))
            {
#if SHOW_WRAPPED_TEXT
                hideFlags = HideFlags.DontSave | HideFlags.NotEditable
#else
                hideFlags = HideFlags.DontSave | HideFlags.NotEditable | HideFlags.HideInHierarchy
#endif
            };
            var rectTransform = tmpGameObject.GetComponent<RectTransform>();
            GameObjectUtility.SetParentAndAlign(tmpGameObject, gameObject);
            rectTransform.SetAsFirstSibling();
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            tmpText = tmpGameObject.GetComponent<TextMeshProUGUI>();
        }

        public void DestroyText()
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

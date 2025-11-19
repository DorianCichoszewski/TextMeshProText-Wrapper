using TMPro;
using UnityEngine;

namespace QDC.Text
{
    // Custom Editor: TextConfigEditor
    [CreateAssetMenu(menuName = "QDC/TextConfig")]
    public class TextConfig : ScriptableObject
    {
        [SerializeField] private string jsonData;

        public string JsonData
        {
            get { return jsonData; }
            set { jsonData = value; }
        }

        public void LoadFrom(TextMeshProUGUI source)
        {
            // Remove text temporarily, so that it won't be stored in config
            var originalText = source.text;
            source.text = string.Empty;
            jsonData = JsonUtility.ToJson(source, true);
            source.text = originalText;
        }

        public void ApplyTo(TextMeshProUGUI target)
        {
            
            if (target == null) return;
            var originalText = target.text;
            JsonUtility.FromJsonOverwrite(jsonData, target);
            target.text = originalText;
        }
    }
}
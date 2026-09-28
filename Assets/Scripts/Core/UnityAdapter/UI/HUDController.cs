using UnityEngine;
using UnityEngine.UI;
using Core.View.Dtos;

namespace UnityAdapter.UI
{
    public class HUDController : MonoBehaviour
    {
        // Paneles
        private GameObject _resourcePanel;
        private Text _woodText, _goldText, _stoneText, _metalText, _foodText;
        private Text _selectionText;

        // Referencia legacy pública por si es asignado por el SetupTool
        [HideInInspector] public Text resourceText;

        private bool _builtAtRuntime = false;

        void Awake()
        {
            BuildHUD();
        }

        private void BuildHUD()
        {
            // Panel superior de recursos
            _resourcePanel = new GameObject("ResourcePanel");
            _resourcePanel.transform.SetParent(transform, false);

            var panelImg = _resourcePanel.AddComponent<Image>();
            panelImg.color = new Color(0f, 0f, 0f, 0.7f);

            var panelRect = _resourcePanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.pivot = new Vector2(0.5f, 1);
            panelRect.anchoredPosition = new Vector2(0, 0);
            panelRect.sizeDelta = new Vector2(0, 36);

            // Fila de íconos y textos de recursos
            string[] labels = { "🌲 Mad:", "💰 Oro:", "🪨 Pie:", "⚙ Met:", "🍞 Com:" };
            float startX = -460f;
            float step = 190f;

            _woodText  = CreateResourceLabel(_resourcePanel, labels[0], new Vector2(startX + step * 0, 0));
            _goldText  = CreateResourceLabel(_resourcePanel, labels[1], new Vector2(startX + step * 1, 0));
            _stoneText = CreateResourceLabel(_resourcePanel, labels[2], new Vector2(startX + step * 2, 0));
            _metalText = CreateResourceLabel(_resourcePanel, labels[3], new Vector2(startX + step * 3, 0));
            _foodText  = CreateResourceLabel(_resourcePanel, labels[4], new Vector2(startX + step * 4, 0));

            // Texto de selección abajo
            GameObject selGo = new GameObject("SelectionText");
            selGo.transform.SetParent(transform, false);
            _selectionText = selGo.AddComponent<Text>();
            _selectionText.text = "";
            _selectionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _selectionText.color = Color.white;
            _selectionText.fontSize = 15;
            _selectionText.alignment = TextAnchor.LowerLeft;
            var selRect = selGo.GetComponent<RectTransform>();
            selRect.anchorMin = new Vector2(0, 0);
            selRect.anchorMax = new Vector2(0, 0);
            selRect.pivot = new Vector2(0, 0);
            selRect.anchoredPosition = new Vector2(10, 10);
            selRect.sizeDelta = new Vector2(400, 28);

            _builtAtRuntime = true;
        }

        private Text CreateResourceLabel(GameObject parent, string label, Vector2 pos)
        {
            GameObject go = new GameObject("Res_" + label);
            go.transform.SetParent(parent.transform, false);
            var text = go.AddComponent<Text>();
            text.text = label + " 0";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.color = Color.white;
            text.fontSize = 16;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleLeft;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(180, 30);
            return text;
        }

        public void UpdateResources(FactionResourcesRenderState res)
        {
            if (!_builtAtRuntime) return;
            if (_woodText)  _woodText.text  = $"🌲 Mad: {res.Wood}";
            if (_goldText)  _goldText.text  = $"💰 Oro: {res.Gold}";
            if (_stoneText) _stoneText.text = $"🪨 Pie: {res.Stone}";
            if (_metalText) _metalText.text = $"⚙ Met: {res.Metal}";
            if (_foodText)  _foodText.text  = $"🍞 Com: {res.Food}";
        }

        public void SetSelectionInfo(string info)
        {
            if (_selectionText != null)
                _selectionText.text = info;
        }
    }
}

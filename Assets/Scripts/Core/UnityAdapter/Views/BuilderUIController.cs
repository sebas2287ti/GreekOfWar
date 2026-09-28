using UnityEngine;
using UnityEngine.UI;
using Core.Controllers;
using UnityAdapter.Views;
using Core.Model.Enums;

public class BuilderUIController : MonoBehaviour
{
    private CoreGameController gameController;
    private GameObject buildOptionsPanel;
    private UnityUnitView selectedBuilder;

    void Start()
    {
        // Buscamos cualquier componente en la escena cuyo nombre de clase sea "GameBridgeAdapter"
        // Esto evita errores de namespaces o falta de using directives.
        foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mb.GetType().Name == "GameBridgeAdapter")
            {
                // Extraemos el CoreGameController mediante reflexión (propiedad o campo)
                var prop = mb.GetType().GetProperty("GameController");
                if (prop != null)
                {
                    gameController = (CoreGameController)prop.GetValue(mb);
                }
                else
                {
                    var field = mb.GetType().GetField("gameController");
                    if (field != null)
                    {
                        gameController = (CoreGameController)field.GetValue(mb);
                    }
                }

                if (gameController != null)
                {
                    Debug.Log("[BuilderUI] CoreGameController vinculado exitosamente a través de GameBridgeAdapter.");
                    break;
                }
            }
        }

        if (gameController == null)
        {
            Debug.LogError("[BuilderUI] ¡No se pudo extraer el CoreGameController del GameBridgeAdapter en la escena!");
        }
        
        // Generar la interfaz procedural
        CreateBuildUIProcedurally();
    }

    void Update()
    {
        // Detectar clic izquierdo del mouse para seleccionar unidades
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main == null) return;

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos);

            if (hitCollider != null)
            {
                UnityUnitView unitView = hitCollider.GetComponent<UnityUnitView>();
                
                if (unitView != null)
                {
                    // Verificamos si seleccionó un constructor de la facción 1 (Jugador)
                    if (unitView.Type == UnitType.Builder && unitView.FactionId == 1)
                    {
                        selectedBuilder = unitView;
                        if (buildOptionsPanel != null)
                        {
                            buildOptionsPanel.SetActive(true);
                            Debug.Log("[BuilderUI] ¡Panel inferior mostrado con éxito!");
                        }
                        return;
                    }
                }
            }
        }
    }

    private void CreateBuildUIProcedurally()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("GeneratedCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        buildOptionsPanel = new GameObject("BuildOptionsPanel");
        buildOptionsPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = buildOptionsPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 30f);
        panelRect.sizeDelta = new Vector2(400f, 90f);

        Image panelImage = buildOptionsPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        HorizontalLayoutGroup layout = buildOptionsPanel.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 20;
        layout.padding = new RectOffset(15, 15, 15, 15);
        layout.childControlWidth = true;
        layout.childControlHeight = true;

        CreateButton(buildOptionsPanel.transform, "Base (100 Oro)", () => OrderBuild(BuildingType.Base));
        CreateButton(buildOptionsPanel.transform, "Cuartel (50 Oro)", () => OrderBuild(BuildingType.Barracks));

        buildOptionsPanel.SetActive(false);
    }

    private Button CreateButton(Transform parent, string buttonText, UnityEngine.Events.UnityAction onClickAction)
    {
        GameObject btnObj = new GameObject("Btn_" + buttonText);
        btnObj.transform.SetParent(parent, false);

        LayoutElement layoutElement = btnObj.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 150f;
        layoutElement.preferredHeight = 50f;

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.5f, 0.8f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClickAction);

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = buttonText;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        return btn;
    }

    private void OrderBuild(BuildingType buildingType)
    {
        if (selectedBuilder == null) return;

        if (gameController == null)
        {
            Debug.LogError("[BuilderUI] El controlador del juego no está inicializado.");
            return;
        }

        float targetX = selectedBuilder.transform.position.x;
        float targetY = selectedBuilder.transform.position.y - 1.5f;

        bool success = gameController.OrderBuildStructure(selectedBuilder.UnitId, targetX, targetY, buildingType);

        if (success)
        {
            Debug.Log($"¡Orden de construcción enviada con éxito para: {buildingType}!");
            if (buildOptionsPanel != null)
                buildOptionsPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No se pudo construir (¿Faltan recursos / oro insuficiente?).");
        }
    }
}
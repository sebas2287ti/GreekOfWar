using UnityEngine;
using UnityEngine.UI;
using Core.Controllers;
using UnityAdapter.Views;
using Core.Model.Enums;
using System.Reflection;

public class BuilderUIController : MonoBehaviour
{
    [Header("Sprites de Botones UI")]
    [SerializeField] private Sprite btnBaseIcon;     
    [SerializeField] private Sprite btnBarracksIcon; 

    [Header("Sprites de Edificios en Mapa")]
    [SerializeField] private Sprite allyBaseSprite;     
    [SerializeField] private Sprite allyBarracksSprite; 
    [SerializeField] private Sprite enemyBaseSprite;    
    [SerializeField] private Sprite enemyBarracksSprite;

    private CoreGameController gameController;
    private GameObject buildOptionsPanel;
    private UnityUnitView selectedBuilder;

 
    private const int COST_BASE = 100;
    private const int COST_BARRACKS = 50;

    void Start()
    {
        foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mb.GetType().Name == "GameBridgeAdapter")
            {
                var componentType = mb.GetType();
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

                foreach (var prop in componentType.GetProperties(flags))
                {
                    if (prop.PropertyType == typeof(CoreGameController))
                    {
                        gameController = (CoreGameController)prop.GetValue(mb);
                        if (gameController != null) break;
                    }
                }

                if (gameController == null)
                {
                    foreach (var field in componentType.GetFields(flags))
                    {
                        if (field.FieldType == typeof(CoreGameController))
                        {
                            gameController = (CoreGameController)field.GetValue(mb);
                            if (gameController != null) break;
                        }
                    }
                }

                if (gameController != null) break;
            }
        }

        GiveInitialGoldIfNeeded();

        CreateVerticalBuilderUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Camera.main == null) return;

            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mouseWorldPos);

            if (hitCollider != null)
            {
                UnityUnitView unitView = hitCollider.GetComponent<UnityUnitView>();
                
                if (unitView != null && unitView.Type == UnitType.Builder && unitView.FactionId == 1)
                {
                    selectedBuilder = unitView;
                    if (buildOptionsPanel != null)
                    {
                        buildOptionsPanel.SetActive(true);
                    }
                    return;
                }
            }
        }
    }

    private void GiveInitialGoldIfNeeded()
    {
        if (gameController == null) return;
        var controllerType = gameController.GetType();
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        foreach (var prop in controllerType.GetProperties(flags))
        {
            if (prop.PropertyType == typeof(int) && (prop.Name.Equals("Gold", System.StringComparison.OrdinalIgnoreCase) || prop.Name.Equals("PlayerGold", System.StringComparison.OrdinalIgnoreCase)))
            {
                if (prop.CanWrite && (int)prop.GetValue(gameController) == 0)
                {
                    prop.SetValue(gameController, 200); // Les damos 200 de oro inicial para empezar a construir
                    Debug.Log("[BuilderUI] Se han otorgado 200 de oro iniciales para pruebas.");
                }
                return;
            }
        }

        foreach (var field in controllerType.GetFields(flags))
        {
            if (field.FieldType == typeof(int) && (field.Name.Equals("Gold", System.StringComparison.OrdinalIgnoreCase) || field.Name.Equals("PlayerGold", System.StringComparison.OrdinalIgnoreCase)))
            {
                if ((int)field.GetValue(gameController) == 0)
                {
                    field.SetValue(gameController, 200);
                    Debug.Log("[BuilderUI] Se han otorgado 200 de oro iniciales en campo para pruebas.");
                }
                return;
            }
        }
    }

    private void CreateVerticalBuilderUI()
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

        buildOptionsPanel = new GameObject("BuildOptionsPanel_Vertical");
        buildOptionsPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = buildOptionsPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0.5f);
        panelRect.anchorMax = new Vector2(0f, 0.5f);
        panelRect.pivot = new Vector2(0f, 0.5f);
        panelRect.anchoredPosition = new Vector2(20f, 0f);
        panelRect.sizeDelta = new Vector2(70f, 160f);

        Image panelImage = buildOptionsPanel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        VerticalLayoutGroup vLayout = buildOptionsPanel.AddComponent<VerticalLayoutGroup>();
        vLayout.childAlignment = TextAnchor.MiddleCenter;
        vLayout.spacing = 15;
        vLayout.padding = new RectOffset(10, 10, 15, 15);
        vLayout.childControlWidth = true;
        vLayout.childControlHeight = true;

        CreateSmallButton(buildOptionsPanel.transform, btnBaseIcon, () => {
            TryOrderBuild(BuildingType.Base, COST_BASE);
        });

        CreateSmallButton(buildOptionsPanel.transform, btnBarracksIcon, () => {
            TryOrderBuild(BuildingType.Barracks, COST_BARRACKS);
        });

        buildOptionsPanel.SetActive(false);
    }

    private Button CreateSmallButton(Transform parent, Sprite iconSprite, UnityEngine.Events.UnityAction onClickAction)
    {
        GameObject btnObj = new GameObject("Btn_BuildingOption");
        btnObj.transform.SetParent(parent, false);

        LayoutElement layoutElement = btnObj.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = 45f;
        layoutElement.preferredHeight = 45f;

        Image img = btnObj.AddComponent<Image>();
        if (iconSprite != null)
        {
            img.sprite = iconSprite;
            img.color = Color.white;
        }
        else
        {
            img.color = new Color(0.3f, 0.3f, 0.3f);
        }

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClickAction);

        return btn;
    }

    private void TryOrderBuild(BuildingType buildingType, int cost)
    {
        if (selectedBuilder == null)
        {
            Debug.LogWarning("[BuilderUI] Ningún constructor seleccionado.");
            return;
        }

        if (gameController == null)
        {
            Debug.LogError("[BuilderUI] CoreGameController es nulo.");
            return;
        }

        int currentGold = GetCurrentGold();
        if (currentGold < cost)
        {
            Debug.LogWarning($"[BuilderUI] ¡Oro insuficiente! Necesitas {cost} de oro, pero solo tienes {currentGold}.");
            return;
        }

        float targetX = selectedBuilder.transform.position.x;
        float targetY = selectedBuilder.transform.position.y - 1.5f;

        bool success = gameController.OrderBuildStructure(selectedBuilder.UnitId, targetX, targetY, buildingType);

        if (success)
        {
            Debug.Log($"¡Construcción exitosa de {buildingType}! Se descontaron {cost} de oro.");
            SpawnBuildingVisual(buildingType, targetX, targetY, selectedBuilder.FactionId);

            if (buildOptionsPanel != null)
                buildOptionsPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[BuilderUI] El backend rechazó la orden de construir '{buildingType}'. (Posible posición bloqueada)");
        }
    }

    private int GetCurrentGold()
    {
        if (gameController == null) return 0;
        var controllerType = gameController.GetType();
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        var factionsProp = controllerType.GetProperty("Factions", flags);
        if (factionsProp != null)
        {
            var factionsDict = factionsProp.GetValue(gameController);
            if (factionsDict != null)
            {
                var tryGetMethod = factionsDict.GetType().GetMethod("TryGetValue");
                if (tryGetMethod != null)
                {
                    object[] args = new object[] { 1, null };
                    bool found = (bool)tryGetMethod.Invoke(factionsDict, args);
                    if (found && args[1] != null)
                    {
                        var factionRes = args[1];
                        var goldProp = factionRes.GetType().GetProperty("Gold");
                        if (goldProp != null)
                        {
                            float goldFloat = System.Convert.ToSingle(goldProp.GetValue(factionRes));
                            return Mathf.FloorToInt(goldFloat);
                        }
                    }
                }
            }
        }
        return 0;
    }



    private void SpawnBuildingVisual(BuildingType buildingType, float x, float y, int factionId)
    {
        Sprite spriteToAssign = null;
        string objName = "";
        bool isAlly = (factionId == 1);

        if (buildingType == BuildingType.Base)
        {
            spriteToAssign = isAlly ? allyBaseSprite : enemyBaseSprite;
            objName = isAlly ? "Building_Base_Ally" : "Building_Base_Enemy";
        }
        else if (buildingType == BuildingType.Barracks)
        {
            spriteToAssign = isAlly ? allyBarracksSprite : enemyBarracksSprite;
            objName = isAlly ? "Building_Barracks_Ally" : "Building_Barracks_Enemy";
        }

        GameObject buildingObj = new GameObject(objName);
        buildingObj.transform.position = new Vector3(x, y, 0f);
        buildingObj.transform.localScale = Vector3.one; 

        SpriteRenderer sr = buildingObj.AddComponent<SpriteRenderer>();
        if (spriteToAssign != null)
        {
            sr.sprite = spriteToAssign;
            sr.sortingLayerName = "Default"; 
            sr.sortingOrder = 10; 
        }
        else
        {
            Debug.LogWarning($"[BuilderUI] El sprite para {buildingType} es NULO.");
        }

        buildingObj.AddComponent<BoxCollider2D>();
    }
}
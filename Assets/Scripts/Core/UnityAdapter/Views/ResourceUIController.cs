using UnityEngine;
using UnityEngine.UI;
using Core.Controllers;
using System.Reflection;

public class ResourceUIController : MonoBehaviour
{
    private CoreGameController gameController;
    private Text resourceTextLabel;

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

        CreateResourceHUD();
    }

    void Update()
    {
        UpdateResourceDisplay();
    }

    private void CreateResourceHUD()
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

        GameObject hudObj = new GameObject("ResourceHUD");
        hudObj.transform.SetParent(canvas.transform, false);

        RectTransform hudRect = hudObj.AddComponent<RectTransform>();
        hudRect.anchorMin = new Vector2(0f, 1f);
        hudRect.anchorMax = new Vector2(0f, 1f);
        hudRect.pivot = new Vector2(0f, 1f);
        hudRect.anchoredPosition = new Vector2(20f, -50f); 
        hudRect.sizeDelta = new Vector2(160f, 35f);

        Image hudImage = hudObj.AddComponent<Image>();
        hudImage.color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        GameObject textObj = new GameObject("ResourceText");
        textObj.transform.SetParent(hudObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        resourceTextLabel = textObj.AddComponent<Text>();
        resourceTextLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        resourceTextLabel.fontSize = 14;
        resourceTextLabel.alignment = TextAnchor.MiddleCenter;
        resourceTextLabel.color = Color.yellow;
        resourceTextLabel.text = "🪙 Oro: 0";
    }

    private void UpdateResourceDisplay()
    {
        if (gameController == null || resourceTextLabel == null) return;

        float currentGold = 0f;
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
                            currentGold = System.Convert.ToSingle(goldProp.GetValue(factionRes));
                        }
                    }
                }
            }
        }

        resourceTextLabel.text = $"🪙 Oro: {Mathf.FloorToInt(currentGold)}";
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameDebugConsole : MonoBehaviour
{
    [Header("Configuración")]
    public KeyCode toggleKey = KeyCode.F1;
    public int maxLines = 30;
    public Font font;
    public int fontSize = 14;

    private Text consoleText;
    private ScrollRect scrollRect;
    private GameObject consolePanel;

    private Queue<string> logLines = new Queue<string>();
    private bool isVisible = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.logMessageReceived += HandleLog;

        CreateConsoleUI();
        consolePanel.SetActive(isVisible);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isVisible = !isVisible;
            consolePanel.SetActive(isVisible);
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (consoleText == null) return;  // Evita el error si fue destruido

        if (logLines.Count > maxLines)
            logLines.Dequeue();

        logLines.Enqueue(logString);

        consoleText.text = string.Join("\n", logLines.ToArray());
        LayoutRebuilder.ForceRebuildLayoutImmediate(consoleText.rectTransform);

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
    }


    private void CreateConsoleUI()
    {
        // Panel root
        consolePanel = new GameObject("DebugConsole");
        consolePanel.transform.SetParent(this.transform);  // Hijo del GameManager
        var canvas = consolePanel.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        consolePanel.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        consolePanel.AddComponent<GraphicRaycaster>();

        // Background
        var background = new GameObject("Background", typeof(Image));
        background.transform.SetParent(consolePanel.transform);
        var bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.25f, 0.25f);
        bgRect.anchorMax = new Vector2(0.75f, 0.75f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        background.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);

        // Viewport
        var viewportGO = new GameObject("Viewport", typeof(RectMask2D), typeof(Image));
        viewportGO.transform.SetParent(background.transform);
        var viewportRect = viewportGO.GetComponent<RectTransform>();
        viewportRect.anchorMin = new Vector2(0, 0);
        viewportRect.anchorMax = new Vector2(1, 1);
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;
        viewportGO.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        // Scroll View
        var scrollGO = new GameObject("ScrollView", typeof(ScrollRect));
        scrollGO.transform.SetParent(background.transform);
        var scrollRectTransform = scrollGO.GetComponent<RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0, 0);
        scrollRectTransform.anchorMax = new Vector2(1, 1);
        scrollRectTransform.offsetMin = Vector2.zero;
        scrollRectTransform.offsetMax = Vector2.zero;

        scrollRect = scrollGO.GetComponent<ScrollRect>();
        scrollRect.viewport = viewportRect;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.vertical = true;
        scrollRect.horizontal = false;

        // Content inside Viewport
        var contentGO = new GameObject("Content", typeof(Text));
        contentGO.transform.SetParent(viewportGO.transform);
        var contentRect = contentGO.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        consoleText = contentGO.GetComponent<Text>();
        consoleText.font = font ? font : Resources.GetBuiltinResource<Font>("Arial.ttf");
        consoleText.fontSize = fontSize;
        consoleText.color = Color.white;
        consoleText.alignment = TextAnchor.UpperLeft;
        consoleText.horizontalOverflow = HorizontalWrapMode.Wrap;
        consoleText.verticalOverflow = VerticalWrapMode.Overflow;

        scrollRect.content = contentRect;

    }
}

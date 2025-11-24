using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UIAutoBuilder
{
    [MenuItem("Tools/UI/Generate Menu UI In Current Scene")]
    public static void GenerateMenuUI()
    {
        EnsureEventSystem();
        var canvas = CreateCanvas("MainMenuCanvas");
        var cComp = canvas.GetComponent<Canvas>();
        cComp.sortingOrder = 1000;
        var loaderGO = new GameObject("MainMenuController");
        loaderGO.transform.SetParent(canvas.transform, false);
        var loader = loaderGO.AddComponent<SceneLoader>();
        var menu = loaderGO.AddComponent<MainMenuController>();
        menu.gameSceneName = SceneManager.GetActiveScene().name;
        menu.overlayMode = true;
        menu.rootCanvas = canvas;

        var panel = CreatePanel(canvas.transform, "MenuPanel");
        var startBtn = CreateButton(panel.transform, "StartButton", new Vector2(0, 60), "Start");
        var quitBtn = CreateButton(panel.transform, "QuitButton", new Vector2(0, -60), "Quit");
        UnityEventTools.AddPersistentListener(startBtn.onClick, menu.StartGame);
        UnityEventTools.AddPersistentListener(quitBtn.onClick, menu.QuitGame);
    }

    [MenuItem("Tools/UI/Generate Pause UI In Current Scene")]
    public static void GeneratePauseUI()
    {
        EnsureEventSystem();
        var canvas = CreateCanvas("PauseCanvas");
        var cComp = canvas.GetComponent<Canvas>();
        cComp.sortingOrder = 900;
        var ctrlGO = new GameObject("PauseController");
        ctrlGO.transform.SetParent(canvas.transform, false);
        var loader = ctrlGO.AddComponent<SceneLoader>();
        var pause = ctrlGO.AddComponent<PauseController>();

        var panel = CreatePanel(canvas.transform, "PausePanel");
        pause.pausePanel = panel;
        var resumeBtn = CreateButton(panel.transform, "ResumeButton", new Vector2(0, 80), "Resume");
        var restartBtn = CreateButton(panel.transform, "RestartButton", new Vector2(0, 0), "Restart");
        var menuBtn = CreateButton(panel.transform, "MenuButton", new Vector2(0, -80), "Main Menu");
        UnityEventTools.AddPersistentListener(resumeBtn.onClick, pause.Resume);
        UnityEventTools.AddPersistentListener(restartBtn.onClick, pause.RestartLevel);
        UnityEventTools.AddPersistentListener(menuBtn.onClick, pause.GoToMainMenu);
        panel.SetActive(false);
    }

    [MenuItem("Tools/UI/Generate End UI In Current Scene")]
    public static void GenerateEndUI()
    {
        EnsureEventSystem();
        var canvas = CreateCanvas("EndCanvas");
        var cComp = canvas.GetComponent<Canvas>();
        cComp.sortingOrder = 900;
        var ctrlGO = new GameObject("EndGameController");
        ctrlGO.transform.SetParent(canvas.transform, false);
        var loader = ctrlGO.AddComponent<SceneLoader>();
        var end = ctrlGO.AddComponent<EndGameController>();
        end.requiredDiamonds = 4;
        end.autoShowOnDiamonds = true;

        var panel = CreatePanel(canvas.transform, "EndPanel");
        end.endPanel = panel;
        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        var titleText = titleGO.AddComponent<Text>();
        titleText.text = "Mission Complete";
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        var titleRt = titleGO.GetComponent<RectTransform>();
        titleRt.sizeDelta = new Vector2(480, 60);
        titleRt.anchoredPosition = new Vector2(0, 120);

        var menuBtn = CreateButton(panel.transform, "MenuButton", new Vector2(0, 40), "Main Menu");
        var quitBtn = CreateButton(panel.transform, "QuitButton", new Vector2(0, -40), "Quit");
        UnityEventTools.AddPersistentListener(menuBtn.onClick, end.GoToMainMenu);
        UnityEventTools.AddPersistentListener(quitBtn.onClick, end.QuitGame);
        panel.SetActive(false);
    }

    [MenuItem("Tools/UI/Generate All UI In Current Scene")]
    public static void GenerateAll()
    {
        GenerateMenuUI();
        GeneratePauseUI();
        GenerateEndUI();
        GenerateHeartsHUD();
        GenerateDeathUI();
        GenerateDiamondsHUD();
    }

    static GameObject CreateCanvas(string name)
    {
        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    static GameObject CreatePanel(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.6f);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(600, 360);
        rt.anchoredPosition = Vector2.zero;
        return go;
    }

    static Button CreateButton(Transform parent, string name, Vector2 pos, string label)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        var btn = go.AddComponent<Button>();
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(260, 50);
        rt.anchoredPosition = pos;
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var txt = textGO.AddComponent<Text>();
        txt.text = label;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        var trt = textGO.GetComponent<RectTransform>();
        trt.sizeDelta = new Vector2(240, 40);
        trt.anchoredPosition = Vector2.zero;
        return btn;
    }

    static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null) return;
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    [MenuItem("Tools/UI/Generate Hearts HUD In Current Scene")]
    public static void GenerateHeartsHUD()
    {
        EnsureEventSystem();
        var canvas = CreateCanvas("HUDCanvas");
        var cComp = canvas.GetComponent<Canvas>();
        cComp.sortingOrder = 800;

        var textGO = new GameObject("HeartsText");
        textGO.transform.SetParent(canvas.transform, false);
        var t = textGO.AddComponent<Text>();
        t.text = "♥♥♥";
        t.alignment = TextAnchor.UpperLeft;
        t.color = Color.white;
        var rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = new Vector2(16, -16);
        rt.sizeDelta = new Vector2(300, 60);

        var heartsGO = new GameObject("HeartsUI");
        heartsGO.transform.SetParent(canvas.transform, false);
        var ui = heartsGO.AddComponent<HeartsUI>();
        var ph = Object.FindObjectOfType<PlayerHealth>();
        ui.health = ph;
        ui.heartsText = t;
    }

    [MenuItem("Tools/UI/Generate Death UI In Current Scene")]
    public static void GenerateDeathUI()
    {
        EnsureEventSystem();
        var canvas = CreateCanvas("DeathCanvas");
        var cComp = canvas.GetComponent<Canvas>();
        cComp.sortingOrder = 950;

        var ctrlGO = new GameObject("DeathController");
        ctrlGO.transform.SetParent(canvas.transform, false);
        var loader = ctrlGO.AddComponent<SceneLoader>();
        var dc = ctrlGO.AddComponent<DeathPanelController>();
        var ph = Object.FindObjectOfType<PlayerHealth>();
        dc.health = ph;
        dc.loader = loader;

        var panel = CreatePanel(canvas.transform, "DeathPanel");
        dc.deathPanel = panel;

        var titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        var titleText = titleGO.AddComponent<Text>();
        titleText.text = "Perdiste";
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        var titleRt = titleGO.GetComponent<RectTransform>();
        titleRt.sizeDelta = new Vector2(480, 60);
        titleRt.anchoredPosition = new Vector2(0, 40);
        panel.SetActive(false);
    }

    [MenuItem("Tools/UI/Generate Diamonds HUD In Current Scene")]
    public static void GenerateDiamondsHUD()
    {
        EnsureEventSystem();
        var canvas = CreateCanvas("DiamondsHUDCanvas");
        var cComp = canvas.GetComponent<Canvas>();
        cComp.sortingOrder = 800;

        var textGO = new GameObject("DiamondsText");
        textGO.transform.SetParent(canvas.transform, false);
        var t = textGO.AddComponent<Text>();
        t.text = "◇◇◇◇";
        t.alignment = TextAnchor.UpperRight;
        t.color = Color.white;
        var rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-16, -16);
        rt.sizeDelta = new Vector2(300, 60);

        var uiGO = new GameObject("DiamondsUI");
        uiGO.transform.SetParent(canvas.transform, false);
        var ui = uiGO.AddComponent<DiamondsUI>();
        ui.diamondsText = t;
        ui.maxDiamonds = 4;
    }
}

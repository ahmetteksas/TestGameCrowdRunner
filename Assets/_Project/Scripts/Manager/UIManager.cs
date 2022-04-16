using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Variables
    [Header("GameData")]
    [SerializeField] private GameData data;
    [Header("Panels")]
    [SerializeField] private GameObject splashPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject tutorialPanel;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI levelText;


    private bool justOnce = true;
    #endregion

    private void OnEnable()
    {
        EventManager.updateLevelText += UpdateLevelText;
        EventManager.OnLevelEnd += TriggerLevelEndCanvas;
        EventManager.OnTriggerFail += TriggerLevelFailed;
    }

    private void OnDisable()
    {
        EventManager.updateLevelText -= UpdateLevelText;
        EventManager.OnLevelEnd -= TriggerLevelEndCanvas;
        EventManager.OnTriggerFail -= TriggerLevelFailed;
    }

    private void Start()
    {
        ArrangeFirstAppearance();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && justOnce)
        {
            StartButtonPressed();
            justOnce = false;
        }
    }

    private void ArrangeFirstAppearance()
    {
        UpdateLevelText();
        SaveManager.LoadData(data);
        CloseAllPanels();
        bool loadingScene = EventManager.checkingSceneType.Invoke();
        if (loadingScene)
        {
            splashPanel.SetActive(true);
        }
        else if (!loadingScene)
        {
            gamePanel.SetActive(true);
            tutorialPanel.SetActive(true);
        }
    }

    private void CloseAllPanels()
    {
        splashPanel.SetActive(false);
        gamePanel.SetActive(false);
        winPanel.SetActive(false);
        failPanel.SetActive(false);
        tutorialPanel.SetActive(false);
    }

    private void TriggerLevelEndCanvas()
    {
        winPanel.SetActive(true);
    }

    private void UpdateLevelText()
    {
        levelText.text = data.levelValue.ToString();
    }

    public void TryAgainButtonPressed()
    {
        EventManager.loadSameScene.Invoke();
    }

    public void NextLevelButtonPressed()
    {
        EventManager.loadNextScene.Invoke();

    }

    private void TriggerLevelFailed()
    {
        failPanel.SetActive(true);
    }

    public void StartButtonPressed()
    {
        EventManager.OnStartButton.Invoke();
        tutorialPanel.SetActive(false);
    }
}

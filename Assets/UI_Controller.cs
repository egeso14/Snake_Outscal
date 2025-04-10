using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUiElements
{
    public GameObject scoreText;
    public GameObject powerUpPanel;
    public PowerUpPanelContainer shieldPanel;
    public PowerUpPanelContainer scoreBoostPanel;
    public PowerUpPanelContainer speedUpPanel;
}

public class PowerUpPanelContainer 
{
    public GameObject panel;
    public TextMeshProUGUI text;
    public string name;
    public float timer;
}

public class UI_Controller : MonoBehaviour
{
    public static UI_Controller instance;
    [SerializeField] private TextMeshProUGUI blueScoreText;
    [SerializeField] private TextMeshProUGUI greenScoreText;
    
    [SerializeField] private GameObject bluePowerUpPanel;
    [SerializeField] private GameObject blueShieldPanel;
    [SerializeField] private GameObject blueShieldText;
    [SerializeField] private GameObject blueScoreBoostPanel;
    [SerializeField] private GameObject blueScoreBoostText;
    [SerializeField] private GameObject blueSpeedUpPanel;
    [SerializeField] private GameObject blueSpeedUpText;

    [SerializeField] private GameObject greenPowerUpPanel;
    [SerializeField] private GameObject greenShieldPanel;
    [SerializeField] private GameObject greenShieldText;
    [SerializeField] private GameObject greenScoreBoostPanel;
    [SerializeField] private GameObject greenScoreBoostText;
    [SerializeField] private GameObject greenSpeedUpPanel;
    [SerializeField] private GameObject greenSpeedUpText;

    private PlayerUiElements greenPlayerUiElements;
    private PlayerUiElements bluePlayerUiElements;

    private List<PowerUpPanelContainer> activeTimers;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    void Start()
    {
        activeTimers = new List<PowerUpPanelContainer>();

        greenPlayerUiElements = new PlayerUiElements()
        {
            scoreText = greenScoreText.gameObject,
            powerUpPanel = greenPowerUpPanel,
            shieldPanel = new PowerUpPanelContainer
            {
                panel = greenShieldPanel,
                text = greenShieldText.GetComponent<TextMeshProUGUI>(),
                name = "Shield: ",
                timer = 0f
            },
            scoreBoostPanel = new PowerUpPanelContainer
            {
                panel = greenScoreBoostPanel,
                text = greenScoreBoostText.GetComponent<TextMeshProUGUI>(),
                name = "ScoreBoost: ",
                timer = 0f
            },
            speedUpPanel = new PowerUpPanelContainer
            {
                panel = greenSpeedUpPanel,
                text = greenSpeedUpText.GetComponent<TextMeshProUGUI>(),
                name = "SpeedUp: ",
                timer = 0f
            }
        };

        bluePlayerUiElements = new PlayerUiElements()
        {
            scoreText = blueScoreText.gameObject,
            powerUpPanel = bluePowerUpPanel,
            shieldPanel = new PowerUpPanelContainer
            {
                panel = blueShieldPanel,
                text = blueShieldText.GetComponent<TextMeshProUGUI>(),
                name = "Shield: ",
                timer = 0f
            },
            scoreBoostPanel = new PowerUpPanelContainer
            {
                panel = blueScoreBoostPanel,
                text = blueScoreBoostText.GetComponent<TextMeshProUGUI>(),
                name = "ScoreBoost: ",
                timer = 0f
            },
            speedUpPanel = new PowerUpPanelContainer
            {
                panel = blueSpeedUpPanel,
                text = blueSpeedUpText.GetComponent<TextMeshProUGUI>(),
                name = "SpeedUp: ",
                timer = 0f
            }
        };

        greenShieldPanel.SetActive(false);
        greenScoreBoostPanel.SetActive(false);
        greenSpeedUpPanel.SetActive(false);

        blueShieldPanel.SetActive(false);
        blueScoreBoostPanel.SetActive(false);
        blueSpeedUpPanel.SetActive(false);
    }

        

    // Update is called once per frame
    void FixedUpdate()
    {
        TickTimers();
    }

    public void UpdateScore(E_SnakeColor color, int score)
    {
        TextMeshProUGUI textToUpdate = color == E_SnakeColor.Blue ? blueScoreText : greenScoreText;
        textToUpdate.text = score.ToString();
    }


    public void AddPowerUp(E_SnakeColor color, E_PowerUp powerUp, float maxTime)
    {
        PlayerUiElements appropriateUI = color == E_SnakeColor.Green ? greenPlayerUiElements : bluePlayerUiElements;
        PowerUpPanelContainer appropriatePowerUpPanel = powerUp == E_PowerUp.Shield ? appropriateUI.shieldPanel :
                                                (powerUp == E_PowerUp.ScoreBoost ? appropriateUI.scoreBoostPanel 
                                                : appropriateUI.speedUpPanel);
        
        appropriatePowerUpPanel.panel.SetActive(true);
        appropriatePowerUpPanel.text.text = appropriatePowerUpPanel.name + maxTime.ToString();
        activeTimers.Add(appropriatePowerUpPanel);
        appropriatePowerUpPanel.timer = maxTime;


    }

    public void RemovePowerUp(E_SnakeColor color, E_PowerUp powerUp)
    {
        PlayerUiElements appropriateUI = color == E_SnakeColor.Green ? greenPlayerUiElements : bluePlayerUiElements;
        PowerUpPanelContainer appropriatePowerUpPanel = powerUp == E_PowerUp.Shield ? appropriateUI.shieldPanel :
                                                (powerUp == E_PowerUp.ScoreBoost ? appropriateUI.scoreBoostPanel : 
                                                appropriateUI.speedUpPanel);
        
        appropriatePowerUpPanel.panel.SetActive(false);
        activeTimers.Remove(appropriatePowerUpPanel);
    }

    public void IndicatePowerUpsOnCooldown(E_SnakeColor color)
    {
        GameObject appropriatePanel = color == E_SnakeColor.Blue ? bluePowerUpPanel : greenPowerUpPanel;
        appropriatePanel.GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void IndicatePowerUpsOffCooldown(E_SnakeColor color)
    {
        GameObject appropriatePanel = color == E_SnakeColor.Blue ? bluePowerUpPanel : greenPowerUpPanel;
        appropriatePanel.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void TickTimers()
    {
        for (int i = 0; i < activeTimers.Count; i++)
        {
            activeTimers[i].timer -= Time.fixedDeltaTime;
            activeTimers[i].text.text = activeTimers[i].name + activeTimers[i].timer.ToString();
        }
    }
}

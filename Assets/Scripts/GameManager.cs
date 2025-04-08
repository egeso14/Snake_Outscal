using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SpriteReferences spriteReferences;
    public float foodGenerationInterval;
    public float customUpdateDeltaTime;
    public int startingSnakeLengths;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple GameManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetCustomUpdateDeltaTime()
    {
        return customUpdateDeltaTime;
    }
    public float GetFoodGenerationInterval()
    {
        return foodGenerationInterval;
    }

    public void GameOver(E_SnakeColor winnerColor)
    {
        Time.timeScale = 0;
        Debug.Log("Game Over! " + winnerColor.ToString() + " wins!");
    }
}

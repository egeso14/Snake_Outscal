using System.Collections.Generic;
using UnityEngine;

public class PowerUpGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Dictionary<Vector2, GameObject> powerUpObjects;
    private float powerUpGenerationInterval;
    private float timer;
    public static PowerUpGenerator instance;
    public int totalPowerUpLimit;
    private int numberOfPowerUps;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple FoodManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    void Start()
    {
        powerUpObjects = new Dictionary<Vector2, GameObject>();
        powerUpGenerationInterval = GameManager.instance.GetPowerUpGenerationInterval();

        numberOfPowerUps = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > powerUpGenerationInterval)
        {
            GeneratePowerUp();
            timer = 0;
        }
    }

    void GeneratePowerUp()
    {
        E_PowerUp powerUpType = RandomlyGeneratePowerUpType();
        if (numberOfPowerUps >= totalPowerUpLimit)
        {
            return;
        }
        Vector2 randomPosition = GridManager.Instance.GetLocationOfEmptyCell();
        CreatePowerUpObjectAt(randomPosition, powerUpType);
        GridManager.Instance.AddPowerUpToCell(randomPosition, powerUpType);

    }

    private E_PowerUp RandomlyGeneratePowerUpType()
    {
        int randomValue = Random.Range(0, 3);
        E_PowerUp result = randomValue == 0 ? E_PowerUp.Shield :
                                                (randomValue == 1 ? E_PowerUp.ScoreBoost : E_PowerUp.SpeedUp);
        return result;

    }
    void CreatePowerUpObjectAt(Vector2 position, E_PowerUp powerUpType)
    {
        string powerUpTypePrefix = powerUpType == E_PowerUp.Shield ? "Shield_" : 
                                                    (powerUpType == E_PowerUp.ScoreBoost ?
                                                    "ScoreBoost_" : "SpeedUp_");
        GameObject powerUpObject = new GameObject(powerUpTypePrefix + "PowerUp");
        powerUpObject.transform.position = position;
        SpriteRenderer spriteRenderer = powerUpObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.instance.spriteReferences.powerUp;
        spriteRenderer.sortingLayerName = "MapBorders";
        if (powerUpType ==  E_PowerUp.ScoreBoost)
        {
            spriteRenderer.color = Color.red;
        }
        if (powerUpType == E_PowerUp.Shield)
        {
            spriteRenderer.color = Color.blue;
        }
        if (powerUpType == E_PowerUp.SpeedUp)
        {
            spriteRenderer.color = Color.green;
        }
        powerUpObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
        numberOfPowerUps++;
        powerUpObjects[position] = powerUpObject;
    }



    public void DestroyPowerUpObjectAt(Vector2 position, E_PowerUp powerUpType)
    {
        // Find the food object at the given position and destroy it
        if (powerUpObjects.ContainsKey(position))
        {
            Destroy(powerUpObjects[position]);
            powerUpObjects.Remove(position);
            numberOfPowerUps--;
            return;
        }
        Debug.LogWarning("PowerUp not found at position: " + position);

    }
}

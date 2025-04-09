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
        E_Food powerUpType = RandomlyGeneratePowerUpType();
        if (numberOfPowerUps >= totalPowerUpLimit)
        {
            return;
        }
        Vector2 randomPosition = GridManager.Instance.GetLocationOfEmptyCell();
        CreateFoodObjectAt(randomPosition, foodType);
        GridManager.Instance.AddFoodToCell(randomPosition, foodType);

    }

    private E_Food RandomlyGeneratePowerUpType()
    {
        E_Food result = Random.Range(0, 2) == 0 ? E_Food.MassBurner : E_Food.MassGainer;
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
        spriteRenderer.sprite = GameManager.instance.spriteReferences.;
        spriteRenderer.sortingLayerName = "MapBorders";
        if (foodType == E_Food.MassBurner)
        {
            spriteRenderer.color = Color.green;
        }
        if (foodType == E_Food.MassGainer) numberOfGainers++;
        else numberOfBurners++;

        powerUpObjects[position] = powerUpObject;
    }



    public void DestroyFoodObjectAt(Vector2 position, E_Food foodType)
    {
        // Find the food object at the given position and destroy it
        if (powerUpObjects.ContainsKey(position))
        {
            Destroy(powerUpObjects[position]);
            powerUpObjects.Remove(position);
            if (foodType == E_Food.MassGainer) numberOfGainers--;
            else numberOfBurners--;
        }


    }
}

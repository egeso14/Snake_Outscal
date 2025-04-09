using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FoodGenerator: MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Dictionary<Vector2, GameObject> foodObjects;
    private float foodGenerationInterval;
    private float timer;
    public static FoodGenerator instance;
    public int foodLimit;
    private int numberOfGainers;
    private int numberOfBurners;
    private E_Food nextFoodType;
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
        foodObjects = new Dictionary<Vector2, GameObject>();
        foodGenerationInterval = GameManager.instance.GetFoodGenerationInterval();

        numberOfBurners = 0;
        numberOfGainers = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > foodGenerationInterval)
        {
            GenerateFood();
            timer = 0;
        }
    }

    void GenerateFood()
    {
        E_Food foodType = RandomlyGenerateFoodType();
        int numberOfType = foodType == E_Food.MassGainer ? numberOfGainers : numberOfBurners;
        if (numberOfType >= foodLimit)
        {
            return;
        }
        Vector2 randomPosition = GridManager.Instance.GetLocationOfEmptyCell();
        CreateFoodObjectAt(randomPosition, foodType);
        GridManager.Instance.AddFoodToCell(randomPosition, foodType);
        
    }

    private E_Food RandomlyGenerateFoodType()
    {
        E_Food result = Random.Range(0, 2) == 0 ? E_Food.MassBurner : E_Food.MassGainer;
        return result;

    }
    void CreateFoodObjectAt(Vector2 position, E_Food foodType)
    {
        string foodTypePrefix = foodType == E_Food.MassGainer ? "MassGainer_" : "MassLoser_";
        GameObject foodObject = new GameObject(foodTypePrefix + "Food");
        foodObject.transform.position = position;
        SpriteRenderer spriteRenderer = foodObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.instance.spriteReferences.Food;
        spriteRenderer.sortingLayerName = "MapBorders";
        if (foodType == E_Food.MassBurner)
        {
            spriteRenderer.color = Color.green;
        }
        if (foodType == E_Food.MassGainer) numberOfGainers++;
        else numberOfBurners++;
        
        foodObjects[position] = foodObject;
    }

    

    public void DestroyFoodObjectAt(Vector2 position, E_Food foodType)
    {
        // Find the food object at the given position and destroy it
        if (foodObjects.ContainsKey(position))
        {
            Destroy(foodObjects[position]);
            foodObjects.Remove(position);
            if (foodType == E_Food.MassGainer) numberOfGainers--;
            else numberOfBurners--;
        }


    }
}

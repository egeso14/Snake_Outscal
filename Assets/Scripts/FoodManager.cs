using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class FoodManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Dictionary<Vector2, GameObject> foodObjects;
    private float foodGenerationInterval;
    private float timer;
    public static FoodManager instance;
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
        Vector2 randomPosition = GridManager.Instance.GetLocationOfEmptyCell();
        CreateFoodObjectAt(randomPosition);
        GridManager.Instance.AddFoodToCell(randomPosition);
    }
    void CreateFoodObjectAt(Vector2 position)
    {
        GameObject foodObject = new GameObject("Food");
        foodObject.transform.position = position;
        SpriteRenderer spriteRenderer = foodObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.instance.spriteReferences.Food;
        spriteRenderer.sortingLayerName = "Food";
    }

    public void DestroyFoodObjectAt(Vector2 position)
    {
        // Find the food object at the given position and destroy it
        if (foodObjects.ContainsKey(position))
        {
            Destroy(foodObjects[position]);
            foodObjects.Remove(position);
        }
    }
}

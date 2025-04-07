using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public enum E_MovementDirections // identical to E_Routing
{
    None,
    Left,
    Right,
    Up,
    Down,
}

public enum E_SnakeBodyParts
{
    Head,
    Body,
    Tail
}

[RequireComponent(typeof(SpriteRenderer))]
public class SnakeHead : MonoBehaviour
{
    private List<SnakeBody> snakeBodyComponents; // sorted from head to tail
    private float customUpdateDeltaTime;
    private float timer;
    private E_MovementDirections movementDirection;
    private GridManager gridManager;
    private E_SnakeColor my_color;
    private SpriteRenderer spriteRenderer;
    private float spriteSideLen;


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > customUpdateDeltaTime)
        {
            CustomUpdate();
            timer = 0;
        }
    }
    private void CustomUpdate()
    {
        E_MovementDirections oldDirection = movementDirection;
        PollInputToGetDirection();
        if (oldDirection != movementDirection)
        {
            GridManager.Instance.DeclareIntentionToTurn(transform.position, oldDirection);
            Debug.Log("intention to turn is recognized");
        }
        if (gridManager.IsCellOccupied(transform.position, movementDirection))
        {
            E_SnakeColor color = my_color == E_SnakeColor.Blue ? E_SnakeColor.Green : E_SnakeColor.Blue;
            GameManager.instance.GameOver(color);
            return;
        }

        transform.position = GridManager.Instance.AskForNextPosition(transform.position, movementDirection);

        var food = gridManager.GetFoodAtCell(transform.position, movementDirection);
        if (food != E_Food.None)
        {
            gridManager.EatFoodAtCell(transform.position, movementDirection);
            GrowSnake();
        }

        RotateHeadSprite();
        UpdateBodyComponents();
    }

    public void InitializeSnake(List<Vector2> headToToePositions, E_SnakeColor color, float spriteSideLength)
    {
        customUpdateDeltaTime = GameManager.instance.GetCustomUpdateDeltaTime();
        timer = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameManager.instance.spriteReferences.snakeHead;
        spriteRenderer.sortingLayerName = "Snakes";
        gridManager = GridManager.Instance;
        movementDirection = E_MovementDirections.Up;
        snakeBodyComponents = new List<SnakeBody>();
        spriteSideLen = spriteSideLength;

        my_color = color;
        transform.position = headToToePositions[0];
        DetermineSpriteSize(spriteSideLen);

        for (int i = 1; i < headToToePositions.Count; i++)
        {
            GameObject bodyObject = new GameObject();
            bodyObject.AddComponent<SnakeBody>();
            SnakeBody bodyComponent = bodyObject.GetComponent<SnakeBody>();
            if (i == headToToePositions.Count - 1)
            {
                bodyComponent.InitializeBody(true, headToToePositions[i], E_MovementDirections.Up, color, spriteSideLength); 
            }
            else
            {
                bodyComponent.InitializeBody(false, headToToePositions[i], E_MovementDirections.Up, color, spriteSideLength);
            }
            snakeBodyComponents.Add(bodyComponent);
        }
    }

    public void UpdateBodyComponents()
    {
        foreach (var bodyComponent in snakeBodyComponents)
        {
            bodyComponent.UpdateBody();
        }
    }

    private void GrowSnake()
    {
        snakeBodyComponents[snakeBodyComponents.Count - 1].SetIsTail(false);
        E_MovementDirections tailDirection = snakeBodyComponents[snakeBodyComponents.Count - 1].movementDirection;
        Vector2 lastTailPosition = snakeBodyComponents[snakeBodyComponents.Count - 1].transform.position;

        GameObject bodyObject = new GameObject();
        bodyObject.AddComponent<SnakeBody>();
        SnakeBody bodyComponent = bodyObject.GetComponent<SnakeBody>();

        Vector2 newTailPosition = gridManager.AskForNextPosition(lastTailPosition, GetOppositeDirection(tailDirection));
        bodyComponent.InitializeBody(true, newTailPosition, tailDirection, my_color, spriteSideLen);
    }

    public void DetermineSpriteSize(float spriteSideLength)
    {
        Debug.Log(spriteRenderer.sprite);
        float worldSideLength = spriteRenderer.sprite.bounds.size.x;
        float scale = spriteSideLength / worldSideLength;
        transform.localScale = new Vector2(scale, scale);
    }

    private void PollInputToGetDirection()
    {
        if (my_color == E_SnakeColor.Blue)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                movementDirection = E_MovementDirections.Up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                movementDirection = E_MovementDirections.Down;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                movementDirection = E_MovementDirections.Right;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                movementDirection = E_MovementDirections.Left;
            }
        }

        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                movementDirection = E_MovementDirections.Up;
            }
            if (Input.GetKey(KeyCode.S))
            {
                movementDirection = E_MovementDirections.Down;
            }
            if (Input.GetKey(KeyCode.D))
            {
                movementDirection = E_MovementDirections.Right;
            }
            if (Input.GetKey(KeyCode.A))
            {
                movementDirection = E_MovementDirections.Left;
            }
        }


    }


    private E_MovementDirections GetOppositeDirection(E_MovementDirections direction)
    {
        switch (direction)
        {
            case E_MovementDirections.Left:
                return E_MovementDirections.Right;
            case E_MovementDirections.Right:
                return E_MovementDirections.Left;
            case E_MovementDirections.Up:
                return E_MovementDirections.Down;
            case E_MovementDirections.Down:
                return E_MovementDirections.Up;
            default:
                return E_MovementDirections.None;
        }
    }

    private void RotateHeadSprite()
    {

        switch (movementDirection)
        {
            case E_MovementDirections.None:
                break;
            case E_MovementDirections.Left:
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case E_MovementDirections.Right:
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
            case E_MovementDirections.Up:
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case E_MovementDirections.Down:
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
        }

    }
}

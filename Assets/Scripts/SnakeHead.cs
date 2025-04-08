using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Serialization;
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
    private Vector2 spriteScale;

    private SpriteReferences spriteReferences;


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
        FixDirectionForConstraints(oldDirection, movementDirection);
        if (oldDirection != movementDirection)
        {
            GridManager.Instance.DeclareIntentionToTurn(transform.position, movementDirection);
            Debug.Log("intention to turn is recognized");
        }
        if (gridManager.IsCellOccupied(transform.position, movementDirection))
        {
            E_SnakeColor color = my_color == E_SnakeColor.Blue ? E_SnakeColor.Green : E_SnakeColor.Blue;
            GameManager.instance.GameOver(color);
            return;
        }

        
        transform.position = GridManager.Instance.AskForNextPosition(transform.position, movementDirection);
        gridManager.DeclareHeadPosition(my_color, transform.position);

        var food = gridManager.GetFoodAtCell(transform.position, movementDirection);
        if (food != E_Food.None)
        {
            gridManager.EatFoodAtCell(transform.position, movementDirection);
            GrowSnake();
        }
        UpdateBodyComponents();
        SetSprites(0);
    }

    public void InitializeSnake(List<Vector2> headToToePositions, E_SnakeColor color, float spriteSideLength)
    {
        customUpdateDeltaTime = GameManager.instance.GetCustomUpdateDeltaTime();
        timer = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Snakes";
        gridManager = GridManager.Instance;
        movementDirection = E_MovementDirections.Up;
        snakeBodyComponents = new List<SnakeBody>();
        spriteReferences = GameManager.instance.spriteReferences;

        my_color = color;
        transform.position = headToToePositions[0];
        spriteScale = DetermineSpriteSize(spriteSideLength);
        transform.localScale = spriteScale;

        for (int i = 1; i < headToToePositions.Count; i++)
        {
            GameObject bodyObject = new GameObject();
            bodyObject.AddComponent<SnakeBody>();
            SnakeBody bodyComponent = bodyObject.GetComponent<SnakeBody>();
            if (i == headToToePositions.Count - 1)
            {
                bodyComponent.InitializeBody(true, headToToePositions[i], E_MovementDirections.Up, color, spriteScale); 
            }
            else
            {
                bodyComponent.InitializeBody(false, headToToePositions[i], E_MovementDirections.Up, color, spriteScale);
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

    private void FixDirectionForConstraints(E_MovementDirections oldDirection, E_MovementDirections newDirection)
    {
        if (newDirection == GetOppositeDirection(oldDirection))
        {
            movementDirection = oldDirection;
            return;
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
        bodyComponent.InitializeBody(true, newTailPosition, tailDirection, my_color, spriteScale);
        SetSprites(snakeBodyComponents.Count - 1);

    }

    public Vector2 DetermineSpriteSize(float spriteSideLength)
    {
        float worldSideLength = spriteReferences.snakeHeadUp.bounds.size.x;
        float scale = spriteSideLength / worldSideLength;
        return new Vector2(scale, scale);
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

    private void SetSprites(int startIndex)
    {
        spriteRenderer.color = my_color == E_SnakeColor.Blue ? Color.white : Color.green;
        spriteRenderer.sortingOrder = 900 + 1;
        // set head sprite
        switch (movementDirection)
        {
            case E_MovementDirections.Left:
                spriteRenderer.sprite = GameManager.instance.spriteReferences.snakeHeadLeft;
                break;
            case E_MovementDirections.Right:
                spriteRenderer.sprite = GameManager.instance.spriteReferences.snakeHeadRight;
                break;
            case E_MovementDirections.Up:
                spriteRenderer.sprite = GameManager.instance.spriteReferences.snakeHeadUp;
                break;
            case E_MovementDirections.Down:
                spriteRenderer.sprite = GameManager.instance.spriteReferences.snakeHeadDown;
                break;
        }
        
        E_MovementDirections directionOfLastBody = movementDirection;

        //iterate over bodies and set their sprites
        for (int i = startIndex; i < snakeBodyComponents.Count; i++)
        {
            SnakeBody body = snakeBodyComponents[i];
            body.SetColor(my_color == E_SnakeColor.Blue ? Color.white : Color.green);
            body.SetRenderOrder(900 - i); 
            if (body.isTail)
            {
                switch (directionOfLastBody)
                {
                    case E_MovementDirections.Left:
                        body.SetSprite(spriteReferences.snakeTailLeft);
                        break;
                    case E_MovementDirections.Right:
                        body.SetSprite(spriteReferences.snakeTailRight);
                        break;
                    case E_MovementDirections.Up:
                        body.SetSprite(spriteReferences.snakeTailUp);
                        break;
                    case E_MovementDirections.Down:
                        body.SetSprite(spriteReferences.snakeTailDown);
                        break;
                }
                continue;
            }
            

            // body parts that are not head or tail
            switch (directionOfLastBody)
            {
                case E_MovementDirections.Left:
                    if (body.movementDirection == E_MovementDirections.Up)
                    {
                        body.SetSprite(spriteReferences.snakeBodyDownLeft);
                    }
                    else if (body.movementDirection == E_MovementDirections.Down)
                    {
                        body.SetSprite(spriteReferences.snakeBodyUpLeft);
                    }
                    else
                    {
                        body.SetSprite(spriteReferences.snakeBodyHorizontal);
                    }
                    break;
                case E_MovementDirections.Right:
                    if (body.movementDirection == E_MovementDirections.Up)
                    {
                        body.SetSprite(spriteReferences.snakeBodyDownRight);
                    }
                    else if (body.movementDirection == E_MovementDirections.Down)
                    {
                        body.SetSprite(spriteReferences.snakeBodyUpRight);
                    }
                    else
                    {
                        body.SetSprite(spriteReferences.snakeBodyHorizontal);
                    }
                    break;
                case E_MovementDirections.Up:
                    if (body.movementDirection == E_MovementDirections.Left)
                    {
                        body.SetSprite(spriteReferences.snakeBodyUpRight);
                    }
                    else if (body.movementDirection == E_MovementDirections.Right)
                    {
                        body.SetSprite(spriteReferences.snakeBodyUpLeft);
                    }
                    else
                    {
                        body.SetSprite(spriteReferences.snakeBodyVertical);
                    }
                    break;
                case E_MovementDirections.Down:
                    if (body.movementDirection == E_MovementDirections.Left)
                    {
                        body.SetSprite(spriteReferences.snakeBodyDownRight);
                    }
                    else if (body.movementDirection == E_MovementDirections.Right)
                    {
                        body.SetSprite(spriteReferences.snakeBodyDownLeft);
                    }
                    else
                    {
                        body.SetSprite(spriteReferences.snakeBodyVertical);
                    }
                    break;
            }

            directionOfLastBody = body.movementDirection;
            
        }
    }

        
}

using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class SnakeBody : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public E_MovementDirections movementDirection;
    public bool isTail;
    private SpriteRenderer spriteRenderer;
    private E_SnakeColor snakeColor;
    void Start()
    {
        // START DOES NOT GET CALLED IF THE GAME OBJECT IS CREATED BY A CUSTOM SCRIPT
    }

    public void InitializeBody(bool tail, Vector2 position, E_MovementDirections direction, E_SnakeColor color, Vector2 scale)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Snakes";
        transform.position = position;
        movementDirection = direction;
        snakeColor = color;
        transform.localScale = scale;
        SetIsTail(tail);
        if (tail)
        {
            GridManager.Instance.DeclareNewTailPosition(color, position);
            
        }
        
    }

    public void SetIsTail(bool _isTail)
    {
        isTail = _isTail;
    }

    public void UpdateBody()
    {
        E_MovementDirections nextDirection = GridManager.Instance.AskForMovementDirection(transform.position);
        movementDirection = nextDirection == E_MovementDirections.None ? movementDirection : nextDirection;
        Debug.Log(nextDirection);               
        if (isTail)
        {
            GridManager.Instance.DeclareTailPositionChange(snakeColor, transform.position);
        }
        transform.position = GridManager.Instance.AskForNextPosition(transform.position, movementDirection);

    }

     public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void SetRenderOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }

}

   
    
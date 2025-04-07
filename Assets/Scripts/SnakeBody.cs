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

    public void InitializeBody(bool tail, Vector2 position, E_MovementDirections direction, E_SnakeColor color, float spriteSideLength)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Snakes";
        transform.position = position;
        movementDirection = direction;
        snakeColor = color;
        SetIsTail(tail);
        if (tail)
        {
            GridManager.Instance.DeclareNewTailPosition(color, position);
            
        }
        DetermineSpriteSize(spriteSideLength);
    }

    public void DetermineSpriteSize(float spriteSideLength)
    {
        float worldSideLength = spriteRenderer.sprite.bounds.size.x;
        float scale = spriteSideLength / worldSideLength;
        transform.localScale = new Vector2(scale, scale);
    }

    public void SetIsTail(bool _isTail)
    {
        isTail = _isTail;
        SetSprite(isTail);
    }
    
    private void SetSprite(bool isTail)
    {
        if (isTail)
        {
            spriteRenderer.sprite = GameManager.instance.spriteReferences.snakeTail;
        }
        else
        {
            spriteRenderer.sprite = GameManager.instance.spriteReferences.snakeBody;
        }
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



    void Update()
    {
        if (isTail)
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
                    spriteRenderer.transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case E_MovementDirections.Down:
                    spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
            }
        }
    }

    

}

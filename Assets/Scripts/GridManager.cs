using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
public enum E_SnakeColor
{
    Empty,
    Blue,
    Green,
   
}
public enum E_Routing // identical to E_MovementDirections
{
    None,
    GoLeft,
    GoRight,
    GoUp,
    GoDown,
}
public enum E_Food
{
    None,
    MassGainer,
    MassBurner,
}

public enum E_PowerUp 
{
    None,
    Shield,
    ScoreBoost,
    SpeedUp
}

public struct Cell
{
 
    public E_SnakeColor occupation;
    public E_Routing routing;
    public E_Food food;
    public E_PowerUp powerUp;

}
public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private Vector2 gridDimensions;
    [SerializeField] private Cell[,] cells;
    [SerializeField] private Dictionary<(int, int), Vector2> gridToWorldPositions;
    [SerializeField] private Dictionary<Vector2, (int, int)> worldToGridPositions;
    

    private int startingSnakeLengths;
    // caching variables
    private Vector2 lastOldWorldPos;
    private (int, int) lastNewGridPos;
    private Cell lastCell;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    void Start()
    {
        // initialize variables
        gridToWorldPositions = new Dictionary<(int, int), Vector2>();
        worldToGridPositions = new Dictionary<Vector2, (int, int)>();
        cells = new Cell[(int)gridDimensions.x, (int)gridDimensions.y];
        BoxCollider2D boxCollider = GetComponentInChildren<BoxCollider2D>();
        Bounds bounds = boxCollider.bounds;
        float startingX = bounds.min.x;
        float startingY = bounds.max.y;
        float localCellCenterX = (bounds.size.x / gridDimensions.x) / 2;
        float localCellCenterY = (bounds.size.y / gridDimensions.y) / 2;
        startingSnakeLengths = GameManager.instance.startingSnakeLengths;
        

        Vector2 gridLeftCornerWorldPosition = new Vector2(startingX + localCellCenterX, startingY - localCellCenterY);
        for (int i = 0; i < gridDimensions.y; i++)
        {
            for (int j = 0; j < gridDimensions.x; j++)
            {
                Vector2 worldPos = new Vector2(gridLeftCornerWorldPosition.x + j * 2 * localCellCenterX,
                                                            gridLeftCornerWorldPosition.y - i * 2 * localCellCenterY);
                (int, int) gridPositions = (j, i);
                gridToWorldPositions[gridPositions] = worldPos;
                worldToGridPositions[worldPos] = gridPositions;
            }
        }

        float cellSideLengthX = bounds.size.x / gridDimensions.x;
        float cellSideLengthY = bounds.size.y / gridDimensions.y;

        Debug.Log(bounds.size.x * 1.1);
        Debug.Log(bounds.size.y * 1.1);

        // create the snakes
        // they should start at 1/3 and 2/3 of the grid respectively
        (int, int) greenNextGridPos = ((int)gridDimensions.x / 3, (int)gridDimensions.y  * 2/ 3);
        (int, int) blueNextGridPos = ((int)gridDimensions.x * 2 / 3, (int)gridDimensions.y * 2/ 3);
        List<Vector2> blueHeadToToeList = new List<Vector2>();
        List<Vector2> greenHeadToToeList = new List<Vector2>();

        blueHeadToToeList.Add(gridToWorldPositions[blueNextGridPos]);
        greenHeadToToeList.Add(gridToWorldPositions[greenNextGridPos]);
        for (int i = 0; i < startingSnakeLengths; i++)
        {
            blueNextGridPos = (blueNextGridPos.Item1, blueNextGridPos.Item2 + 1 );
            greenNextGridPos = (greenNextGridPos.Item1, greenNextGridPos.Item2 + 1 );
            if (greenNextGridPos.Item2 < (int)gridDimensions.y)
            {
                blueHeadToToeList.Add(gridToWorldPositions[blueNextGridPos]);
                greenHeadToToeList.Add(gridToWorldPositions[greenNextGridPos]);
            }
        }

        GameObject blueHead = new GameObject();
        GameObject greenHead = new GameObject();

        blueHead.AddComponent<SnakeHead>();
        greenHead.AddComponent<SnakeHead>();

        blueHead.GetComponent<SnakeHead>().InitializeSnake(blueHeadToToeList, E_SnakeColor.Blue, new Vector2(cellSideLengthX * 1.1f, cellSideLengthY * 1.1f));
        greenHead.GetComponent<SnakeHead>().InitializeSnake(greenHeadToToeList, E_SnakeColor.Green, new Vector2(cellSideLengthX * 1.1f, cellSideLengthY * 1.1f));
    }

    public Vector2 GetLocationOfEmptyCell()
    {
        // generate food in random empty cell
        List<(int, int)> emptyCells = new List<(int, int)>();
        for (int i = 0; i < gridDimensions.y; i++)
        {
            for (int j = 0; j < gridDimensions.x; j++)
            {
                if (cells[j, i].occupation == E_SnakeColor.Empty)
                {
                    emptyCells.Add((j, i));
                }
            }
        }
        
        int randomIndex = Random.Range(0, emptyCells.Count);
        (int, int) randomCell = emptyCells[randomIndex];
        return gridToWorldPositions[randomCell];
    } 

    public void AddFoodToCell(Vector2 position, E_Food foodType)
    {
        (int, int) gridPosition = worldToGridPositions[position];
        cells[gridPosition.Item1, gridPosition.Item2].food = foodType;
    }

    public bool IsCellOccupied(Vector2 position, E_MovementDirections movementDirection)
    {
        (int, int) gridPositionOld = worldToGridPositions[position];
        (int, int) gridPositionNew = GetNextPositionOnGrid(gridPositionOld, movementDirection);
        if (cells[gridPositionNew.Item1, gridPositionNew.Item2].occupation == E_SnakeColor.Empty)
        {
            return false;
        }
        return true;
    }

    public E_Food GetFoodAtCell(Vector2 position, E_MovementDirections movementDirection)
    {
        if (position == lastOldWorldPos)
        {
            return cells[lastNewGridPos.Item1, lastNewGridPos.Item2].food;
        }
        else
        {
            (int, int) gridPositionOld = worldToGridPositions[position];
            (int, int) gridPositionNew = GetNextPositionOnGrid(gridPositionOld, movementDirection);
            return cells[gridPositionNew.Item1, gridPositionNew.Item2].food;
        }
    }



    public void EatFoodAtCell(Vector2 position, E_MovementDirections movementDirection)
    {
        if (position == lastOldWorldPos)
        {
            FoodGenerator.instance.DestroyFoodObjectAt(gridToWorldPositions[lastNewGridPos]
                                                        ,cells[lastNewGridPos.Item1, lastNewGridPos.Item2].food);
            cells[lastNewGridPos.Item1, lastNewGridPos.Item2].food = E_Food.None;
        }
        else
        {
            (int, int) gridPositionOld = worldToGridPositions[position];
            (int, int) gridPositionNew = GetNextPositionOnGrid(gridPositionOld, movementDirection);
           
            FoodGenerator.instance.DestroyFoodObjectAt(gridToWorldPositions[gridPositionNew],
                                                       cells[gridPositionNew.Item1, gridPositionNew.Item2].food);
            cells[gridPositionNew.Item1, gridPositionNew.Item2].food = E_Food.None;
        }
    }

    public Vector2 AskForNextPosition(Vector2 currentPosition, E_MovementDirections movementDirection)
    {
        lastOldWorldPos = currentPosition;
        // calculate place in grid
        (int, int) gridPositionOld = worldToGridPositions[currentPosition];
        // find the next position on the grid
        (int, int) gridPositionNew = GetNextPositionOnGrid(gridPositionOld, movementDirection);
        lastNewGridPos = gridPositionNew;
        // translate the position to world coordinates and return
        return gridToWorldPositions[gridPositionNew];
    }

    public E_MovementDirections AskForMovementDirection(Vector2 position)
    {
        (int, int) gridPosition = worldToGridPositions[position];
        return (E_MovementDirections)(int)cells[gridPosition.Item1, gridPosition.Item2].routing;
    }

    private (int, int) GetNextPositionOnGrid((int, int) currentPosition, E_MovementDirections movementDirection)
    {

        switch (movementDirection)
        {
            case E_MovementDirections.Left:
                if (currentPosition.Item1 == 0)
                {
                    return ((int)gridDimensions.x - 1, currentPosition.Item2);
                }
                return (currentPosition.Item1 - 1, currentPosition.Item2);
            case E_MovementDirections.Right:
                if (currentPosition.Item1 == (int)gridDimensions.x - 1)
                {
                    return (0, currentPosition.Item2);
                }
                return (currentPosition.Item1 + 1, currentPosition.Item2);
            case E_MovementDirections.Up:
                if (currentPosition.Item2 == 0)
                {
                    return (currentPosition.Item1, (int)gridDimensions.y - 1);
                }
                return (currentPosition.Item1, currentPosition.Item2 - 1);
            case E_MovementDirections.Down:
                if (currentPosition.Item2 == (int) gridDimensions.y - 1)
                {
                    return (currentPosition.Item1, 0);
                }
                return (currentPosition.Item1, currentPosition.Item2 + 1);
        }
        Debug.Log("Returning the same position on grid");
        return currentPosition;
    }
    public void DeclareIntentionToTurn(Vector2 oldPosition, E_MovementDirections newDirection)
    {
        E_Routing routing_direction = (E_Routing)(int)newDirection;
        (int, int) oldGridPosition = worldToGridPositions[oldPosition];
        cells[oldGridPosition.Item1, oldGridPosition.Item2].routing = routing_direction;
    }
    public void DeclareHeadPositionChange(E_SnakeColor color, Vector2 newPosition)
    {
        // we will update the cells to reflect the change
        (int, int) newGridPosition = worldToGridPositions[newPosition];
        cells[newGridPosition.Item1, newGridPosition.Item2].food = E_Food.None;
        cells[newGridPosition.Item1, newGridPosition.Item2].occupation = color;
    }

    public void DeclareTailPositionChange(E_SnakeColor color, Vector2 oldPosition)
    {
        (int, int) oldTailGridPosition = worldToGridPositions[oldPosition];
        cells[oldTailGridPosition.Item1, oldTailGridPosition.Item2] = new Cell();
    }

    public void DeclareNewTailPosition(E_SnakeColor color, Vector2 position)
    {
        (int, int) newTailGridPosition = worldToGridPositions[position];
        cells[newTailGridPosition.Item1, newTailGridPosition.Item2].occupation = color;
    }
    
}

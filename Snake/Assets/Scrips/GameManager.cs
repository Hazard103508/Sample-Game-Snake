using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Objects
    [SerializeField] private GameObject[] TilePrefabs;
    [SerializeField] private Text labelPoints;
    [SerializeField] private Snake snake;
    
    public GameObject apple;
    public int width;
    public int height;

    private List<Vector2Int> gridLocations;
    private int points;
    private Directions nextDirection =  Directions.Right;

    public UnityEvent appledEaten = new UnityEvent();
    public UnityEvent snakeDie = new UnityEvent();
    #endregion

    #region Unity Methods
    private void Start()
    {
        Load_Board();
        Load_Snake();
        Load_Apple();

        InvokeRepeating("MoveSnakeForward", 1, 0.5f);
    }
    private void Update()
    {
        if (this.snake.LookingDirection != Directions.Left && Input.GetKeyDown(KeyCode.RightArrow)) this.nextDirection = Directions.Right;
        if (this.snake.LookingDirection != Directions.Right && Input.GetKeyDown(KeyCode.LeftArrow)) this.nextDirection = Directions.Left;
        if (this.snake.LookingDirection != Directions.Down && Input.GetKeyDown(KeyCode.UpArrow)) this.nextDirection = Directions.Up;
        if (this.snake.LookingDirection != Directions.Up && Input.GetKeyDown(KeyCode.DownArrow)) this.nextDirection = Directions.Down;

        labelPoints.text = points.ToString();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Inicializa el tablero
    /// </summary>
    private void Load_Board()
    {
        gridLocations = new List<Vector2Int>();
        var parent = transform.Find("Tiles");

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                int index = (x + y) % 2;
                var prefab = TilePrefabs[index];

                var tile = Instantiate(prefab, parent);
                tile.transform.localPosition = new Vector3(x, y, 0);
                gridLocations.Add(new Vector2Int(x, y));
            }
    }
    /// <summary>
    /// Inicializa la serpiente
    /// </summary>
    private void Load_Snake()
    {
        snake.Clear();
        snake.Initialize(new Vector2Int(this.width / 2, this.height / 2));
    }
    /// <summary>
    /// Carga la manzana en una ubicacion aleatoria
    /// </summary>
    private void Load_Apple()
    {
        var availableLocations = new List<Vector2Int>();
        availableLocations.AddRange(gridLocations);

        var snakeLocations = snake.GetLocation();
        snakeLocations.ForEach(loc => availableLocations.Remove(loc));

        int index = Random.Range(0, availableLocations.Count);
        var position = availableLocations[index];
        apple.transform.localPosition = new Vector3(position.x, position.y);
    }
    /// <summary>
    /// Determina si la serpiente se puede mover
    /// </summary>
    /// <param name="location">Ubicacion a mover</param>
    /// <returns></returns>
    private bool CanMove(Vector2Int location)
    {
        if (location.x < 0 || location.x >= this.width)
            return false;

        if (location.y < 0 || location.y >= this.height)
            return false;

        var snakeLocations = snake.GetLocation().Skip(2).ToList();
        if (snakeLocations.Contains(location))
            return false;

        return true;
    }
    /// <summary>
    /// Mueve la serpiente hacia delante
    /// </summary>
    public void MoveSnakeForward()
    {
        Vector2Int displacement =
            this.nextDirection == Directions.Right ? Vector2Int.right :
            this.nextDirection == Directions.Left ? Vector2Int.left :
            this.nextDirection == Directions.Up ? Vector2Int.up :
            this.nextDirection == Directions.Down ? Vector2Int.down :
            Vector2Int.zero;

        MoveSnake(displacement);
    }
    /// <summary>
    /// Mueve la serpiente
    /// </summary>
    /// <param name="displacement">Desplazamiento que realiza la serpiente</param>
    public void MoveSnake(Vector2Int displacement)
    {
        if (displacement == Vector2Int.zero)
            return;

        var nextLocation = new Vector2Int(snake.Location.x + (int)displacement.x, snake.Location.y + (int)displacement.y);
        if (!CanMove(nextLocation))
        {
            snakeDie.Invoke();
            Restart();
            return;
        }

        if (nextLocation.x == this.apple.transform.localPosition.x && nextLocation.y == this.apple.transform.localPosition.y)
        {
            snake.AddNode(displacement);
            Load_Apple();
            appledEaten.Invoke();
            points++;
        }
        else
            snake.Move(displacement);
    }
    /// <summary>
    /// Reinicia el juego
    /// </summary>
    private void Restart()
    {
        Load_Snake();
        Load_Apple();

        points = 0;
    }
    #endregion
}

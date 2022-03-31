using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{
    #region Objects
    [SerializeField] private Templates templates;
    private List<SnakeNode> nodes;
    #endregion

    #region Properties
    /// <summary>
    /// Ubicacion de la cabeza de la serpiente
    /// </summary>
    public Vector2Int Location { get => this.nodes[0].Location; }
    public Directions LookingDirection { get => ((SnakeHead)nodes[0]).LookingDirection; }
    #endregion

    #region Public Methods
    public void Clear()
    {
        if (nodes != null)
        {
            nodes.ForEach(node => Destroy(node.gameObject));
            nodes.Clear();
        }
    }
    /// <summary>
    /// Inicializa la serpiente
    /// </summary>
    /// <param name="startPosition"></param>
    public void Initialize(Vector2Int startPosition)
    {
        this.nodes = new List<SnakeNode>();

        var obj = Instantiate(this.templates.head, this.transform);
        var head = obj.GetComponent<SnakeHead>();
        head.LookingDirection = Directions.Right;
        head.Location = startPosition;

        obj = Instantiate(this.templates.tail, this.transform);
        var tail = obj.GetComponent<SnakeTail>();
        tail.Direction = Directions.Right;
        tail.Location = startPosition - new Vector2Int(1, 0);

        nodes.Add(head);
        nodes.Add(tail);
    }
    /// <summary>
    /// Obtiene las posiciones de todos los nodos de la serpiente
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetLocation()
    {
        return this.nodes.Select(node => node.Location).ToList();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Mueve la serpiente
    /// </summary>
    /// <param name="displacement">Desplazamiento que debe realizar la serpiente</param>
    public void Move(Vector2Int displacement)
    {
        var head = (SnakeHead)nodes[0];
        var direction =
            displacement.x == 1 ? Directions.Right :
            displacement.x == -1 ? Directions.Left :
            displacement.y == 1 ? Directions.Up :
            displacement.y == -1 ? Directions.Down :
            Directions.None;

        if (head.LookingDirection == Directions.Up && direction == Directions.Down)
            return;

        if (head.LookingDirection == Directions.Down && direction == Directions.Up)
            return;

        if (head.LookingDirection == Directions.Left && direction == Directions.Right)
            return;

        if (head.LookingDirection == Directions.Right && direction == Directions.Left)
            return;

        head.LookingDirection = direction;
        var nextLocation = new Vector2Int(head.Location.x + (int)displacement.x, head.Location.y + (int)displacement.y);

        foreach (SnakeNode node in this.nodes)
        {
            var currentLocation = node.Location;
            node.Location = nextLocation;
            nextLocation = currentLocation;
        }

        for (int i = 1; i < this.nodes.Count - 1; i++)
        {
            var prev = this.nodes[i - 1];
            var body = (SnakeBody)this.nodes[i];
            var next = this.nodes[i + 1];

            Set_BodyDirection(prev, body, next);
        }

        Set_TailDirection();
    }

    /// <summary>
    /// Agrega un nuevo nodo a la serpuente
    /// </summary>
    /// <param name="displacement">Desplazamiento que debe realizar la serpiente</param>
    public void AddNode(Vector2Int displacement)
    {
        var head = (SnakeHead)nodes[0];
        var next = nodes[1];

        var nextLocation = new Vector2Int(head.Location.x + (int)displacement.x, head.Location.y + (int)displacement.y);
        var distance = nextLocation - next.Location; // distancia entre la nueva posicion que tendra la cabeza de la serpiente y el nodo siguiente

        var obj = Instantiate(this.templates.body, this.transform);
        var body = obj.GetComponent<SnakeBody>();
        body.Location = head.Location; // al nuevo nodo le asigno la posicion de la cabeza

        head.Location = nextLocation; // mueve la cabeza de la serpiente a la nueva ubicacion
        head.LookingDirection =
            displacement.x == 1 ? Directions.Right :
            displacement.x == -1 ? Directions.Left :
            displacement.y == 1 ? Directions.Up :
            displacement.y == -1 ? Directions.Down :
            Directions.None; ;

        Set_BodyDirection(head, body, next);
        nodes.Insert(1, body); // agrea un nuevo nodo en la ubicacion donde estaba la cabeza
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Determina la direccion que tomara el nuevo nodo de la serpiente
    /// </summary>
    /// <param name="prev">Nodo anterior de la serpiente</param>
    /// <param name="body">Nodo actual de la serpiente</param>
    /// <param name="next">Nodo siguiente</param>
    /// <returns></returns>
    private void Set_BodyDirection(SnakeNode prev, SnakeBody body, SnakeNode next)
    {
        var headDistance = prev.Location - body.Location;
        var bodyDistance = next.Location - body.Location;

        var headDirection =
            headDistance.x == 1 ? Directions.Right :
            headDistance.x == -1 ? Directions.Left :
            headDistance.y == 1 ? Directions.Up :
            Directions.Down;

        var nodeDirection =
            bodyDistance.x == 1 ? Directions.Right :
            bodyDistance.x == -1 ? Directions.Left :
            bodyDistance.y == 1 ? Directions.Up :
            Directions.Down;

        body.Direction =
            headDirection == Directions.Down && nodeDirection == Directions.Right ? SnakeNodeDirection.RightDown :
            headDirection == Directions.Down && nodeDirection == Directions.Up ? SnakeNodeDirection.Vertical :
            headDirection == Directions.Down && nodeDirection == Directions.Left ? SnakeNodeDirection.LeftDown :

            headDirection == Directions.Up && nodeDirection == Directions.Left ? SnakeNodeDirection.LeftUp :
            headDirection == Directions.Up && nodeDirection == Directions.Down ? SnakeNodeDirection.Vertical :
            headDirection == Directions.Up && nodeDirection == Directions.Right ? SnakeNodeDirection.RightUp :

            headDirection == Directions.Left && nodeDirection == Directions.Up ? SnakeNodeDirection.LeftUp :
            headDirection == Directions.Left && nodeDirection == Directions.Right ? SnakeNodeDirection.Horizontal :
            headDirection == Directions.Left && nodeDirection == Directions.Down ? SnakeNodeDirection.LeftDown :

            headDirection == Directions.Right && nodeDirection == Directions.Up ? SnakeNodeDirection.RightUp :
            headDirection == Directions.Right && nodeDirection == Directions.Left ? SnakeNodeDirection.Horizontal :
            headDirection == Directions.Right && nodeDirection == Directions.Down ? SnakeNodeDirection.RightDown :

            SnakeNodeDirection.None;
    }
    /// <summary>
    /// Determina la direccion que tomara la cola de la serpiente
    /// </summary>
    private void Set_TailDirection()
    {
        var tail = (SnakeTail)nodes.Last();
        var lastNode = nodes[nodes.Count - 2];

        tail.Direction =
            lastNode.Location.x < tail.Location.x ? Directions.Left :
            lastNode.Location.x > tail.Location.x ? Directions.Right :
            lastNode.Location.y < tail.Location.y ? Directions.Down :
            Directions.Up;
    }
    #endregion

    #region Structures
    [Serializable]
    public class Templates
    {
        public GameObject head;
        public GameObject body;
        public GameObject tail;
    }
    #endregion
}

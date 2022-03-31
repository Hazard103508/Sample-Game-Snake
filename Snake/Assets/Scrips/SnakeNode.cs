using UnityEngine;

public class SnakeNode : MonoBehaviour
{
    #region Objects
    protected Animator animator;
    #endregion


    #region Unity Methods
    void Awake()
    {
        this.animator = GetComponent<Animator>();
    }
    #endregion

    #region Propierties
    /// <summary>
    /// Direccion del nodo en la grilla
    /// </summary>
    public Vector2Int Location { get => new Vector2Int((int)this.transform.localPosition.x, (int)this.transform.localPosition.y); set => this.transform.localPosition = new Vector3(value.x, value.y, 0); }
    #endregion
}

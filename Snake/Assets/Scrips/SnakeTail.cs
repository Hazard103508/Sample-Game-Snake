using System;
using UnityEngine;

public class SnakeTail : SnakeNode
{
    #region Objects
    private Directions direction;
    #endregion

    #region Propierties
    /// <summary>
    /// Direccion donde mira la serpiente
    /// </summary>
    public Directions Direction
    {
        get => direction;
        set
        {
            direction = value;
            base.animator.SetInteger("Direction", (int)value);
        }
    }
    #endregion
}

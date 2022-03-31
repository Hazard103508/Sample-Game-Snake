using System;
using UnityEngine;

public class SnakeHead : SnakeNode
{
    #region Objects
    private Directions lookingDirection;
    #endregion

    #region Propierties
    /// <summary>
    /// Direccion donde mira la serpiente
    /// </summary>
    public Directions LookingDirection
    {
        get => lookingDirection;
        set
        {
            lookingDirection = value;
            base.animator.SetInteger("Direction", (int)value);
        }
    }
    #endregion
}

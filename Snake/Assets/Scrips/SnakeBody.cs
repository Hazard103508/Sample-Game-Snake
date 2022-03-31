using System;
using UnityEngine;

public class SnakeBody : SnakeNode
{
    #region Objects
    private SnakeNodeDirection direction;
    #endregion

    #region Propierties
    public SnakeNodeDirection Direction
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

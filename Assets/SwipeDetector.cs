using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 fingerUpPos;
    private Vector2 fingerDownPos;
    private SwipeDirection lockedDirection = SwipeDirection.None;

    [SerializeField]
    private float minDistanceForMovementDetection = 15f;

    [SerializeField]
    private float minDistanceForSwipeDetection = 50f;

    //Action lorsque le mouvement est détecté
    public static event Action<SwipeData> OnMovement = delegate { };

    //Action lorsque le mouvement est terminé
    public static event Action<SwipeData> OnSwipeEnd = delegate { };

    private void Update()
    {
        
        //Unity Remote a décidé de pas marcher, donc on débug à la souris.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            fingerUpPos = Input.mousePosition;
            fingerDownPos = Input.mousePosition;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            fingerDownPos = Input.mousePosition;
            DetectMovement();
        }

        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            fingerDownPos = Input.mousePosition;
            DetectMovement();

            EndMovement();
        }
        

        //Version mobile
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerUpPos = touch.position;
                fingerDownPos = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                fingerDownPos = touch.position;
                DetectMovement();
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDownPos = touch.position;
                DetectMovement();

                EndMovement();
            }
        }
    }

    private void DetectMovement()
    {
        if (MovementDistanceCheck())
        {
            SwipeDirection direction;

            //On regarde la direction selon les points de départ et d'arrivée
            if (IsVerticalMovement())
            {
                direction = fingerDownPos.y - fingerUpPos.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            }
            else
            {
                direction = fingerDownPos.x - fingerUpPos.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }

            if (lockedDirection == SwipeDirection.None) lockedDirection = direction;

            OnMovement(new SwipeData
            {
                startPos = fingerDownPos,
                endPos = fingerUpPos,
                direction = lockedDirection
            });
        }
    }

    private void EndMovement()
    {
        OnSwipeEnd(new SwipeData
        {
            startPos = fingerDownPos,
            endPos = fingerUpPos,
            direction = lockedDirection
        });

        lockedDirection = SwipeDirection.None;
    }

    private bool MovementDistanceCheck()
    {
        return VerticalMovementDistance() > minDistanceForMovementDetection || HorizontalMovementDistance() > minDistanceForMovementDetection;
    }

    private float HorizontalMovementDistance()
    {
        return Mathf.Abs(fingerDownPos.x - fingerUpPos.x);
    }

    private float VerticalMovementDistance()
    {
        return Mathf.Abs(fingerDownPos.y - fingerUpPos.y);
    }

    private bool IsVerticalMovement()
    {
        return VerticalMovementDistance() > HorizontalMovementDistance();
    }
}

public struct SwipeData
{
    public Vector2 startPos;
    public Vector2 endPos;
    public SwipeDirection direction;
}

public enum SwipeDirection
{
    Up,
    Down,
    Left,
    Right,
    None
}


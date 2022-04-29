using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeHandler : MonoBehaviour
{
    GameManager gm;

    void OnEnable()
    {
        SwipeDetector.OnMovement += MovementEvent;

        SwipeDetector.OnSwipeEnd += SwipeEvent;
    }

    void OnDisable()
    {
        SwipeDetector.OnMovement -= MovementEvent;

        SwipeDetector.OnSwipeEnd -= SwipeEvent;
    }

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void MovementEvent(SwipeData data)
    {
        //Debug.Log(data.direction);

        //On notifie le GameManager qu'un mouvement a été fait
        gm.OnMovementDetected(data);
    }

    private void SwipeEvent(SwipeData data)
    {
        Debug.Log("Movement ended" + data.direction);

        gm.OnSwipeDetected(data);
    }


}

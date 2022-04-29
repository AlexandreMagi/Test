using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        SwipeDetector.OnMovement += LogMovement;

        SwipeDetector.OnSwipeEnd += LogMovementEnd;
    }

    private void LogMovement(SwipeData data)
    {
        Debug.Log(data.direction);
    }

    private void LogMovementEnd(SwipeData data)
    {
        Debug.Log("Movement ended" + data.direction);
    }


}

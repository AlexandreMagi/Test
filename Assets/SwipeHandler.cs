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

        //On notifie le GameManager qu'un mouvement a été fait
        GameManager.instance.OnMovementDetected(data);
    }

    private void LogMovementEnd(SwipeData data)
    {
        Debug.Log("Movement ended" + data.direction);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsChoices : MonoBehaviour
{
    public static void TogglePlayerMoving()
    {
        PlayerAction.displayMovingForPlayer =
            !PlayerAction.displayMovingForPlayer;

        if (PlayerAction.displayMovingForPlayer)
        {
            PlayerAction.displayFiringForPlayer = false;
        }
        // Debug.Log("TacticsChoices.TogglePlayerMoving - PlayerAction.displayMovingForPlayer : " + PlayerAction.displayMovingForPlayer);
    }

    public static void TogglePlayerShooting()
    {
        PlayerAction.displayFiringForPlayer =
            !PlayerAction.displayFiringForPlayer;

        if (PlayerAction.displayFiringForPlayer)
        {
            PlayerAction.displayMovingForPlayer = false;
        }

        // Debug.Log("TacticsChoices.TogglePlayerShooting - PlayerAction.displayFiringForPlayer : " + PlayerAction.displayFiringForPlayer);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}

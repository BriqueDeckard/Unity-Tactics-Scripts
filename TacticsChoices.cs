using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsChoices : MonoBehaviour
{
    public static void TogglePlayerMoving()
    {
        PlayerAction.displayMovingForPlayer =
            !PlayerAction.displayMovingForPlayer;
        Debug.Log("TacticsChoices.TogglePlayerMoving - PlayerAction.displayMovingForPlayer : " + PlayerAction.displayMovingForPlayer);
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

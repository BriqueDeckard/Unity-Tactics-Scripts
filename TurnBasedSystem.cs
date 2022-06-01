using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBasedSystem : MonoBehaviour
{
    // Only true when it is this unit turn
    protected bool turn = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected void Init()
    {
        // add ourselves to the turn manager (to the dictionnary)
        TurnManager.AddUnit(this);
    }

    public void BeginTurn()
    {
        Debug.Log("Begin turn");
        turn = true;
    }

    public void EndTurn()
    {
        Debug.Log("End turn");
        turn = false;
        TurnManager.EndTurn();
    }
}

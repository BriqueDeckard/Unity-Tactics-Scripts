using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// All is static beacause it must be accessed everywhere
///</summary>
public class TurnManager : MonoBehaviour
{
    ///<summary>
    /// all units in the game.
    ///
    /// the string is for a tag as each team is based on a tag (NPC team / Player Team)
    /// the list hold all the member of a team
    ///</summary>
    static Dictionary<string, List<TacticsAction>>
        units = new Dictionary<string, List<TacticsAction>>();

    // basically the key for who the turn is. String is the tag of the team that
    static Queue<string> turnKey = new Queue<string>();

    ///<summary>
    /// Queue for current team who's turn is
    ///</summary>
    static Queue<TacticsAction> turnTeam = new Queue<TacticsAction>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("1. TurnManager.Start - BEGIN");
        Debug.Log("2. TurnManager.Start - END");
    }

    // Update is called once per frame
    void Update()
    {
        // if nobody take the turn then it's time to initialize
        // this happen only the first time so this is mostly for the first run
        if (turnTeam.Count == 0)
        {
            InitTeamTurnQueue();
        }
    }

    /// Initialize the team turn queue
    static void InitTeamTurnQueue()
    {
        Debug.Log("13. TurnManager.InitTeamTurnQueue - BEGIN");

        // Get it from the units --> turnKey is the currently active team
        // we're peeking into the units with the head of the keys
        // Get the current team key
        string currentTeam = turnKey.Peek();
        Debug
            .Log("14. TurnManager.InitTeamTurnQueue - current team: " +
            currentTeam);

        // Get the current team list of move
        List<TacticsAction> teamMoveList = units[currentTeam];
        Debug
            .Log("15. TurnManager.InitTeamTurnQueue - " +
            teamMoveList.Count +
            " moves for " + currentTeam);

        // Enqueue each move of the current team
        foreach (TacticsAction unit in teamMoveList)
        {
            Debug.Log("16. TurnManager.InitTeamTurnQueue - Enqueue " + unit.ToString() + " unit.");
            turnTeam.Enqueue (unit);
        }

        StartTurn();
        Debug.Log("21. TurnManager.InitTeamTurnQueue - END");
    }

    public static void StartTurn()
    {
        Debug.Log("17. TurnManager.StartTurn - BEGIN");

        // Check that the team is not empty
        if (turnTeam.Count > 0)
        {
            turnTeam.Peek().BeginTurn();
        }
        Debug.Log("20. TurnManager.StartTurn - END");
    }

    public static void EndTurn()
    {
        Debug.Log("22. TurnManager.EndTurn - BEGIN");
        TacticsAction unit = turnTeam.Dequeue();
        Debug.Log("23. TurnManager.EndTurn - unit: " + unit.ToString());

        unit.EndTurn();

        if (turnTeam.Count > 0)
        {
            StartTurn();
        }
        else
        {
            string team = turnKey.Dequeue();
            turnKey.Enqueue (team);
            InitTeamTurnQueue();
        }
        Debug.Log("TurnManager.EndTurn - END");
    }

    // how do we had a unit
    public static void AddUnit(TacticsAction unit)
    {
        // TODO: 4
        Debug.Log("5. TurnManager.AddUnit - BEGIN");
        List<TacticsAction> list;
        
        Debug.Log("6. TurnManager.AddUnit - unit.tag: " + unit.tag);
        // we have to make sur that the unit tag has been added to the dictionary
        if (!units.ContainsKey(unit.tag))
        {
            list = new List<TacticsAction>();
            units[unit.tag] = list;

            // Add the team to the turn queue
            if (!turnKey.Contains(unit.tag))
            {
                turnKey.Enqueue(unit.tag);
            }
        } // tag successfuly found
        else
        {
            list = units[unit.tag];
        }
        list.Add (unit);
        Debug.Log("7. TurnManager.AddUnit - " + list.Count + " units");

        Debug.Log("8. TurnManager.AddUnit - END");
    }
}

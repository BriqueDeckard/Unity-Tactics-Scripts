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
    static Dictionary<string, List<TacticsMove>>
        units = new Dictionary<string, List<TacticsMove>>();

    // basically the key for who the turn is. String is the tag of the team that
    static Queue<string> turnKey = new Queue<string>();

    ///<summary>
    /// Queue for current team who's turn is
    ///</summary>
    static Queue<TacticsMove> turnTeam = new Queue<TacticsMove>();

    // Start is called before the first frame update
    void Start()
    {
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
        // Get it from the units --> turnKey is the currently active team
        // we're peeking into the units with the head of the keys
        //
        List<TacticsMove> teamList = units[turnKey.Peek()];

        foreach (TacticsMove unit in teamList)
        {
            turnTeam.Enqueue (unit);
        }

        StartTurn();
    }

    public static void StartTurn()
    {
        // Check that the team is not empty
        if (turnTeam.Count > 0)
        {
            turnTeam.Peek().BeginTurn();
        }
    }

    public static void EndTurn()
    {
        TacticsMove unit = turnTeam.Dequeue();

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
    }

    // how do we had a unit
    public static void AddUnit(TacticsMove unit)
    {
        List<TacticsMove> list;

        // we have to make sur that the unit tag has been added to the dictionary
        if (!units.ContainsKey(unit.tag))
        {
            list = new List<TacticsMove>();
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
    }
}

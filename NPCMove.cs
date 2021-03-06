using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Npc movement
/// </summary>
public class NPCMove : TacticsMove
{
    GameObject target;

    /// <summary>
    /// Target where to move
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    new public void FindSelectableTiles(){
        ComputeAdjacencyLists(jumpHeight, null);

        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue (currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add (t);
            t.selectableByNpc = true;

            if (t.distance < move)
            {
                foreach (Tile tile in t.adjacencyList)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue (tile);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward);

        if (!turn)
        {
            // don't move or don't allow to select
            return;
        }

        if (!moving)
        {
            FindNearestTarget();
            CalculatePath();
            FindSelectableTiles();

            // find the target and turn it green
            actualTargetTile.target = true;
        }
        else
        {
            Move();
        }
    }

    /// <summary>
    /// Calculate the path to the target.
    /// </summary>
    void CalculatePath()
    {
        Tile targetTile = GetTargetTile(target);
        FindPath (targetTile);
    }

    void FindNearestTarget()
    {
        // Find all GameObjects that have the "player" tag ( == the player)
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        // TODO : Complexify the attack code
        GameObject nearest = null;
        float distance = Mathf.Infinity;

        // we need to check for distance
        foreach (GameObject obj in targets)
        {
            // Calculate the distance between this object and the target
            float d =
                Vector3.Distance(transform.position, obj.transform.position);

            // If the object is near
            if (d < distance)
            {
                distance = d;
                nearest = obj;
            }
        }

        target = nearest;
    }
}

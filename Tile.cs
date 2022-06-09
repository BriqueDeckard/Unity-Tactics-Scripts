using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    /// <summary>
    /// Tile height
    /// </summary>
    int tileHeight = 1;

    /// <summary>
    /// Does the tile is current ?
    /// </summary>
    public bool current = false;

    /// <summary>
    /// Does the tile is the target ?
    /// </summary>
    public bool target = false;

    /// <summary>
    /// Does the tile is selectable ?
    /// </summary>
    public bool selectable = false;

    /// <summary>
    /// Does the tile is selectable by NPC?
    /// </summary>
    public bool selectableByNpc = false;

    /// <summary>
    /// Does the tile is walkable ?
    /// </summary>
    public bool walkable = true;

    /// <summary>
    /// can you shoot something that is on the tile
    /// </summary>
    public bool attackable = false;

    public bool hasEnemy = false;

    public bool hasSomethingOnIt = false;

    /// <summary>
    ///
    /// </summary>
    public List<Tile> adjacencyListForMoving = new List<Tile>();

    public List<Tile> adjacencyListForShooting = new List<Tile>();

    // **** NEEDED FOR BFS ****
    /// <summary>
    /// Does the tile is has been visited ?
    /// </summary>
    public bool visited = false;

    /// <summary>
    /// Parent of the tile in the path.
    /// </summary>
    public Tile parent = null;

    /// <summary>
    /// Distance the player can go.
    /// </summary>
    public int distance = 0;

    // *** For A* ***
    // Used for finding a best-case path
    /// <summary>
    /// The cost from the parent to the current tile
    /// </summary>
    public float f = 0;

    /// <summary>
    /// The cost from the processed tile to the destination
    /// </summary>
    public float g = 0;

    /// <summary>
    /// The heuristic cost ( G + H )
    /// </summary>
    public float h = 0;

    private Vector3[]
        directions =
            new Vector3[] {
                Vector3.forward,
                -Vector3.forward,
                Vector3.right,
                -Vector3.right
            };

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (current)
        {
            GetComponent<Renderer>().material.color = Color.magenta;
        }
        if (hasSomethingOnIt && !current)
        {
            GetComponent<Renderer>().material.color = Color.black;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else if (selectableByNpc)
        {
            GetComponent<Renderer>().material.color = Color.yellow;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
        if (attackable && !hasEnemy)
        {
            // GetComponent<Renderer>().material.color = Color.cyan;
        }
        else if (attackable && hasEnemy && !current)
        {
            GetComponent<Renderer>().material.color = Color.cyan;
        }
    }

    public void Reset()
    {
        adjacencyListForMoving.Clear();
        adjacencyListForShooting.Clear();

        current = false;
        target = false;
        selectable = false;
        selectableByNpc = false;
        attackable = false;
        hasEnemy = false;
        hasSomethingOnIt = false;

        visited = false;
        parent = null;
        distance = 0;

        f = g = h = 0;
    }

    /// <summary>
    /// Reset the tile and check for all the directions if there is a "neighbor".
    /// </summary>
    public void FindNeighborsForMoving(float jumpHeight, Tile target)
    {
        // Reset the tile
        Reset();

        // Check all the directions
        CheckTileForMoving(Vector3.forward, jumpHeight, target);
        CheckTileForMoving(-Vector3.forward, jumpHeight, target);
        CheckTileForMoving(Vector3.right, jumpHeight, target);
        CheckTileForMoving(-Vector3.right, jumpHeight, target);
    }

    public void CheckTileForShooting(
        Vector3 direction,
        float jumpHeight,
        Tile target
    )
    {
        // Half the size of the box in each dimension.
        Vector3 halfExtents =
            new Vector3(0.25f, (tileHeight + jumpHeight) / 2, 0.25f);

        // Find all colliders touching or inside of the given box.
        Collider[] colliders =
            Physics.OverlapBox(transform.position + direction, halfExtents);

        // For each collider
        foreach (Collider item in colliders)
        {
            // get the tile object
            Tile tile = item.GetComponent<Tile>();

            // There is a tile and we can get there.
            if (tile != null && tile.walkable)
            {
                // make sure there is nothing on top of the tile and if so, add it to the adjacency list.
                RaycastHit hit;

                if (
                    !Physics
                        .Raycast(tile.transform.position,
                        Vector3.up,
                        out hit,
                        1) ||
                    (tile == target)
                )
                {
                    adjacencyListForShooting.Add (tile);
                }
                else
                {
                    if (!tile.current)
                    {
                        tile.hasEnemy = true;
                    }

                    adjacencyListForShooting.Add (tile);
                }
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public void CheckTileForMoving(
        Vector3 direction,
        float jumpHeight,
        Tile target
    )
    {
        // Half the size of the box in each dimension.
        Vector3 halfExtents =
            new Vector3(0.25f, (tileHeight + jumpHeight) / 2, 0.25f);

        // Find all colliders touching or inside of the given box.
        Collider[] colliders =
            Physics.OverlapBox(transform.position + direction, halfExtents);

        // For each collider
        foreach (Collider item in colliders)
        {
            // get the tile object
            Tile tile = item.GetComponent<Tile>();

            // There is a tile and we can get there.
            if (tile != null && tile.walkable)
            {
                // make sure there is nothing on top of the tile and if so, add it to the adjacency list.
                RaycastHit hit;
                if (
                    !Physics
                        .Raycast(tile.transform.position,
                        Vector3.up,
                        out hit,
                        1) ||
                    (tile == target)
                )
                {
                    adjacencyListForMoving.Add (tile);
                }
                else
                {
                    tile.hasSomethingOnIt = true;
                    // adjacencyListForMoving.Add (tile);
                }
            }
        }
    }

    public void FindNeighborsForShooting(float jumpHeight, Tile target)
    {
        Reset();

        CheckTileForShooting(Vector3.forward, jumpHeight, target);
        CheckTileForShooting(-Vector3.forward, jumpHeight, target);
        CheckTileForShooting(Vector3.right, jumpHeight, target);
        CheckTileForShooting(-Vector3.right, jumpHeight, target);
    }

    public Collider FindTargetOnTopOfTheTile()
    {
        Debug.Log("Tile.FindTargetOnTopOfTheTile");
        Collider[] colliders =
            Physics.OverlapBox(transform.position, Vector3.up);

        Debug
            .Log("Tile.FindTargetOnTopOfTheTile: Colliders: " +
            colliders.Length);

        if (colliders.Length > 0)
        {
            return colliders[0];
        }
        return null;
    }
}

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
    ///
    /// </summary>
    public List<Tile> adjacencyList = new List<Tile>();

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
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (selectable)
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
    }

    public void Reset()
    {
        adjacencyList.Clear();

        current = false;
        target = false;
        selectable = false;
        selectableByNpc = false;

        visited = false;
        parent = null;
        distance = 0;

        f = g = h = 0;
    }

    public void FindNeighbors(float jumpHeight, Tile target)
    {
        Reset();

        CheckTile(Vector3.forward, jumpHeight, target);
        CheckTile(-Vector3.forward, jumpHeight, target);
        CheckTile(Vector3.right, jumpHeight, target);
        CheckTile(-Vector3.right, jumpHeight, target);
    }

    public void CheckTile(Vector3 direction, float jumpHeight, Tile target)
    {
        Vector3 halfExtents =
            new Vector3(0.25f, (tileHeight + jumpHeight) / 2, 0.25f);

        Collider[] colliders =
            Physics.OverlapBox(transform.position + direction, halfExtents);

        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();

            // There is a tile and we can get there.
            if (tile != null && walkable)
            {
                RaycastHit hit;

                // make sure that there is nothing on the top of the tile
                if (
                    !Physics
                        .Raycast(tile.transform.position,
                        Vector3.up,
                        out hit,
                        1) ||
                    (tile == target)
                )
                {
                    adjacencyList.Add (tile);
                }
            }
        }
    }
}

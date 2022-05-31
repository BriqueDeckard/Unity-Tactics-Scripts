using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    protected List<Tile> selectableTiles = new List<Tile>();

    /// <summary>
    /// All the tiles of the map
    /// </summary>
    GameObject[] tiles;

    Stack<Tile> path = new Stack<Tile>();

    protected Tile currentTile;

    /// <summary>
    /// Move capability of the player
    /// </summary>
    public int move = 5;

    /// <summary>
    /// Jump height capability of the player
    /// </summary>
    public int jumpHeight = 2;

    /// <summary>
    /// Move speed capability of the player
    /// </summary>
    public int moveSpeed = 2;

    public float jumpVelocity = 4.5f;

    float halfHeight = 0;

    /// <summary>
    /// Is the player moving ?
    /// </summary>
    public bool moving = false;

    /// <summary>
    /// Is the player falling down ?
    /// </summary>
    bool fallingDown = false;

    bool jumpingUp = false;

    bool movingEdge = false;

    // Only true when it is this unit turn
    protected bool turn = false;

    Vector3 velocity = new Vector3();

    Vector3 heading = new Vector3();

    Vector3 jumpTarget;

    public Tile actualTargetTile;

    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        // Debug.Log(tiles.Length + " tiles.");
        halfHeight = GetComponent<Collider>().bounds.extents.y;

        // add ourselves to the turn manager (to the dictionnary)
        TurnManager.AddUnit(this);
    }

    public void GetCurrentTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;

        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }

    /// <summary> 
    /// Compute each tile of the map and find the neighbors if needed.
    /// </summary>
    public void ComputeAdjacencyLists(float jumpHeight, Tile target)
    {
        // For each tile of the map
        foreach (GameObject tile in tiles)
        {
            // Check if this is a tile
            Tile t = tile.GetComponent<Tile>();
            if(t == null){
                return;
            }
            // else find the neighbors
            t.FindNeighbors (jumpHeight, target);
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyLists(jumpHeight, null);

        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue (currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            selectableTiles.Add (t);
            t.selectable = true;

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

    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;

        // create the path
        Tile next = tile;
        while (next != null)
        {
            path.Push (next);
            next = next.parent;
        }
    }

    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            // Calculate the unit's position on top of the target tile.
            target.y +=
                halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            // Reach the tile ?
            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump (target);
                }
                else
                {
                    // Calculate heading
                    CalculateHeadingTo (target);

                    // Set velocity
                    SetHorizontalVelocity();
                }

                // Locomotion
                transform.forward = heading;
                transform.position += velocity * Time.deltaTime;
            }
            else
            {
                // Tile center reached
                transform.position = target;

                // we don't need that tile in the stacking
                path.Pop();
            }
        }
        else
        {
            RemoveSelectableTiles();
            moving = false;

            TurnManager.EndTurn();
        }
    }

    protected void RemoveSelectableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();
        }
        selectableTiles.Clear();
    }

    void CalculateHeadingTo(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }

    void SetHorizontalVelocity()
    {
        // direction * moveSpeed
        velocity = heading * moveSpeed;
    }

    /// <summary>
    // o the jump
    /// </summary>
    void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallDownward (target);
        }
        else if (jumpingUp)
        {
            JumpUpward (target);
        }
        else if (movingEdge)
        {
            MoveToEdge();
        }
        else
        {
            PrepareJump (target);
        }
    }

    void PrepareJump(Vector3 target)
    {
        float targetY = target.y;

        target.y = transform.position.y;

        CalculateHeadingTo (target);

        if (transform.position.y > targetY)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;

            // Calculate where the edge is
            jumpTarget =
                transform.position + (target - transform.position) / 2.0f;
        }
        else
        {
            // jump upward
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;

            velocity = heading * moveSpeed / 3.0f;

            // how far we are jumping up
            float difference = targetY - transform.position.y;

            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);
        }
    }

    void FallDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y <= target.y)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;

            Vector3 p = transform.position;
            p.y = target.y;

            transform.position = p;

            velocity = new Vector3();
        }
    }

    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;

        if (transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;
        }
    }

    /// Move us to the edge of a tile
    void MoveToEdge()
    {
        if (Vector3.Distance(transform.position, jumpTarget) >= 0.05f)
        {
            SetHorizontalVelocity();
        }
        else
        // One we reach the edge
        {
            movingEdge = false;
            fallingDown = true;

            velocity /= 4.0f;

            // lil' boost upward
            velocity.y = 1.5f;
        }
    }

    protected void FindPath(Tile target)
    {
        ComputeAdjacencyLists (jumpHeight, target);
        GetCurrentTile();

        // Any tile that has not been processed
        List<Tile> openList = new List<Tile>();

        // All the tiles that have been processed
        List<Tile> closedList = new List<Tile>();

        openList.Add (currentTile);
        currentTile.h =
            Vector3
                .Distance(currentTile.transform.position,
                target.transform.position);
        currentTile.f = currentTile.h;

        while (openList.Count > 0)
        {
            Tile t = FindLowestF(openList);

            //1. Do not process the tile again
            closedList.Add (t);

            if (t == target)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile (actualTargetTile);
                return;
            }

            foreach (Tile tile in t.adjacencyList)
            {
                if (closedList.Contains(tile))
                {
                    // Do nothing, already processed
                } // If the tile is in the open list, maybe there is a faster way to go to the target ?
                else if (openList.Contains(tile))
                {
                    // compare g scores
                    float tempG =
                        t.g +
                        Vector3
                            .Distance(tile.transform.position,
                            t.transform.position);

                    // if it's less we found a faster way
                    if (tempG < tile.g)
                    {
                        tile.parent = t;

                        tile.g = tempG;
                        tile.f = tile.g + tile.h;
                    }
                }
                else
                {
                    // this is the first time wee see this node so we add its parent
                    tile.parent = t;

                    // then we calculate the cost
                    tile.g =
                        t.g +
                        Vector3
                            .Distance(tile.transform.position,
                            t.transform.position);
                    tile.h =
                        Vector3
                            .Distance(tile.transform.position,
                            target.transform.position);

                    tile.f = tile.g + tile.h;

                    openList.Add (tile);
                }
            }
        }

        // TODO: What do you do if there is no path to the target tile ?
        Debug.Log("Path not found");
    }

    protected Tile FindEndTile(Tile t)
    {
        Stack<Tile> tempPath = new Stack<Tile>();

        Tile next = t.parent;

        while (next != null)
        {
            tempPath.Push (next);
            next = next.parent;
        }

        if (tempPath.Count <= move)
        {
            return t.parent;
        }

        Tile endTile = null;

        for (int i = 0; i <= move; i++)
        {
            endTile = tempPath.Pop();
        }

        return endTile;
    }

    protected Tile FindLowestF(List<Tile> tiles)
    {
        Debug.Log("FindLowestF");
        Debug.Log("tiles : " + tiles.Count);
        Tile lowest = tiles[0];

        foreach (Tile t in tiles)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        tiles.Remove (lowest);

        return lowest;
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
    }
}

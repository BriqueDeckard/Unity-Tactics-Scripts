using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

public class TacticsAction : MonoBehaviour
{
    protected List<Tile> selectableTiles = new List<Tile>();

    protected List<Tile> attackableTiles = new List<Tile>();

    public TacticsHealth health;

    /// <summary>
    /// All the tilesMap of the map
    /// </summary>
    GameObject[] tilesMap;

    Stack<Tile> path = new Stack<Tile>();

    Stack<Tile> shootingZone = new Stack<Tile>();

    protected Tile currentTile;

    /// <summary>
    /// Move capability of the player
    /// </summary>
    public int move = 5;

    public int attackRange = 6;

    public int firepower = 10;

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
    /// Is the player firing ?
    /// </summary>
    public bool firing = false;

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
        // TODO: 3
        Debug.Log("4. TacticsAction.Init() - BEGIN");
        tilesMap = GameObject.FindGameObjectsWithTag("Tile");

        // Debug.Log(tilesMap.Length + " tilesMap.");
        halfHeight = GetComponent<Collider>().bounds.extents.y;

        // add ourselves to the turn manager (to the dictionnary)
        TurnManager.AddUnit(this);
        Debug.Log("9. TacticsAction.Init() - END");
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
    public void ComputeAdjacencyListsForMoving(float jumpHeight, Tile target)
    {
        // For each GO of the map
        foreach (GameObject tile in tilesMap)
        {
            // Check if this is a tile
            Tile t = tile.GetComponent<Tile>();

            // else find the neighbors
            t.FindNeighborsForMoving (jumpHeight, target);
        }
    }

    public void FindSelectableTiles()
    {
        ComputeAdjacencyListsForMoving(jumpHeight, null);

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
                foreach (Tile tile in t.adjacencyListForMoving)
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

    public void ComputeAdjacencyListsForAttack(float jumpHeight, Tile target)
    {
        // For each tile of the map
        foreach (GameObject tile in tilesMap)
        {
            // Check if this is a tile
            Tile t = tile.GetComponent<Tile>();

            // else find the neighbors
            t.FindNeighborsForShooting (jumpHeight, target);
        }
    }

    public void FindAttackableTiles()
    {
        ComputeAdjacencyListsForAttack(jumpHeight, null);

        GetCurrentTile();

        Queue<Tile> process = new Queue<Tile>();

        process.Enqueue (currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();

            attackableTiles.Add (t);
            t.attackable = true;

            if (t.distance < attackRange)
            {
                foreach (Tile tile in t.adjacencyListForShooting)
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

    protected void RemoveAttackableTiles()
    {
        // Debug.Log("TacticsAction.RemoveAttackableTiles() - BEGIN");
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        foreach (Tile tile in attackableTiles)
        {
            tile.Reset();
        }
        attackableTiles.Clear();
        // Debug.Log("TacticsAction.RemoveAttackableTiles() - END");
    }

    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;

        // We set moving to true so that the "update" method of the current team (player or NPC) can launch the "Move()" method.
        moving = true;

        // -- CREATE THE PATH
        // We start from the target tile.
        Tile next = tile;
        while (next != null)
        {
            // we add the tile to the path
            path.Push (next);

            // and the parent tile becomes the next one, to start again until there are no more parent tilesMap, and the path has been traced.
            next = next.parent;
        }
    }

    public void ShootTheTile(Tile tile)
    {
        Debug.Log("TacticsAction.ShootTheTile() - BEGIN");
        tile.target = true;

        firing = true;

        // -- CREATE THE SHOOTING ZONE
        Tile targetForShoot = tile;

        shootingZone.Push (targetForShoot);
        Debug.Log("TacticsAction.ShootTheTile() - END");
    }

    public void Shoot()
    {
        //Debug.Log("Shoot");

        if (shootingZone.Count > 0)
        {
            //Debug.Log("FIRE ! ");
            Tile t = shootingZone.Peek();

            Collider collider = t.FindTargetOnTopOfTheTile();
            Debug.Log("Collider: " + collider == null + " " + collider);
            if (collider == null)
            {
                return;
            }

            TacticsHealth health = collider.GetComponent<TacticsHealth>();
            health.health = health.health - firepower;
        }

        firing = false;

        // End the turn
        TurnManager.EndTurn();
    }

    /// <summary>
    /// Move() is called at each frame if the boolean "moving" is true.
    /// </summary>
    public void Move()
    {
        // If a path exists (defined by "MoveToTile()")
        if (path.Count > 0)
        {
            // We create a vector that corresponds to the position of the tile that is on top of the stack
            // Debug.Log("TacticsAction.Move() - path.Count: " + path.Count);
            Tile t = path.Peek();
            Vector3 target = t.transform.position;

            // Calculate the unit's position on top of the target tile.
            target.y +=
                halfHeight + t.GetComponent<Collider>().bounds.extents.y;

            // If there is a significant distance between the position of the current unit and the target tile
            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                // Check if there is a difference in height to know if you should jump
                bool jump = transform.position.y != target.y;

                if (jump)
                {
                    Jump (target);
                }
                else
                {
                    // Calculate and normalize the direction to take.
                    CalculateHeadingTo (target);

                    // Set velocity (direction * moveSpeed)
                    SetHorizontalVelocity();
                }

                // Locomotion
                // Set the direction of this unit
                transform.forward = heading;

                // Initiate the movement of this unit
                transform.position += velocity * Time.deltaTime;
            }
            else
            // The distance between this unit and the target tile is not significant
            {
                // Tile center reached
                transform.position = target;

                // we don't need that tile in the stacking
                path.Pop();
            }
        }
        else
        // If a path does not exist (has not been defined by MoveToTile)
        {
            // Delete the display of the accessible tilesMap.
            RemoveSelectableTiles();

            // define moving to false to give back the hand to this unit
            moving = false;

            // End the turn
            TurnManager.EndTurn();
        }
    }

    public void RemoveSelectableTiles()
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

        // Makes this vector have a magnitude of 1.
        // When normalized, a vector keeps the same direction but its length is 1.0.
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
        ComputeAdjacencyListsForMoving (jumpHeight, target);
        GetCurrentTile();

        // Any tile that has not been processed
        List<Tile> openList = new List<Tile>();

        // All the tilesMap that have been processed
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

            foreach (Tile tile in t.adjacencyListForMoving)
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

    protected Tile FindLowestF(List<Tile> tilesMap)
    {
        // Debug.Log("TacticsAction.FindLowestF()");
        // Debug.Log("tilesMap : " + tilesMap.Count);
        Tile lowest = tilesMap[0];

        foreach (Tile t in tilesMap)
        {
            if (t.f < lowest.f)
            {
                lowest = t;
            }
        }

        tilesMap.Remove (lowest);

        return lowest;
    }

    public void BeginTurn()
    {
        Debug.Log("18. TacticsAction.BeginTurn() - BEGIN");
        turn = true;
        Debug.Log("19. TacticsAction.BeginTurn() - END");
    }

    public void EndTurn()
    {
        Debug.Log("24. TacticsAction.EndTurn() - BEGIN");
        turn = false;
        Debug.Log("25. TacticsAction.EndTurn() - END");
    }
}

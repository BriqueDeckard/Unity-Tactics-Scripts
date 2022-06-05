using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player movement
/// </summary>
public class PlayerAction : TacticsAction
{
    public static bool displayMovingForPlayer = true;

    public static bool displayFiringForPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("3. PlayerAction.Start() - BEGIN");
        Init();
        Debug.Log("10. PlayerAction.Start() - END");
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

        if (!moving && !firing)
        {
            if (displayMovingForPlayer)
            {
                FindSelectableTiles();
            }
            else if (!displayMovingForPlayer)
            {
                RemoveSelectableTiles();
            }
            if (displayFiringForPlayer)
            {
                FindAttackableTiles();
            }
            else if (!displayFiringForPlayer)
            {
                RemoveAttackableTiles();
            }

            CheckMouse();
        }
        else if (moving)
        {
            Move();
        }
    }

    /// <summary>
    /// Check the mouse button
    /// </summary>
    void CheckMouse()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse click");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>();

                    if (t.selectable && !t.hasSomethingOnIt)
                    {
                        MoveToTile (t);
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player movement
/// </summary>
public class PlayerMove : TacticsMove
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
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
            FindSelectableTiles();
            CheckMouse();
        }
        else
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

                    if (t.selectable)
                    {
                        MoveToTile (t);
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHealth : TacticsHealth
{
    // Start is called before the first frame update
    void Start()
    {
        health = 50;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Debug.Log("DEAD");
            NPCAction nPCAction = GetComponent<NPCAction>();
            nPCAction.RemoveSelectableTiles();
            Destroy(transform.gameObject);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Select stage
                if (hit.transform.name == "NPC")
                {
                    if (PlayerAction.displayFiringForPlayer)
                    {
                        Debug.Log("NPC.Hey !");
                    }
                    else
                    {
                        Debug.Log("NPC.Ho ! ");
                    }
                }
            }
        }

        if (attackable)
        {
            Debug.Log("NPC is Attackable");
            Material redMaterial =
                Resources.Load("red", typeof (Material)) as Material;
            gameObject.GetComponent<Renderer>().material = redMaterial;
        }
    }
}

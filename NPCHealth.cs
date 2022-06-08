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
            Destroy(transform.gameObject);
        }
    }
}

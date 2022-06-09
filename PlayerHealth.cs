using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : TacticsHealth
{
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackable)
        {
            Debug.Log("Attackable");
            Material redMaterial =
                Resources.Load("red", typeof (Material)) as Material;
            gameObject.GetComponent<Renderer>().material = redMaterial;
        }
    }
}

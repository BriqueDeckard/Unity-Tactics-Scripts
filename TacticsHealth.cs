using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsHealth : MonoBehaviour
{
    public int health;

    public bool attackable = false;

    // Start is called before the first frame update
    void Start()
    {
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

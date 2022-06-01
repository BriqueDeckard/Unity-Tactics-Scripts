using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : TacticsShoot
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(!turn){
            // don't fire or don't allow to select
            return;
        }
        
    }
}

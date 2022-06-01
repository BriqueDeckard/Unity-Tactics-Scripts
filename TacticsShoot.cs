using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsShoot : TurnBasedSystem
{

    protected List<Tile> accessibleToShootingTiles = new List<Tile>();

    /// <summary>
    /// All the tiles of the map
    /// </summary>
    GameObject[] tiles;

    new protected void Init(){
        base.Init();
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        Debug.Log("INFO: " + tiles.Length + " tiles.");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

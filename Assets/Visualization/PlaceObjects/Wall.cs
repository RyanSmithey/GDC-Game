using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public int WallID = 0;

    // Update is called once per frame
    void Update()
    {
        uint[] Loc = Abstract.AllWalls[WallID].Location;

        gameObject.transform.position = Abstract.Grid[Loc[0], Loc[1]].Location;
    }
}

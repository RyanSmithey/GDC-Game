using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OverallManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Wall;

    private List<GameObject> PlayerObjects;
    private List<GameObject> WallObjects;

    void Start()
    {
        Abstract.GenGridLocations();
        this.GetComponent<MeshFilter>().mesh = Abstract.GenerateMesh();

        PlayerObjects = new List<GameObject>();
        WallObjects = new List<GameObject>();
    }
    
    void Update()
    {
        if (Abstract.AllPlayers != null)
        {
            if (CountPlayers() != PlayerObjects.Count)
            {
                DeletePlayers();
                SpawnPlayers();
            }
        }
        if (Abstract.AllWalls != null)
        {
            if (CountWalls() != WallObjects.Count)
            {
                DeleteWalls();
                SpawnWalls();
            }
        }


        Abstract.CleanGrid();
        Abstract.SetPlayerLocations();
    }

    public void SpawnPlayers()
    {
        PlayerObjects = new List<GameObject>();

        int i = 0;
        int j = 0;
        foreach (Abstract.Player P1 in Abstract.AllPlayers)
        {
            if (P1.Health > 0)
            {
                PlayerObjects.Add(Instantiate(Player));
                PlayerObjects[j].GetComponent<Player>().PlayerID = i;

                PlayerObjects[j].transform.parent = this.transform;

                j++;
            }

            i++;
        }
    }
    public void DeletePlayers()
    {
        foreach (GameObject PO in PlayerObjects)
        {
            Destroy(PO);
        }
    }
    public int CountPlayers()
    {
        int Final = 0;

        foreach (Abstract.Player P1 in Abstract.AllPlayers)
        {
            if (P1.Health > 0)
            {
                Final += 1;
            }
        }
        return Final;
    }

    public void SpawnWalls()
    {
        WallObjects = new List<GameObject>();

        int i = 0;
        foreach (Abstract.Wall W1 in Abstract.AllWalls)
        {
            WallObjects.Add(Instantiate(Wall));
            WallObjects[i].GetComponent<Wall>().WallID = i;

            WallObjects[i].transform.parent = this.transform;

            i++;
        }
    }
    public void DeleteWalls()
    {
        foreach (GameObject WO in WallObjects)
        {
            Destroy(WO);
        }
    }
    public int CountWalls()
    {
        return Abstract.AllWalls.Count;
    }
}

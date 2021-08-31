using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Mirror;

public class NetworkFunctions : NetworkBehaviour
{
    public static NetworkFunctions Self;

    private DateTime StartTime;
    int NumIPCSAdded = 0;

    [SerializeField] private GameObject ToggleJoin;

    void Start()
    {
        Self = this;

        if (isServer)
        {
            if (File.ReadAllText(Application.dataPath + "/SaveFiles/TimeFile.txt") == "")
            {
                StartTime = DateTime.Now;
                SaveTimeData();
            }
            else { LoadTimeData(); }

            ToggleJoin.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            double TimeElapsed = DateTime.Now.Subtract(StartTime).TotalHours;

            if (TimeElapsed > 24.0 * (NumIPCSAdded + 1))
            {
                Abstract.AddIPC();
                Abstract.AddVoteIPC();
                NumIPCSAdded += 1;
                Abstract.SaveData();
                SaveTimeData();
            }

            //RpcUpdateClientPlayers(Abstract.AllPlayers);
        }
    }

    void OnApplicationQuit()
    {
        Abstract.SaveData();
    }

    //[ClientRpc]
    //public void RpcUpdateClientPlayers(List<Abstract.Player> NewPlayerData)
    //{
    //    Abstract.AllPlayers = NewPlayerData;
    //}
    [ClientRpc]
    public void RpcUpdateintValue(int Player, int value, int PropertyIndex)
    {
        if (PropertyIndex == 0) { Abstract.AllPlayers[Player].Health = value; }
        if (PropertyIndex == 1) { Abstract.AllPlayers[Player].ActionPoints = value; }
        if (PropertyIndex == 2) { Abstract.AllPlayers[Player].Location[0] = (uint)value; }
        if (PropertyIndex == 3) { Abstract.AllPlayers[Player].Location[1] = (uint)value; }
        if (PropertyIndex == 4) { Abstract.AllPlayers[Player].Vote = value; }
        if (PropertyIndex == 5) { Abstract.AllPlayers[Player].Range = value; }
        if (PropertyIndex == 6) { Abstract.AllPlayers[Player].FreeMovesPerDay = value; }
        if (PropertyIndex == 7) { Abstract.AllPlayers[Player].IPCsPerTurn = value; }
        if (PropertyIndex == 8) { Abstract.AllPlayers[Player].FreeMoves = value; }
    }
    [ClientRpc]
    public void RpcAddPlayer(Abstract.Player Player)
    {
        if (isServer)
        {
            return;
        }
        Abstract.AllPlayers.Add(Player);
    }
    [ClientRpc]
    public void RpcAddWall(Abstract.Wall W)
    {
        Abstract.AllWalls.Add(W);
    }
    [ClientRpc]
    public void RpcRemoveWall(int WallIndex)
    {
        Abstract.AllWalls.RemoveAt(WallIndex);
    }

    public void SaveAllData()
    {
        Abstract.NetworkedPlayer.SaveGame();
    }
    
    public void GetServerLog()
    {
        Abstract.NetworkedPlayer.CmdGetServerLog();
    }

    public void SaveTimeData()
    {
        string Path = Application.dataPath + "/SaveFiles/TimeFile.txt";
        File.WriteAllText(Path, StartTime.ToString() + "\n" + NumIPCSAdded.ToString());
    }
    void LoadTimeData()
    {
        string filePath = Application.dataPath + "/SaveFiles/TimeFile.txt";
        string[] lines = File.ReadAllLines(filePath);

        StartTime = DateTime.Parse(lines[0]);
        NumIPCSAdded = Int32.Parse(lines[1]);
    }
    public static void ClearData()
    {
        string Path = Application.dataPath + "/SaveFiles/TimeFile.txt";
        File.WriteAllText(Path, "");
    }
}

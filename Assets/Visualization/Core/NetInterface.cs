using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetInterface : NetworkBehaviour
{
    void Start()
    {
        if (isClient && hasAuthority)
        {
            Abstract.NetworkedPlayer = this;

            //It's a client
            Abstract.LoginRequest LR = new Abstract.LoginRequest();
            LR.Name = Cursor.Name;
            LR.PlayerID = PFPlayerStats.MyPlayfabID;
            LR.SkinColor = Cursor.SkinColor;
            LR.SkinName = Cursor.SkinName;

            Debug.Log(LR.SkinColor);

            CmdRequestPlayerData();
            CmdRequestWallData();
            CmdSignIn(LR, PFPlayerStats.SessionID);
        }
    }
    #region PlayerControls
    [Command] public void CmdTryAttackPlayer(int Player, uint[] NewLocation, string SessionID)
    {
        Abstract.TryAttackPlayer(Player, NewLocation, SessionID);
    }
    [Command] public void CmdTryAttackWall(int Player, uint[] NewLocation, string SessionID)
    {
        Abstract.TryAttackWall(Player, NewLocation, SessionID);
    }
    [Command] public void CmdTryMovePlayer(int Player, uint[] NewLocation, string SessionID)
    {
        Abstract.TryMovePlayer(Player,NewLocation, SessionID);
    }
    [Command] public void CmdTryGivePlayer(int Player, uint[] NewLocation, string SessionID)
    {
        Abstract.TryGivePlayer(Player, NewLocation, SessionID);
    }
    [Command] public void CmdTryVotePlayer(int DeadPlayer, int AlivePlayer, string SessionID)
    {
        Abstract.TryVotePlayer(DeadPlayer, AlivePlayer, SessionID);
    }
    [Command] public void CmdTryAddWall(int Player, uint[] NewLocation, string SessionID)
    {
        Abstract.TryPlaceWall(Player, NewLocation, SessionID);
    }
    [Command] public void CmdTryUpgradeRange(int Player, string SessionID)
    {
        Abstract.TryUpgradeRange(Player, SessionID);
    }
    [Command] public void CmdTryUpgradeFreeMoves(int Player, string SessionID)
    {
        Abstract.TryUpgradeFreeMoves(Player, SessionID);
    }
    [Command] public void CmdTryUpgradeIPCS(int Player, string SessionID)
    {
        Abstract.TryUpgradeIPCSPerTurn(Player, SessionID);
    }
    #endregion

    #region SignIn
    [Command] public void CmdSignIn(Abstract.LoginRequest LR, string SessionID)
    {
        Abstract.Login(LR, SessionID); 
    }

    [Command] public void CmdRequestPlayerData()
    {
        TargetSendPlayerData(Abstract.AllPlayers);
    }
    [Command] public void CmdRequestWallData()
    {
        TargetSendWallData(Abstract.AllWalls);
    }
    #endregion

    [Command] public void SaveGame()
    {
        Abstract.SaveData();
    }
    [Command] public void CmdGetServerLog()
    {
        string Path = Application.dataPath + "/ServerLog.txt";

        RpcSendServerLog(File.ReadAllText(Path));
    }

    [TargetRpc] public void TargetSendPlayerData(List<Abstract.Player> PlayerData)
    {
        Abstract.AllPlayers = PlayerData;
    }
    [TargetRpc] public void TargetSendWallData(List<Abstract.Wall> WallData)
    {
        Abstract.AllWalls = WallData;
    }

    [ClientRpc] public void RpcSendServerLog(string AllText)
    {
        string Path = Application.dataPath + "/ServerLog.txt";

        if (isClient && hasAuthority)
        {
            File.WriteAllText(Path, AllText);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using PlayFab;
#if ENABLE_PLAYFABSERVER_API
using PlayFab.ServerModels;
#endif

//Range Static Cost = 2
//Free Move Static Cost = 1
//IPC static cost = 3

public static class Abstract
{
    //
    public static NetInterface NetworkedPlayer;
    public static bool IsAcceptingPlayers = true;

    public static float Width = 10;
    public static float WallWidth = 0.25f;

    public static int XSize = 10;
    public static int YSize = 10;
    
    public static List<Player> AllPlayers;
    public static List<Wall> AllWalls;
    public static GridSlot[,] Grid;

    public static List<string> SessionTickets;

    private static string LogPath = Application.dataPath + "/SaveFiles/ServerLog.txt";

    public class Player
    {
        //Identification
        public string Name;
        public string PlayerID;
        public string SkinName;
        public Color SkinColor;

        //Update 1 Core Values
        public int Health = 2;
        public int ActionPoints = 0;
        public uint[] Location;
        public int Vote = -1;

        //Update 2 Upgradable stats
        public int Range = 1;
        public int FreeMovesPerDay = 1;
        public int IPCsPerTurn = 1;
        public int FreeMoves = 1;
    }
    public class Wall
    {
        public uint[] Location;
    }
    public class GridSlot
    {
        public int Player = -1;
        public int Wall = -1;
        public Vector3 Location;
    }

    public class LoginRequest
    {
        public string Name;
        public string PlayerID;
        public string SkinName;
        public Color SkinColor;
    }

//#if ENABLE_PLAYFABSERVER_API
    public static void Login(LoginRequest LR, string SessionID)
    {
        var request = new AuthenticateSessionTicketRequest() { SessionTicket = SessionID };
        PlayFabServerAPI.AuthenticateSessionTicket(request, OnAuthenticateSuccess, OnAuthenticateFailure);

        void OnAuthenticateSuccess(AuthenticateSessionTicketResult result)
        {
            //Figure out which Player Is Effected
            int i = 0;
            int PlayerIndex = -1;
            foreach (Player P in AllPlayers)
            {
                if (P.PlayerID == result.UserInfo.PlayFabId)
                {
                    PlayerIndex = i;
                }
                i++;
            }
            if (PlayerIndex == -1)
            {
                //Create New Player
                SessionTickets.Add(SessionID);
                AddPlayer(LR);
            }
            else
            {
                //UpdateExisting Info if player Exists
                SessionTickets[PlayerIndex] = SessionID;
            }
        }
        void OnAuthenticateFailure(PlayFabError error)
        {
            Debug.LogError(error.GenerateErrorReport());
        }
    }
//#endif
    private static void AddPlayer(LoginRequest LR)
    {
        if (IsAcceptingPlayers)
        {
            Player P1 = new Player();

            P1.Name = LR.Name;
            P1.PlayerID = LR.PlayerID;
            P1.SkinName = LR.SkinName;
            P1.SkinColor = LR.SkinColor;

            foreach (Player P2 in AllPlayers)
            {
                if (P1.PlayerID == P2.PlayerID) { return; }
            }

            bool failed = true;
            while (failed)
            {
                failed = false;
                P1.Location = new uint[] { (uint)Mathf.CeilToInt(UnityEngine.Random.Range(-0.99999f, Width - 1.000001f)), (uint)Mathf.CeilToInt(UnityEngine.Random.Range(-0.99999f, Width - 1.000001f)) };
                foreach (Player P2 in AllPlayers)
                {
                    if (P2.Location[0] == P1.Location[0] && P2.Location[1] == P1.Location[1]) { failed = true; }
                }
                foreach (Wall W in AllWalls)
                {
                    if (W.Location[0] == P1.Location[0] && W.Location[1] == P1.Location[1]) { failed = true; }
                }
            }

            DateTime Time = DateTime.Now;
            File.AppendAllText(LogPath, P1.Name + " Joined " +
                    " Time: " + Time.ToString() +
                    "\n");

            AllPlayers.Add(P1);
            NetworkFunctions.Self.RpcAddPlayer(P1);
        }
    }

    public static void GenGridLocations()
    {
        Grid = new GridSlot[XSize, YSize];

        for (int i = 0; i < XSize; i++)
        {
            for (int j = 0; j < YSize; j++)
            {
                Vector2 Offset = Vector2.zero;
                Offset.x = Width / (XSize * 2.0f);
                Offset.y = Width / (YSize * 2.0f);
                Grid[i, j] = new GridSlot();

                Grid[i, j].Location = new Vector3((1 + (i * 2)) * Offset.x - Width / 2.0f, 
                                                  (1 + (j * 2)) * Offset.y - Width / 2.0f,
                                                   0);
            }
        }
    }

    public static Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();

        List<Vector3> Verticies = new List<Vector3>();
        List<int> Triangles = new List<int>();

        int Index = 0;
        for (int i = 0; i < XSize + 1; i++)
        {
            Verticies.Add(new Vector3(-(Width * 0.5f) - WallWidth * 0.5f + (i * (Width / XSize)), -Width * 0.5f, 0.0f));
            Verticies.Add(new Vector3(-(Width * 0.5f) + WallWidth * 0.5f + (i * (Width / XSize)), -Width * 0.5f, 0.0f));

            Verticies.Add(new Vector3(-(Width * 0.5f) - WallWidth * 0.5f + (i * (Width / XSize)), Width * 0.5f, 0.0f));
            Verticies.Add(new Vector3(-(Width * 0.5f) + WallWidth * 0.5f + (i * (Width / XSize)), Width * 0.5f, 0.0f));

            Triangles.Add(Index + 2);
            Triangles.Add(Index + 1);
            Triangles.Add(Index);

            Triangles.Add(Index + 1);
            Triangles.Add(Index + 2);
            Triangles.Add(Index + 3);

            Index += 4;
        }

        for (int i = 0; i < YSize + 1; i++)
        {
            Verticies.Add(new Vector3(-Width * 0.5f, -(Width * 0.5f) - WallWidth * 0.5f + (i * (Width / YSize)), 0.0f));
            Verticies.Add(new Vector3(-Width * 0.5f, -(Width * 0.5f) + WallWidth * 0.5f + (i * (Width / YSize)), 0.0f));

            Verticies.Add(new Vector3(Width * 0.5f, -(Width * 0.5f) - WallWidth * 0.5f + (i * (Width / YSize)), 0.0f));
            Verticies.Add(new Vector3(Width * 0.5f, -(Width * 0.5f) + WallWidth * 0.5f + (i * (Width / YSize)), 0.0f));

            Triangles.Add(Index);
            Triangles.Add(Index + 1);
            Triangles.Add(Index + 2);

            Triangles.Add(Index + 3);
            Triangles.Add(Index + 2);
            Triangles.Add(Index + 1);

            Index += 4;
        }

        mesh.vertices = Verticies.ToArray();
        mesh.triangles = Triangles.ToArray();

        return mesh;
    }

    public static void CleanGrid()
    {
        for (int i = 0; i < Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Grid.GetLength(1); j++)
            {
                Grid[i, j].Player = -1;
                Grid[i, j].Wall = -1;
            }
        }
    }

    public static void SetPlayerLocations()
    {
        int i = 0;
        foreach (Player P1 in AllPlayers)
        {
            if (P1.Health <= 0)
            {
                Grid[P1.Location[0], P1.Location[1]].Player = -1;
            }
            else
            {
                Grid[P1.Location[0], P1.Location[1]].Player = i;
            }
            i++;
        }

        i = 0;
        foreach (Wall W in AllWalls)
        {
            Grid[W.Location[0], W.Location[1]].Wall = i;
            i++;
        }
    }

    //Server Functions that are active
    public static void AddIPC()
    {

        int i = 0;
        foreach (Player P1 in AllPlayers)
        {
            P1.ActionPoints += P1.IPCsPerTurn;
            NetworkFunctions.Self.RpcUpdateintValue(i, AllPlayers[i].ActionPoints, 1);

            P1.FreeMoves = P1.FreeMovesPerDay;
            NetworkFunctions.Self.RpcUpdateintValue(i, AllPlayers[i].FreeMoves, 8);

            i++;
        }
        SaveData();
    }
    public static void AddVoteIPC()
    {
        List<int> PlayerID = new List<int>();
        List<int> Count = new List<int>();

        //Tally Votes
        foreach (Player P1 in AllPlayers)
        {
            if (P1.Vote == -1) { continue; }
            else if (PlayerID.Contains(P1.Vote))
            {
                for (int i = 0; i < PlayerID.Count; i++)
                {
                    if (PlayerID[i] == P1.Vote)
                    {
                        Count[i] += 1;
                    }
                }
            }
            else
            {
                PlayerID.Add(P1.Vote);
                Count.Add(1);
            }
        }

        if (PlayerID.Count == 0) { return; }

        int Max = 0;
        for (int i = 0; i < Count.Count; i++) { if (Count[i] > Max) { Max = Count[i]; } }
        List<int> MaxValues = new List<int>();

        //Get list of tied people
        for (int i = 0; i < PlayerID.Count; i++)
        {
            if (Count[i] == Max) { MaxValues.Add(PlayerID[i]); }
        }

        //Randomly Select Winner
        int LuckyPlayer = Mathf.CeilToInt(UnityEngine.Random.Range(-0.5f, -0.5f + MaxValues.Count - 1.0f));
        AllPlayers[LuckyPlayer].ActionPoints += 1;
        
        //Reset Everyones vote
        foreach (Player P1 in AllPlayers)
        {
            P1.Vote = -1;
        }
    }

    public static void TryAttackPlayer(int Player, uint[] NewLocation, string SessionID)
    {
        if (SessionTickets[Player] != SessionID) { return; }

        uint[] Playerlocation = AllPlayers[Player].Location;

        if (AllPlayers[Player].ActionPoints > 0 && AllPlayers[Player].Health > 0
            && Grid[NewLocation[0], NewLocation[1]].Player != -1
            && Cursor.Range[0] >= Mathf.Abs((int)Playerlocation[0] - (int)NewLocation[0]) + Mathf.Abs((int)Playerlocation[1] - (int)NewLocation[1])
            )
        {
            AllPlayers[Player].ActionPoints -= 1;
            AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].Health -= 1;

            if (AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].Health <= 0)
            {
                AllPlayers[Player].ActionPoints += AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].ActionPoints;
                AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].ActionPoints = 0;

                NetworkFunctions.Self.RpcUpdateintValue(Grid[NewLocation[0], NewLocation[1]].Player, AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].ActionPoints, 1);
            }

            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].ActionPoints, 1);
            NetworkFunctions.Self.RpcUpdateintValue(Grid[NewLocation[0], NewLocation[1]].Player, AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].Health, 0);

            DateTime Time = DateTime.Now;

            File.AppendAllText(LogPath, AllPlayers[Player].Name + " Attacked " + AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].Name +
                " Time: " + Time.ToString() + 
                "\n");

            SaveData();
        }
    }
    public static void TryAttackWall(int Player, uint[] NewLocation, string SessionID)
    {
        if (SessionTickets[Player] != SessionID) { return; }

        //This function requires a host player to function properly THis is bad as it does a poor job of allowing

        uint[] Playerlocation = AllPlayers[Player].Location;

        if (AllPlayers[Player].ActionPoints > 0 && AllPlayers[Player].Health > 0
            && Grid[NewLocation[0], NewLocation[1]].Wall != -1
            && AllPlayers[Player].Range >= Mathf.Abs((int)Playerlocation[0] - (int)NewLocation[0]) + Mathf.Abs((int)Playerlocation[1] - (int)NewLocation[1])
            )
        {
            AllPlayers[Player].ActionPoints -= 1;

            //Update Clients
            NetworkFunctions.Self.RpcRemoveWall(Grid[NewLocation[0], NewLocation[1]].Wall);
            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].ActionPoints, 1);
            //Correct if Only Server
            if (PFSignIn.IsServer)
            {
                AllWalls.RemoveAt(Grid[NewLocation[0], NewLocation[1]].Wall);
            }

            SaveData();
        }
    }
    public static void TryMovePlayer(int Player, uint[] NewLocation, string SessionID)
    {
        if (SessionTickets[Player] != SessionID) { return; }

        uint[] Playerlocation = AllPlayers[Player].Location;

        if (AllPlayers[Player].Health <= 0) { return; }
        if (Grid[NewLocation[0], NewLocation[1]].Player != -1) { return; }
        if (Grid[NewLocation[0], NewLocation[1]].Wall != -1) { return; }
        if (1 != Mathf.Abs((int)Playerlocation[0] - (int)NewLocation[0]) + Mathf.Abs((int)Playerlocation[1] - (int)NewLocation[1])) { return; }
        if (AllPlayers[Player].ActionPoints + AllPlayers[Player].FreeMoves <= 0) { return; }

        if (AllPlayers[Player].FreeMoves > 0)
        {
            AllPlayers[Player].FreeMoves -= 1;
            AllPlayers[Player].Location = NewLocation;

            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].FreeMoves, 8);
        }
        else 
        {
            AllPlayers[Player].ActionPoints -= 1;
            AllPlayers[Player].Location = NewLocation;

            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].ActionPoints, 1);
        }

        NetworkFunctions.Self.RpcUpdateintValue(Player, (int)AllPlayers[Player].Location[0], 2);
        NetworkFunctions.Self.RpcUpdateintValue(Player, (int)AllPlayers[Player].Location[1], 3);

        DateTime Time = DateTime.Now;

        File.AppendAllText(LogPath, AllPlayers[Player].Name + " Moved To " + NewLocation[0].ToString() + ", " + NewLocation[1].ToString() +
            " Time: " + Time.ToString() + "\n");

        SaveData();
    }
    public static void TryGivePlayer(int Player, uint[] NewLocation, string SessionID)
    {
        if (SessionTickets[Player] != SessionID) { return; }

        uint[] Playerlocation = AllPlayers[Player].Location;

        if (AllPlayers[Player].ActionPoints > 0 && AllPlayers[Player].Health > 0
            && Grid[NewLocation[0], NewLocation[1]].Player != -1
            && Cursor.Range[2] >= Mathf.Abs((int)Playerlocation[0] - (int)NewLocation[0]) + Mathf.Abs((int)Playerlocation[1] - (int)NewLocation[1])
            )
        {
            AllPlayers[Player].ActionPoints -= 1;
            AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].ActionPoints += 1;

            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].ActionPoints, 1);
            NetworkFunctions.Self.RpcUpdateintValue(Grid[NewLocation[0], NewLocation[1]].Player, AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].ActionPoints, 1);

            DateTime Time = DateTime.Now;

            File.AppendAllText(LogPath, AllPlayers[Player].Name + " Gave Token " + AllPlayers[Grid[NewLocation[0], NewLocation[1]].Player].Name +
                " Time: " + Time.ToString() +
                "\n");

            SaveData();
        }
    }
    public static void TryPlaceWall(int Player, uint[] NewLocation, string SessionID)
    {
        if (SessionTickets[Player] != SessionID) { return; }

        uint[] Playerlocation = AllPlayers[Player].Location;
        if (AllPlayers[Player].Health <= 0) { return; }
        if (Grid[NewLocation[0], NewLocation[1]].Player != -1) { return; }
        if (Grid[NewLocation[0], NewLocation[1]].Wall != -1) { return; }
        if (AllPlayers[Player].Range < Mathf.Abs((int)Playerlocation[0] - (int)NewLocation[0]) + Mathf.Abs((int)Playerlocation[1] - (int)NewLocation[1])) { return; }
        if (AllPlayers[Player].ActionPoints <= 0) { return; }
        
        Wall W = new Wall();
        W.Location = new uint[] { NewLocation[0], NewLocation[1]};

        AllPlayers[Player].ActionPoints -= 1;
        
        //Update Clients
        NetworkFunctions.Self.RpcAddWall(W);
        NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].ActionPoints, 1);
        //Correct Non Client Server
        if (PFSignIn.IsServer)
        {
            AllWalls.Add(W);
        }
        SaveData();
    }
    public static void TryVotePlayer(int DeadPlayer, int AlivePlayer, string SessionID)
    {
        if (SessionTickets[AlivePlayer] != SessionID) { return; }

        if (AlivePlayer == -1) { AllPlayers[DeadPlayer].Vote = -1; }
        else if (AllPlayers[DeadPlayer].Health <= 0 && AllPlayers[AlivePlayer].Health > 0)
        {
            AllPlayers[DeadPlayer].Vote = AlivePlayer;
        }
        
        NetworkFunctions.Self.RpcUpdateintValue(DeadPlayer, AlivePlayer, 4);

        SaveData();
    }

    public static void TryUpgradeRange(int Player, string SessionID)
    {
        if (SessionTickets[Player] != SessionID) { return; }

        if (AllPlayers[Player].Health > 0 && AllPlayers[Player].Range <= AllPlayers[Player].ActionPoints)
        {
            AllPlayers[Player].ActionPoints -= AllPlayers[Player].Range;
            AllPlayers[Player].Range += 1;

            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].ActionPoints, 1);
            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].Range, 5);

            SaveData();
        }
    }
    public static void TryUpgradeFreeMoves(int Player, string SessionID)
    {
        if (SessionTickets[Player] != SessionID) { return; }

        if (AllPlayers[Player].Health > 0 && AllPlayers[Player].FreeMovesPerDay <= AllPlayers[Player].ActionPoints)
        {
            AllPlayers[Player].ActionPoints -= AllPlayers[Player].FreeMovesPerDay;
            AllPlayers[Player].FreeMovesPerDay += 1;

            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].ActionPoints, 1);
            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].FreeMovesPerDay, 6);

            SaveData();
        }
    }
    public static void TryUpgradeIPCSPerTurn(int Player, string SessionID)
    {
        if (SessionTickets[Player] != SessionID) { return; }

        if (AllPlayers[Player].Health > 0 && AllPlayers[Player].IPCsPerTurn * 2 <= AllPlayers[Player].ActionPoints)
        {
            AllPlayers[Player].ActionPoints -= AllPlayers[Player].IPCsPerTurn * 2;
            AllPlayers[Player].IPCsPerTurn += 1;

            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].ActionPoints, 1);
            NetworkFunctions.Self.RpcUpdateintValue(Player, AllPlayers[Player].IPCsPerTurn, 7);

            SaveData();
        }
    }

    public static void LoadData()
    {
        AllPlayers = new List<Player>();
        AllWalls = new List<Wall>();
        SessionTickets = new List<string>();

        Player Temp;

        string Path = Application.dataPath + "/SaveFiles/SaveFile.txt";
        string[] AllLines = File.ReadAllLines(Path);

        for (int i = 0; i < AllLines.Length;i += 0)
        {
            Temp = new Player();
            Temp.Location = new uint[2];

            Temp.Name =                         AllLines[i];  i++;
            Temp.PlayerID =                     AllLines[i];  i++;
            Temp.SkinName =                     AllLines[i];  i++;
            Temp.SkinColor= ParseColor(         AllLines[i]); i++;
            Temp.Health = Int32.Parse(          AllLines[i]); i++;
            Temp.ActionPoints = Int32.Parse(    AllLines[i]); i++;
            Temp.Location[0] = UInt32.Parse(    AllLines[i]); i++;
            Temp.Location[1] = UInt32.Parse(    AllLines[i]); i++;
            Temp.Vote = Int32.Parse(            AllLines[i]); i++;
            Temp.Range = Int32.Parse(           AllLines[i]); i++;
            Temp.FreeMovesPerDay = Int32.Parse( AllLines[i]); i++;
            Temp.IPCsPerTurn = Int32.Parse(     AllLines[i]); i++;
            Temp.FreeMoves = Int32.Parse(       AllLines[i]); i++;

            AllPlayers.Add(Temp);
        }

        Path = Application.dataPath + "/SaveFiles/WallSave.txt";
        AllLines = File.ReadAllLines(Path);

        Wall W;

        for (int i = 0; i < AllLines.Length; i += 2)
        {
            W = new Wall();
            W.Location = new uint[2];

            W.Location[0] = UInt32.Parse(AllLines[i]);
            W.Location[1] = UInt32.Parse(AllLines[i + 1]);

            AllWalls.Add(W);
        }
    }
    public static void SaveData()
    {
        string Path = Application.dataPath + "/SaveFiles/SaveFile.txt";
        File.WriteAllText(Path, "");

        foreach (Player P1 in AllPlayers)
        {
            File.AppendAllText(Path, P1.Name                     + "\n");
            File.AppendAllText(Path, P1.PlayerID                 + "\n");
            File.AppendAllText(Path, P1.SkinName                 + "\n");
            File.AppendAllText(Path, P1.SkinColor.ToString()     + "\n");
            File.AppendAllText(Path, P1.Health.ToString()        + "\n");
            File.AppendAllText(Path, P1.ActionPoints.ToString()  + "\n");
            File.AppendAllText(Path, P1.Location[0].ToString()   + "\n");
            File.AppendAllText(Path, P1.Location[1].ToString()   + "\n");
            File.AppendAllText(Path, P1.Vote.ToString()          + "\n");
            File.AppendAllText(Path, P1.Range.ToString()         + "\n");
            File.AppendAllText(Path, P1.FreeMovesPerDay.ToString() + "\n");
            File.AppendAllText(Path, P1.IPCsPerTurn.ToString()   + "\n");
            File.AppendAllText(Path, P1.FreeMoves.ToString()     + "\n");
        }

        Path = Application.dataPath + "/SaveFiles/WallSave.txt";
        File.WriteAllText(Path, "");

        foreach (Wall W in AllWalls)
        {
            File.AppendAllText(Path, W.Location[0].ToString() + "\n");
            File.AppendAllText(Path, W.Location[1].ToString() + "\n");
        }
    }
    public static void ClearData()
    {
        string Path = Application.dataPath + "/SaveFiles/SaveFile.txt";
        File.WriteAllText(Path, "");

        Path = Application.dataPath + "/SaveFiles/WallSave.txt";
        File.WriteAllText(Path, "");
    }

    private static Color ParseColor(string C)
    {


        return new Color();
    }
}

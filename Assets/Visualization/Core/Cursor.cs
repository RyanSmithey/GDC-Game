using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cursor : MonoBehaviour
{
    public Camera Cam;

    public static  int ActivePlayer;
    public         int Setting = 0;
    public static int[] Range = new int[] { 2, 1, 2, 1000, 1000};

    public static int[] ActiveCell;
    #region First Sign In Only
    public static string Name;
    public static string SkinName;
    public static Color SkinColor;
    #endregion
    void Start()
    {
        if (PFSignIn.IsServer) { gameObject.SetActive(false); }
        Setting = 3;
    }

    void Update()
    {
        FindCorrectPlayer();
        Vector3 Position = FindGridPosition();
        
        if (ActivePlayer != -1)
        {
            if (Abstract.AllPlayers[ActivePlayer].Health <= 0) { Setting = 4; }

            if (EventSystem.current.IsPointerOverGameObject() || ActiveCell[0] < 0)
            {
                gameObject.transform.position = new Vector3(-1000, -1000, -1000);
            }
            else if (Abstract.Grid[ActiveCell[0], ActiveCell[1]].Player == ActivePlayer)
            {
                gameObject.transform.position = new Vector3(-1000, -1000, -1000);
            }
            else if (!IsInteractable())
            {
                gameObject.transform.position = Position;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
            }
            else
            {
                gameObject.transform.position = Position;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 0.5f);

                if (Input.GetMouseButtonDown(0))
                {
                    InteractWithCell();
                }
            }
        }
    }

    private Vector3 FindGridPosition()
    {
        Vector3 MouseLocation = Cam.ScreenToWorldPoint(Input.mousePosition);

        int[] Location = new int[] { -1, -1};
        float Distance = 0.5f;

        for (int i = 0; i < Abstract.Grid.GetLength(0); i++)
        {
            for (int j = 0; j < Abstract.Grid.GetLength(1); j++)
            {
                float NewDistance = SimpleDistance(Abstract.Grid[i, j].Location, MouseLocation);

                if (NewDistance < Distance)
                {
                    Location[0] = i;
                    Location[1] = j;
                    Distance = NewDistance;
                }
            }
        }

        ActiveCell = new int[] { -1, -1 };

        if (Location[0] < 0)
        {
            return new Vector3(-1000, -1000, -1000);
        }
        else
        {
            ActiveCell[0] = Location[0];
            ActiveCell[1] = Location[1];
            return Abstract.Grid[Location[0], Location[1]].Location - new Vector3(0.0f, 0.0f, 0.1f);
        }
    }

    private float SimpleDistance(Vector3 V1, Vector3 V2)
    {
        Vector3 V3 = V1 - V2;

        return Mathf.Abs(V3.x) + Mathf.Abs(V3.y);
    }

    private bool IsInteractable()
    {
        if (Setting == 0)
        {
            return (Abstract.Grid[ActiveCell[0], ActiveCell[1]].Player >= 0 || Abstract.Grid[ActiveCell[0], ActiveCell[1]].Wall >= 0) && 
                Abstract.AllPlayers[ActivePlayer].Range >=
                Mathf.Abs((int)Abstract.AllPlayers[ActivePlayer].Location[0] - (int)ActiveCell[0]) +
                Mathf.Abs((int)Abstract.AllPlayers[ActivePlayer].Location[1] - (int)ActiveCell[1]);
        }               //Attack Setting
        if (Setting == 1)
        {
            return  Abstract.Grid[ActiveCell[0], ActiveCell[1]].Player < 0 &&
                    Abstract.Grid[ActiveCell[0], ActiveCell[1]].Wall < 0 &&
                    1 ==
                    Mathf.Abs((int)Abstract.AllPlayers[ActivePlayer].Location[0] - (int)ActiveCell[0]) +
                    Mathf.Abs((int)Abstract.AllPlayers[ActivePlayer].Location[1] - (int)ActiveCell[1]); ;
        }               //Move Setting
        if (Setting == 2)
        {
            return Abstract.Grid[ActiveCell[0], ActiveCell[1]].Player >= 0 && Abstract.AllPlayers[ActivePlayer].Range >=
                Mathf.Abs((int)Abstract.AllPlayers[ActivePlayer].Location[0] - (int)ActiveCell[0]) +
                Mathf.Abs((int)Abstract.AllPlayers[ActivePlayer].Location[1] - (int)ActiveCell[1]);
        }               //Give Setting
        if (Setting == 3) { return true; } //Inspect Setting
        if (Setting == 4) { return true; } //Vote Setting
        if (Setting == 5) { return false; }//Upgrade Setting
        if (Setting == 6)
        {
            return Abstract.Grid[ActiveCell[0], ActiveCell[1]].Player + Abstract.Grid[ActiveCell[0], ActiveCell[1]].Wall < 0 &&
                Abstract.AllPlayers[ActivePlayer].Range >=
                Mathf.Abs((int)Abstract.AllPlayers[ActivePlayer].Location[0] - (int)ActiveCell[0]) +
                Mathf.Abs((int)Abstract.AllPlayers[ActivePlayer].Location[1] - (int)ActiveCell[1]);
        }               //Place Wall Setting

        return false;
    }

    private void InteractWithCell()
    {
        if (Setting == 0)
        {
            if (Abstract.Grid[ActiveCell[0], ActiveCell[1]].Player != -1)
            {
                Abstract.NetworkedPlayer.CmdTryAttackPlayer(ActivePlayer, new uint[] { (uint)ActiveCell[0], (uint)ActiveCell[1] }, PFPlayerStats.SessionID);
            }
            else
            {
                Abstract.NetworkedPlayer.CmdTryAttackWall(ActivePlayer, new uint[] { (uint)ActiveCell[0], (uint)ActiveCell[1] }, PFPlayerStats.SessionID);
            }
        }
        if (Setting == 1)
        {
            Abstract.NetworkedPlayer.CmdTryMovePlayer(ActivePlayer, new uint[] { (uint)ActiveCell[0], (uint)ActiveCell[1] }, PFPlayerStats.SessionID);
        }
        if (Setting == 2)
        {
            Abstract.NetworkedPlayer.CmdTryGivePlayer(ActivePlayer, new uint[] { (uint)ActiveCell[0], (uint)ActiveCell[1] }, PFPlayerStats.SessionID);
        }
        
        if (Setting == 4)
        {
            Abstract.NetworkedPlayer.CmdTryVotePlayer(ActivePlayer, Abstract.Grid[ActiveCell[0], ActiveCell[1]].Player, PFPlayerStats.SessionID);
        }
        if (Setting == 6)
        {
            Abstract.NetworkedPlayer.CmdTryAddWall(ActivePlayer, new uint[] { (uint)ActiveCell[0], (uint)ActiveCell[1] }, PFPlayerStats.SessionID);
        }
    }

    private void FindCorrectPlayer()
    {
        ActivePlayer = -1;

        for (int i = 0; i < Abstract.AllPlayers.Count; i++)
        {
            if (Abstract.AllPlayers[i].PlayerID == PFPlayerStats.MyPlayfabID)
            {
                ActivePlayer = i;
            }
        }
    }

    public void SetAttack() { Setting = 0; }
    public void SetMove() { Setting = 1; }
    public void SetGive() { Setting = 2; }
    public void SetInspect() { Setting = 3; }
    public void SetUpgrade() { Setting = 5; }
    public void SetAddWall() { Setting = 6; }
}

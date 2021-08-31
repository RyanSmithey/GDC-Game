using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeMethods : MonoBehaviour
{
    public void UpgradeRange()
    {
        Abstract.NetworkedPlayer.CmdTryUpgradeRange(Cursor.ActivePlayer, PFPlayerStats.SessionID);
    }
    public void UpgradeIPCS()
    {
        Abstract.NetworkedPlayer.CmdTryUpgradeIPCS(Cursor.ActivePlayer, PFPlayerStats.SessionID);
    }
    public void UpgradeFreeMoves()
    {
        Abstract.NetworkedPlayer.CmdTryUpgradeFreeMoves(Cursor.ActivePlayer, PFPlayerStats.SessionID);
    }

    [SerializeField] private TextMeshProUGUI RangeCost;
    [SerializeField] private TextMeshProUGUI RangeValue;
    [SerializeField] private TextMeshProUGUI IPCSCost;
    [SerializeField] private TextMeshProUGUI IPCSValue;
    [SerializeField] private TextMeshProUGUI FreeMovesCost;
    [SerializeField] private TextMeshProUGUI FreeMovesValue;

    void Update()
    {
        RangeValue.text = Abstract.AllPlayers[Cursor.ActivePlayer].Range.ToString();
        RangeCost.text = Abstract.AllPlayers[Cursor.ActivePlayer].Range.ToString();

        IPCSCost.text  = (Abstract.AllPlayers[Cursor.ActivePlayer].IPCsPerTurn * 2).ToString();
        IPCSValue.text = Abstract.AllPlayers[Cursor.ActivePlayer].IPCsPerTurn.ToString();

        FreeMovesCost.text = Abstract.AllPlayers[Cursor.ActivePlayer].FreeMovesPerDay.ToString();
        FreeMovesValue.text = Abstract.AllPlayers[Cursor.ActivePlayer].FreeMovesPerDay.ToString();
    }
}

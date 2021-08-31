using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inspection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Health;
    [SerializeField] private TextMeshProUGUI IPCS;
    [SerializeField] private TextMeshProUGUI FreeMoves;
    [SerializeField] private TextMeshProUGUI Range;
    [SerializeField] private TextMeshProUGUI FMPT;
    [SerializeField] private TextMeshProUGUI IPCSPT;

    private void Start()
    {
        if (PFSignIn.IsServer) { gameObject.SetActive(false); }
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.ActiveCell[0] == -1) { return; }
        else if (Abstract.Grid[Cursor.ActiveCell[0], Cursor.ActiveCell[1]].Player != -1)
        {
            Name.text =     Abstract.AllPlayers[Abstract.Grid[Cursor.ActiveCell[0], Cursor.ActiveCell[1]].Player].Name.ToString();
            Health.text =   Abstract.AllPlayers[Abstract.Grid[Cursor.ActiveCell[0], Cursor.ActiveCell[1]].Player].Health.ToString();
            IPCS.text =     Abstract.AllPlayers[Abstract.Grid[Cursor.ActiveCell[0], Cursor.ActiveCell[1]].Player].ActionPoints.ToString();
            FreeMoves.text =Abstract.AllPlayers[Abstract.Grid[Cursor.ActiveCell[0], Cursor.ActiveCell[1]].Player].FreeMoves.ToString();
            Range.text =    Abstract.AllPlayers[Abstract.Grid[Cursor.ActiveCell[0], Cursor.ActiveCell[1]].Player].Range.ToString();
            FMPT.text =     Abstract.AllPlayers[Abstract.Grid[Cursor.ActiveCell[0], Cursor.ActiveCell[1]].Player].FreeMovesPerDay.ToString();
            IPCSPT.text =   Abstract.AllPlayers[Abstract.Grid[Cursor.ActiveCell[0], Cursor.ActiveCell[1]].Player].IPCsPerTurn.ToString();
        }
        else
        {
            Name.text = "";
            Health.text = "";
            IPCS.text = "";
            FreeMoves.text = "";
            Range.text = "";
            FMPT.text = "";
            IPCSPT.text = "";
        }
    }
}

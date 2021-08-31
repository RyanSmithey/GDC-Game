using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI HP;
    [SerializeField] private TextMeshProUGUI IPCs;
    [SerializeField] private TextMeshProUGUI FreeMoves;
    [SerializeField] private TextMeshProUGUI Range;
    [SerializeField] private TextMeshProUGUI IPCSPT;
    [SerializeField] private TextMeshProUGUI FMPT;
    private void Start()
    {
        if (PFSignIn.IsServer) { gameObject.SetActive(false); }
    }
    // Update is called once per frame
    void Update()
    {
        if (Cursor.ActivePlayer != -1)
        {
            HP.text = Abstract.AllPlayers[Cursor.ActivePlayer].Health.ToString();
            IPCs.text = Abstract.AllPlayers[Cursor.ActivePlayer].ActionPoints.ToString();
            FreeMoves.text = Abstract.AllPlayers[Cursor.ActivePlayer].FreeMoves.ToString();
            Range.text = Abstract.AllPlayers[Cursor.ActivePlayer].Range.ToString();
            IPCSPT.text = Abstract.AllPlayers[Cursor.ActivePlayer].IPCsPerTurn.ToString();
            FMPT.text = Abstract.AllPlayers[Cursor.ActivePlayer].FreeMovesPerDay.ToString();
        }
        else 
        { 
            HP.text = "";
            IPCs.text = "";
            FreeMoves.text = "";
            Range.text = "";
            IPCSPT.text = "";
            FMPT.text = "";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class VoteMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI YourVote;
    [SerializeField] private TextMeshProUGUI AllVotesNames;
    [SerializeField] private TextMeshProUGUI AllVotesCount;

    // Update is called once per frame
    void Update()
    {
        SetYourVoteText();
        SetAllVotes();
    }

    private void SetYourVoteText()
    {
        if (Cursor.ActivePlayer != -1 && Abstract.AllPlayers[Cursor.ActivePlayer].Vote != -1)
        {
            YourVote.text = Abstract.AllPlayers[Abstract.AllPlayers[Cursor.ActivePlayer].Vote].Name;
        }
        else
        {
            YourVote.text = "None";
        }
    }

    private void SetAllVotes()
    {
        List<int> PlayerID = new List<int>();
        List<int> Count = new List<int>();

        foreach (Abstract.Player P1 in Abstract.AllPlayers)
        {
            if (PlayerID.Contains(P1.Vote))
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

        string Final = "";
        string FinalC = "";

        for (int i = 0; i < PlayerID.Count; i++)
        {
            if (PlayerID[i] != -1)
            {
                Final += Abstract.AllPlayers[PlayerID[i]].Name + "\n";
                FinalC += Count[i] + "\n";
            }
        }

        AllVotesNames.text = Final;
        AllVotesCount.text = FinalC;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject PauseScreen;
    public GameObject VoteMenu;
    public GameObject PlayMenu;
    public TextMeshProUGUI AllowPlayerJoin;
    private void Start()
    {
        if (PFSignIn.IsServer) { gameObject.SetActive(false); }
    }

    // Update is called once per frame
    void Update()
    {
        AllowPlayerJoin.text = "Join State: " + Abstract.IsAcceptingPlayers.ToString();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseScreen.SetActive(!PauseScreen.activeSelf);
        }
        if (Cursor.ActivePlayer >= 0)
        {
            if (Abstract.AllPlayers[Cursor.ActivePlayer].Health <= 0)
            {
                VoteMenu.SetActive(true);
                PlayMenu.SetActive(false);
            }
            else
            {
                VoteMenu.SetActive(false);
                PlayMenu.SetActive(true);
            }
        }
    }

    public void Resume() { PauseScreen.SetActive(false); }
    public void Quit() 
    { 
        if (NetworkFunctions.Self.isServer)
        {
            Abstract.SaveData();
            NetworkFunctions.Self.SaveTimeData();
        }
        Application.Quit();
    }
    
    public void ToggleAccept() { Abstract.IsAcceptingPlayers = !Abstract.IsAcceptingPlayers; }
}

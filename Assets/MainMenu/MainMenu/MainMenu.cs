using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using PlayFab;
using PlayFab.MultiplayerModels;

using Mirror;

using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LeaderBoard LB;

    [SerializeField] private NetworkManager NM;

    [SerializeField] private TMP_InputField NameInput;
    [SerializeField] private TMP_InputField RInput;
    [SerializeField] private TMP_InputField GInput;
    [SerializeField] private TMP_InputField BInput;

    private void Start()
    {
        if (PFSignIn.IsServer)
        {
            //Abstract.ClearData();
            //Abstract.LoadData();
            Abstract.AllPlayers = new List<Abstract.Player>();
            Abstract.AllWalls = new List<Abstract.Wall>();
            Abstract.SessionTickets = new List<string>();
            //for (int i = 0; i < Abstract.AllPlayers.Count; i++)
            //{
            //    Abstract.SessionTickets.Add("");
            //}

            NM.StartServer();
        }
        else
        {
            //Gather Stuff from PlayerPrefs to Input Areas
            if (PlayerPrefs.HasKey("NAME"))
            {
                NameInput.text = PlayerPrefs.GetString("NAME");
                SkinSelection.SpritePath = PlayerPrefs.GetString("SELECTEDSPRITE");
                RInput.text = PlayerPrefs.GetFloat("RINPUT").ToString();
                GInput.text = PlayerPrefs.GetFloat("GINPUT").ToString();
                BInput.text = PlayerPrefs.GetFloat("BINPUT").ToString();
            }
        }
    }

    public void Play()
    {
        //Assign Active values to PlayerPrefs, Cursor
        PlayerPrefs.SetString("NAME", NameInput.text);
        Cursor.Name = NameInput.text;

        PlayerPrefs.SetString("SELECTEDSPRITE", SkinSelection.SpritePath);
        Cursor.SkinName = SkinSelection.SpritePath;
        
        PlayerPrefs.SetFloat("RINPUT", float.Parse(RInput.text));
        PlayerPrefs.SetFloat("GINPUT", float.Parse(GInput.text));
        PlayerPrefs.SetFloat("BINPUT", float.Parse(BInput.text));
        Cursor.SkinColor = new Color(float.Parse(RInput.text), float.Parse(GInput.text), float.Parse(BInput.text));

        //LoadIntoLevel
        //RequestMultiplayerServer();
        NM.networkAddress = "ec2-3-16-216-208.us-east-2.compute.amazonaws.com";
        NM.StartClient();
    }


    private void RequestMultiplayerServer()
    {
        Debug.Log("[ClientStartUp].RequestMultiplayerServer");
        RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest();
        requestData.BuildId = "c0fcc143-a888-491a-9585-a4c23fe783bb";
        requestData.SessionId = System.Guid.NewGuid().ToString();
        requestData.PreferredRegions = new List<string>() { "East US" };
        //requestData.PreferredRegions = new List<AzureRegion>() { AzureRegion.EastUs };
        PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);

        void OnRequestMultiplayerServer(RequestMultiplayerServerResponse result)
        {
            NM.networkAddress = result.IPV4Address;
            NM.StartClient();
        }
        void OnRequestMultiplayerServerError(PlayFabError Error)
        {
            Debug.LogError(Error.GenerateErrorReport());
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
    public void SignOut()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene("Login");
    }

    //public void GetAnotherWin()
    //{
    //    //PFPlayerStats.UpdateStatsAsServer();
    //}
    public void UpdateLeaderBoard()
    {
        LB.GetLeaderBoard();
    }
}

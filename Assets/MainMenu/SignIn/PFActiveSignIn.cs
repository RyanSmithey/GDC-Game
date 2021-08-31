using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PFActiveSignIn : MonoBehaviour
{
    private PFActiveSignIn Self;

    private bool FlipFlop = false;
    // Start is called before the first frame update
    //void Start()
    //{
    //    if (Self != null)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }
    //    Self = this;
    //    DontDestroyOnLoad(gameObject);
    //}

    // Update is called once per frame
    void Update()
    {
        //If it is on the 5 minute Mark
        if (DateTime.Now.Minute % 5 == 0 && FlipFlop)
        {
            StartCloudStaySignIn();
            FlipFlop = false;
        }
        else if (DateTime.Now.Minute % 5 != 0)
        {
            FlipFlop = true;
        }
    }


    private void StartCloudStaySignIn()
    {
        //Convert Time to correct Int Value
        DateTime Time = DateTime.Now;

        int Year = Time.Year;
        string Month = Time.Month.ToString();
        if (Month.Length < 2) { Month = "0" + Month; }
        string Day = Time.Day.ToString();
        if (Day.Length < 2) { Day = "0" + Day; }
        string Hour = Time.Hour.ToString();
        if (Hour.Length < 2) { Hour = "0" + Hour; }
        string Minute = Time.Minute.ToString();
        if (Minute.Length < 2) { Minute = "0" + Minute; }

        int IntTime = int.Parse(Month + Day + Hour + Minute);

        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdateSignInTime",                        // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { YearSignIn = Year, TimeSignIn = IntTime },  // The parameter provided to your function
            //GeneratePlayStreamEvent = true,                         // Optional - Shows this event in PlayStream
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnStaySignedIn, OnSignConnect);
    }
    private void OnStaySignedIn(ExecuteCloudScriptResult result)
    {
        Debug.Log("Staying Connected");
    }
    private void OnSignConnect(PlayFabError error)
    {
        


        Debug.LogError(error.GenerateErrorReport());
    }
}

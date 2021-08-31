using PlayFab;
using PlayFab.ClientModels;

using System.Collections.Generic;
using UnityEngine;

public static class PFPlayerStats
{
    public static string MyPlayfabID;
    public static int NumberOfWins = 0;
    public static string SessionID;


    #region ClientUpdateDatabase Client insecure, Server insecure
    public static void UpdateNumberOfWins()
    {
        var Stats = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "Total Wins", Value = NumberOfWins + 1 } };
        var request = new UpdatePlayerStatisticsRequest { Statistics = Stats };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateStatsSuccess, OnUpdateStatsFailed);

        NumberOfWins += 1;
    }
    private static void OnUpdateStatsSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Player Wins Updated");
    }
    private static void OnUpdateStatsFailed(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region CustomCloudCalls Client Secure, Server Secure
    public static void StartCloudUpdatePlayerStats()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdateWinsStat",                        // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { inputValue = NumberOfWins },  // The parameter provided to your function
            GeneratePlayStreamEvent = true,                         // Optional - Shows this event in PlayStream
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnCloudHelloWorld, OnErrorShared);
    }
    private static void OnCloudHelloWorld(ExecuteCloudScriptResult result)
    {

    }
    private static void OnErrorShared(PlayFabError error)
    {

    }
    #endregion

    #region ServerUpdateDatabase, Client Insecure, Request Secure, Result Secure
    #if ENABLE_PLAYFABSERVER_API
    public static void UpdateStatsAsServer()
    {
        var Stats = new List<PlayFab.ServerModels.StatisticUpdate> { new PlayFab.ServerModels.StatisticUpdate { StatisticName = "Total Wins", Value = NumberOfWins + 1 } };
        PlayFab.ServerModels.UpdatePlayerStatisticsRequest request = new PlayFab.ServerModels.UpdatePlayerStatisticsRequest { PlayFabId = PFPlayerStats.MyPlayfabID, Statistics = Stats };
        PlayFabServerAPI.UpdatePlayerStatistics(request, OnServerUpdateStat, OnUpdateServerStatsFailed);

        NumberOfWins += 1;
    }
    private static void OnServerUpdateStat(PlayFab.ServerModels.UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Client Successfully updated statistic as server");
    }
    private static void OnUpdateServerStatsFailed(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    #endif
    #endregion

    #region GetPlayerStats
    public static void GetStatistics()
    {
        var request = new GetPlayerStatisticsRequest();
        PlayFabClientAPI.GetPlayerStatistics(request, OnGetStatistics, OnGetStatisticsFailed);
    }
    private static void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Statistics:");
        foreach (var eachStat in result.Statistics)
            Debug.Log("Statistic (" + eachStat.StatisticName + "): " + eachStat.Value);
    }
    private static void OnGetStatisticsFailed(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion
}

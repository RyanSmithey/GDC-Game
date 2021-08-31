using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class LeaderBoard : MonoBehaviour
{
    public GameObject BackButton;
    public GameObject NextButton;

    public GameObject IndividualEntrySample;
    public GameObject[] AllEntries;

    public GameObject Container;

    private int CurrentIndex = 0;
    
    private void Start()
    {
        if (CurrentIndex <= 0)
        {
            BackButton.SetActive(false);
        }

        AllEntries = new GameObject[10];

        for (int i = 0; i < 10; i++)
        {
            AllEntries[i] = Instantiate(IndividualEntrySample, Container.transform);

            //GameObjectLocation
            RectTransform R = AllEntries[i].GetComponent<RectTransform>();

            R.anchorMin = new Vector2(R.anchorMin[0], (9 - i) / 10.0f);
            R.anchorMax = new Vector2(R.anchorMax[0], (10 - i) / 10.0f);

            AllEntries[i].SetActive(false);
        }

        GetLeaderBoard();
    }

    public void GetLeaderBoard()
    {
        NextButton.SetActive(false);

        var requestLeaderBoard = new GetLeaderboardRequest { StartPosition = CurrentIndex, StatisticName = "Total Wins", MaxResultsCount = 11};
        PlayFabClientAPI.GetLeaderboard(requestLeaderBoard, GetLeaderBoardSuccess, GetLeaderBoardFailure);
    }

    private void GetLeaderBoardSuccess(GetLeaderboardResult result)
    {
        int C = Mathf.Min(result.Leaderboard.Count, 10);

        for (int i = 0; i < C; i++)
        {
            //Write to GameObject   P.DisplayName   P.StatValue
            //Assign Values to GameObject
            AllEntries[i].SetActive(true);
            AllEntries[i].GetComponent<SetValues>().SetValue(0, result.Leaderboard[i].DisplayName);
            AllEntries[i].GetComponent<SetValues>().SetValue(1, result.Leaderboard[i].StatValue.ToString());
            AllEntries[i].GetComponent<SetValues>().SetValue(2, (result.Leaderboard[i].Position + 1).ToString());
        }
        for (int i = C; i < 10; i++)
        {
            //Set GameObject to invisible
            AllEntries[i].SetActive(false);
        }

        if (result.Leaderboard.Count == 11)
        {
            NextButton.SetActive(true);
        }
        else
        {
            NextButton.SetActive(false);
        }
    }

    private void GetLeaderBoardFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void SetStartIndex(int input)
    {
        int i = input;
        Mathf.Min(i, 0);

        CurrentIndex = i;
        GetLeaderBoard();
    }

    public void NextButtonAction()
    {
        CurrentIndex += 10;
        GetLeaderBoard();

        BackButton.SetActive(true);//Be a little more subtle
    }
    public void BackButtonAction()
    {
        CurrentIndex -= 10;
        CurrentIndex = Mathf.Min(CurrentIndex, 0);
        GetLeaderBoard();

        if (CurrentIndex <= 0)
        {
            BackButton.SetActive(false);
        }
    }
    public void SetCurrentPosition()
    {
        //Add Code to find current position in the stack
    }
}

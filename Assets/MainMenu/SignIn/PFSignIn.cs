using System.IO;

using PlayFab;
using PlayFab.ClientModels;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class PFSignIn : MonoBehaviour
{
    public static bool IsServer = false;

    [SerializeField] private GameObject LoadingIcon;
    [SerializeField] private GameObject Navigation;

    [SerializeField] private TextMeshProUGUI ErrorText;
    [SerializeField] private GameObject ResetPassword;


    [SerializeField] private TMP_InputField UserName;
    [SerializeField] private TMP_InputField Password;

    [SerializeField] private Toggle SaveInputs;
    [SerializeField] private Toggle AutoLogin;

    private static bool PreviouslyLogedIn = false;
//#if UNITY_STANDALONE
    // Start is called before the first frame update
    void Start()
    {
        //Check if Necessary Save files exist
        try
        {
            File.ReadAllText(Application.dataPath + "/SaveFiles/SaveFile.txt");
            File.ReadAllText(Application.dataPath + "/SaveFiles/WallSave.txt");
            File.ReadAllText(Application.dataPath + "/SaveFiles/ServerLog.txt");
            File.ReadAllText(Application.dataPath + "/SaveFiles/TimeFile.txt");
        }
        catch
        {
            Directory.CreateDirectory(Application.dataPath + "/SaveFiles");
            File.WriteAllText(Application.dataPath + "/SaveFiles/SaveFile.txt", "");
            File.WriteAllText(Application.dataPath + "/SaveFiles/WallSave.txt", "");
            File.WriteAllText(Application.dataPath + "/SaveFiles/ServerLog.txt", "");
            File.WriteAllText(Application.dataPath + "/SaveFiles/TimeFile.txt", "");
        }
        //Check If it is a server
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null  || false)
        {
            IsServer = true;
            SceneManager.LoadScene("MainMenu");

            return;
        }
        
        Navigation.SetActive(true);
        LoadValues();

        if (AutoLogin.isOn && !PreviouslyLogedIn)
        {
            AttemptLogin();
        }
    }
#region Login
    public void AttemptLogin()
    {
        SaveValues();

        var request = new LoginWithPlayFabRequest { TitleId = "3AF8F", Username = UserName.text, Password = Password.text};
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }
    private void OnLoginSuccess(LoginResult result)
    {
        PreviouslyLogedIn = true;

        PFPlayerStats.MyPlayfabID = result.PlayFabId;
        PFPlayerStats.SessionID = result.SessionTicket;
        //Get Player Stats
        PFPlayerStats.GetStatistics();

        //Load Into Main Menu
        SceneManager.LoadScene("MainMenu");
    }
    private void OnLoginFailure(PlayFabError result)
    {
        //Display Error Type
        ResetPassword.SetActive(true);
        ErrorText.text = "UserName Or Password is wrong";
    }
    private void SaveValues()
    {
        if (SaveInputs.isOn)
        {
            PlayerPrefs.SetString("USERNAME", UserName.text);
            PlayerPrefs.SetString("PASSWORD", Password.text);
            PlayerPrefs.SetInt("SAVELOGIN", 1);
        }
        else
        {
            PlayerPrefs.SetString("USERNAME", "");
            PlayerPrefs.SetString("PASSWORD", "");
            PlayerPrefs.SetInt("SAVELOGIN", 0);
        }

        if (AutoLogin.isOn) { PlayerPrefs.SetInt("AUTOLOGIN", 1); }
        else { PlayerPrefs.SetInt("AUTOLOGIN", 0); }
    }
    private void LoadValues()
    {
        if (PlayerPrefs.HasKey("SAVELOGIN") && PlayerPrefs.HasKey("AUTOLOGIN"))
        {
            UserName.text = PlayerPrefs.GetString("USERNAME");
            Password.text = PlayerPrefs.GetString("PASSWORD");
            Debug.Log("1");
            SaveInputs.isOn = PlayerPrefs.GetInt("SAVELOGIN") == 1;
            Debug.Log("2");
            AutoLogin.isOn = PlayerPrefs.GetInt("AUTOLOGIN") == 1;
            Debug.Log("3");
        }
    }
    public void OnToggleAutoLogin()
    {
        if (AutoLogin.isOn)
        {
            SaveInputs.isOn = AutoLogin.isOn;
        }
    }
#endregion
//#endif

//#if UNITY_ANDROID
//    void Start(){
//        if (!PreviouslyLogedIn)
//        {
//            LoadingIcon.SetActive(true);
//            Navigation.SetActive(false);

//            var request = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
//            PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnMobileLoginSuccess, OnMobileLoginFailure);
//        }
//        else
//        {
//            LoadingIcon.SetActive(false);
//            Navigation.SetActive(true);
//        }
//    }
//#endif


//#if UNITY_IOS
//    //AutoLogin With DeviceID
//    void Start(){
//        LoadingIcon.SetActive(true);
//        Navigation.SetActive(false);

//        var request = new LoginWithIOSDeviceIDRequest { AndroidDeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true};
//        PlayFabClientAPI.LoginWithAndroidDeviceID(request, OnMobileLoginSuccess, OnMobileLoginFailure);
//    }
//#endif
    private void OnMobileLoginSuccess(LoginResult result)
    {
        PFPlayerStats.MyPlayfabID = result.PlayFabId;

        //Get Player Stats
        PFPlayerStats.GetStatistics();

        //Load Into Main Menu
        SceneManager.LoadSceneAsync("MainMenu");
    }
    private void OnMobileLoginFailure(PlayFabError error)
    {
        LoadingIcon.SetActive(false);
        Navigation.SetActive(true);
        
        Debug.LogError(error.GenerateErrorReport());
    }
}

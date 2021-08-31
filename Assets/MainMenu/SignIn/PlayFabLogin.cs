using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;

public class PlayFabLogin : MonoBehaviour
{
    private void Start()
    {
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            UserEmail = PlayerPrefs.GetString("EMAIL");
            UserPassword = PlayerPrefs.GetString("PASSWORD");
            UserName = PlayerPrefs.GetString("USERNAME");

            UserNameInput.text = UserName;
            EmailInput.text = UserEmail;
            PasswordInput.text = UserPassword;

            TryLogin();
        }
    }

    private string UserName;
    private string UserEmail;
    private string UserPassword;

    [SerializeField] private TMP_InputField UserNameInput;
    [SerializeField] private TMP_InputField EmailInput;
    [SerializeField] private TMP_InputField PasswordInput;

    public void TryLogin()
    {
        SetValuesFromInput();

        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "3AF8F";
        }

        //var request = new LoginWithCustomIDRequest { CustomId = "GettingStartedGuide", CreateAccount = true };
        //PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);

        var request = new LoginWithEmailAddressRequest { Email = UserEmail, Password = UserPassword };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("You Logged into Game Of Dots");

        PlayerPrefs.SetString("EMAIL", UserEmail);
        PlayerPrefs.SetString("PASSWORD", UserPassword);
        PlayerPrefs.SetString("USERNAME", UserName);
    }
    private void OnLoginFailure(PlayFabError error)
    {
        //Debug.LogWarning("Something went wrong with your first API call.  :(");
        //Debug.LogError("Here's some debug information:");
        //Debug.LogError(error.GenerateErrorReport());
        var registerRequest = new RegisterPlayFabUserRequest { Email = UserEmail, Password = UserPassword, Username = UserName };
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        PlayerPrefs.SetString("EMAIL", UserEmail);
        PlayerPrefs.SetString("PASSWORD", UserPassword);
        PlayerPrefs.SetString("USERNAME", UserName);
    }
    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    public void SetValuesFromInput()
    {
        UserName = UserNameInput.text;
        UserEmail = EmailInput.text;
        UserPassword = PasswordInput.text;
    }

    public void OnClickLogin()
    {
        SetValuesFromInput();
        TryLogin();
    }
}
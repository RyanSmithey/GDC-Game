using PlayFab;
using PlayFab.ClientModels;

using UnityEngine;
using TMPro;

public class PFCreatAccount : MonoBehaviour
{
    [SerializeField] private TMP_InputField UserName;  
    [SerializeField] private TMP_InputField Email; 
    [SerializeField] private TMP_InputField Password; 
    [SerializeField] private TMP_InputField ConfirmPassword;

    [SerializeField] private TMP_InputField UserName_Start;
    [SerializeField] private TMP_InputField Password_Start;
    [SerializeField] private PFSignIn PFSign;

    private void Update()
    {
        if (Password != ConfirmPassword)
        {
            //Tell The user they are actually retarded
        }
        else
        {
            //Clear text
        }
    }

    public void CreateAccount()
    {
        if (Password.text == ConfirmPassword.text)
        {
            var registerRequest = new RegisterPlayFabUserRequest { Email = Email.text, Password = Password.text, Username = UserName.text };
            PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
        }
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        //Change Text to indicate Successful register
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = UserName.text}, OnUpdateDisplayNameSuccess, OnUpdateDisplayNameFailure);

        UserName_Start.text = UserName.text;
        Password_Start.text = Password.text;

        PFSign.AttemptLogin();
    }
    private void OnRegisterFailure(PlayFabError error)
    {
        //Change Text to indicate register failure

    }


    private void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("DisplayName Updated On Newly Registered account");
    }
    private void OnUpdateDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}

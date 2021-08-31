using System.IO;
using UnityEngine;
using TMPro;
using Mirror;

public class JoinGame : MonoBehaviour
{
    public NetworkManager NetManger;

    public TextMeshProUGUI IP;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Password;

    public TMP_InputField NameInputField;
    public TMP_InputField IPInputField;
    public TMP_InputField PasswordInputField;

    void OnEnable()
    {
        Application.targetFrameRate = 30;
        LoadValues();
    }
    

    public void UpdateName()
    {
        
    }
    public void UpdatePassword()
    {
        
    }
    public void UpdateIP()
    {
        if (IP.text.Length == 1) 
        {
            NetManger.networkAddress = "";// "71.76.65.21"; 
        }
        else 
        {
            string Text = IP.text;

            Text = Text.Remove(Text.Length - 1, 1);

            NetManger.networkAddress = Text;
        }
    }

    public void HostNewGame()
    {
        UpdateName();
        UpdatePassword();

        SaveValues();

        string Path = Application.dataPath + "/Resources/ServerLog.txt";
        File.WriteAllText(Path, "");

        Abstract.ClearData();
        Abstract.LoadData();
        NetworkFunctions.ClearData();
        NetManger.StartHost();
    }
    public void ContinueHost()
    {
        UpdateName();
        UpdatePassword();

        SaveValues();

        Abstract.LoadData();
        NetManger.StartHost();
    }
    public void JoinExistingGame()
    {
        UpdateName();
        UpdatePassword();
        UpdateIP();
        try
        {
            SaveValues();
        }
        catch
        {

        }

        NetManger.StartClient();
    }

    public void SaveValues()
    {
        string Path = Application.dataPath + "/Resources/SaveFiles/NamesSave.txt";
        File.WriteAllText(Path, "");

        File.AppendAllText(Path, IPInputField.text + "\n");
        File.AppendAllText(Path, NameInputField.text + "\n");
        File.AppendAllText(Path, PasswordInputField.text + "\n");
    }
    public void LoadValues()
    {
        string Path = Application.dataPath + "/Resources/SaveFiles/NamesSave.txt";
        string[] AllLines = File.ReadAllLines(Path);

        IPInputField.text = AllLines[0];
        NameInputField.text = AllLines[1];
        PasswordInputField.text = AllLines[2];
    }
}

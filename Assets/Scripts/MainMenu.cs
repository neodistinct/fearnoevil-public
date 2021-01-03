using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public InputField PlayerNameInputField;

    public void Start()
    {

        LevelHub.menuCanvas = GameObject.Find("MenuCanvas").GetComponent<Canvas>();
        LevelHub.loadingCanvas = GameObject.Find("LoadingCanvas").GetComponent<Canvas>();
        LevelHub.titlesCanvas = GameObject.Find("TitlesCanvas").GetComponent<Canvas>();

        if (PlayerNameInputField)
        {
            PlayerNameInputField.onEndEdit.AddListener(OnPlayerNameChanged);
        }

        Debug.Log("Menu module started");
    }

    public void OnExitMenu()
    {
        Application.Quit();
    }

    public void OnPlayerNameChanged(string value)
    {
        Library.GetInstance().SetPlayerName(value);
    }

    public void OnLoadLevelMenu(string levelName)
    {

        LevelHub.LoadLevel(levelName);

    }


}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager_StartScene : MonoBehaviour
{
    public GameObject mainMenu;

    public AudioClip clickClip;

    public void OnButtonClick()
    {
        AudioManager.Instance.PlaySFX(clickClip);
        Debug.Log("Button clicked!");
    }

    private void Start()
    {
        mainMenu = GameObject.Find("MainMenu");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("OptionsScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}


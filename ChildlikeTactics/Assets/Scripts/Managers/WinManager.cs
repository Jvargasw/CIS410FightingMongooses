using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinManager : MonoBehaviour
{
    public Button homeButton;
    // Use this for initialization
    void Start()
    {
        homeButton = homeButton.GetComponent<Button>();
    }

    public void returnHome()
    {
        SceneManager.LoadScene("Start_screen");
    }

    public void goToCredits()
    {
        SceneManager.LoadScene("Credits_Scene");
    }
}

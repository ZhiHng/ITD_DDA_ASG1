/*
* Author: Zhi hng
* Date: 28 November 2026
* Description: UI Manager to manage UI between scenes
*/

using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameUIParent;
    [SerializeField] GameObject infoUIParent;
    [SerializeField] GameObject homeUIParent;
    [SerializeField] GameObject accountUIParent;
    [SerializeField] GameObject gameEndScreen;
    [SerializeField] GameObject levelSelection;
    

    public static UIManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum MenuType
    {
        ACCOUNT,
        HOME,
        INFO,
        GAME
    }

    public void SwitchMenu(int menu)
    {
        MenuType menuType = (MenuType)menu;
        accountUIParent.SetActive(false);
        homeUIParent.SetActive(false);
        infoUIParent.SetActive(false);
        gameUIParent.SetActive(false);
        ShowGameEndScreen(false);

        switch (menuType)
        {
            case MenuType.ACCOUNT:
                accountUIParent.SetActive(true);
                SceneManager.LoadScene("Account");
                break;

            case MenuType.HOME:
                homeUIParent.SetActive(true);
                UpdateHomeScreen();
                UpdatePlayableLevels();
                SceneManager.LoadScene("Home");
                break;

            case MenuType.INFO:
                infoUIParent.SetActive(true);
                SceneManager.LoadScene("Info");
                break;

            case MenuType.GAME:
                gameUIParent.SetActive(true);
                SceneManager.LoadScene("Game");
                break;
        }
    }

    public void ChangeSignUpNLogin()
    {
        if (accountUIParent.activeSelf == true)
        {
            foreach (Transform child in accountUIParent.transform)
            {
                if (child.CompareTag("SignInUpChange"))
                {
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
                }
            }
        }
    }

    public void ShowGameEndScreen(bool isShown, bool isWin = false, int level = 0, string stats = "")
    {
        gameEndScreen.SetActive(isShown);
        if (isShown)
        {
            TextMeshProUGUI[] textComponents = gameEndScreen.GetComponentsInChildren<TextMeshProUGUI>();
            if (isWin)
                textComponents[0].text = "Victory";
            else
                textComponents[0].text = "Defeat";

            textComponents[1].text = "Level " + level + 1;
            textComponents[2].text = stats;
        }
    }

    public void SignIn()
    {
        GameManager.instance.gameObject.GetComponent<FirebaseAuthentication>().SignIn();
    }

    public void SignUp()
    {
        GameManager.instance.gameObject.GetComponent<FirebaseAuthentication>().SignUp();
    }

    public void UpdateHomeScreen()
    {
        TextMeshProUGUI[] textComponents = homeUIParent.GetComponentsInChildren<TextMeshProUGUI>();
        print(GameManager.instance.username);
        textComponents[0].text = "UID: " + GameManager.instance.userId;
        textComponents[1].text = GameManager.instance.username;
        textComponents[2].text = "Level " + GameManager.instance.level;
        textComponents[3].text = GameManager.instance.experience + " / " + GameManager.instance.GetRequiredXP(GameManager.instance.level);
    }

    public void HomeMenuChange()
    {
        if (homeUIParent.activeSelf == true)
        {
            foreach (Transform child in homeUIParent.transform)
            {
                if (child.CompareTag("HomeMenuChange"))
                {
                    child.gameObject.SetActive(!child.gameObject.activeSelf);
                }
            }
        }
    }

    public void UpdatePlayableLevels()
    {
        int highestLevel = GameManager.instance.gameLevel;
        Button[] buttonComponents = levelSelection.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttonComponents.Length; i++)
        {
            if (i <= highestLevel)
            {
                buttonComponents[i].interactable = true;
            }
            else
            {
                buttonComponents[i].interactable = false;
            }
        }
    }
}

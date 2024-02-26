using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Arc
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("---MainMenu---")]
        [SerializeField] GameObject mainMenuPopup;

        [Header("---Pause Menu---")]
        [SerializeField] GameObject pauseMenuPopup;

        [Header("---Final Popup---")]
        [SerializeField] GameObject finalPopup;
        [SerializeField] TextMeshProUGUI finalPopupText;


        public void TwoPlayerGame()
        {
            GameManager.Instance.IsPlayingWithAI = false;
            mainMenuPopup.SetActive(false);
            GameManager.Instance.Intialize();
        }

        public void PlayWithAIGame()
        {
            GameManager.Instance.IsPlayingWithAI = true;
            mainMenuPopup.SetActive(false);
            GameManager.Instance.Intialize();
        }

        public void OpenPopup(GameObject popup)
        {
            popup.SetActive(true);
        }

        public void ClosePopup(GameObject popup)
        {
            popup.SetActive(false);
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        public void OpenFinalPopup(string teamName)
        {
            finalPopup.SetActive(true);
            finalPopupText.text = "Team Lost: " + teamName;
        }

    }
}

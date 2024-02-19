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
        [SerializeField] GameObject finalPopup;
        [SerializeField] TextMeshProUGUI finalPopupText;

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

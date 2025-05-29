using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsController : MonoBehaviour
{
    public void OnScoreButtonClicked()
    {
        SceneManager.LoadScene("FinalScore");
    }

    public void OnDataButtonClicked() {
        Application.OpenURL("https://www.stat2games.sites.grinnell.edu/data/roller-coaster/roller-coaster.php");
    }

}

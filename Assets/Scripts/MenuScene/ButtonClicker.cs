using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClicker : MonoBehaviour
{
    // Prereq: have assigned group id, player id, level
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.DesignTracks);
    }

    // TODO: what does Options actually mean bruh
    //public void onOptionsButtonClicked()
    //{
    //    DataManager.instance.OnOptionsButtonClicked();
    //}

    public void OnHomeButtonClicked()
    {
        SceneManager.LoadScene(SceneNames.MenuScene);
        SegmentsManager.instance.DestroySegmentsManager();
    }

    public void OnReplayButtonClicked()
    {
        SegmentsManager.instance.DestroySegmentsManager();

        switch (DataManager.instance.Level())
        {
            case 1:
                SceneManager.LoadScene(SceneNames.Level1);
                //SceneManager.LoadScene(SceneNames.DesignTracks);
                break;

            case 2:
                SceneManager.LoadScene(SceneNames.Level2);
                break;

            default:
                Debug.LogWarning("Unhandled level in switch case!");
                break;
        }
    }

    // Assigned to "PLAY" on MenuScene
    public static void OnChooseLevelButtonClicked()
    {
        DataManager.instance.OnPlayButtonClicked();
        SceneManager.LoadScene(SceneNames.SelectScene);
    }

    public static void OnChooseLevelWhileInMainGameButtonClicked()
    {
        SegmentsManager.instance.DestroySegmentsManager();
        SceneManager.LoadScene(SceneNames.SelectScene);
    }

    // Assigned to "LEVEL 1" on SelectScene
    public void OnChooseLevel1ButtonClicked()
    {
        DataManager.instance.OnLevelButtonClicked(1);
        SceneManager.LoadScene(SceneNames.Level1);
    }

    // Assigned to "LEVEL 2" on SelectScene
    public void OnChooseLevel2ButtonClicked()
    {
        DataManager.instance.OnLevelButtonClicked(2);
        SceneManager.LoadScene(SceneNames.Level2);
    }
}

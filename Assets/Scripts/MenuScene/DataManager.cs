using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public static Data.datum gameData = new();
    public static DataLevel2.datum gameDataLvl2 = new();

    public TMP_InputField playerIDInputField;
    public TMP_InputField groupIDInputField;

    private void Start()
    {
        StartCoroutine(parseIds());

    }

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuScene")
        {
            ResetGameData();
        }

        StartCoroutine(parseIds());

        //parseIds();
    }

    private IEnumerator parseIds()
    {
        if (SceneManager.GetActiveScene().name != "MenuScene") 
        {
            Debug.Log("parseIds skipped. Not in MenuScene.");
            yield break;
        }

        yield return new WaitForEndOfFrame();

        var inputFields = FindObjectsOfType<TMP_InputField>();
        Debug.Log("Number of input fields found: " + inputFields.Length);

        foreach (var inputField in inputFields)
        {
            Debug.Log("Found Input Field: " + inputField.name);

            if (inputField.name == "PlayerIDInput")
            {
                playerIDInputField = inputField;
                Debug.Log("Assigned playerIDInputField");
            }
            else if (inputField.name == "GroupIDInput")
            {
                groupIDInputField = inputField;
                Debug.Log("Assigned groupIDInputField");
            }
        }
    }


    //public void OnOptionsButtonClicked()
    //{
    //    gameData.playerID = playerIDInputField.text;
    //    gameData.groupID = groupIDInputField.text;
    //    gameData.level = 1;
    //    Debug.Log(playerIDInputField.text);
    //    Debug.Log(groupIDInputField.text);
    //}


    public void OnPlayButtonClicked()
    {
        try
        {
            gameData.playerID = playerIDInputField.text;
            gameData.groupID = groupIDInputField.text;
            Debug.Log(playerIDInputField.text);
            Debug.Log(groupIDInputField.text);
            //    SceneManager.LoadScene(1);
            //}
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void OnLevelButtonClicked(int num)
    {
        try
        {
            gameData.level = num;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public int Level()
    {
        return gameData.level;
    }


    public void OnFinishSelecting()
    {
        // Implement
    }

    public void OnGameEnd()
    {
        Debug.Log("OnGameEnd is triggered");
        gameData.date = DateTime.Now;
        //StartCoroutine(SendData());
    }

    public static IEnumerator SendData()
    {
        Debug.Log("Sending End Data...");
        WWWForm form = new WWWForm();
        form.AddField("playerID", gameData.playerID);
        form.AddField("groupID", gameData.groupID);
        form.AddField("level", gameData.level);
        form.AddField("eq1a", gameData.eq1a.ToString());
        form.AddField("eq1b", gameData.eq1b.ToString());
        form.AddField("eq1c", gameData.eq1c.ToString());
        form.AddField("x1max", gameData.x1max.ToString());
        form.AddField("eq2a", gameData.eq2a.ToString());
        form.AddField("eq2b", gameData.eq2b.ToString());
        form.AddField("eq2c", gameData.eq2c.ToString());
        form.AddField("x2max", gameData.x2max.ToString());
        form.AddField("eq3a", gameData.eq3a.ToString());
        form.AddField("eq3b", gameData.eq3b.ToString());
        form.AddField("eq3c", gameData.eq3c.ToString());
        form.AddField("mathCheck", gameData.mathCheck ? "Yes" : "No");
        form.AddField("score", gameData.score.ToString());
        form.AddField("success", gameData.success ? "Yes" : "No");

        if (gameData.level > 1) {
            form.AddField("x3max", gameData.x3max.ToString());
            form.AddField("x4max", gameDataLvl2.x4max.ToString());
            form.AddField("x5max", gameDataLvl2.x5max.ToString());
            form.AddField("eq1d", gameDataLvl2.eq1d.ToString());
            form.AddField("eq2d", gameDataLvl2.eq2d.ToString());
            form.AddField("eq3d", gameDataLvl2.eq3d.ToString());
            form.AddField("eq4a", gameDataLvl2.eq4a.ToString());
            form.AddField("eq4b", gameDataLvl2.eq4b.ToString());
            form.AddField("eq4c", gameDataLvl2.eq4c.ToString());
            form.AddField("eq4d", gameDataLvl2.eq4d.ToString());
            form.AddField("eq5a", gameDataLvl2.eq5a.ToString());
            form.AddField("eq5b", gameDataLvl2.eq5b.ToString());
            form.AddField("eq5c", gameDataLvl2.eq5c.ToString());
            form.AddField("eq5d", gameDataLvl2.eq5d.ToString());
        }

        // public bool success;
        string url = (gameData.level == 1) ? "https://stat2games.sites.grinnell.edu/php/sendRollerCoasterGameInfo.php" : "https://stat2games.sites.grinnell.edu/php/sendRollerCoasterGameInfoLvl2.php";
        UnityWebRequest www = UnityWebRequest.Post(url, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("End Game data created successfully");
            Debug.Log("Server Response: " + www.downloadHandler.text);
        }
    }

    public void ResetGameData()
    {
        gameData = new Data.datum();
    }
}

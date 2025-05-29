using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public float delay = 12f; // Time before switching
    public string sceneName = SceneNames.FinalScore; // Scene to load

    void Start()
    {
        Invoke("ChangeScene", delay);
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class InputValidator : MonoBehaviour
{
    public TMP_InputField playerIDInput;
    public TMP_InputField groupIDInput;
    public GameObject badWordText;
    public TextAsset blockedWordsFile;

    private string[] blockedWords;

    void Start()
    {
        badWordText.SetActive(false);
        blockedWords = blockedWordsFile.text.Split(',').Select(word => word.Trim().ToLower()).ToArray();
        playerIDInput.onValueChanged.AddListener(delegate { ValidateInput(); });
        groupIDInput.onValueChanged.AddListener(delegate { ValidateInput(); });
    }

    void ValidateInput()
    {
        if (IsBadWord(playerIDInput.text) || IsBadWord(groupIDInput.text))
        {
            badWordText.SetActive(true);
        }
        else
        {
            badWordText.SetActive(false);
        }
    }

    private bool IsBadWord(string input)
    {
        string[] words = input.ToLower().Split(new char[] { ' ', '.', ',', ';' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
        {
            if (blockedWords.Contains(word))
                return true;
        }
        return false;
    }

    public void PlayDefault()
    {
        if (IsValidInput(playerIDInput.text) && IsValidInput(groupIDInput.text))
        {
            //DataManager.instance.ResetGameData();
            SceneManager.LoadScene("DesignTracks");
        }
        else
        {
            badWordText.SetActive(true);
        }
    }

    public void PlayOption()
    {
        if (IsValidInput(playerIDInput.text) && IsValidInput(groupIDInput.text))
        {
            ButtonClicker.OnChooseLevelButtonClicked();
        }
        else
        {
            badWordText.SetActive(true);
        }
    }

    private bool IsValidInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        return !IsBadWord(input);
    }

    //This is for the Back button on Select Scene
    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MenuScene");
    }

}

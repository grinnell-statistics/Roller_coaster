using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButtonController : MonoBehaviour
{
    public GameObject optionsPopup;
    public Button closeButton;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the panel is hidden initially
        optionsPopup.SetActive(false);

        // Assign button listeners
        closeButton.onClick.AddListener(CloseOptions);
    }

    public void ShowOptions()
    {
        optionsPopup.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPopup.SetActive(false);
    }
}

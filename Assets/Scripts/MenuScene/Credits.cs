using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public GameObject creditsPopup;
    public Button closeButton;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the panel is hidden initially
        creditsPopup.SetActive(false);

        // Assign button listeners
        closeButton.onClick.AddListener(CloseCredits);
    }

    public void ShowCredits()
    {
        creditsPopup.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPopup.SetActive(false);
    }
}

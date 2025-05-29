using UnityEngine;
using UnityEngine.UI;

public class MuteController : MonoBehaviour
{
    public Button unmute;
    public Button mute;
    private void Start()
    {
        bool start = true;
        unmute.gameObject.SetActive(start);
        mute.gameObject.SetActive(!start);
        AudioListener.volume = 1;
        unmute.onClick.AddListener(() =>
        {
            OnToggleChanged(start);
            start = !start;
        });
        mute.onClick.AddListener(() =>
        {
            OnToggleChanged(start);
            start = !start;
        });
    }
    public void OnToggleChanged(bool isMuted)
    {
        if (isMuted)
        {
            AudioListener.volume = 0;
            unmute.gameObject.SetActive(!isMuted);
            mute.gameObject.SetActive(isMuted);
        } else
        {
            AudioListener.volume = 1;
            mute.gameObject.SetActive(isMuted);    
            unmute.gameObject.SetActive(!isMuted);
        }
    }
}

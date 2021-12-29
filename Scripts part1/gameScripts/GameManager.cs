
using AnilTools;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public AudioSource audioSource;
    public AudioClip RockSound;
    public AudioClip WoodSound;

    private void Awake()
    {
        Application.focusChanged += (focused) =>
        {
            if (Application.isPlaying) {
                Cursor.lockState = focused ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !focused;
            }
        };
        audioSource = GetComponent<AudioSource>();
    }
}

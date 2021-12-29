using AnilTools;
using UnityEngine;
using UnityEngine.UI;

public class Warning : Singleton<Warning>
{
    [SerializeField]
    private Text MainText;
    [SerializeField]
    private Text ButtonText;
    [SerializeField]
    private Button button;
    
    private GameObject window;

    public static bool warningOpen;

    private void Start()
    {
        window = transform.GetChild(0).gameObject;
        button.onClick.AddListener(CloseWarningWindow);
    }

    public void Warn(string text , string buttonText)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameMenu.instance.canvases.WarningCanvas.SetVisuality(true);
        InputListenner.Add(KeyCode.Escape, CloseWarningWindow);
        InputListenner.Add(KeyCode.KeypadEnter, CloseWarningWindow);

        window.SetActive(true);
        MainText.text = text;
        ButtonText.text = buttonText;
        warningOpen = true;
    }

    public void CloseWarningWindow()
    {
        InputListenner.Remove(KeyCode.Escape, CloseWarningWindow);
        InputListenner.Remove(KeyCode.KeypadEnter, CloseWarningWindow);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (PopUpPref.ActivePopUpCount.Equals(0))
            GameMenu.instance.canvases.WarningCanvas.SetVisuality(false);

        warningOpen = false;
        window.SetActive(false);
    }
}
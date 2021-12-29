
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UrFairy;

[RequireComponent(typeof(Image))]
public class PopUpPref : MonoBehaviour
{
    public Text text;
    public Image icon;

    private Image BgImage;
    private readonly WaitForSecondsRealtime UpdateTime = new WaitForSecondsRealtime(.06f);
    private readonly WaitForSecondsRealtime WaitTime = new WaitForSecondsRealtime(1.5f);

    private static int activePopUpCount;
    public static int ActivePopUpCount
    {
        get
        {
            return activePopUpCount;
        }
        set
        {
            activePopUpCount = value;
            
            if (activePopUpCount == 0 && !Warning.warningOpen)
            {
                GameMenu.instance.canvases.WarningCanvas.SetVisuality(false);
            }
        }
    }

    public void Spawn(string text, Color color , Sprite icon)
    {
        if (!BgImage)
        BgImage = GetComponent<Image>();
        BgImage.color = color;
        this.text.text = text;
        this.text.color = Color.black;
        this.icon.color = color;
        this.icon.sprite = icon;
        StartCoroutine(Fade());
        ActivePopUpCount++;
    }

    public void Spawn(string text, Color color)
    {
        if (!BgImage)
        BgImage = GetComponent<Image>();
        BgImage.color = color;
        this.text.text = text;
        this.text.color = Color.black;
        StartCoroutine(Fade());
        ActivePopUpCount++;
    }

    private IEnumerator Fade()
    {
        yield return WaitTime;

        while (BgImage.color.a > 0.1f){
            BgImage.color = BgImage.color.APlus(- 0.1f);
            text.color = text.color.APlus(- 0.1f);
            icon.color = icon.color.APlus(- 0.1f);
            yield return UpdateTime;
        }
        transform.parent = null;
        ActivePopUpCount--;
    }

}


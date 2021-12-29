
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class AnilButton : MonoBehaviour
{
    public enum SellectAction
    {
        none, ColorTint, Border
    }

    public enum ClickMode
    { 
        click,          // > tıklayınca action
        clickAndRelease // > bırakınca release action
    }
    
    public enum ButtonState
    {
        none, Holding
    }

    public bool Sellected;

    private Image image;
    public Image Image
    {
        get
        {
            if (image == null)
            {
                image = GetComponent<Image>();
            }
            return image;
        }
    }

    protected static bool CanClick = true;

    public ClickMode clickMode;
    public ButtonState buttonState;
    private Color TintColor = Color.grey;

    [SerializeField] private SellectAction sellectAction;
    public Image BorderImage;

    public UnityEvent UnityEvent;
    public UnityEvent ReleaseAction;

    public bool Border => sellectAction == SellectAction.Border;
    public bool Release => clickMode == ClickMode.clickAndRelease;

    public virtual void Sellect()
    {
        Sellected = true;
        switch (sellectAction)
        {
            case SellectAction.ColorTint:
                Image.color = TintColor;
                break;
            case SellectAction.Border:
                BorderImage.enabled = true;
                break;
        }
    }

    public virtual void Desellect()
    {
        Sellected = false;
        switch (sellectAction)
        {
            case SellectAction.ColorTint:
                Image.color = Color.white;
                break;
            case SellectAction.Border:
                BorderImage.enabled = false;
                break;
        }
    }

    protected static IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(1f);
        CanClick = true;
    }

}
using UnityEngine;
using UnityEngine.UI;
using AnilTools;

public enum Header
{
    H1, H2, H3
}

public class CanvasManager : Singleton<CanvasManager>
{
    
    [Header("UI")]
    public Text moneyText;
    
    public Text H1, H2, H3;

    public GameObject kanEfekti, playerMenu;

    public static void ShowTxt(Text text,string yazı, Color color , byte duration = 4)
    {
        text.text = yazı;
        text.color = color;
        
        new Timer(duration,() =>
        { 
            text.text = string.Empty;
        });
    }

    public void ShowTxt(Header header, string yazı, Color color, byte duration = 4)
    {
        Text text = ProceedText(header);

        text.text = yazı;
        text.color = color;

        new Timer(duration, () =>
        {
            text.text = string.Empty;
        });
    }

    private Text ProceedText(Header header)
    {
        return header switch
        {
            Header.H1 => H1,
            Header.H2 => H1,
            Header.H3 => H2,
            _ => H3,
        };
    }

}
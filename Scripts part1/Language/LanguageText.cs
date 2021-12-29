
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LanguageText : MonoBehaviour
{
    private Text _text;
    public Text text
    {
        get
        {
            if (! _text)
            {
                _text = GetComponent<Text>();
            }
            return _text;   
        }
    }

    public string English;

    // on component added
    public void Reset()
    {
        _text = GetComponent<Text>();
        English = text.text;
    }

}

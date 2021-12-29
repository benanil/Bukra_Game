using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using NTextCat;
using NTextCat.Commons;

public class LanguageManager : MonoBehaviour
{
    // unity system.language ile uyumlu
    public enum LanguageEnum
    {
        [InspectorName("english")]
        en = 10,  // english
        [InspectorName("franchis")]
        fr = 14,  // franchis
        [InspectorName("germanish")]
        gr = 15,  // germanish
        [InspectorName("japan")]
        ja = 22,  // japan
        [InspectorName("portequise")]
        pt = 28,  // portequise
        [InspectorName("Turkish")]
        tr = 37,  // Turkish
        [InspectorName("chinese")]
        zh = 40,  // chinese
        [InspectorName("spanish")]
        es = 34,  // spanish
        [InspectorName("hindu")]
        hi = 99   // hindu
    }

    public LanguageEnum CurrentLanguage = LanguageEnum.en;
    public int ToLanguage = (int)LanguageEnum.tr;

    [Tooltip("test amaçlı")]
    public string word;

    public int iteration;

    public List<LanguagePice> languagePices;
    public List<LanguageText> languageTexts;


    public void TranslateLanguagesAsync(int target)
    {
        var to = (LanguageEnum)target;

        if (CurrentLanguage != to)
        {
            for (int i = 0; i < languageTexts.Count; i++)
            {
                if (!Contain(languageTexts[i].text.text)){
                    GenerateWordLanguage(languageTexts[i].text.text, CurrentLanguage);
                    languageTexts[i].text.text = Find(to, languageTexts[i].text.text);
                }
            }

            CurrentLanguage = to;
        }
    }

    public void LoadLanguages()
    {
        for (int i = 0; i < languageTexts.Count; i++){
            if (!Contain(languageTexts[i].text.text)){
                languageTexts[i].text.text = Find((LanguageEnum)ToLanguage, languageTexts[i].text.text);
            }
        }
    }

    private string Find(LanguageEnum to, string word)
    {
        for (int i = 0; i < languagePices.Count; i++)
            for (int j = 0; j < languagePices[i].AllLanguageMeanings.Count; j++)
                if (languagePices[i].AllLanguageMeanings[j].word.Equals(word,StringComparison.OrdinalIgnoreCase))
                    return languagePices[i].AllLanguageMeanings.Find(x => x.languageEnum == to).word;

        return "not localised text";
    }

    private bool Contain(string text)
    {
        if (languagePices.Count == 0)
            return false;

        for (int i = 0; i < languagePices.Count; i++)
                if (languagePices[i].Name.Equals(text,StringComparison.OrdinalIgnoreCase))
                    return true;

        return false;
    }

    public string Detect(string word)
    {
        var factory = new RankedLanguageIdentifierFactory();
        
        var identifier = factory.Load("Assets/Resources/Scripts/Language/LanguageDetection/src/LanguageModels/Core14.profile.xml"); // can be an absolute or relative path. Beware of 260 chars limitation of the path length in Windows. Linux allows 4096 chars.
        var languages = identifier.Identify(word);
        var mostCertainLanguage = languages.FirstOrDefault();

        if (mostCertainLanguage.Item1.Iso639_3 == null){
            return string.Empty;
        }

        return mostCertainLanguage.Item1.Iso639_3;
    }

    [ContextMenu("Clean")]
    public void Clean()
    {
        languagePices.Clear();
        languageTexts.Clear();
    }

    [ContextMenu("translate")]
    public void Translate()
    {
        Debug.Log(Translator.Translate(word, CurrentLanguage.ToString(), ((LanguageEnum)ToLanguage).ToString()));
    }

    [ContextMenu("Fix")]
    public void Fix()
    {
        foreach (LanguageText item in languageTexts)
        {
            item.text.text = item.English;
        }
    }

    // base en
    private void GenerateWordLanguage(string word, LanguageEnum from)
    {
        languagePices.Add(new LanguagePice(word));
        var index = languagePices.Count - 1;
        /*
        if (Application.isPlaying){
            StartCoroutine(Translator.Translate(word, from.ToString(), "en",(x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.en, x))));
            StartCoroutine(Translator.Translate(word, from.ToString(), "fr",(x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.fr, x))));
            StartCoroutine(Translator.Translate(word, from.ToString(), "es",(x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.es, x))));
            StartCoroutine(Translator.Translate(word, from.ToString(), "pt",(x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.pt, x))));
            StartCoroutine(Translator.Translate(word, from.ToString(), "gr",(x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.gr, x))));
            StartCoroutine(Translator.Translate(word, from.ToString(), "hi",(x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.hi, x))));
            StartCoroutine(Translator.Translate(word, from.ToString(), "ja",(x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.ja, x))));
            StartCoroutine(Translator.Translate(word, from.ToString(), "zh",(x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.zh, x))));
        }
        else{
            EditorCoroutineUtility.StartCoroutine(Translator.Translate(word, from.ToString(), "en", (x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.en, x))),this);
            EditorCoroutineUtility.StartCoroutine(Translator.Translate(word, from.ToString(), "fr", (x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.fr, x))),this);
            EditorCoroutineUtility.StartCoroutine(Translator.Translate(word, from.ToString(), "es", (x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.es, x))),this);
            EditorCoroutineUtility.StartCoroutine(Translator.Translate(word, from.ToString(), "pt", (x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.pt, x))),this);
            EditorCoroutineUtility.StartCoroutine(Translator.Translate(word, from.ToString(), "gr", (x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.gr, x))),this);
            EditorCoroutineUtility.StartCoroutine(Translator.Translate(word, from.ToString(), "hi", (x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.hi, x))),this);
            EditorCoroutineUtility.StartCoroutine(Translator.Translate(word, from.ToString(), "ja", (x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.ja, x))),this);
            EditorCoroutineUtility.StartCoroutine(Translator.Translate(word, from.ToString(), "zh", (x) => languagePices[index].Add(new WordAndLanguageTuple(LanguageEnum.zh, x))),this);
        }
        */
    }

    [Serializable]
    public struct LanguagePice
    {
        public string Name;
        public List<WordAndLanguageTuple> AllLanguageMeanings;

        public LanguagePice(List<WordAndLanguageTuple> allLanguageMeanings)
        {
            Name = allLanguageMeanings[0].word;
            AllLanguageMeanings = allLanguageMeanings;
        }

        public void Add(WordAndLanguageTuple wordAndLanguage)
        {
            AllLanguageMeanings.Add(wordAndLanguage);
        }

        public LanguagePice(string name)
        {
            Name = name;
            AllLanguageMeanings = new List<WordAndLanguageTuple>();
        }

        public override bool Equals(object obj)
        {
            return obj is LanguagePice pice &&
                   EqualityComparer<List<WordAndLanguageTuple>>.Default.Equals(AllLanguageMeanings, pice.AllLanguageMeanings);
        }
        public override int GetHashCode()
        {
            return 1409883250 + EqualityComparer<List<WordAndLanguageTuple>>.Default.GetHashCode(AllLanguageMeanings);
        }
        public static bool operator ==(LanguagePice a, LanguagePice b)
        {
            return a.Equals(null);
        }
        public static bool operator !=(LanguagePice a, LanguagePice b)
        {
            return !a.Equals(null);
        }

    }

    [Serializable]
    public struct WordAndLanguageTuple
    {
        public string word;
        public LanguageEnum languageEnum;

        public WordAndLanguageTuple(LanguageEnum languageEnum,string word)
        {
            this.word = word;
            this.languageEnum = languageEnum;
        }

        public override bool Equals(object obj)
        {
            return obj is WordAndLanguageTuple tuple &&
                   word == tuple.word &&
                   languageEnum == tuple.languageEnum;
        }
        public override int GetHashCode()
        {
            int hashCode = 1131092055;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(word);
            hashCode = hashCode * -1521134295 + languageEnum.GetHashCode();
            return hashCode;
        }
        public static bool operator ==(WordAndLanguageTuple a, WordAndLanguageTuple b)
        {
            return a.word.Equals(b.word, StringComparison.OrdinalIgnoreCase) && a.languageEnum == b.languageEnum;
        }
        public static bool operator !=(WordAndLanguageTuple a, WordAndLanguageTuple b)
        {
            return !a.word.Equals(b.word, StringComparison.OrdinalIgnoreCase) && a.languageEnum != b.languageEnum;
        }

    }

}

#if UNITY_EDITOR

[CustomEditor(typeof(LanguageManager))]
public class LanguageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var _target = (LanguageManager)target;

        if (GUILayout.Button("translate Languages"))
        {
            _target.TranslateLanguagesAsync(_target.ToLanguage);
        }

        if (GUILayout.Button("Refresh"))
        {
            _target.Clean();
            _target.languageTexts = FindObjectsOfType<LanguageText>().ToList().FindAll(x => !x.English.IsNullOrEmpty());
        }

        if (GUILayout.Button("Detect"))
        {
            _target.Detect("some text");
        }

        if (GUILayout.Button("translate"))
        {
            _target.Translate();
        }

    }
}



#endif

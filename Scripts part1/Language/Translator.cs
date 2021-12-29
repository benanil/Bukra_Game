using System;
using System.Collections;

public static class Translator
{
    [Obsolete]
    public static IEnumerator Translate(string word, string from = "en", string to = "tr", Action<string> endAction = null)
    {
        /*
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={from}&tl={to}&dt=t&q={HttpUtility.UrlEncode(word)}";
                UnityWebRequest req = UnityWebRequest.Get(url);

                yield return req.SendWebRequest();

                string result = req.downloadHandler.text;//.Substring(4, req.downloadHandler.text.IndexOf("\"", 4, StringComparison.Ordinal) - 4);

                if (string.IsNullOrEmpty(result) || result.Contains("null")){
                    result = word;
                }

                Debug.Log($"from {from} to {to} result: {result}");

                endAction(result);

                req.Dispose();
          */
        yield return word;
    }

}

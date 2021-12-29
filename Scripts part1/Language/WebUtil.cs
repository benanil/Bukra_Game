/*
 *Author: Jeff Liu 
 *
 *Under MIT License
 *jebberwocky@gmail.com
 */
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Web;

/// <summary>
/// Summary description for WebUtil
/// </summary>
public class WebUtil
{
	public WebUtil()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    [Obsolete("not ready")]
    public static String WebRequestPost(String url, byte[] data)
    {
       // try
       // {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentLength = data.Length;
            request.Referer = "http://auto.volx.cn";
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();
            WebResponse response =  request.GetResponse();
            if(response !=null)
            {
                using(StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            return null;
        //}
    }

    /// <summary>
    /// General WebRequest
    /// </summary>
    /// <param name="url">URL</param>
    /// <returns>Result as string</returns>
    public static String WebRequest(String url)
    {
        HttpWebResponse response = null;
        try
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            // Get response  
            using (response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream  
                StreamReader reader = new StreamReader(response.GetResponseStream());

                // Console application output  
                return reader.ReadToEnd();
            }
        }
        catch (WebException wex)
        {
            // This exception will be raised if the server didn't return 200 - OK  
            // Try to retrieve more information about the network error  
            if (wex.Response != null)
            {
                using (HttpWebResponse errorResponse = (HttpWebResponse)wex.Response)
                {
                    throw new Exception("language translate error");
                }
            }
        }
        finally
        {
            if (response != null) { response.Close(); }
        }
        return "{}";
    }
}

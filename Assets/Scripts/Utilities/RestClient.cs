using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class RestClient
{

    public delegate void WebResponse(JSONObject obj);

    public const string API_URL = "https://super-game-api.herokuapp.com/";
    //public const string API_URL = "http://localhost:8000/";

    public static IEnumerator RunRequest(UnityWebRequest uwr, WebResponse response)
    {
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
            response(null);
        }
        else
        {
            Debug.Log(uwr.downloadHandler.text);
            response(new JSONObject(uwr.downloadHandler.text));
        }
    }

    public static IEnumerator GetScore(WebResponse response)
    {
        UnityWebRequest www = UnityWebRequest.Get(API_URL + "scores");
        yield return RunRequest(www, response);
    }
    public static IEnumerator GetUsers(WebResponse response)
    {
        UnityWebRequest www = UnityWebRequest.Get(API_URL + "users");
        yield return RunRequest(www, response);
    }
    public static IEnumerator GetUser(string id, WebResponse response)
    {
        UnityWebRequest www = UnityWebRequest.Get(API_URL + "user/" + id);
        yield return RunRequest(www, response);
    }
    public static IEnumerator GetItems(WebResponse response)
    {
        UnityWebRequest www = UnityWebRequest.Get(API_URL + "items");
        yield return RunRequest(www, response);
    }
    public static IEnumerator UpdateUser(string id, string icon, WebResponse response)
    {
        byte[] myData = System.Text.Encoding.UTF8.GetBytes("{\"icon\":" + icon + "}");
        UnityWebRequest www = UnityWebRequest.Put(API_URL + "user/update/" + id, myData);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return RunRequest(www, response);
    }
    public static IEnumerator CreateUser(string username, int avatar, WebResponse response)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("username", username));
        formData.Add(new MultipartFormDataSection("icon", avatar.ToString()));
        formData.Add(new MultipartFormDataSection("money", "500"));
        formData.Add(new MultipartFormDataSection("score", "0"));

        UnityWebRequest www = UnityWebRequest.Post(API_URL + "user", formData);
        www.chunkedTransfer = false;
        yield return RunRequest(www, response);
    }
    public static IEnumerator BuyItem(string id, string  item, WebResponse response)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("item", item));

        UnityWebRequest www = UnityWebRequest.Post(API_URL + "user/buy/"+id, formData);
        www.chunkedTransfer = false;
        yield return RunRequest(www, response);
    }
}

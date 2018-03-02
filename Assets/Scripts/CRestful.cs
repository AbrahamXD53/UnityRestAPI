using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class CRestful : MonoBehaviour
{

    //public const string API_URL="http://localhost:8000/";
    public const string API_URL = "https://super-game-api.herokuapp.com/";

    public delegate void WebResponse(JSONObject obj);

    public InputField inputField, inputFieldId;

    public GameObject button;
    public List<Button> buttons;

    public Transform parent;
    IEnumerator GetScore(WebResponse response)
    {
        UnityWebRequest www = UnityWebRequest.Get(API_URL + "scores");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            JSONObject jsonObj = new JSONObject(www.downloadHandler.text);
            response(jsonObj);
        }
    }

    IEnumerator SetScore(string value, WebResponse response)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("value", value));

        UnityWebRequest www = UnityWebRequest.Post(API_URL + "score/store", formData);
        www.chunkedTransfer = false;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

        }
    }

    IEnumerator UpdateScore(string id, string value, WebResponse response)
    {

        byte[] myData = System.Text.Encoding.UTF8.GetBytes("{\"value\":" + value + "}");
        UnityWebRequest www = UnityWebRequest.Put(API_URL + "score/update/" + id, myData);
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

        }
    }

    public void SendScore()
    {
        StartCoroutine(SetScore(inputField.text, (response) =>
        {
            print(response.ToString());
        }));
    }

    public void SendUpdateScore()
    {
        StartCoroutine(UpdateScore(inputFieldId.text, inputField.text, (response1) =>
        {
            print(response1.ToString());
        }));
    }


    public void Refresh()
    {
        print("Refreshing");
        StartCoroutine(RestClient.GetScore((response) =>
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            buttons = new List<Button>();
            for (int i = 0; i < response.Count; i++)
            {
                var go = Instantiate(button, parent);
                buttons.Add(go.GetComponent<Button>());
                buttons[i].transform.GetComponentInChildren<Text>().text = response[i]["id"].ToString() + " : " + response[i]["value"].ToString();
                print(response[i]["id"].ToString() + " : " + response[i]["value"].ToString());
            }
        }));
    }
    void Start()
    {
        StartCoroutine(RestClient.GetScore((response) =>
        {
            buttons = new List<Button>();
            for (int i = 0; i < response.Count; i++)
            {
                var go = Instantiate(button, parent);
                buttons.Add(go.GetComponent<Button>());
                buttons[i].transform.GetComponentInChildren<Text>().text = response[i]["id"].ToString() + " : " + response[i]["value"].ToString();
                print(response[i]["id"].ToString() + " : " + response[i]["value"].ToString());
            }
        }));
        InvokeRepeating("Refresh", 15, 15);
    }



}

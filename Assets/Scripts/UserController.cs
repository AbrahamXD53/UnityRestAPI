using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserController : MonoBehaviour
{

    /*User Data */
    [SerializeField] Dropdown avatar;
    [SerializeField] Button saveBtn;
    [SerializeField] Text usernameLabel, moneyLabel, scoreLabel;

    [SerializeField] Transform itemsParent;
    [SerializeField] GameObject itemPrefab;


    [SerializeField] Sprite[] sprites;
    [SerializeField] Sprite[] currencySprites;

    /*Store */
    [SerializeField] GameObject itemStorePrefab;
    [SerializeField] Transform itemStoreParent;

    private string playerId = "";
    private Dictionary<int, int> items = new Dictionary<int, int>();

    void Awake()
    {
        playerId = PlayerPrefs.GetString("PlayerId", "1");

        StartCoroutine(RestClient.GetItems((response) =>
        {
            StartCoroutine(RestClient.GetUser(playerId, (responseUser) =>
            {
                if (responseUser)
                {
                    UpdateUIUser(responseUser);
                }
            }));
            if (response)
            {
                for (int i = 0; i < response.Count; i++)
                {
                    var go = Instantiate(itemStorePrefab, itemStoreParent);
                    go.GetComponent<Button>().onClick.AddListener(() => { OnUIButtonClick(go); });
                    go.name = response[i]["id"].i.ToString();
                    go.transform.GetChild(0).GetComponent<Image>().sprite = sprites[response[i]["icon"].i];
                    go.transform.GetChild(4).GetComponent<Image>().sprite = currencySprites[response[i]["currency"].i];
                    go.transform.GetChild(2).GetComponent<Text>().text = response[i]["money"].i > 0 ? response[i]["money"].i.ToString() : response[i]["points"].i.ToString();
                    go.transform.GetChild(3).GetComponent<Text>().text = response[i]["cost"].i > 0 ? response[i]["cost"].i.ToString() : "Free";
                    items.Add((int)response[i]["id"].i, (int)response[i]["icon"].i);
                }
            }
        }));
    }
    private void OnUIButtonClick(GameObject button)
    {
        print("Buy " + button.name);
        StartCoroutine(RestClient.BuyItem(playerId, button.name, (response) =>
        {
            if (response != null)
                UpdateUIUser(response);
        }));
    }
    public void Close()
    {
        SceneManager.UnloadSceneAsync(2);
    }

    private void UpdateUIUser(JSONObject response)
    {
        avatar.value = (int)response["icon"].i;
        usernameLabel.text = response["username"].str;
        moneyLabel.text = response["money"].i.ToString();
        scoreLabel.text = response["score"].i.ToString();

        for (int i = 0; i < response["purchases"].Count; i++)
        {
            GameObject go;
            if (itemsParent.childCount > i && itemsParent.GetChild(i))
            {
                go = itemsParent.GetChild(i).gameObject;
            }
            else
            {
                go = Instantiate(itemPrefab, itemsParent);
            }
            go.GetComponent<Image>().sprite = sprites[items[(int)response["purchases"][i]["item_id"].i]];
        }
    }

    public void Save()
    {
        StartCoroutine(RestClient.UpdateUser(playerId, avatar.value.ToString(), (response) =>
        {
            if (response != null)
            {
                UpdateUIUser(response);
            }
        }));
    }

}

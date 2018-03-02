using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Leaderboards : MonoBehaviour
{

    [SerializeField]
    Text lastUpdateText;

    [SerializeField]
    Sprite[] userIcons;

    List<GameObject> buttons=new List<GameObject>();

    [SerializeField]
    GameObject button;

    [SerializeField]
    Transform parent;
    void Start()
    {
		InvokeRepeating("Refresh",0,15);
    }

    public void Refresh()
    {
        StartCoroutine(RestClient.GetUsers((response) =>
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            lastUpdateText.text = string.Format("{0} usuarios, ultima actualización: {1}", response["data"].Count, response["time"].str);
            buttons = new List<GameObject>();
            for (int i = 0; i < response["data"].Count; i++)
            {
                var go = Instantiate(button, parent);
                buttons.Add(go);
                buttons[i].transform.GetComponentInChildren<Text>().text = response["data"][i]["id"].ToString() + " : " + response["data"][i]["username"].str;
                buttons[i].transform.GetChild(1).GetComponent<Image>().sprite = userIcons[response["data"][i]["icon"].i];
                go.name = response["data"][i]["id"].i + "";
                go.GetComponent<Button>().onClick.AddListener(() => OnUIButtonClick(go));
            }
        }));
    }
    private void OnUIButtonClick(GameObject button)
    {
        PlayerPrefs.SetString("PlayerId",button.name);
		SceneManager.LoadScene(2,LoadSceneMode.Additive);
    }

	public void CreateUser(){
		SceneManager.LoadScene(1,LoadSceneMode.Additive);
	}

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserEditor : MonoBehaviour
{

    [SerializeField]
    InputField username;
    [SerializeField]
    Dropdown avatar;

    void Start()
    {

    }

    void Update()
    {

    }
	public void Close(){
		SceneManager.UnloadSceneAsync(1);
	}
    public void Create()
    {
        if (username.text.Length > 0)
            StartCoroutine(RestClient.CreateUser(username.text, avatar.value, (response) =>
            {
                if (response)
                {
					Close();
                    print(response.ToString());
                }
				else{
					print("error");
				}
            }));
    }
}

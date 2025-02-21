using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using TMPro;
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class HttpHandlerFakeAPI : MonoBehaviour
{
    [SerializeField]
    private RawImage picture;
    [SerializeField]
    private string url = "https://rickandmortyapi.com/api/character";
    [SerializeField] private string DBurl;
    [SerializeField] private GameObject[] cards; 

    public void SendRequest(int id)
    {
        StartCoroutine(GetFakeUser(id));
        //StartCoroutine(GetCharacter(56));    
    }

    IEnumerator GetCharacter(int id, int index)
    {
        UnityWebRequest www = UnityWebRequest.Get(url + "/" + id);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.responseCode == 200)
            {

                Personaje personaje = JsonUtility.FromJson<Personaje>(www.downloadHandler.text);
                cards[index].GetComponentInChildren<TMP_Text>().text = personaje.name;
                Debug.Log(personaje.name + " is a " + personaje.species);
                StartCoroutine(GetImage(personaje.image, index));


            }
            else
            {
                string mensaje = "status:" + www.responseCode;
                mensaje += "\nErro: " + www.error;
                Debug.Log(mensaje);
            }
        }
    }
    IEnumerator GetImage(string imageUrl, int index)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            cards[index].GetComponentInChildren<RawImage>().texture = texture;
            //picture.texture = texture;
        }
    }
    IEnumerator GetFakeUser(int id)
    {
        UnityWebRequest request = UnityWebRequest.Get(DBurl + "/users/" + id);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
            Debug.Log(request.error);
        else
        {
            if(request.responseCode == 200)
            {
                FakeUser user = JsonUtility.FromJson<FakeUser>(request.downloadHandler.text);
                GameObject.Find("username").GetComponent<TMP_Text>().text = $"User: {user.username}";
                Console.WriteLine(user.username);
                for (int i = 0; i < user.deck.Length; i++)
                {
                    var cardId = user.deck[i];
                    StartCoroutine(GetCharacter(cardId, i));
                }
                
            }
            else 
            {
                string mensaje = "status:" + request.responseCode;
                mensaje += "Error" + request.error;
                Debug.Log(mensaje);
            }
        }
    }

}

[System.Serializable]
public class PersonajeAPI
{
    public int id;
    public string name;
    public string species;
    public string image;
}


[System.Serializable]
public class FakeUser
{
    public string username;
    public string state;
    public int[] deck;

}

/*[System.Serializable]
public class ListaDePersonajes
{
    public Personaje[] results;
}
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;


public class ApiUpdate : MonoBehaviour
{
    // [SerializeField] private GameObject playerPrefab;
    
    [SerializeField] private TMP_InputField idResult;
    [SerializeField] private TMP_Text userNameResult;
    [SerializeField] private RectTransform playerSprite;
    [SerializeField] private TMP_InputField healthResult;
    [SerializeField] private TMP_InputField scoreResult;

    private string apiUrl = "http://192.168.1.201:7265/Player";

    public void PullOnlinePlayer(GameObject playerPrefab) {
        StartCoroutine(PullPlayer(playerPrefab));
    }
    
    public void PushUserPlayer(GameObject playerPrefab) {
        StartCoroutine(PushPlayer(playerPrefab));
    }

    private IEnumerator PullPlayer(GameObject playerPrefab) 
    {
        // GET request for weather data
        var httpRequest = NewRequest(apiUrl + "?id=" + idResult.text, RequestType.GET);
        yield return httpRequest.SendWebRequest();
        var responseFromJson = JsonUtility.FromJson<PlayerStatus>(httpRequest.downloadHandler.text);

        Vector3 playerPos = playerSprite.forward;
        
        string xVal = playerPos.x.ToString();
        string yVal = playerPos.y.ToString();
        string angleVal = playerSprite.pivot.y.ToString();
        
        idResult.text = responseFromJson.id;
        userNameResult.text = responseFromJson.userName;
        xVal = responseFromJson.posX;
        yVal = responseFromJson.posY;
        angleVal = responseFromJson.angle;
        healthResult.text = responseFromJson.health;
        scoreResult.text = responseFromJson.score;
    }

    private IEnumerator PushPlayer(GameObject playerPrefab)
    {
        var requestBody = new PlayerStatus()
        {
            id = idResult.text,
            userName = userNameResult.text,
            posX = playerSprite.forward.x.ToString(),
            posY = playerSprite.forward.y.ToString(),
            angle = playerSprite.pivot.y.ToString(),
            health = healthResult.text,
            score = scoreResult.text
        };

    // PUT request for player data
        var httpRequest = NewRequest(apiUrl + "?id=" + idResult.text, RequestType.PUT, requestBody);
        yield return httpRequest.SendWebRequest();
    }

    private UnityWebRequest NewRequest(string path, RequestType type, object data = null) 
    {
        var request = new UnityWebRequest(path, type.ToString());

        if (data != null) {
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }
}

public enum RequestType 
{
    GET = 0,
    POST = 1,
    PUT = 2
}

public class PlayerStatus
{
    public string id;
    public string userName;
    public string posX;
    public string posY;
    public string angle;
    public string health;
    public string score;
}

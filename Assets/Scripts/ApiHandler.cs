using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


public class ApiHandler : MonoBehaviour
{

    [SerializeField] private TMP_InputField idResult;
    [SerializeField] private TMP_InputField userNameResult;
    [SerializeField] private TMP_InputField posXResult;
    [SerializeField] private TMP_InputField posYResult;
    [SerializeField] private TMP_InputField angleResult;
    [SerializeField] private TMP_InputField healthResult;
    [SerializeField] private TMP_InputField scoreResult;

    private string apiUrl = "http://192.168.1.201:7265/Player";
    
    public void ReadPlayer() {
        StartCoroutine(ReadRequests(apiUrl));
    }
    
    public void WritePlayer() {
        StartCoroutine(WriteRequests(apiUrl));
    }

    private IEnumerator ReadRequests(string apiUrl) 
    {
        // GET request for weather data
        var httpRequest = NewRequest(apiUrl + "?id=" + idResult.text, RequestType.GET);
        yield return httpRequest.SendWebRequest();
        var responseFromJson = JsonUtility.FromJson<PlayerStatus>(httpRequest.downloadHandler.text);

        idResult.text = responseFromJson.id;
        userNameResult.text = responseFromJson.userName;
        posXResult.text = responseFromJson.posX;
        posYResult.text = responseFromJson.posY;
        angleResult.text = responseFromJson.angle;
        healthResult.text = responseFromJson.health;
        scoreResult.text = responseFromJson.score;
    }

    private IEnumerator WriteRequests(string apiUrl)
    {
        var requestBody = new PlayerStatus()
        {
            id = idResult.text,
            userName = userNameResult.text,
            posX = posXResult.text,
            posY = posYResult.text,
            angle = angleResult.text,
            health = healthResult.text,
            score = scoreResult.text
        };

    // PUT request for player data
        var httpRequest = NewRequest(apiUrl + "?id=" + idResult.text, RequestType.PUT, requestBody);
        yield return httpRequest.SendWebRequest();
        var responseFromJson = JsonUtility.FromJson<PlayerStatus>(httpRequest.downloadHandler.text);
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


/*
[Serializable]
public class PlayerId {
    public string id;
}
*/

// public class PostResult
// {
//     public string success { get; set; }
// }
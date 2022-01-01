using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;


public class ApiHandler : MonoBehaviour
{
    private string apiUrl = "http://192.168.1.201:7265/Players";

    private PlayerStatus userPlayer = new PlayerStatus();
    private List<PlayerStatus> onlinePlayers = new List<PlayerStatus>();
    
    public void PushUserPlayer(string userId) {
        // GET response for opc players to spawn players
        var httpRequest = NewRequest(apiUrl + "/User?userId=" + userId, RequestType.GET);
        
        //Execute asynchronous request
        StartCoroutine(Init_PushUserPlayer(httpRequest));
    }

    public PlayerStatus PullUserPlayer(string userId) 
    {
        // GET response for opc players to spawn players
        var httpRequest = NewRequest(apiUrl + "/User?userId=" + userId, RequestType.GET);

        //Execute asynchronous request
        StartCoroutine(Init_PullUserPlayer(httpRequest));

        // onlinePlayers = JsonUtility.FromJson<List<PlayerStatus>>(httpRequest.downloadHandler.text);
        return userPlayer;
    }
    
    public List<PlayerStatus> PullOnlinePlayers(string userId) 
    {
        // GET response for opc players to spawn players
        var httpRequest = NewRequest(apiUrl + "/Online?userId=" + userId, RequestType.GET);

        //Execute asynchronous request
        StartCoroutine(Init_PullOnlinePlayers(httpRequest));

        // onlinePlayers = JsonUtility.FromJson<List<PlayerStatus>>(httpRequest.downloadHandler.text);
        return onlinePlayers;
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
    private IEnumerator Init_PullUserPlayer(UnityWebRequest request)
    {
        // yield to wait for the request to return
        yield return request.SendWebRequest();

        // after this, you will have a result
        string result = request.downloadHandler.text;
        
        // update user 
        userPlayer = JsonUtility.FromJson<PlayerStatus>(result);
    }
    
    private static IEnumerator Init_PushUserPlayer(UnityWebRequest request)
    {
        yield return request.SendWebRequest();
    }
    
    private IEnumerator Init_PullOnlinePlayers(UnityWebRequest request)
    {
        // yield to wait for the request to return
        yield return request.SendWebRequest();

        // after this, you will have a result
        string result = request.downloadHandler.text;

        try
        {
            // reset onlinePlayers
            onlinePlayers = new List<PlayerStatus>();
            
            if (result[0] == '[')
            {
                // format response ready for splitting 
                result = result.Replace("[{", "").Replace("}]", "").Replace("},{", "[");
        
                // update onlinePlayers for each entry returned
                foreach (var playerSubString in result.Split('['))
                {
                    onlinePlayers.Add(JsonUtility.FromJson<PlayerStatus>("{" + playerSubString + "}"));
                }
            }
            else if (result[0] == '{')
            {
                // treat result as json
            
            } else {
                // treat result as error
                Debug.LogError("Invalid JSON response from server.");
            }
        } catch (IndexOutOfRangeException)
        {
            // treat result as error
            Debug.LogError("Invalid JSON response from server.");
        }
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

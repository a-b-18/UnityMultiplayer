using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;


public class ApiHandler : MonoBehaviour
{
    private string apiUrl = "http://192.168.1.201:7265/Players";

    public PlayerStatus PullUserPlayer(string userId)
    {
        // GET request for user player to spawn
        UnityWebRequest pullUserRequest = CreateRequest(apiUrl + "/User?userId=" + userId, RequestType.GET);

        // Execute request asynchronously
        StartCoroutine(SendRequestAsync(pullUserRequest));
        
        // Pass result to PlayerStatus
        string result = pullUserRequest.downloadHandler.text;
        return JsonUtility.FromJson<PlayerStatus>(result);
    }
    
    public List<PlayerStatus> PullOnlinePlayers(string userId) 
    {
        // GET request for opc players to spawn players
        UnityWebRequest pullUsersRequest = CreateRequest(apiUrl + "/Online?userId=" + userId, RequestType.GET);

        // Execute request asynchronously
        StartCoroutine(SendRequestAsync(pullUsersRequest));
        
        //  Pass result to List<PlayerStatus>
        string result = pullUsersRequest.downloadHandler.text;
        return JsonToPlayerList(result);
    }
    
    public bool PushUserPlayer(PlayerStatus userPlayer) {
        // PUT request for API to update player
        UnityWebRequest pushUserRequest = CreateRequest(apiUrl + "/User", RequestType.PUT, userPlayer);
        
        // Execute request asynchronously
        StartCoroutine(SendRequestAsync(pushUserRequest));
        
        //  Pass result to List<PlayerStatus>
        string result = pushUserRequest.downloadHandler.text;

        return bool.Parse(result);
    }

    private UnityWebRequest CreateRequest(string path, RequestType type, object data = null) 
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

    private IEnumerator SendRequestAsync (UnityWebRequest request)
    {
        // yield to wait for the request to return
        UnityWebRequestAsyncOperation apiResponse = request.SendWebRequest();
        yield return apiResponse;
    }

    private List<PlayerStatus> JsonToPlayerList(string result)
    {
        // reset onlinePlayers
        var onlinePlayers = new List<PlayerStatus>();
        
        try
        {
            // format response ready for splitting 
            result = result.Replace("[{", "").Replace("}]", "").Replace("},{", "[");
    
            // update onlinePlayers for each entry using JsonUtility
            foreach (var playerSubString in result.Split('['))
            {
                onlinePlayers.Add(JsonUtility.FromJson<PlayerStatus>("{" + playerSubString + "}"));
            }
        } catch (IndexOutOfRangeException)
        {
            // treat result as error
            Debug.LogError($"'{result}' is not a valid JSON array.");
        }
        
        return onlinePlayers;
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

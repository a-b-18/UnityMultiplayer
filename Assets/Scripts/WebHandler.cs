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


public class WebHandler : MonoBehaviour
{
    [SerializeField] private LoginHandler loginHandler;
    [SerializeField] private InstanceHandler instanceHandler;
    private string apiUrl = "http://192.168.1.201:7265/Players";

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(PullUserDto(loginHandler.GetUser()));
    }

    // Update is called once per frame
    private void Update()
    {
        StartCoroutine(PushUserDto(instanceHandler.GetUserInstance()));
        StartCoroutine(PullOnlineDto(loginHandler.GetUser()));
    }

    private IEnumerator PullUserDto(string userId)
    {
        // GET request for user player to spawn
        var pullUserRequest = CreateRequest(apiUrl + "/User?userId=" + userId, RequestType.GET);
        
        // Execute request asynchronously
        yield return SendRequestAsync(pullUserRequest);
        
        // Check if response has been received yet
        var result = pullUserRequest.downloadHandler.text;
        var resultDto = JsonUtility.FromJson<PlayerDto>(result);

        // Return response as PlayerDto
        instanceHandler.SetUserInstance(resultDto);
    }

    private IEnumerator PullOnlineDto(string userId) 
    {
        // GET request for opc players to spawn players
        var pullUsersRequest = CreateRequest(apiUrl + "/Online?userId=" + userId, RequestType.GET);

        // Execute request asynchronously
        yield return SendRequestAsync(pullUsersRequest);
        
        // Check if response has been received yet
        var result = pullUsersRequest.downloadHandler.text;

        // Set online instances in game
        instanceHandler.SetOnlineInstances(JsonToPlayerList(result));
        
    }

    private IEnumerator PushUserDto(PlayerDto userPlayer) {
        // PUT request for API to update player
        var pushUserRequest = CreateRequest(apiUrl + "/User", RequestType.PUT, userPlayer);
        
        // Execute request asynchronously
        yield return SendRequestAsync(pushUserRequest);
        
        // Check if response has been received yet
        var result = pushUserRequest.downloadHandler.text;

        // Log error if request unsuccessful
        if (!bool.Parse(result)) {Debug.LogError($"Server error pushing user {userPlayer.id} to database.");};
    }

    private UnityWebRequest CreateRequest(string path, RequestType type, object data = null) 
    {
        var request = new UnityWebRequest(path, type.ToString());

        if (data != null) {
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("accept", "text/plain");
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    private static IEnumerator SendRequestAsync (UnityWebRequest request)
    {
        // yield to wait for the request to return
        var apiResponse = request.SendWebRequest();
        yield return apiResponse;

        switch (request.result)
        {
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError("Error: " + request.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(": HTTP Error: " + request.error);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log(":\nReceived: " + request.downloadHandler.text);
                break;
        }
    }

    private static List<PlayerDto> JsonToPlayerList(string result)
    {
        // reset onlinePlayers
        var onlinePlayers = new List<PlayerDto>();
        
        try
        {
            // format response ready for splitting 
            result = result.Replace("[{", "").Replace("}]", "").Replace("},{", "[");
    
            // update onlinePlayers for each entry using JsonUtility
            foreach (var playerSubString in result.Split('['))
            {
                onlinePlayers.Add(JsonUtility.FromJson<PlayerDto>("{" + playerSubString + "}"));
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

public class PlayerDto
{
    public int id;
    public string userName;
    public float posX;
    public float posY;
    public float angle;
    public int health;
    public int score;
}

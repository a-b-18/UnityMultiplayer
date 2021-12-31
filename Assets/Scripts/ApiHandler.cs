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
    private string apiUrl = "http://192.168.1.201:7265/Player";

    private PlayerStatus userStatus = new PlayerStatus();
    private List<PlayerStatus> opcStatusList = new List<PlayerStatus>();

    private void Update()
    {
        // PullOnlinePlayers();
    }

    public void PushUserPlayer(GameObject userPlayer) {
        StartCoroutine(PushUser(userPlayer));
    }

    public List<PlayerStatus> PullOnlinePlayers() 
    {
        // GET response for opc players to spawn players
        var httpRequest = NewRequest(apiUrl + "/List", RequestType.GET);

        StartCoroutine(LoadHttpResponse(httpRequest));
        // httpRequest.SendWebRequest();
        
        // opcStatusList = JsonUtility.FromJson<List<PlayerStatus>>(httpRequest.downloadHandler.text);
        return opcStatusList;
    }

    private IEnumerator PushUser(GameObject userPlayer)
    {
        var userTransform = userPlayer.transform;
        var userLocalPosition = userPlayer.transform.localPosition;
        
        userStatus = new PlayerStatus()
        {
            id = 4.ToString(),
            userName = "alex",
            posX = userLocalPosition.x.ToString(),
            posY = userLocalPosition.y.ToString(),
            angle = userTransform.localRotation.y.ToString(),
            health = "100",
            score = "787"
        };

        // PUT request for player data
        var httpRequest = NewRequest(apiUrl + "?id=" + "4", RequestType.PUT, userStatus);
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
    
    public IEnumerator LoadHttpResponse(UnityWebRequest request)
    {
        // yield to wait for the request to return
        yield return request.SendWebRequest();

        // after this, you will have a result
        string result = request.downloadHandler.text;

        if (result[0] == '[')
        {
            // treat result as opcStatusList
            RefreshOpcStatusList(request.downloadHandler.text);
        }
        else if (result[0] == '{')
        {
            // treat result as json
            
        }
        else
        {
            // treat result as error
            Debug.LogError("Invalid JSON response from server.");
        }
    }

    private void RefreshOpcStatusList(string response)
    {
        // reset opcStatusList
        opcStatusList = new List<PlayerStatus>();
        
        // format response ready for splitting 
        response = response.Replace("[{", "").Replace("}]", "").Replace("},{", "[");
        
        // update opcStatusList for each entry returned
        foreach (var playerSubString in response.Split('['))
        {
            opcStatusList.Add(JsonUtility.FromJson<PlayerStatus>("{" + playerSubString + "}"));
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

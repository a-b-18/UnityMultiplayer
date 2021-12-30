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
    [SerializeField] private TMP_Text userNameResult;
    [SerializeField] private TMP_Text posXResult;
    [SerializeField] private TMP_Text posYResult;
    [SerializeField] private TMP_Text angleResult;
    [SerializeField] private TMP_Text healthResult;
    [SerializeField] private TMP_Text scoreResult;
    
    public void ReadPlayer() {
        StartCoroutine(MakeRequests());
    }

    private IEnumerator MakeRequests() 
    {
        // GET request for weather data
        // var httpBody = new PlayerId(){id = idResult.text};
        var httpRequest = NewRequest("http://192.168.1.201:7265/Player?id=" + idResult.text, RequestType.GET);
        yield return httpRequest.SendWebRequest();
        var responseFromJson = JsonUtility.FromJson<PlayerStatus>(httpRequest.downloadHandler.text);

        idResult.text = responseFromJson.id.ToString();
        userNameResult.text = responseFromJson.userName;
        posXResult.text = responseFromJson.posX.ToString(CultureInfo.InvariantCulture);
        posYResult.text = responseFromJson.posY.ToString(CultureInfo.InvariantCulture);
        angleResult.text = responseFromJson.angle.ToString(CultureInfo.InvariantCulture);
        healthResult.text = responseFromJson.health.ToString();
        scoreResult.text = responseFromJson.score.ToString();
        
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
    public int id;
    public string userName;
    public double posX;
    public double posY;
    public double angle;
    public int health;
    public int score;
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
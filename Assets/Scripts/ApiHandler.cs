using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class ApiHandler : MonoBehaviour
{

    [SerializeField] private TMP_Text tmpText;
    
    public void SetText() {
        StartCoroutine(MakeRequests());
    }

    private IEnumerator MakeRequests() 
    {
        // GET request for weather data
        var httpRequest = NewRequest("https://localhost:7265/WeatherForecast", RequestType.GET);
        yield return httpRequest.SendWebRequest();
        var responseFromJson = JsonUtility.FromJson<WeatherForecast>(httpRequest.downloadHandler.text);

        tmpText.text = responseFromJson.summary;
        
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

public class WeatherForecast
{
    public DateTime date;
    public int temperatureC;
    public int temperatureF;
    public string summary;
}


// [Serializable]
// public class PostData {
//     public string Hero;
//     public int PowerLevel;
// }

// public class PostResult
// {
//     public string success { get; set; }
// }
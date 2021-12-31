using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpcSession : MonoBehaviour
{
    private GameObject apiHandler;
    private PlayerStatus opcStatus;
    
    // Start is called before the first frame update
    void Start()
    {
        apiHandler = GameObject.FindGameObjectWithTag("API Handler");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RefreshOpc()
    {
        // opcStatus = apiHandler.transform.GetComponent<ApiHandler>().PullOnlinePlayer(opcStatus.id);
        // transform.localPosition = new Vector3(x: float.Parse(opcStatus.posX), y: float.Parse(opcStatus.posY), z: 0);
    }
}

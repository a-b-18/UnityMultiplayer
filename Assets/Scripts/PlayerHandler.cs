using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private GameObject userPlayerPrefab;
    [SerializeField] private GameObject opcPlayerPrefab;
    [SerializeField] public TMP_InputField UserInput;
    
    private bool userConnected = false;
    private bool userSpawned = false;
    private bool playersSpawned = false;
    private string userId = "0";
    private GameObject userInstance;
    private PlayerStatus userStatus;
    private GameObject apiHandler;
    private List<GameObject> opcInstances = new List<GameObject>();
    private List<PlayerStatus> playerStatuses = new List<PlayerStatus>();
    

    // Start is called before the first frame update
    void Start()
    {
        apiHandler = GameObject.FindGameObjectWithTag("API Handler");
    }

    // Update is called once per frame
    void Update()
    {
        switch (userConnected)
        {
            case true:
                if (!userSpawned)
                {
                    // for testing user id
                    userId = UserInput.text;
                    userSpawned = PullUserPlayer();
                } else if (!playersSpawned)
                {
                    playersSpawned = PullOnlinePlayers();
                } else {
                    PushUserPlayer();
                }
                break;
            case false:
                RemoveOpcInstances();
                playersSpawned = false;
                break;
        }
    }

    public void SimulateUserConnect()
    {
        userConnected = true;
    }

    public void SimulateUserDisconnect()
    {
        userConnected = false;
    }
    
    private bool PullUserPlayer()
    {
        // remove instance from game
        if (userInstance) {Destroy(userInstance);}
        
        userStatus = new PlayerStatus();
        userStatus = apiHandler.transform.GetComponent<ApiHandler>().PullUserPlayer(userId);
        
        try
        {
            userInstance = Instantiate(userPlayerPrefab,
                new Vector3(x: float.Parse(userStatus.posX), y: float.Parse(userStatus.posY), z: 0),
                Quaternion.identity);
            userInstance.transform.SetParent(GameObject.FindGameObjectWithTag("Game Canvas").transform, false);
        }
        catch (ArgumentNullException)
        {
            Debug.Log("Awaiting to receive user player ...");
            return false;
        }
        return true;
    }
    
    private void PushUserPlayer()
    {
        var userTransform = userInstance.transform;
        var userPosition = userTransform.localPosition;
        
        var userStatus = new PlayerStatus()
        {
            id = userId,
            userName = "alex",
            posX = userPosition.ToString(),
            posY = userPosition.y.ToString(),
            angle = userTransform.localRotation.y.ToString(),
            health = "100",
            score = "787"
        };
        apiHandler.transform.GetComponent<ApiHandler>().PushUserPlayer(userId);
    }
    
    private bool PullOnlinePlayers()
    {
        GameObject currentInstance;
        playerStatuses = new List<PlayerStatus>();
        playerStatuses = apiHandler.transform.GetComponent<ApiHandler>().PullOnlinePlayers(userId);
        
        if (playerStatuses.Count == 0) {return false;}
        
        foreach (var opcStatus in playerStatuses)
        {
            currentInstance = Instantiate(opcPlayerPrefab, new Vector3(x: float.Parse(opcStatus.posX), y: float.Parse(opcStatus.posY), z: 0), Quaternion.identity);
            currentInstance.transform.SetParent (GameObject.FindGameObjectWithTag("Game Canvas").transform, false);
            opcInstances.Add(currentInstance);
        }

        return true;
    }

    private void RemoveOpcInstances()
    {
        foreach (var opcItem in opcInstances)
        {
            // remove instance from game
            Destroy(opcItem);
        }

        // remove GameObjects from list
        opcInstances = new List<GameObject>();
    }
}

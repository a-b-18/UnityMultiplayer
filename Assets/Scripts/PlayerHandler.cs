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
    private bool userPlaying = false;
    private bool playersSpawned = true;
    private int userId = 4;
    private GameObject userInstance;
    private GameObject apiHandler;
    private List<GameObject> playerInstances = new List<GameObject>();
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
                if (!playersSpawned)
                {
                    // for testing user id
                    userId = int.Parse(UserInput.text);
                    
                    playersSpawned = SpawnPlayers();
                } else {
                    PushUserPlayer();
                }
                break;
            case false:
                DestroyOpcPlayers();
                playersSpawned = false;
                break;
        }
    }

    private void PushUserPlayer()
    {
        var userTransform = userInstance.transform;
        var userPosition = userTransform.localPosition;
        
        var userStatus = new PlayerStatus()
        {
            id = 4.ToString(),
            userName = "alex",
            posX = userPosition.ToString(),
            posY = userPosition.y.ToString(),
            angle = userTransform.localRotation.y.ToString(),
            health = "100",
            score = "787"
        };
        apiHandler.transform.GetComponent<ApiHandler>().PushUserPlayer(userStatus);
    }

    public void SimulateUserConnect()
    {
        userConnected = true;
    }

    public void SimulateUserDisconnect()
    {
        userConnected = false;
    }
    
    private bool SpawnPlayers()
    {
        GameObject currentInstance;
        playerStatuses = new List<PlayerStatus>();
        playerStatuses = apiHandler.transform.GetComponent<ApiHandler>().PullOnlinePlayers();
        
        if (playerStatuses.Count == 0) {return false;}
        
        foreach (var opcStatus in playerStatuses)
        {
            
            GameObject playerPrefab = opcPlayerPrefab;
            if (opcStatus.id == userId.ToString())
            {
                currentInstance = Instantiate(userPlayerPrefab, new Vector3(x: float.Parse(opcStatus.posX), y: float.Parse(opcStatus.posY), z: 0), Quaternion.identity);
                userInstance = currentInstance;
            }
            else
            {
                currentInstance = Instantiate(opcPlayerPrefab, new Vector3(x: float.Parse(opcStatus.posX), y: float.Parse(opcStatus.posY), z: 0), Quaternion.identity);
            }
            currentInstance.transform.SetParent (GameObject.FindGameObjectWithTag("Game Canvas").transform, false);
            playerInstances.Add(currentInstance);
        }

        return true;
    }

    private void DestroyOpcPlayers()
    {
        foreach (var opcItem in playerInstances)
        {
            Destroy(opcItem);
            playerInstances.Remove(opcItem);
        }
    }
}

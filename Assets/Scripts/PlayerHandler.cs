using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private GameObject userPlayerPrefab;
    [SerializeField] private GameObject opcPlayerPrefab;

    private bool userConnected = false;
    private bool opcRespawn = true;
    private GameObject userInstance;
    private GameObject opcInstance;
    private GameObject apiHandler;
    private List<GameObject> opcInstances = new List<GameObject>();
    private List<PlayerStatus> opcStatuses = new List<PlayerStatus>();
    

    // Start is called before the first frame update
    void Start()
    {
        SpawnUserPlayer();
        apiHandler = GameObject.FindGameObjectWithTag("API Handler");
    }

    // Update is called once per frame
    void Update()
    {
        switch (userConnected)
        {
            case true:
                PushUserPlayer();
                if (opcRespawn)
                {
                    SpawnOpcPlayers(); 
                    opcRespawn = false;
                }
                break;
            case false:
                DestroyOpcPlayers();
                opcRespawn = true;
                break;
        }
    }

    private void PushUserPlayer()
    {
        apiHandler.transform.GetComponent<ApiHandler>().PushUserPlayer(userInstance);
    }

    private void SpawnUserPlayer()
    {
        userInstance = Instantiate(userPlayerPrefab, userPlayerPrefab.transform.position, Quaternion.identity);
        userInstance.transform.SetParent (GameObject.FindGameObjectWithTag("Game Canvas").transform, false);
    }
    
    public void SimulateUserConnect()
    {
        userConnected = true;
    }
    
    public void SimulateUserDisconnect()
    {
        userConnected = false;
    }
    
    private void SpawnOpcPlayers()
    {
        opcStatuses = apiHandler.transform.GetComponent<ApiHandler>().PullOnlinePlayers();
        
        foreach (var opcStatus in opcStatuses)
        {
            opcInstance = Instantiate(opcPlayerPrefab, new Vector3(x: float.Parse(opcStatus.posX), y: float.Parse(opcStatus.posY), z: 0), Quaternion.identity);
            opcInstance.transform.SetParent (GameObject.FindGameObjectWithTag("Game Canvas").transform, false);
            opcInstances.Add(opcInstance);
        }
    }

    private void DestroyOpcPlayers()
    {
        foreach (var opcItem in opcInstances)
        {
            Destroy(opcItem);
            opcInstances.Remove(opcItem);
        }
    }
}

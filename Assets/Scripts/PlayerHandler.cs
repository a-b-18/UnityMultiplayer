using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerHandler : MonoBehaviour
{
    [SerializeField] private GameObject userPlayerPrefab;
    [SerializeField] private GameObject opcPlayerPrefab;

    private bool userConnected = false;
    private GameObject opcInstance;
    private List<GameObject> opcInstances = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnUserPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        CheckUserConnection();
    }

    private void SpawnUserPlayer()
    {
        GameObject userInstance = Instantiate(userPlayerPrefab, userPlayerPrefab.transform.position, Quaternion.identity);
        userInstance.transform.SetParent (GameObject.FindGameObjectWithTag("Game Canvas").transform, false);
    }
    
    private void CheckUserConnection()
    {
        switch (userConnected)
        {
            case true:
                SpawnOpcPlayers();
                break;
            case false:
                DestroyOpcPlayers();
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
    
    private void SpawnOpcPlayers()
    {
        opcInstance = Instantiate(opcPlayerPrefab, new Vector3(x: Random.value * 1400 - 700, y: Random.value * 2800 - 1400, z: 0), Quaternion.identity);
        opcInstance.transform.SetParent (GameObject.FindGameObjectWithTag("Game Canvas").transform, false);
        opcInstances.Add(opcInstance);
        // if (opcInstances == null) {opcInstances = new GameObject[] {opcInstance};}
        // else {opcInstances[opcInstances.Length] = opcInstance;}
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

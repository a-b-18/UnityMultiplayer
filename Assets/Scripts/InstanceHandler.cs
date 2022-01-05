using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class InstanceHandler : MonoBehaviour
{
    [SerializeField] private LoginHandler loginHandler;
    [SerializeField] private WebHandler webHandler;
    [SerializeField] private GameObject userPlayerPrefab;
    [SerializeField] private GameObject onlinePlayerPrefab;
    [SerializeField] public string userHealthInput;
    [SerializeField] public string userScoreInput;

    // Game instances
    private PlayerInstance _userInstance = new PlayerInstance();
    private List<PlayerInstance> _onlineInstances = new List<PlayerInstance>();
    private bool writeUser = false;
    private int userSpeed = 10;

    private void Start()
    {
    }

    private void Update()
    {
        if (writeUser) {WriteToUserDto();}
    }

    private void WriteToUserDto()
    {
        // _userInstance.dto.userName = _loginHandler.GetUser();
        _userInstance.dto.posX = _userInstance.gameObject.transform.localPosition.x;
        _userInstance.dto.posY = _userInstance.gameObject.transform.localPosition.y;
        _userInstance.dto.angle = _userInstance.gameObject.transform.rotation.z;
        _userInstance.dto.userName = ChildObjectbyName(ChildObjectbyName(_userInstance.gameObject, "Canvas"), "UserName").transform.GetComponent<TextMeshProUGUI>().text;
        _userInstance.dto.health = Convert.ToInt32(ChildObjectbyName(ChildObjectbyName(_userInstance.gameObject, "Canvas"), "Health").transform.GetComponent<TextMeshProUGUI>().text);
        _userInstance.dto.score = Convert.ToInt32(ChildObjectbyName(ChildObjectbyName(_userInstance.gameObject, "Canvas"), "Score").transform.GetComponent<TextMeshProUGUI>().text);
    }

    public PlayerDto GetUserInstance()
    {
        return _userInstance.dto;
    }

    public List<PlayerDto> GetOnlineInstances()
    {
        return _onlineInstances.Select(instance => instance.dto).ToList();
    }
    
    public void SetUserInstance(PlayerDto playerDto)
    {
        // Set player dto within instance
        _userInstance.dto = playerDto;
        
        // Instantiate user object as per dto
        _userInstance.gameObject = Instantiate(userPlayerPrefab,
            new Vector3(x: playerDto.posX, y: playerDto.posY, z: 0), Quaternion.identity);
        ChildObjectbyName(ChildObjectbyName(_userInstance.gameObject, "Canvas"), "UserName").transform.GetComponent<TextMeshProUGUI>().SetText(playerDto.userName);
        ChildObjectbyName(ChildObjectbyName(_userInstance.gameObject, "Canvas"), "Health").transform.GetComponent<TextMeshProUGUI>().SetText(playerDto.health.ToString());
        ChildObjectbyName(ChildObjectbyName(_userInstance.gameObject, "Canvas"), "Score").transform.GetComponent<TextMeshProUGUI>().SetText(playerDto.score.ToString());
        writeUser = true;
    }

    public void MoveUserInstance(Vector3 direction)
    {
        direction *= userSpeed;
        var userPosition = _userInstance.gameObject.transform.localPosition;
        // var x = userPosition.x + direction.x;
        // var y = userPosition.y + direction.y;
        // userPosition.Set(x, y, 0);
        _userInstance.gameObject.transform.localPosition = userPosition + direction;
        Debug.Log(_userInstance.gameObject.transform.localPosition);
    }

    public void SetOnlineInstances(List<PlayerDto> newPlayersDto)
    {
        // Remove old instances no longer existing
        foreach (var playerInstance in _onlineInstances.Where(oldPlayer => newPlayersDto.Count(newPlayer => newPlayer.id == oldPlayer.dto.id) == 0))
        {
            RemoveOnlineInstance(playerInstance.dto);
        }
        
        // Instantiate new instances added to latest Dto
        foreach (var playerDto in newPlayersDto.Where(newPlayer => _onlineInstances.Count(oldPlayer => oldPlayer.dto.id == newPlayer.id) == 0))
        {
            AddOnlineInstance(playerDto);
        }
        
        // Refresh instances currently existing
        foreach (var playerInstance in _onlineInstances.Where(existingPlayer => newPlayersDto.Count(newPlayer => newPlayer.id == existingPlayer.dto.id) > 0))
        {
            playerInstance.dto = newPlayersDto.First(newPlayer => newPlayer.id == playerInstance.dto.id);
        }
        RefreshOnlineInstances();

    }

    private void AddOnlineInstance(PlayerDto playerDto)
    {
        var instantiate = Instantiate(onlinePlayerPrefab,
            new Vector3(x: playerDto.posX, y: playerDto.posY, z: 0),
            Quaternion.identity);
        ChildObjectbyName(ChildObjectbyName(instantiate, "Canvas"), "UserName").transform.GetComponent<TextMeshProUGUI>().SetText(playerDto.userName);
        ChildObjectbyName(ChildObjectbyName(instantiate, "Canvas"), "Health").transform.GetComponent<TextMeshProUGUI>().SetText(playerDto.health.ToString());
        ChildObjectbyName(ChildObjectbyName(instantiate, "Canvas"), "Score").transform.GetComponent<TextMeshProUGUI>().SetText(playerDto.score.ToString());
        
        _onlineInstances.Add(new PlayerInstance
        {
            gameObject = instantiate,
            dto = playerDto
        });
    }
    
    private void RemoveOnlineInstance(PlayerDto playerDto)
    {
        // Select instance with dto id
        var selectedInstance = _onlineInstances.First(instance => instance.dto.id == playerDto.id);
        
        // Destroy game object and remove instance
        Destroy(selectedInstance.gameObject);
        _onlineInstances.Remove(selectedInstance);
    }
    
    private void RefreshOnlineInstances()
    {
        foreach (var onlineInstance in _onlineInstances)
        {
            onlineInstance.gameObject.transform.localPosition.Set(onlineInstance.dto.posX,onlineInstance.dto.posY,0);
            ChildObjectbyName(ChildObjectbyName(onlineInstance.gameObject, "Canvas"), "UserName").transform.GetComponent<TextMeshProUGUI>().SetText(onlineInstance.dto.userName);
            ChildObjectbyName(ChildObjectbyName(onlineInstance.gameObject, "Canvas"), "Health").transform.GetComponent<TextMeshProUGUI>().SetText(onlineInstance.dto.health.ToString());
            ChildObjectbyName(ChildObjectbyName(onlineInstance.gameObject, "Canvas"), "Score").transform.GetComponent<TextMeshProUGUI>().SetText(onlineInstance.dto.score.ToString());
        }
    }

    private GameObject ChildObjectbyName(GameObject gameObject, string objectName)
    {
        for (int i = 0; i < gameObject.transform.childCount; ++i)
        {
            Transform currentItem = gameObject.transform.GetChild(i);
 
            //Search by name
            if (currentItem.name.Equals(objectName))
            {
                return currentItem.gameObject;
            }
        }
        // No child object with name found
        return new GameObject();
    }
    
    private class PlayerInstance
    {
        public GameObject gameObject;
        public PlayerDto dto;
    }
    
}


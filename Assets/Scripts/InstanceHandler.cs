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

    private void Start()
    {
    }

    private void Update()
    {
        WriteToUserDto();
    }

    private void WriteToUserDto()
    {
        // _userInstance.dto.userName = _loginHandler.GetUser();
        _userInstance.dto.posX = _userInstance.gameObject.transform.localPosition.x;
        _userInstance.dto.posY = _userInstance.gameObject.transform.localPosition.y;
        _userInstance.dto.angle = _userInstance.gameObject.transform.rotation.y;
        _userInstance.dto.health = Convert.ToInt32(userHealthInput);
        _userInstance.dto.score = Convert.ToInt32(userHealthInput);
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
            new Vector3(x: playerDto.posX, y: playerDto.posY, z: 0),
            Quaternion.identity);
        _userInstance.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Game Canvas").transform, false);
    }

    public void SetOnlineInstances(List<PlayerDto> playersDto)
    {
        // Instantiate online objects as per dto
        foreach (var playerDto in playersDto)
        {
            _userInstance.gameObject = Instantiate(onlinePlayerPrefab,
                new Vector3(x: playerDto.posX, y: playerDto.posY, z: 0),
                Quaternion.identity);
            _userInstance.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Game Canvas").transform, false);
        }
    }
    
    public void AddUserInstance(PlayerDto playerDto)
    {

    }

    public void AddOnlineInstance(PlayerDto playerDto)
    {
        
    }
    
    public void RemoveUserInstance()
    {
        // Destroy game object and remove instance
        Destroy(_userInstance.gameObject);
        _userInstance = new PlayerInstance();
    }

    public void RemoveOnlineInstance(PlayerDto playerDto)
    {
        // Select instance with dto id
        var selectedInstance = _onlineInstances.First(instance => instance.dto.id == playerDto.id);
        
        // Destroy game object and remove instance
        Destroy(selectedInstance.gameObject);
        _onlineInstances.Remove(selectedInstance);
    }
    
    private class PlayerInstance
    {
        public GameObject gameObject;
        public PlayerDto dto;
    }
    
}


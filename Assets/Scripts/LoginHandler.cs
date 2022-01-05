using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LoginHandler : MonoBehaviour
{
    [SerializeField] private InstanceHandler instanceHandler;
    [SerializeField] private WebHandler webHandler;
    [SerializeField] public string userIdInput;
    
    private string _userId;
    
    // Start is called before the first frame update
    private void Start()
    {
        SetUser(userIdInput);
    }
    private void Update()
    {
        SetUser(userIdInput);
    }

    public string GetUser()
    {
        return _userId;
    }

    private void SetUser(string userId)
    {
        _userId = userId;
    }
    
}

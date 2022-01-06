using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LoginHandler : MonoBehaviour
{
    private string _userId = "0";
    private string _userKey = "USERNAME";
    
    // Start is called before the first frame update
    private void Start()
    {
        _userId = PlayerPrefs.GetString(_userKey);
    }
    private void Update()
    {
    }

    public string GetUser()
    {
        return _userId;
    }

    public void SetUser(string userId)
    {
        _userId = userId;
        PlayerPrefs.SetString(_userKey, userId);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("GameSpace01");
    }
    
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}

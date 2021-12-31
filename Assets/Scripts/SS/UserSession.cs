using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserSession : MonoBehaviour
{
    public PlayerStatus UserStatus()
    {
        // Return user status to ApiHandler
        return new PlayerStatus()
        {
            id = 4.ToString(),
            userName = "alex",
            posX = transform.localPosition.x.ToString(),
            posY = transform.localPosition.y.ToString(),
            angle = transform.localRotation.y.ToString(),
            health = "100",
            score = "787"
        };
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchHandler : MonoBehaviour
{
    [SerializeField] private InstanceHandler instanceHandler;
    private Camera mainCamera;
    private bool isDragging;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            isDragging = false;
            
            return;
        }

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        Vector3 currentPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        if (!isDragging) { startPosition = currentPosition; }

        Vector3 dragVector = RoundVector((currentPosition - startPosition).normalized);
        
        if (dragVector.magnitude == 1) {instanceHandler.MoveUserInstance(dragVector);}
        
        isDragging = true;

    }

    private Vector3 RoundVector(Vector3 vector)
    {
        if (vector.x > Math.Sqrt(0.5)) {vector.x = 1;}
        else if (vector.x < -Math.Sqrt(0.5)) {vector.x = -1;}
        else  {vector.x = 0;}
        
        if (vector.y > Math.Sqrt(0.5)) {vector.y = 1;}
        else if (vector.y < -Math.Sqrt(0.5)) {vector.y = -1;}
        else {vector.y = 0;}

        return vector;
    }
}

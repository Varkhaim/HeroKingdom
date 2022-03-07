using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    public int Boundary = 50; // distance from edge scrolling starts
    public int speed = 5;
    public Camera camera;
    private float TargetSize;

    private float timer = 0f;

    private void Update()
    {
        float val = speed * Time.deltaTime;
        if (Mouse.current.position.ReadValue().x > Screen.width - Boundary)
        {
            transform.Translate(new Vector3(val, 0f, -val));
        }
        if (Mouse.current.position.ReadValue().x < 0 + Boundary)
        {
            transform.Translate(new Vector3(-val, 0f, val));
        }
        if (Mouse.current.position.ReadValue().y > Screen.height - Boundary)
        {
            transform.Translate(new Vector3(val, 0f, val));
        }
        if (Mouse.current.position.ReadValue().y < 0 + Boundary)
        {
            transform.Translate(new Vector3(-val, 0f, -val));
        }

        HandleScroll();
    }

    private void HandleScroll()
    {
        if (IsHoveringUI()) return;
        Vector2 mouseScroll = Mouse.current.scroll.ReadValue();
        if (mouseScroll.y != 0)
        {
            timer = 0f;
        }
        if (timer < 1f)
            timer += Time.deltaTime;
        TargetSize = Mathf.Clamp(TargetSize - (mouseScroll.y / 100f), 8f, 16f);
        camera.orthographicSize = Mathf.SmoothStep(camera.orthographicSize, TargetSize, timer);
    }

    private bool IsHoveringUI()
    {        
        return false;
    }

    public void MoveToUnit(Vector3 unitPosition)
    {
        transform.position = new Vector3(unitPosition.x-5f, transform.position.y, unitPosition.z-5f);
    }
}

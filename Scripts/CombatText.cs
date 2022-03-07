using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatText : MonoBehaviour
{
    private float lifetime = 3f;
    public TMPro.TMP_Text textMesh;
    public float scrollSpeed = 0.1f;

    void Update()
    {
        lifetime -= Time.deltaTime;
        textMesh.alpha = lifetime/3f;
        if (lifetime < 0)
            Destroy(gameObject);
        transform.Translate(0, Time.deltaTime*scrollSpeed, 0);

        transform.rotation = Camera.main.transform.rotation;
    }

    internal void SetText(string value, bool isAlly)
    {
        textMesh.text = value;
        if (isAlly)
            textMesh.color = Color.red;
        else
            textMesh.color = Color.green;
        //transform.rotation = Camera.main.transform.rotation;
    }

    internal void SetText(string value, Color color)
    {
        textMesh.text = value;
        textMesh.color = color;
        //transform.rotation = Camera.main.transform.rotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatBox : MonoBehaviour
{
    public Image icon;
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI valueText;

    public void SetStatBox(string statValue)
    {
        valueText.text = statValue;
    }
}

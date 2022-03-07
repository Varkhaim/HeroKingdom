using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Refs")]

    public TMPro.TextMeshProUGUI NameText;
    public TMPro.TextMeshProUGUI MessageText;
    public Image HealthBar;
    public Image ExpBar;
    public Image SwingBar;

    public List<GameObject> HoverOnlyList = new List<GameObject>();

    private void Start()
    {

        HideHoverables();
    }

    private void HideHoverables()
    {
        foreach (var h in HoverOnlyList)
        {
            h.SetActive(false);
        }
    }

    private void ShowHoverables()
    {
        foreach (var h in HoverOnlyList)
        {
            //h.SetActive(true);
        }
    }

    public void SetHealthLevel(float level)
    {
        HealthBar.fillAmount = level;
    }

    public void SetSwingTimer(float progress)
    {
        SwingBar.fillAmount = progress;
    }

    public void SetText(string nameText)
    {
        NameText.text = nameText;
    }

    public void SetMessage(string message)
    {
        MessageText.text = message;
    }

    public void UpdatePosition(Vector3 position)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
        transform.position = screenPos;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideHoverables();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowHoverables();
    }

    public void SetExpBar(float level)
    {
        ExpBar.fillAmount = level;
    }
}

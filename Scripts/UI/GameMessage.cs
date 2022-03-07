using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMessage : MonoBehaviour
{
    public TMPro.TextMeshProUGUI messageText;
    public Image icon;
    private float lifetime;
    private bool fading = false;
    private float maxLifetime;

    public void SetMessage(string message, Sprite icon = null, float lifetime = 6f, bool fading = true)
    {
        this.messageText.text = message;
        this.icon.sprite = icon;
        this.lifetime = lifetime;
        maxLifetime = lifetime;
        this.fading = fading;
    }

    private void Update()
    {
        if (fading)
        {
            float fadingInCap = maxLifetime - 1f;
            if (lifetime > fadingInCap)
            {
                messageText.alpha = 1f - (lifetime - fadingInCap);
            }
            float fadingOutCap = 1f;
            if (lifetime < fadingOutCap)
            {
                messageText.alpha = fadingOutCap;
            }
        }
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
            Destroy(gameObject);
    }
}

using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HurtIndicator : MonoBehaviour
{
    
    public float showDuration;
    public float fadeOutDuration;

    RectTransform rectTransform;
    Image image;

    [HideInInspector] public Transform damageSource;
    Transform player;
    float angle;
    Color originColor;
    float showCounter;
    float fadeCounter;
    bool hiding;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        originColor = image.color;

        // Move pivot to anchor
        // var pivotY = -(rectTransform.anchoredPosition.y - rectTransform.rect.height / 2) / rectTransform.rect.height;
        // rectTransform.pivot = new Vector2(rectTransform.pivot.x, pivotY);
    }

    void Update()
    {
        if (showCounter > 0)
        {
            showCounter -= Time.deltaTime;
        }
        else if (!hiding)
        {
            hiding = true;
            image.DOColor(new Color(originColor.r, originColor.g, originColor.b, 0f), fadeOutDuration);
            fadeCounter = fadeOutDuration;
        }
        else
        {
            fadeCounter -= Time.deltaTime;
            if (fadeCounter <= 0)
                Destroy(gameObject);
        }
        if (damageSource == null) return;

        // Rotate
        var dir = damageSource.position - player.position;
        dir = Vector3.ProjectOnPlane(dir, player.up);
        angle = Vector3.SignedAngle(player.forward, dir, player.up);
        
        rectTransform.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    public void Init(Transform player, Transform damageSource = null)
    {
        this.player = player;
        this.damageSource = damageSource;
        showCounter = showDuration;
        hiding = false;
        image.DOColor(originColor, fadeOutDuration);
    }

    IEnumerator DelayDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ValueBar : MonoBehaviour
{

    public Image effectImage;
    public Image valueImage;
    public float effectAnimSpeed;
    public float valueAnimSpeed;
    public float maxValue;
    public float value;

    Image background;
    Tweener valueTweener;
    Tweener effectTweener;

    void Start()
    {
        background = GetComponent<Image>();

        UpdateValue();
    }

    public void UpdateValue(float maxValue, float value)
    {
        this.maxValue = maxValue;
        this.value = Mathf.Clamp(value, 0, maxValue);
        Debug.Log("Value:" + this.value);
        UpdateValue();
    }

    public void UpdateValue()
    {
        var targetAmount = value / maxValue;
        var delta = targetAmount - valueImage.fillAmount;
        var deltaAbs = Mathf.Abs(delta);
        if (delta < 0)
        {
            if (effectImage.fillAmount < valueImage.fillAmount)
                effectImage.fillAmount = valueImage.fillAmount;
            var valueAnimDuration = deltaAbs / valueAnimSpeed;
            var effectAnimDuration = deltaAbs / effectAnimSpeed;

            if (valueTweener != null)
                valueTweener.Kill();
            if (effectTweener != null)
                effectTweener.Kill();
            valueTweener = valueImage.DOFillAmount(targetAmount, valueAnimDuration)
                .SetEase(Ease.OutQuint);
            effectTweener = effectImage.DOFillAmount(targetAmount, effectAnimDuration)
                .SetEase(Ease.InOutExpo);
        }
        else
        {
            var valueAnimDuration = deltaAbs / valueAnimSpeed;

            if (valueTweener != null)
                valueTweener.Kill();
            valueTweener = valueImage.DOFillAmount(targetAmount, valueAnimDuration)
                .SetEase(Ease.OutQuint);
        }
    }

}

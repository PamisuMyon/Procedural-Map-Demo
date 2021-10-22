using UnityEngine;
using DG.Tweening;

[System.Obsolete]
public class CustomSlider : MonoBehaviour
{
    public float value;
    public float maxValue;
    public float animDuration;

    SpriteRenderer spriteRenderer;
    Material material;
    Tweener valueTweener;

    private void Start() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.sharedMaterial;
    }

    public void UpdateValue()
    {
        var percent = value / maxValue;
        material.SetFloat("_Value", percent);

        var value2 = material.GetFloat("_Value2");
        if (value2 <= percent)
        {
            if (valueTweener != null && valueTweener.IsPlaying())
            {
                valueTweener.Kill();
            }
            material.SetFloat("_Value2", percent);
        }
        else
        {
            valueTweener = DOTween.To(() => value2, x => material.SetFloat("_Value2", x), percent, animDuration);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }

    public GameObject hurtInidcatorPrefab;
    public Transform hurtIndicatorPanel;
    public TMP_Text subtitle;
    public Interaction interaction;
    public HUD hud;

    Canvas canvas;
    List<HurtIndicator> hurtIndicators;

    [System.Serializable]
    public struct Interaction
    {
        public RectTransform parent;
        public RectTransform self;
        public TMP_Text buttonText;
        public TMP_Text text;
    }

    [System.Serializable]
    public struct HUD
    {
        public ValueBar hpBar;
    }

    private void Awake() 
    {
        Instance = this;
    }

    void Start()
    {
        canvas = GetComponent<Canvas>();

        hurtIndicators = new List<HurtIndicator>();
        
        subtitle.color = new Color(1, 1, 1, 0);
        Invoke("ShowSubtitle", 4.8f);
    }

    public void ShowSubtitle()
    {
        subtitle.DOColor(new Color(1, 1, 1, 1), 1f);
    }

    public void ShowHurtIndicator(Transform player, Transform damageSource = null)
    {
        // Update if there exists indicator which has the same damdage source
        foreach (var item in hurtIndicators)
        {
            if (item == null) continue;
            if (item.damageSource == damageSource)
            {
                item.Init(player, damageSource);
                return;
            }
        }
        
        // Otherwise create a new one
        var go = Instantiate(hurtInidcatorPrefab, hurtIndicatorPanel);
        var indicator = go.GetComponent<HurtIndicator>();
        indicator.Init(player, damageSource);
        hurtIndicators.Add(indicator);
    }

    public void ShowInteractionHint(Vector3 worldPosition, string text = null)
    {
        interaction.self.gameObject.SetActive(true);
        var screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(interaction.parent, screenPoint, canvas.worldCamera, out uiPosition);
        interaction.self.localPosition = uiPosition;
        if (text != null)
            interaction.text.text = text;
    }

    public void HideInteractionHint()
    {
        interaction.self.gameObject.SetActive(false);
    }

    public void UpdateHp(float maxValue, float value)
    {
        hud.hpBar.UpdateValue(maxValue, value);
    }
    
}

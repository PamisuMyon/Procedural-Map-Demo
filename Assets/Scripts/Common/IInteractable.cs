using UnityEngine;

public interface IInteractable 
{
    GameObject gameObject { get; }
    bool IsHintShowing { get; set; }

    bool IsVisible();

    void ShowHint();

    void HideHint();
}
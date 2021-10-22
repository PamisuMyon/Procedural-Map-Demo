using UnityEngine;

public class Gate : MonoBehaviour, IInteractable
{
    public bool isOpen;
    public Transform hintShowPoint;

    public bool IsHintShowing { get => isHintShowing; set => isHintShowing = value; }

    GameObject warp;
    bool warped;
    bool isVisible;
    bool isHintShowing;

    private void Start() 
    {
        warp = transform.Find("Warp").gameObject;
    }

    private void Update() 
    {
        if (isHintShowing)
        {
            var input = GameObject.FindObjectOfType<PlayerInput>();
            if (input.Interact)
            {
                HideHint();
                GameManager.Instance.NextLevel();
                Destroy(gameObject);
            }
        }    
    }

    public void Toggle(bool isOpen)
    {
        warp.gameObject.SetActive(isOpen);
    }

    void OnBecameVisible() => isVisible = true;
    void OnBecameInvisible() => isVisible = false;
    public bool IsVisible() => isVisible;

    public void ShowHint()
    {
        UIManager.Instance.ShowInteractionHint(hintShowPoint.position, "Warp");
        isHintShowing = true;
    }

    public void HideHint()
    {
        if (isHintShowing)
        {
            UIManager.Instance.HideInteractionHint();
            isHintShowing = false;
        }
    }
}
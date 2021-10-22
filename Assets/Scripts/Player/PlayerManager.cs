using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour 
{
    
    public static PlayerManager Instance { get; private set; }

    // public bool IsMovingEnabled { get; private set; }
    public bool IsMovingEnabled;

    WarpScreenEffect warpEffect;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() 
    {
        warpEffect = GetComponentInChildren<WarpScreenEffect>();
        IsMovingEnabled = true;
    }

    public void CameraLoadingFadeIn(System.Action onComplete)
    {
        IsMovingEnabled = false;
        warpEffect.enabled = true;
        var duration = warpEffect.duration;
        DOTween.To(() => warpEffect.twirlAngle, x => warpEffect.twirlAngle = x, warpEffect.maxTwirlAngle, duration);
        var tweener = DOTween.To(() => warpEffect.progress, x => warpEffect.progress = x, 1, duration);
        tweener.OnComplete(() => 
        {
            onComplete();
            warpEffect.enabled = false;
        });
    }

    public void CameraLoadingFadeOut()
    {
        warpEffect.enabled = true;
        var duration = warpEffect.duration;
        DOTween.To(() => warpEffect.twirlAngle, x => warpEffect.twirlAngle = x, 0, duration);
        var tweener = DOTween.To(() => warpEffect.progress, x => warpEffect.progress = x, 0, duration);
        tweener.OnComplete(() => 
        {
            IsMovingEnabled = true;
            warpEffect.enabled = false;
        });
    }
}
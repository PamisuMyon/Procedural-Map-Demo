using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class WarpScreenEffect : ScreenEffect
{
    public float duration;
    public float maxTwirlAngle;
    [Range(0f, 1f)]
    public float progress;
    
    [HideInInspector] public float twirlAngle;

    private void OnRenderImage(RenderTexture src, RenderTexture dest) 
    {
        Material.SetFloat("_Twirl", twirlAngle);
        Material.SetFloat("_Fade", progress);
        Graphics.Blit(src, dest, Material);
    }
}

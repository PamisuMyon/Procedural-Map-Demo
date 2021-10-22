using UnityEngine;

public abstract class ScreenEffect : MonoBehaviour
{
    public Shader shader;

    [SerializeField]
    protected Material material;
    protected Material Material 
    {
        get 
        {
            if (material == null)
            {
                material = new Material(shader);
                material.hideFlags = HideFlags.DontSave;
            }
            return material;
        }
    }

    protected virtual void OnDisable() 
    {
        if (material)
        {
            DestroyImmediate(material);
        }
    }

}

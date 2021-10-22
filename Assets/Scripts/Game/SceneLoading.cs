using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{

    public Slider progressSlider;

    void Start()
    {
        Debug.Log("加载中场景");
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        // 需要注意编辑器中依然是同步加载，无法看到效果
        Debug.Log("加载Level...");
        var operation = SceneManager.LoadSceneAsync("Level");
        operation.allowSceneActivation = false;
        while(!operation.isDone)
        {
            Debug.Log("加载进度: " + operation.progress);
            if (operation.progress < .9f)
                progressSlider.value = operation.progress;
            else
                progressSlider.value = 1f;

            if (operation.progress >= .9f)
            {
                // 设为true之后将立即跳转至下一场景，yield之后的代码将不会执行
                operation.allowSceneActivation = true;  
                Debug.Log("加载完成");
                PlayerManager.Instance.CameraLoadingFadeOut();
            }
            
            yield return null;
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(MakeTheLoad(LevelLoader.NextLevel));
    }

    private static IEnumerator MakeTheLoad(int nextLevel)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(nextLevel);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}

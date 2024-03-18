using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Carga asincrona de la escena a la que se quiere ir.
/// </summary>
public class Loader : MonoBehaviour
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
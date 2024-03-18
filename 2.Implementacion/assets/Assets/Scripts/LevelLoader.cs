using UnityEngine.SceneManagement;

/// <summary>
/// Clase que se encarga de guardar el nivel al que se quiere ir y carga la escena de Loading....
/// </summary>
public static class LevelLoader
{
    public static int NextLevel;
    
    public static void LoadLevel(int level)
    {
        NextLevel = level;
        SceneManager.LoadScene(2);
    }
}

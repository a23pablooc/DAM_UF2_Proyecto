using UnityEngine.SceneManagement;

public static class LevelLoader
{
    public static int NextLevel;
    
    public static void LoadLevel(int level)
    {
        NextLevel = level;
        SceneManager.LoadScene(2);
    }
}

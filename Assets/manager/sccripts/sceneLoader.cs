using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadFirstScene()
    {
        SceneManager.LoadSceneAsync(1); // Loads scene at build index 1
    }
}
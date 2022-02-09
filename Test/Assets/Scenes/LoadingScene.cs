using System.Collections;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitForSceneToEnd());
    }

    IEnumerator WaitForSceneToEnd()
    {
        yield return new WaitForSeconds(3);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

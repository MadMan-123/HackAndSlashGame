using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] string sceneName;

    [SerializeField] Button button;

    void Start()
    {
        button.onClick.AddListener(LoadMap);
    }

    void LoadMap()
    {
        SceneManager.LoadScene(sceneName);
    }


}

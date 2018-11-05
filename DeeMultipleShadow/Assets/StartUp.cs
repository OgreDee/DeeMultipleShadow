using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartUp : MonoBehaviour {
    [SerializeField]
    Camera uiCamera;
    [SerializeField]
    GameObject loadingView;
    [SerializeField]
    GameObject uiView;
    [SerializeField]
    GameObject sceneBtn;
    [SerializeField]
    GameObject backBtn;

    const string s_StartUpScene = "StartUp";

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);
        OnLoadingCompleted(s_StartUpScene);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenPlanarScene()
    {
        LoadScene("Dee_ShadowPlaner");
    }

    public void OpenShadowMapScene()
    {
        LoadScene("Dee_ShadowMap");
    }

    public void Back()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if(SceneManager.GetSceneAt(i).name != s_StartUpScene)
            {
                var itor = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).name);
                itor.completed += (AsyncOperation obj) => {
                    OnLoadingCompleted(s_StartUpScene);
                    uiCamera.clearFlags = CameraClearFlags.SolidColor;
                };

                break;
            }
        }

    }

    void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            loadingView.SetActive(true);
            uiView.SetActive(false);

            var itor = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if(itor == null)
            {
                OnLoadingCompleted(string.Empty);
                return;
            }

            itor.completed += (AsyncOperation obj) => {
                OnLoadingCompleted(sceneName);
                uiCamera.clearFlags = CameraClearFlags.Nothing;
            };
        }
    }

    void OnLoadingCompleted(string sceneName)
    {
        loadingView.SetActive(false);
        uiView.SetActive(true);

        sceneBtn.SetActive(sceneName == s_StartUpScene);
        backBtn.SetActive(sceneName != s_StartUpScene);
    }
}

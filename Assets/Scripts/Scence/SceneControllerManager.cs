using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName startingSceneName;


    //this is the main external point of contact and influence from the rest of the project.
    //this will be called when the player wants to switch scenes.
    public void FadeAndLoadScene(String sceneName,Vector3 spawnPosition)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }



    //this is the coroutine where the 'building block 'of the script are put together
    private IEnumerator FadeAndSwitchScenes(string sceneName,Vector3 spawnPosition)//spawnPosition出生位置
    {
        //Call before scene unload fade out event
        EventHander.CallBeforeSceneUnloadFadeOutEvent();

        //Start fading to black and wait for it to finish before continuing
        yield return StartCoroutine(Fade(1f));

        //store scenedata
        SaveLoadManager.Instance.StoreCurrentSceneData();

        //Set player position
        Player.Instance.gameObject.transform.position = spawnPosition;

        //Call before scene unload event
        EventHander.CallBeforeSceneUnloadEvent();

        //Unload the current active scene
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //Start loading the given scene and wait for it to finish 
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        //call after scene load event
        EventHander.CallAfterSceneLoadEvent();

        //restore scenedata
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        yield return StartCoroutine(Fade(0f));
         
       

        EventHander.CallAfterSceneLoadFadeInEvent();


    }

    private IEnumerator LoadSceneAndSetActive(String sceneName)
    {
        //允许给定场景加载多个帧并将其添加到已加载的场景中
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive); 

        //查找最近加载的场景
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        //将新加载的场景设置为活动场景  
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Fade(float finalAlpha)
    {
        //Set the fading to true to the fadeAndSwitchScenes coroutine won't be called again
        isFading = true;

        //make sure the canvasgroup blocks raycasts into the scene so no more input can be accepted
        faderCanvasGroup.blocksRaycasts = true;

        //根据画布组的当前 alpha 、最终 alpha 以及两者之间相差以及有多少时间来变换，来计算画布组的淡入淡出速度。
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        while (!Mathf.Approximately(faderCanvasGroup.alpha, finalAlpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, finalAlpha,fadeSpeed*Time.deltaTime);

            yield return null;//等待一帧再执行下句代码
        }

        //Set a flag to false since the fade has fanished
        isFading = false;

        //Stop the canvasGroup from bloking raycasts to input is no longer ignored
        faderCanvasGroup.blocksRaycasts = false;
    }


    public IEnumerator Start()
    {
        faderImage.color = new Color(0f, 0f, 0f,1f);
        faderCanvasGroup.alpha = 1f;

        //set the first scence loading and wait for it to finish
        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

        //if this event has any suscriber,call it 
        EventHander.CallAfterSceneLoadEvent();

        SaveLoadManager.Instance.RestoreCurrentSceneData();

        //Once the scene is finished loading,start fading in.
        StartCoroutine(Fade(0f)); 
    }


}

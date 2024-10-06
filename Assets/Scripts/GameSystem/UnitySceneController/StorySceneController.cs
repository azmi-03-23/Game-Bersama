using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StorySceneController : MonoBehaviour
{
    [SerializeField]
    AudioSource backsoundAudio, narrationAudio;
    [SerializeField]
    UtilClass myUtilClass;

    [SerializeField]
    List<Image> myBackgroundImage = new List<Image>();
    int activeBGIndex = 1;
    [SerializeField]
    Animator myNextButtonAnimator;
    public static int currentStorySceneIndex = VNManager.STORY_SCENE_FIRST_INDEX;
    private StoryScene currentScene; 
    private VNManager myVNManager; 
    private const string HIGHLIGHT_BUTTON = "Highlight";

    void Awake()
    {
        Debug.Log("Story UnityScene is Loaded");
        myVNManager = VNManager.getInstance();

        narrationAudio.volume = 0.9f;

        setStorySceneFirstTime();
    }

    private void setStorySceneFirstTime(){
        currentStorySceneIndex = myVNManager.getStartStorySceneIndex();
        if(currentStorySceneIndex>VNManager.STORY_SCENE_FIRST_INDEX){
            currentScene = (StoryScene)Resources.Load("StoryScenes/"+myVNManager.getVNCurrentSequence()+"/"+currentStorySceneIndex);
        } else{
            if(currentStorySceneIndex==myVNManager.getLastSceneIndexInStorySequence()){
                currentScene = myVNManager.getLastStoryScene();
            } else {
                currentScene = myVNManager.getFirstStoryScene();
            }
        }
    }

    private void setBacksound()
    {
        float fadeTime = 0.5f;
        backsoundAudio.clip = myVNManager.getBacksound();
        if(backsoundAudio.clip!=null){
            StartCoroutine(FadeInAudio(backsoundAudio, fadeTime));
        } else {
            Debug.Log("backsound is non existent");
        }
    }

    void Start()
    {
        setDisplay();
    }

    private enum FadingState{
        FADE, IDLE
    }

    FadingState fadingOutAudioState = FadingState.IDLE;

    private void showStorySequenceInARow(){
        setStorySceneFirstTime();
        setDisplay();
    }

    private void nextStoryScene()
    {
        currentScene = currentScene.nextScene;
        playingShowScene = showScene();
        StartCoroutine(playingShowScene);
    }

    IEnumerator FadeOutAudio(AudioSource audioSource, float FadeTime) {
        fadingOutAudioState = FadingState.FADE;
        startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }
        audioSource.Stop ();
        audioSource.volume = startVolume;
        fadingOutAudioState = FadingState.IDLE;
    }

    float startVolume = 0.25f;
    IEnumerator FadeInAudio(AudioSource audioSource, float FadeTime) {
        while(fadingOutAudioState==FadingState.FADE){
            yield return null;
        }
        
        audioSource.volume = 0f;
        audioSource.Play();
        while (audioSource.volume < startVolume) {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
        audioSource.volume = startVolume;
    }
    
    [SerializeField]
    GameObject myPrevButtonGO;

    private void setDisplay()
    {
        if(myVNManager.getVNCurrentSequence()==VNManager.VN_FIRST_INDEX && myVNManager.getStartStorySceneIndex()==VNManager.STORY_SCENE_FIRST_INDEX){
            myPrevButtonGO.SetActive(false);
        } else {
            myPrevButtonGO.SetActive(true);
        }
        setBacksound();
        playingShowScene = showScene();
        StartCoroutine(playingShowScene);
    }

    [SerializeField]
    Animator bgList;
    const string FADE_IN_FIRST = "First",
                FADE_IN_SECOND = "Second",
                FADE_FIRST_ANIMATION = "FADE_IN_FIRSTanim",
                FADE_SECOND_ANIMATION = "FADE_IN_SECOND";

    IEnumerator showScene()
    {
        fadeInBackground();
        string myAnimCurrentClip, myAnimPreviousClip = "";
        switch(activeBGIndex){
            case 0:
                myAnimPreviousClip = "SECOND";
                break;
            case 1:
                myAnimPreviousClip = "FIRST";
                break;
            default:
                Debug.Log("Tidak mungkin");
                break;
        }
        do{
            myAnimCurrentClip = bgList.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            Debug.Log("Waiting in " + myAnimCurrentClip);
            yield return null;
        }while(myAnimCurrentClip.Equals(myAnimPreviousClip) || myAnimCurrentClip.Equals(FADE_FIRST_ANIMATION) || myAnimCurrentClip.Equals(FADE_SECOND_ANIMATION));
        yield return new WaitForEndOfFrame();
        if(currentScene.narration!=null){
            narrationAudio.clip = currentScene.narration;
            StartCoroutine(myUtilClass.playOnlyOneAudio(narrationAudio));
        } else {
            Debug.Log("narration doesnt exist currently");
        }
        while (myUtilClass.getAudioState() == UtilClass.State.PLAYING)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        myNextButtonAnimator.SetBool(HIGHLIGHT_BUTTON, true);
    }

    private void fadeInBackground()
    {
        if(currentStorySceneIndex==VNManager.STORY_SCENE_FIRST_INDEX){
            if(activeBGIndex == 0){
                activeBGIndex = 1;
            } else {
                activeBGIndex = 0;
            }
            Debug.Log("this is activebgindex = " + activeBGIndex);
            myBackgroundImage[activeBGIndex].sprite = currentScene.background;
            switch (activeBGIndex)
            {
                case 0:
                    bgList.SetTrigger(FADE_IN_FIRST);
                    break;
                case 1:
                    bgList.SetTrigger(FADE_IN_SECOND);
                    break;
                default:
                    Debug.Log("StorySceneCtrl: Error fading in image");
                    break;
            }
        } else {
            myBackgroundImage[activeBGIndex].sprite = currentScene.background;
        }
    }

    void OnEnable(){
        myVNManager.showStorySequenceAgain += showStorySequenceInARow;
    }

    void stopShowingScene(){
        StopCoroutine(playingShowScene);
        if(currentScene.narration!=null)
            myUtilClass.stopAudio();
    }

    public void goPrev(){
        stopShowingScene();
        myNextButtonAnimator.SetBool(HIGHLIGHT_BUTTON, false);
        currentStorySceneIndex--; 
        if(currentScene.prevScene==null){
            StartCoroutine(FadeOutAudio(backsoundAudio, 0.5f));
            myVNManager.goToPrevSequence();
        } else {
            prevStoryScene();
        }
    }

    private void prevStoryScene()
    {
        currentScene = currentScene.prevScene;
        playingShowScene = showScene();
        StartCoroutine(playingShowScene);
    }

    public void goNext(){
        stopShowingScene();
        myNextButtonAnimator.SetBool(HIGHLIGHT_BUTTON, false);
        currentStorySceneIndex++;
        if(currentScene.nextScene==null){
            StartCoroutine(FadeOutAudio(backsoundAudio, 0.5f));
            myVNManager.goToNextSequence();
        } else {
            nextStoryScene();
        }
    }

    private IEnumerator playingShowScene;
    
    public void RestartAudio(){
        myUtilClass.stopAudio();
        StartCoroutine(myUtilClass.playOnlyOneAudio(narrationAudio));
    }
    
    void OnDisable(){
        myVNManager.showStorySequenceAgain -= showStorySequenceInARow;
        StopAllCoroutines();
    }
}



using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VNManager : MonoBehaviour
{

    public static readonly int VN_FIRST_INDEX = 1,
                                STORY_SCENE_FIRST_INDEX = 1;

    private static VNManager instance = null;
    void Awake(){
        Debug.Log("VNManager.Awake");
        if(instance==null){
            instance = this;
        } else {
            Destroy(this);
        }
        mySceneManager = MySceneManager.getInstance();
        OpenVisualNovelScene();
    }

    public static VNManager getInstance(){
        return instance;
    }

    void Start(){
        DontDestroyOnLoad(this);
    }


    const string RESOURCEPATH  = "VNSequences/";
    MySceneManager mySceneManager;
    public void OpenVisualNovelScene(){
        Debug.Log("VNM.OpenVisualNovelScene");
        PlayerDataManager.ProgressVN progressVN = PlayerDataManager.LoadNGetVNProgress();
        showFromNthVNScene(progressVN.VNSequenceIndex, progressVN.StorySceneIndex);
    }

    int currentVNSequence, startSubSequence = STORY_SCENE_FIRST_INDEX;
    string currentSceneResourcePath;
    public void showFromNthVNScene(int sequenceInd, [Optional] int subsequence){
        Debug.Log("VNM.showFromNthVNScene");
        currentVNSequence = sequenceInd;
        startSubSequence = subsequence;
        currentSceneResourcePath =  RESOURCEPATH + currentVNSequence.ToString();
        whichVNScene();
    }
    
    public int getVNCurrentSequence(){
        return currentVNSequence; 
    }

    public delegate void ShowStorySequenceInARow();
    public event ShowStorySequenceInARow showStorySequenceAgain; 
    Object VNSceneAssets = null;
    private void whichVNScene(bool isPrevious = false){
        Debug.Log("VNM.preloadVNScene");
        VNSceneAssets = Resources.Load(currentSceneResourcePath);
        const string STORY = "Story";
        if((VNScene)VNSceneAssets is StorySequence){
            if(isPrevious)
                startSubSequence = ((StorySequence)VNSceneAssets).sceneCount-1;
            if(SceneManager.GetActiveScene().name.Equals(STORY)){
                showStorySequenceAgain?.Invoke();
            } else{
                mySceneManager.loadScene(STORY);
            }
        } else {
            if((VNScene)VNSceneAssets is GamesPerLevel){
                mySceneManager.loadScene("GameSublevel");
            }
        }
    }

    public StoryScene getFirstStoryScene(){
        return ((StorySequence)VNSceneAssets).firstStorySceneInSequence;
    }
    public StoryScene getLastStoryScene(){
        return ((StorySequence)VNSceneAssets).lastStorySceneInSequence;
    }

    public int getStartStorySceneIndex(){
        return startSubSequence;
    }

    public GamesPerLevel getGames(){
        GamesPerLevel gPL = (GamesPerLevel)VNSceneAssets;
        mySceneManager.setCurrentGameLevel(gPL.level);
        return gPL;
    }

    public void goToNextSequence(){
        Debug.Log("VNManager.goToNextSequence");
        int nextSceneInd = ((VNScene)VNSceneAssets).nextSceneInd;
        if(nextSceneInd!=0){
            currentVNSequence = nextSceneInd;
            startSubSequence = STORY_SCENE_FIRST_INDEX;
            currentSceneResourcePath = RESOURCEPATH + nextSceneInd.ToString();
            whichVNScene();
        } else
        {
            VNSceneAssets = null;
            resetVNProgress();
            mySceneManager.backToParentScene();
        }
    }

    bool isReset = false;

    private void resetVNProgress()
    {
        isReset = true;
        currentVNSequence = VN_FIRST_INDEX;
        startSubSequence = STORY_SCENE_FIRST_INDEX;
        saveVNProgress("Story");
    }

    public void saveVNProgress(string sceneType){
        PlayerDataManager.saveLatestVNSequenceIndex(currentVNSequence);
        if(sceneType.Equals("Story")){
            if(isReset){
                PlayerDataManager.saveLatestStorySceneIndex(startSubSequence);
                isReset = false;
            } else {
                PlayerDataManager.saveLatestStorySceneIndex(StorySceneController.currentStorySceneIndex);
            }
        } else {
            PlayerDataManager.saveLatestStorySceneIndex(STORY_SCENE_FIRST_INDEX);
        }
        PlayerDataManager.SaveVNProgress();
    }

    public AudioClip getBacksound()
    {
        return ((VNScene)VNSceneAssets).backsound;
    }

    public int getLastSceneIndexInStorySequence()
    {
        return ((StorySequence)VNSceneAssets).sceneCount-1;
    }

    public void goToPrevSequence()
    {
        Debug.Log("VNManager.goToNextSequence");
        int prevSceneInd = ((VNScene)VNSceneAssets).prevSceneInd;
        if(prevSceneInd!=0){
            currentVNSequence = prevSceneInd;
            currentSceneResourcePath = RESOURCEPATH + prevSceneInd.ToString();
            whichVNScene(true);
        }
    }
}

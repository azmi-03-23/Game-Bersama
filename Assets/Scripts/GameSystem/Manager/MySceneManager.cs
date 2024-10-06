using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySceneManager : MonoBehaviour
{

    public delegate void loadingScreenOut();
    public event loadingScreenOut screenShownEvent;
    private static MySceneManager instance = null;

    void Awake(){
        loadingScreen.SetActive(false);
        myDialogObject.SetActive(false);

        if(instance==null){
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(myDialogObject);
            DontDestroyOnLoad(loadingScreen);
        } else {
            Destroy(gameObject);
        }
    }

    public static MySceneManager getInstance(){
        if(instance==null)
            Debug.Log("MySceneManager not instantiated");
        return instance;
    } 
    
    bool isGameOrReward;
    int currentGameLevel, currentGameSublevel;
    
    public void setCurrentGameLevel(int levelGame)
    {
        currentGameLevel = levelGame;
    }
    
    public (int, int) getCurrentGameLevelNSublevel(){
        return (currentGameSublevel, currentGameLevel);
    }

    public void loadScene(string tipeScene, int sublevelGame){
        Debug.Log("MSM.loadGame");
        isGameOrReward = true;
        currentGameSublevel = sublevelGame;
        loadScene(tipeScene);
    }
    
    [SerializeField]
    List<Hierarchy> myUnityScenes;
    [System.Serializable]
    public struct Hierarchy{
        public string name;
        public int level;
    }
    string currentSceneName = "MainMenu", parentSceneName = null;
    int parentSceneLevel = -1;

    public void loadScene(string sceneName){
        Debug.Log("MSM.loadScene");
        setParentScene(sceneName);
        StartCoroutine(StartLoad(sceneName));
        currentSceneName = sceneName;
    }

    private void setParentScene(string sceneName){
        if(currentSceneName==null)
            currentSceneName = SceneManager.GetActiveScene().name;
        if(sceneName.Equals("MainMenu")){
            parentSceneLevel = -1;
            parentSceneName = null;
        } else {
            if(sceneName.Equals(parentSceneName)){
                parentSceneLevel = 0;
                parentSceneName = "MainMenu";
            } else {
                int cSLevel=-1, nSLevel=-1;
                for(int i=0;i<myUnityScenes.Count;i++){
                    if(currentSceneName.Equals(myUnityScenes[i].name)){
                        cSLevel = myUnityScenes[i].level;
                    }
                    if(sceneName.Equals(myUnityScenes[i].name)){
                        nSLevel = myUnityScenes[i].level;
                    }
                }
                if(nSLevel!=cSLevel){
                    parentSceneLevel = cSLevel;
                    parentSceneName = currentSceneName;
                }
            }
        }
    }

    [SerializeField]
    GameObject loadingScreen;
    [SerializeField]
    CanvasGroup canvasGroup;
    public AsyncOperation operation;
    [SerializeField]
    ProgressLoader myProgressLoader;
    IEnumerator StartLoad(string sceneName)
    {
        Debug.Log("MSM.StartLoad (ie)");
        loadingScreen.SetActive(true);
        yield return StartCoroutine(FadeLoadingScreen(1, 1));

        operation = SceneManager.LoadSceneAsync(sceneName);
        myProgressLoader.setOperation(operation);
        while (!operation.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeLoadingScreen(0, 1));
        loadingScreen.SetActive(false);
        screenShownEvent?.Invoke();
    }

    IEnumerator FadeLoadingScreen(float targetValue, float duration)
    {
        Debug.Log("MSM.FadeLoadingScreen (ie)");
        float startValue = canvasGroup.alpha;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startValue, targetValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetValue;
    }

    private VNManager myVNManager = null;
    public void CreateVNManager(){
        Debug.Log("MSM.CreateVNManager");
        if(myVNManager==null){
	        myVNManager = gameObject.AddComponent<VNManager>();
        } else {
            enableVNManager();
            myVNManager.OpenVisualNovelScene();
        }
    }

    private void enableVNManager()
    {
        if(!myVNManager.isActiveAndEnabled){
            myVNManager.enabled = true;
        }
    }

    [SerializeField]
    GameObject myDialogObject;
    [SerializeField]
    Button backButton;

    void OnEnable(){
        backButton.onClick.AddListener(activateDialog);
    }
    void OnDisable(){
        backButton.onClick.RemoveListener(activateDialog);
    }


    [SerializeField]
    Button noButton, yesButton;

    private void activateDialog(){
        if(parentSceneLevel==0 && !currentSceneName.Equals("Story") && !currentSceneName.Equals("GameSublevel")){
            backToParentScene();
        } else {
            if(parentSceneLevel==1 && !isGameOrReward){
                backToParentScene();
            } else{
                myDialogObject.SetActive(true);
                addDialogListeners();
            }
        }
    }

    private void addDialogListeners(){
        noButton.onClick.AddListener(closeDialogWindow);
        yesButton.onClick.AddListener(yesAction);
    }

    private void closeDialogWindow(){
        removeDialogListeners();
        myDialogObject.SetActive(false);
    }

    private void removeDialogListeners(){
        noButton.onClick.RemoveListener(closeDialogWindow);
        yesButton.onClick.RemoveListener(yesAction);
    }

    private void yesAction(){
        //setelah tes
        closeDialogWindow();
        if(currentSceneName.Equals("MainMenu")){
            quitApp();
        } else {
            if(!isGameOrReward){
                myVNManager.saveVNProgress(currentSceneName);
                backToParentScene();
            } else {
                bool isRewardTemp;
                if(currentSceneName.Equals("Reward")){
                    isRewardTemp = true;
                } else {
                    isRewardTemp = false;
                }
                backToParentScene(isRewardTemp);
            }
        }
    }

    private void quitApp(){
        // save everything
        Application.Quit();
    }

    public void backToParentScene()
    {
        resetGameOrReward();
        loadScene(parentSceneName);
    }

    bool gameFromReward = false;
    public void backToParentScene(bool isRewardList)
    {
        resetGameOrReward();
        if(parentSceneName.Equals("AllGameList")){
            if(gameFromReward){
                isRewardList = true;
                setGameFromReward(false);
            }
            ListController.setLevel(currentGameLevel);
            CreateGameOrRewardManager(isRewardList);
        } else {
            loadScene(parentSceneName);
        }

    }

    private void resetGameOrReward()
    {
        if (isGameOrReward)
        {
            isGameOrReward = false;
            currentGameSublevel = 0;
        }
    }

    public string getParentSceneName()
    {
        return parentSceneName;
    }

    public void CreateGameOrRewardManager(bool isRewardList)
    {
        loadScene("AllGameList");
        
        ListController.setStrategy(isRewardList);
    }

    public void setGameFromReward(bool value)
    {
        gameFromReward = value;
    }

    public void disableVNManager()
    {
        if(myVNManager!=null){
            myVNManager.enabled = false;
        } else {
            Debug.Log("Opsi cerita hasnt been chosen in this session");
        }
    }
}

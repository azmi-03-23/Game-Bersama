using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameplayController: GameDetailController
{
    protected string highlightWrong = "wrong", highlightCorrect = "correct", highlightDefault = "explain";
    protected UtilClass myUtilityClass;

    protected override void OnEnable(){
        myUtilityClass = gameObject.AddComponent<UtilClass>();
        base.OnEnable();
    }
    
    protected override void screenShown()
    {
        base.screenShown();
        StartCoroutine(startDisplay());
    }

    protected enum State{
        PLAYING, IDLE
    }
    protected IEnumerator playingTutorial;
    protected State tutorialState = State.IDLE;
    IEnumerator startDisplay()
    {
        playingTutorial = showTutorial();
        StartCoroutine(playingTutorial);
        while(tutorialState==State.PLAYING){
            yield return null;
        }
        startGame();
    }

    protected virtual IEnumerator showTutorial(){
        tutorialState = State.PLAYING;
        Debug.Log("Showing tutorial");
        yield return new WaitForEndOfFrame();
        tutorialState = State.IDLE;
    }
    protected virtual void startGame(){
        Debug.Log("Game is starting");
    }

    

    protected bool restart = false;

    protected virtual void restartGame()
    {
        StopAllCoroutines();
        myUtilityClass.setAllStateIDLE();
        restart = true;
        restartReward();
        score = 0;
        wrongAttempt = 0;
        startGame();
    }

    protected Vector2 rewardListTempOrgSize = Vector2.zero;
    [SerializeField]
    protected Transform aRFTransform;

    private void restartReward()
    {
        endingRewardShowGO.SetActive(false);

        rewardListTemp.transform.SetParent(aRFTransform, false);
        RectTransform rewardListRT = rewardListTemp.GetComponent<RectTransform>();
        rewardListRT.pivot = new Vector2(0f, 0.5f);
        rewardListRT.anchoredPosition = rewardListAncPos;
        rewardListTemp.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        if(rewardListTempOrgSize!=Vector2.zero){
            rewardListRT.sizeDelta = rewardListTempOrgSize;
        }
        
        foreach (GameObject reward in acquiredReward)
        {
            Destroy(reward);
        }
        acquiredReward.Clear();
    }
    [SerializeField]
    protected GameObject ceklisPrefab;

    [SerializeField]
    protected GameObject rewardPrefab;
    protected List<GameObject> acquiredReward = new List<GameObject>();
    protected virtual void showReward(){}

    protected int score = 0, wrongAttempt = 0;
    [SerializeField]
    protected GameObject rewardListTemp;

    protected void wrongAttemptMade(){
        wrongAttempt++;
    }

    protected void SetAtRewardList(GameObject gO, GameObject parent){
        Vector2 parentSize = parent.GetComponent<RectTransform>().sizeDelta;
        gO.GetComponent<RectTransform>().sizeDelta = new Vector2(parentSize.x/8, parentSize.y-10);
        gO.transform.SetParent(parent.transform, false);
    }

    protected virtual void hideTemporary(){}

    protected void saveGameNShowEndingReward(){        
        PlayerDataManager.SaveGameAchievement(myGameDetail.sublevel, myGameDetail.level, score, wrongAttempt);
        
        StartCoroutine(waitAudioEndsNShowEndingReward());
    }
    Object endingRewardShowPrefab = null;
    GameObject endingRewardShowGO;

    Vector2 rewardListAncPos;
    [SerializeField]
    Animator correctAnim, wrongAnim;
    const string SHOW = "Show";
    protected IEnumerator showCorrectHint(){
        correctAnim.SetBool(SHOW, true);
        yield return new WaitForSeconds(2f);
        correctAnim.SetBool(SHOW, false);
    }
    protected IEnumerator showWrongHint(){
        wrongAnim.SetBool(SHOW, true);
        yield return new WaitForSeconds(2f);
        wrongAnim.SetBool(SHOW, false);
    }
    IEnumerator waitAudioEndsNShowEndingReward()
    {
        hideTemporary();
        while(myUtilityClass.getAudioState()==UtilClass.State.PLAYING){
            yield return null;
        }
        if(restart is false){
            endingRewardShowPrefab = Resources.Load("Screens/EndingRewardShow");
            endingRewardShowGO = Instantiate((GameObject)endingRewardShowPrefab);
            endingRewardShowGO.transform.SetParent(aRFTransform, false);
            
            eRSNextButton = endingRewardShowGO.transform.Find("NextButton").GetComponent<Button>();
            eRSNextButton.onClick.AddListener(delegate{mySceneManager.backToParentScene(isRewardList: false);});
            restartButton = endingRewardShowGO.transform.Find("RestartButton").GetComponent<Button>();
            restartButton.onClick.AddListener(restartGame);
        } else {
            endingRewardShowGO.SetActive(true);
        }
        RectTransform rewardListRT = rewardListTemp.GetComponent<RectTransform>();
        rewardListAncPos = rewardListRT.anchoredPosition;
        rewardListRT.pivot = Vector2.one * 0.5f;
        
        rewardListTemp.transform.SetParent(endingRewardShowGO.transform, false);
        rewardListTemp.transform.position = endingRewardShowGO.transform.Find("Reward").position;
        
        ReflectionController myReflCtrl = endingRewardShowGO.GetComponent<ReflectionController>();
        StartCoroutine(myReflCtrl.setActivity(myGameDetail.learningActivity));
        StartCoroutine(myReflCtrl.setStars(score, wrongAttempt));
    }
    Button eRSNextButton = null, restartButton = null;
    protected override void OnDisable(){
        if(eRSNextButton!=null)
            eRSNextButton.onClick.RemoveAllListeners();
        if(restartButton!=null)
            restartButton.onClick.RemoveAllListeners();
        base.OnDisable();
    }

}

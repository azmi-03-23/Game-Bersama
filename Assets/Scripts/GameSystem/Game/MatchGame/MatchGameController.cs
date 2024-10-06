using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchGameController : GameplayController
{

    protected int currentQAIndex = 0;
    protected override void restartGame()
    {
        choicesBack();
        currentQAIndex = 0;
        base.restartGame();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        DropSukuKata.correctAnswerEvent -= correctAnswer;
        DropSukuKata.wrongAnswerEvent -= showFeedback;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        DropSukuKata.correctAnswerEvent += correctAnswer;
        DropSukuKata.wrongAnswerEvent += showFeedback;
    }

    protected virtual void showFeedback(GameObject gO)
    {
        StartCoroutine(showWrongHint());
    }

    protected virtual void correctAnswer(GameObject gO)
    {
        StartCoroutine(showCorrectHint());
    }

    protected virtual void choicesBack(){}

    [SerializeField]
    protected Image questionImage;
    
    [SerializeField]
    protected AudioSource salahAud;
    protected override void showReward(){
        acquiredReward.Add(Instantiate(rewardPrefab));
        acquiredReward[score-1].GetComponent<Image>().sprite = questionImage.sprite;
        SetAtRewardList(acquiredReward[score-1], rewardListTemp);
        if(score==8){
            rewardListTempOrgSize = rewardListTemp.GetComponent<RectTransform>().sizeDelta;
            rewardListTemp.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.MinSize;
        }
    }

    [SerializeField]
    GameObject tutorialGO;
    [SerializeField]
    Animator tutorialAnim, closeTutorialAnim;
    protected override IEnumerator showTutorial()
    {
        tutorialState = State.PLAYING;
        tutorialAnim.SetTrigger("Show");
        yield return new WaitForSeconds(2f);
        closeTutorialAnim.SetBool("Highlight", true);
        yield return new WaitForSecondsRealtime(3f);
        closeTutorial();
    }

    public void closeTutorial()
    {
        StopCoroutine(playingTutorial);
        if(tutorialGO!=null)
            Destroy(tutorialGO);
        tutorialState = State.IDLE;
    }

}

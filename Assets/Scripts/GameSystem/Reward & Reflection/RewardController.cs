using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class RewardController : GameDetailController
{
    [SerializeField]
    AudioSource backsoundAudio;
    [SerializeField]
    List<AudioClip> audPerGameLevel;
    
    [SerializeField]
    ReflectionController myReflectionCtrl;
    int score, wrongAttempt;

    protected override void screenShown()
    {
        base.screenShown();
        setBacksound();
        StartCoroutine(showReflection());
    }

    private void setBacksound()
    {
        backsoundAudio.clip = audPerGameLevel[myGameDetail.level - 1];
        backsoundAudio.Play();
    }

    private IEnumerator showReflection() {
        animateReward();

        while(showReward==Show.SHOW){
            yield return null;
        }

        StartCoroutine(myReflectionCtrl.setActivity(myGameDetail.learningActivity));
        
        (score, wrongAttempt) = PlayerDataManager.getScoreNWrongAttempt(myGameDetail.level, myGameDetail.sublevel);
        StartCoroutine(myReflectionCtrl.setStars(score, wrongAttempt));
    }

    [SerializeField]
    Animator characterAnimator;
    const string START = "Start";

    List<GameDetailCard.Partner> cardRewards = new List<GameDetailCard.Partner>();
    List<Sprite> sentenceRewards = new List<Sprite>();
    List<Word> wordRewards = new List<Word>();

    int which = 0;

    private void animateReward()
    {
        Debug.Log("RewardCtrl: Animating the rewards!!!");
        characterAnimator.SetBool(START, true);
        if(myGameDetail is GameDetailCard){
            GameDetailCard myGameDetailCard = (GameDetailCard)myGameDetail;
            cardRewards.AddRange(myGameDetailCard.partners);
            which = 1;
        } else {
            if(myGameDetail is GameDetailKalimat){
                GameDetailKalimat myGameDetailKalimat = (GameDetailKalimat)myGameDetail;
                for(int i=0;i<myGameDetailKalimat.questions.Count;i++){
                    Sprite reward = myGameDetailKalimat.questions[i].gambarKalimat;
                    sentenceRewards.Add(reward);
                }
                which = 3;
            } else {
                GameDetailKata myGameDetailKata = (GameDetailKata)myGameDetail;
                for(int i=0;i<myGameDetailKata.questions.Count;i++){
                    Word reward = new Word{
                        spriteWord = myGameDetailKata.questions[i].word.spriteWord,
                        teksWord = myGameDetailKata.questions[i].word.teksWord
                    };
                    wordRewards.Add(reward);
                }
                which = 2;
            }
        }

        showOneByOne();
    }

    private void showOneByOne(){
        switch(which){
            case 1:
                StartCoroutine(showCardReward());
                break;
            case 2:
                StartCoroutine(showWordReward());
                break;
            case 3:
                StartCoroutine(showSentenceReward());
                break;
            default:
                Debug.Log("There is an error which made no myGameDetail...");
                // audioCharacter.Play();
                break;
        }
    }

    private enum Show{
        SHOW, IDLE
    }
    private Show showReward = Show.IDLE;

    [SerializeField]
    GameObject rewardSpritePrefab, rewardTextPrefab;

    int count;
    private IEnumerator showCardReward()
    {
        Debug.Log("This is Card reward");
        GameObject reward;
        count = cardRewards.Count;

        showReward = Show.SHOW;
        
        for(int c=1;c<3;c++){
            for(int i=0;i<count;i++){
                if(c==2){
                    reward = Instantiate(rewardSpritePrefab);
                    reward.transform.SetParent(bottomT, false);
                    reward.GetComponent<Image>().sprite = cardRewards[i].alphabet.spriteAlpha;
                } else {
                    reward = Instantiate(rewardTextPrefab);
                    reward.GetComponent<Image>().sprite = cardRewards[i].word.spriteWord;
                    reward.GetComponentInChildren<TextMeshProUGUI>().text = cardRewards[i].word.teksWord;
                    reward.transform.SetParent(topT, false);
                }
                yield return new WaitForSeconds(0.3f);
            }
        }

        showReward = Show.IDLE;
        
    }

    private bool isOdd(int number){
        if(number%2!=0){
            return true;
        } else {
            return false;
        }
    }
    [SerializeField]
    Transform bottomT, topT;
    private IEnumerator showWordReward(){
        Debug.Log("This is Word reward");

        GameObject reward;
        count = wordRewards.Count;
        int index = 0;

        showReward = Show.SHOW;

        for(int c=1;c<3;c++){
            for(int i=0;i<(count/2);i++){
                reward = Instantiate(rewardTextPrefab);
                if(c==2){
                    index = i + (count/2);
                    reward.transform.SetParent(bottomT, false);
                } else {
                    index = i;
                    reward.transform.SetParent(topT, false);
                }
                assignWordRewardToReward(reward, index);
                yield return new WaitForSeconds(0.3f);
            }
        }
        if(isOdd(count)){
            assignWordRewardToReward(Instantiate(rewardTextPrefab), index+1);
        }

        showReward = Show.IDLE;
    }
    
    private IEnumerator showSentenceReward(){
        Debug.Log("This is Sentence reward");

        GameObject reward;
        count = sentenceRewards.Count;
        int index = 0;

        showReward = Show.SHOW;

        for(int c=1;c<3;c++){
            for(int i=0;i<(count/2);i++){
                reward = Instantiate(rewardSpritePrefab);
                if(c==2){
                    index = i + (count/2);
                    reward.transform.SetParent(bottomT, false);
                } else {
                    index = i;
                    reward.transform.SetParent(topT, false);
                }
                assignSentenceRewardToReward(reward, index);
                yield return new WaitForSeconds(0.3f);
            }
        }
        if(isOdd(count)){
            assignSentenceRewardToReward(Instantiate(rewardTextPrefab), index+1);
        }

        showReward = Show.IDLE;
    }

    private void assignWordRewardToReward(GameObject reward, int index){
        reward.GetComponentInChildren<TextMeshProUGUI>().text = wordRewards[index].teksWord;
        reward.GetComponent<Image>().sprite = wordRewards[index].spriteWord;
    }

    private void assignSentenceRewardToReward(GameObject reward, int index){
        reward.GetComponent<Image>().sprite = sentenceRewards[index];
    }

    //persistent listener of HomeButton     
    public void HomeButtonClicked(){
        Debug.Log("RewardCtrl: back to parent scene");
	    mySceneManager.backToParentScene(isRewardList: true);
    }

    //persistent listener of RestartButton     
    public void goToGame(){
        Debug.Log("Going to game sublevel: " + myGameDetail.sublevel);
        mySceneManager.setGameFromReward(true);
        mySceneManager.loadScene(myGameDetail.tipe, myGameDetail.sublevel);
    }

}

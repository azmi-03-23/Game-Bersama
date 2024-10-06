using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    MySceneManager mySceneManager;
    [SerializeField]
    Button RewardMO, GameMO;
    PlayerDataManager.GameID highestGameID;

    const string DISABLED = "Disabled";
    void Awake()
    {
        highestGameID = PlayerDataManager.GetHighestGameAchievedID();
        if(highestGameID.level==0){
            RewardMO.interactable = false;
            myButtonAnims[2].SetBool(DISABLED, true);
            GameMO.interactable = false;
            myButtonAnims[0].SetBool(DISABLED, true);
            Debug.Log("No game has been played");
        }
    }

    void Start(){
        mySceneManager = MySceneManager.getInstance();
        mySceneManager.disableVNManager();
        if(highestGameID.level==0){
            StartCoroutine(highlightVNButton());
        } else {
            StartCoroutine(highlightMenuButtons());
        }
        Debug.Log("Havent started?");
    }

    const string HIGHLIGHT = "Highlight";

    IEnumerator highlightVNButton(){
        while(true){
            yield return new WaitForSecondsRealtime(5f);
            Debug.Log("gonna be highlighting");
            myButtonAnims[1].SetBool(HIGHLIGHT, true);
            yield return new WaitForSecondsRealtime(5f);
            myButtonAnims[1].SetBool(HIGHLIGHT, false);
            Debug.Log("stop highlighting");
        }
    }

    [SerializeField]
    List<Animator> myButtonAnims;
    /*
    1. Games
    2. VN
    3. Rewards
    */
    private IEnumerator highlightMenuButtons(){
        while(true){
            if(myButtonAnims.Count>0){
                foreach(Animator anim in myButtonAnims){
                    if(anim){
                        anim.SetBool(HIGHLIGHT, true);
                    } else {
                        yield break;
                    }
                    yield return new WaitForSeconds(5f);
                    if(anim){
                        anim.SetBool(HIGHLIGHT, false);
                    } else {
                        yield break;
                    }
                }
            } else {
                yield break;
            }
        }
    }
    
    public void OpenVisualNovel(){
        Debug.Log("MainMenuCtrl.OpenVisualNovel");
        mySceneManager.CreateVNManager();
    }

    public void OpenRewardListScene(){
        Debug.Log("Opening Reward List Scene");
        mySceneManager.CreateGameOrRewardManager(isRewardList: true);
        // mySceneManager.loadScene("RewardList");
    }
    public void OpenGameListScene(){
        Debug.Log("Opening Game List Scene");
        mySceneManager.CreateGameOrRewardManager(isRewardList: false);
        // mySceneManager.loadScene("GameList");
    }

}

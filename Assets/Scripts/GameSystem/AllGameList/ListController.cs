using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ListController : MonoBehaviour
{
    [SerializeField]
    AudioSource backsoundAudio;
    private const string HIGHLIGHT = "Highlight";
    private static IListMechanism myStrategy;

    public static void setStrategy(bool isRewardList){
        if(isRewardList){
            Debug.Log("RewardListController added");
            myStrategy = new RewardListController();
        } else {
            Debug.Log("GameListCtrl added");
            myStrategy = new GameListController();
        }
    }

    public static void setLevel(int level){
        levelGame = level;
    }

    [SerializeField]
    List<GamesPerLevel> listOfGamesPerLevel;
    
    [SerializeField]
    Transform myGameList;
    
    [SerializeField]
    List<Transform> buttonPosList = new List<Transform>();
    
    UtilClass myUtilityClass;
    
    protected List<GamesPerLevel.GameInLevel> myGames = new List<GamesPerLevel.GameInLevel>();
    
    [SerializeField]
    TextMeshProUGUI namaLevel;
    
    [SerializeField]
    Image backgroundImage;
    
    [SerializeField]
    Button gameButtonPrefab;
    
    List<Button> myGameButtons = new List<Button>();
    
    [SerializeField]
    Button myNextButton;

    private int gameCompletedCount = 0;
    private static int levelGame = 1;
    

    //PreviousButton persistent listener
    public void goPrevious(){
        StopCoroutine(highlightGameButtons());
        changeGames(true);
    }

    private void changeGames(bool isPrevious)
    {
        if(isPrevious){
            levelGame--;
        } else {
            levelGame++;
        }

        removeAllButtonListeners();

        foreach(Button button in myGameButtons){
            Destroy(button.gameObject);
        }
        myGameButtons.Clear();

        setCurrentLevelGames();
    }

    //NextButton persistent listener
    public void goNext(){
        StopCoroutine(highlightGameButtons());
        if(levelGame<3){
            changeGames(false);
        } else {
            mySceneManager.backToParentScene();
        }
    }

    private void setCurrentLevelGames()
    {
        Debug.Log("List Controller set Games");
        GamesPerLevel games = listOfGamesPerLevel[levelGame - 1];
        backgroundImage.sprite = games.bgImg;
        namaLevel.text = games.namaLevel;
        levelGame = games.level;
        myGames = games.games;
        backsoundAudio.clip = games.backsound;

        backsoundAudio.Play();
        
        mySceneManager.setCurrentGameLevel(levelGame);

        showCompletedGames();
    }

    protected MySceneManager mySceneManager;

    void Awake(){
        Debug.Log("AllGameList UnityScene is Loaded");
        mySceneManager = MySceneManager.getInstance();
        myUtilityClass = gameObject.AddComponent<UtilClass>();
        
        myNextButton.gameObject.SetActive(false);
    }

    void Start(){
        setCurrentLevelGames();
    }

    [SerializeField]
    GameObject myPreviousButtonGO;

    private void showCompletedGames(){
        Debug.Log("List Controller show completed games");
        int i0, i1, i2;
        int buttonCount;
        gameCompletedCount = PlayerDataManager.countGameDoneInLevelX(levelGame);
        if(myStrategy is RewardListController){
            buttonCount = gameCompletedCount;
        } else {
            buttonCount = myGames.Count;
        }
        for(int i=0;i<buttonCount;i++){
            myGameButtons.Add(Instantiate(gameButtonPrefab));
            TextMeshProUGUI buttonText = myGameButtons[i].gameObject.GetComponent<RectTransform>().GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = myGames[i].namaGame;
            myGameButtons[i].gameObject.transform.SetParent(myGameList, false);
            myGameButtons[i].gameObject.transform.position = buttonPosList[i].position;
            switch(i){
                case 0:
                    i0 = i;
                    myGameButtons[i].onClick.AddListener(delegate{goToScene(i0);});
                    break;
                case 1:
                    i1 = i;
                    myGameButtons[i].onClick.AddListener(delegate{goToScene(i1);});
                    break;
                case 2:
                    i2 = i;
                    myGameButtons[i].onClick.AddListener(delegate{goToScene(i2);});
                    break;
            }
            //catatan: tidak bisa menggunakan index yang berubah untuk delegate.
            Debug.Log("Index i adalah "+ i.ToString());
        }

        if(levelGame==1){
            myPreviousButtonGO.SetActive(false);
        } else {
            if(myPreviousButtonGO.activeSelf is false)
                myPreviousButtonGO.SetActive(true);
        }
        
        if(myStrategy is GameListController)
        {
            int notCompleted = buttonCount-gameCompletedCount;
            if(notCompleted>gameCompletedCount){
                if(notCompleted==3)
                    notCompleted--;
                for (int i=notCompleted; i>gameCompletedCount; i--)
                {
                    myGameButtons[i].interactable = false;
                }
            }
            if(!myNextButton.isActiveAndEnabled)
                myNextButton.gameObject.SetActive(true);
        }
        else {
            int highestLevelAchieved = PlayerDataManager.GetHighestGameAchievedID().level;
            checkToActivateNextButton(highestLevelAchieved);
        }

        StartCoroutine(highlightGameButtons());
    }

    private void checkToActivateNextButton(int highestLevelAchieved)
    {
        if (highestLevelAchieved > levelGame || highestLevelAchieved == 3)
        {
            if(!myNextButton.isActiveAndEnabled)
                myNextButton.gameObject.SetActive(true);
            if(gameCompletedCount==0){
                myNextButton.GetComponent<Animator>().SetBool(HIGHLIGHT, true);
            } else {
                myNextButton.GetComponent<Animator>().SetBool(HIGHLIGHT, false);
            }
        }
        else
        {
            inactivateNextButton();
        }
    }

    private void inactivateNextButton()
    {
        if (myNextButton.IsActive())
        {
            myNextButton.gameObject.SetActive(false);
        }
    }

    private void goToScene(int gameIndex){
        myStrategy.goToScene(myGames[gameIndex]);
    }

    private IEnumerator highlightGameButtons(){
        Animator myButtonAnim;
        Button button = null;
        while(true){
            if(myGameButtons.Count>0 && myGameButtons.Count>=gameCompletedCount){
                for(int i=0;i<=gameCompletedCount;i++){
                    if(i==3){
                        continue;
                    }
                    Debug.Log("Highlight "+ i+ " from " + gameCompletedCount + " buttons");
                    if(myGameButtons[i]){
                        button = myGameButtons[i];
                        myButtonAnim = button.GetComponent<Animator>();
                        myButtonAnim.SetBool(HIGHLIGHT, true);
                    } else {
                        yield break;
                    }
                    yield return new WaitForSeconds(5f);
                    if(button){
                        myButtonAnim.SetBool(HIGHLIGHT, false);
                    } else {
                        yield break;
                    }
                }
            } else {
                yield break;
            }
        }
    }

    void OnDisable()
    {
        levelGame = 1;
        removeAllButtonListeners();
    }

    private void removeAllButtonListeners()
    {
        foreach (Button button in myGameButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}

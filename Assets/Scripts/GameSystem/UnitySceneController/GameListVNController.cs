using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameListVNController : MonoBehaviour
{
    [SerializeField]
    AudioSource backsoundAudio;
    private void setBacksound()
    {
        backsoundAudio.clip = myVNManager.getBacksound();
        if(backsoundAudio.clip!=null){
            backsoundAudio.Play();
        } else {
            Debug.Log("backsound is non existent");
        }
    }
    List<GamesPerLevel.GameInLevel> myGames = new List<GamesPerLevel.GameInLevel>();
    [SerializeField]
    TextMeshProUGUI namaLevel;
    [SerializeField]
    Image backgroundImage;
    [SerializeField]
    List<Button> myGameButtons;
    [SerializeField]
    Animator myNextButtonAnim;

    int levelGame;

    private MySceneManager mySceneManager;
    private VNManager myVNManager;

    void Awake(){
        Debug.Log("GameListVN UnityScene is Loaded");
        myVNManager = VNManager.getInstance();
        mySceneManager = MySceneManager.getInstance();

        GamesPerLevel games = myVNManager.getGames();
        namaLevel.text = games.namaLevel;
        levelGame = games.level;
        myGames = games.games;
        backgroundImage.sprite = games.bgImg;
    }

    void Start(){
        setBacksound();
        showGames();
        highlightGames();
    }

    public void goPrev(){
        myVNManager.goToPrevSequence();
    }

    private void showGames(){
        int i0, i1, i2;
        for(int i=0;i<myGames.Count;i++){
            RectTransform buttonAsParent = myGameButtons[i].gameObject.GetComponent<RectTransform>();
            TextMeshProUGUI buttonText = buttonAsParent.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = myGames[i].namaGame;
            switch(i){
                case 0:
                    i0 = i;
                    myGameButtons[i].onClick.AddListener(delegate{goToGame(i0);});
                    break;
                case 1:
                    i1 = i;
                    myGameButtons[i].onClick.AddListener(delegate{goToGame(i1);});
                    break;
                case 2:
                    i2 = i;
                    myGameButtons[i].onClick.AddListener(delegate{goToGame(i2);});
                    break;
            }
            //catatan: tidak bisa menggunakan index yang berubah untuk delegate.
            Debug.Log("Index i adalah "+ i.ToString());
        }
    }
    // bool anyDone = false;
    int gameCompletedCount = 0;

    private void highlightGames(){
        Debug.Log("GameListVNCtrl.highlightGames");
        gameCompletedCount = PlayerDataManager.countGameDoneInLevelX(levelGame);
        activateButton(myGames.Count-gameCompletedCount);
        // Debug.Log("Any Done ? A: " + anyDone.ToString());
    }

    private void activateButton(int notCompleted)
    {
        if(notCompleted>gameCompletedCount){
            if(notCompleted==3)
                notCompleted--;
            for (int i=notCompleted; i>gameCompletedCount; i--)
            {
                myGameButtons[i].interactable = false;
            }
        }
        if(gameCompletedCount<3){
            // StartCoroutine(myUtilityClass.highlightOnlyOneElement(myGameButtons[gameCompletedCount].GetComponent<Image>(), highlightDefault, 4, highlightDuration));
            myGameButtons[gameCompletedCount].GetComponent<Animator>().SetBool(HIGHLIGHT, true);
        } else {
            StartCoroutine(highlightNextButton());
        }
    }

    const string HIGHLIGHT = "Highlight";

    private IEnumerator highlightNextButton(){
        myNextButtonAnim.SetBool(HIGHLIGHT, true);
        yield return new WaitForSeconds(5f);
        myNextButtonAnim.SetBool(HIGHLIGHT, false);
    }

    private void goToGame(int gameIndex){
        Debug.Log("Index dari myGames/button yang dipilih = " + gameIndex.ToString());
        GamesPerLevel.GameInLevel game = myGames[gameIndex]; 
        mySceneManager.loadScene(game.tipeGame, game.sublevelGame);
    }

    //persistent listener of myNextButton
    public void goNext(){
        Debug.Log("GameListVNCtrl.goNext");
        myVNManager.goToNextSequence();
    }

    void OnDisable(){
        foreach(Button button in myGameButtons){
            button.onClick.RemoveAllListeners();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDetailController : MonoBehaviour
{
    [SerializeField]
    Image bgImage;
    int sublevel, level;
    const string RESOURCEPATH = "GameDetails/";

    protected GameDetail myGameDetail;
    protected MySceneManager mySceneManager;
    protected virtual void Awake(){
        Debug.Log("Game Detail Ctrl Awake");
        mySceneManager = MySceneManager.getInstance();
        getGame();
    }

    protected virtual void OnEnable(){
        mySceneManager.screenShownEvent += screenShown;
    }
    private void getGame(){
        (sublevel, level) = mySceneManager.getCurrentGameLevelNSublevel();
        string temp = RESOURCEPATH + level + sublevel.ToString();
        myGameDetail = (GameDetail)Resources.Load(temp);
    }

    protected virtual void screenShown(){
        Debug.Log("screenShown GameDetailCtrl");
        setBackground();
    }

    private void setBackground(){
        bgImage.sprite = myGameDetail.backgroundGame;
    }

    protected virtual void OnDisable(){
        mySceneManager.screenShownEvent -= screenShown;
    }
    
}

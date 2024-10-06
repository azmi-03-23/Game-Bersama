using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameListController : IListMechanism
{

    // public static void screenShown(){
    //     instance.setGames();
    // }
    // public static GameListController instance;
    // protected override void Awake(){
    //     base.Awake();
    //     instance = this;
    // }

    public void goToScene(GamesPerLevel.GameInLevel myGame){ 
        MySceneManager.getInstance().loadScene(myGame.tipeGame, myGame.sublevelGame);
    }

}

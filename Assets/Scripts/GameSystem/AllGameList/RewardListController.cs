using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardListController : IListMechanism
{
    
    // public static void screenShown(){
    //     instance.setGames();
    // }
    // public static RewardListController instance;
    // protected override void Awake(){
    //     base.Awake();
    //     instance = this;
    // }
    public void goToScene(GamesPerLevel.GameInLevel myGame){ 
        MySceneManager.getInstance().loadScene("Reward", myGame.sublevelGame);
    }
    
}

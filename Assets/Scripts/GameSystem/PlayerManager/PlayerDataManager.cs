using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public static class PlayerDataManager
{
    
    static List<string> gameLevel = new(){
        "Huruf",
        "Kata",
        "Kalimat"
    };

    static List<int> hurufGame = new(){
        1,2,3
    };

    static List<int> kataGame = new(){
        1,2,3
    };
    
    static List<int> kalimatGame = new(){
        1,2,3
    };

    static List<List<int>> gameSublevel = new(){
        hurufGame,
        kataGame,
        kalimatGame
    };

    static ProgressVN progressVN;
    public struct ProgressVN{
        public int VNSequenceIndex, StorySceneIndex;
    }

    static GameID highestGameAchieved = new GameID{
        level = 0,
        sublevel = 0
    };
    
    public struct GameID{
        public int level, sublevel; 
    }

    public static readonly string SAVE_GAME_DIRECTORY = Application.persistentDataPath + "/GameProgress/",
        SAVE_VN_DIRECTORY = Application.persistentDataPath + "/VNProgress/",
        GAME_FILENAME = "game",
        VN_FILENAME = "vn",
        GENERAL_FILETYPE = ".txt";

    public static void Init(){
        if(!Directory.Exists(SAVE_GAME_DIRECTORY)){
            Directory.CreateDirectory(SAVE_GAME_DIRECTORY);
        }
        if(!Directory.Exists(SAVE_VN_DIRECTORY)){
            Directory.CreateDirectory(SAVE_VN_DIRECTORY);
        }
    }

    public static void LoadHighestGameAchievedInit()
    {
        bool newHighestGame = false;
        for(int l=gameLevel.Count;l>0;l--){
            for(int sl=gameSublevel[l-1].Count;sl>0;sl--)    {
                if(File.Exists(SAVE_GAME_DIRECTORY+GAME_FILENAME+l+sl+GENERAL_FILETYPE)){
                    highestGameAchieved = new GameID{
                        level = l,
                        sublevel = sl
                    };
                    newHighestGame = true;
                    break;
                }
            }
            if(newHighestGame){
                break;
            }
        }
        Debug.Log("highest game achieved from file: level: " + highestGameAchieved.level + " sublevel: " + highestGameAchieved.sublevel);
    }

    public static GameID GetHighestGameAchievedID(){
        return highestGameAchieved;
    }

    public static ProgressVN LoadNGetVNProgress()
    {
        string file = SAVE_VN_DIRECTORY+VN_FILENAME+GENERAL_FILETYPE;
        if(File.Exists(file)){
            string VNData = File.ReadAllText(file);
            progressVN = JsonUtility.FromJson<ProgressVN>(VNData);
        } else {
            progressVN.VNSequenceIndex = VNManager.VN_FIRST_INDEX;
            progressVN.StorySceneIndex = VNManager.STORY_SCENE_FIRST_INDEX;
        }
        Debug.Log(progressVN.VNSequenceIndex + " dan " + progressVN.StorySceneIndex);
        return progressVN;
    }
    
    public static void SaveVNProgress(){
        string saveVNData = JsonUtility.ToJson(progressVN);
        File.WriteAllText(SAVE_VN_DIRECTORY+VN_FILENAME+GENERAL_FILETYPE, saveVNData);
    }
    
    public static void SaveGameAchievement(int newSublevel, int newLevel, int newScore, int newWrongAttempt){
        //bisa jadi game yang dimainkan bukan yang terbaru
        SaveGameProgress(newSublevel, newLevel);
        GameCompleted newGC = new GameCompleted{
            level = newLevel,
            sublevel = newSublevel,
            score = newScore,
            wrongAttempt = newWrongAttempt
        };
        string saveGameData = JsonUtility.ToJson(newGC);
        File.WriteAllText(SAVE_GAME_DIRECTORY + GAME_FILENAME + newLevel + newSublevel + GENERAL_FILETYPE, saveGameData);
    }

    public static void SaveGameProgress(int newSublevel, int newLevel){
        //bisa jadi game yang dimainkan bukan yang terbaru
        if(highestGameAchieved.level<newLevel){
            highestGameAchieved.level = newLevel;
            highestGameAchieved.sublevel = newSublevel;
        }
    }

    public static void saveLatestVNSequenceIndex(int VNIndex)
    {
        progressVN.VNSequenceIndex = VNIndex;
    }

    public static void saveLatestStorySceneIndex(int StorySceneInd)
    {
        progressVN.StorySceneIndex = StorySceneInd;
    }

    public static int countGameDoneInLevelX(int searchLevel)
    {
        int found = 0;
        for(int sl=gameSublevel[searchLevel-1].Count;sl>0;sl--)    {
            if(File.Exists(SAVE_GAME_DIRECTORY+GAME_FILENAME+searchLevel+sl+GENERAL_FILETYPE)){
                found = sl;
                break;
            }
        }
        return found;
    }

    public static (int, int) getScoreNWrongAttempt(int searchLevel, int searchSublevel){
        string searchFile = SAVE_GAME_DIRECTORY+GAME_FILENAME+searchLevel+searchSublevel+GENERAL_FILETYPE;
        if(File.Exists(searchFile)){
            string gameDetailFound = File.ReadAllText(searchFile);
            GameCompleted gameCompeleted = JsonUtility.FromJson<GameCompleted>(gameDetailFound);
            return (gameCompeleted.score, gameCompeleted.wrongAttempt);
        }
        return (0,0);
    }
    
}

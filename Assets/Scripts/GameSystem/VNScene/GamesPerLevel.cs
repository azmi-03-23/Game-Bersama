using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewGamesPerLevel", menuName="ScriptableObject/New GamesPerLevel")]
[System.Serializable]
public class GamesPerLevel : VNScene
{
    public string namaLevel;
    public int level;
    public List<GameInLevel> games;
    public Sprite bgImg;

    [System.Serializable]
    public struct GameInLevel{
        public string namaGame, tipeGame;
        public int sublevelGame;
    }

}

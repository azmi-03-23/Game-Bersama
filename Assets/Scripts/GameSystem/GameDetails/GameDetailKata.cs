using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewGameDetailKata", menuName="ScriptableObject/New GameDetailKata")]
[System.Serializable]
public class GameDetailKata : GameDetail
{
    public List<SLQuestion> questions;
    [System.Serializable]
    public struct SLQuestion{
        public Word word;
        public List<SukuKata> answers;
        public List<SukuKata> otherChoices1;
        public List<SukuKata> otherChoices2;

    }
    [System.Serializable]
    public struct SukuKata{
        public string teksSukuKata;
        public AudioClip audioSukuKata;
    }
    
}

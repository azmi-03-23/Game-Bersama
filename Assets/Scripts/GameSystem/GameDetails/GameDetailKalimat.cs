using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewGameDetailKalimat", menuName="ScriptableObject/New GameDetailKalimat")]
[System.Serializable]
public class GameDetailKalimat : GameDetail
{
    public List<Kalimat> questions;
    [System.Serializable]
    public struct Kalimat{
        public Sprite gambarKalimat;
        public AudioClip audioKalimat;
        public List<Kata> answers;

    }
    [System.Serializable]
    public struct Kata{
        public string teksKata;
    }
    
}

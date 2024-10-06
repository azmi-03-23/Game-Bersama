using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewGameDetailCard", menuName="ScriptableObject/New GameDetailCard")]
[System.Serializable]
public class GameDetailCard : GameDetail
{
    public List<AudioClip> instructions;
    public List <Partner> partners;
    [System.Serializable]
    public struct Partner{
        public Alphabet alphabet;
        public Word word;
    }
}

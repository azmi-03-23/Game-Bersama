using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Word", fileName ="Data/Word")]
[System.Serializable]
public class Word : ScriptableObject
{
    public Sprite spriteWord;
    public AudioClip audioWord; 
    public string teksWord;
}

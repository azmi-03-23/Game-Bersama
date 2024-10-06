using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "New Alphabet", fileName ="Data/Alphabet")]
[System.Serializable]
public class Alphabet : ScriptableObject
{
    public Sprite spriteAlpha;
    public AudioClip audioAlpha;
}

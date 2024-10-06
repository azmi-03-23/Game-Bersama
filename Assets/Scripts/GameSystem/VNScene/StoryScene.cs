using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewStoryScene", menuName ="ScriptableObject/NewStoryScene")]
[System.Serializable]
public class StoryScene : ScriptableObject
{
    public Sprite background;
	public AudioClip narration;
    public StoryScene nextScene = null, prevScene = null;
}
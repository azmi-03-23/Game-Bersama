using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewStorySequence", menuName ="ScriptableObject/New Story Sequence")]
[System.Serializable]
public class StorySequence : VNScene
{
    public StoryScene firstStorySceneInSequence, lastStorySceneInSequence;
    public int sceneCount;
}

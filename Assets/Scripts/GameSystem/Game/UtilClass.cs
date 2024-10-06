using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UtilClass : MonoBehaviour
{
    RuntimeAnimatorController myAnim;

    void Awake(){
        myAnim = (RuntimeAnimatorController)Resources.Load("Animators/highlightAnswer");
    }
    State audioState = State.IDLE;
    State highlightState = State.IDLE;
    State aHState = State.IDLE;

    public enum State{
        PLAYING, IDLE
    }

    public State getAudioState(){
        return audioState;
    }
    public State getHighlightState(){
        return highlightState;
    }

    public State getAHState(){
        return aHState;
    }

    public IEnumerator justHighlight(Image myImage, string highlightPurpose, int times, float duration){
        Animator addedAnim;
        if(myImage.GetComponent<Animator>()==null){
            addedAnim = myImage.AddComponent<Animator>();
        } else {
            addedAnim = myImage.GetComponent<Animator>();
        }
        addedAnim.runtimeAnimatorController = myAnim;
        addedAnim.SetBool(highlightPurpose, true);
        yield return new WaitForSeconds(duration);
        addedAnim.SetBool(highlightPurpose, false);
    }

    public IEnumerator highlightOnlyOneElement(Image myImage, string highlightPurpose, int times, float duration)
    {
        while(highlightState==State.PLAYING){
            yield return null;
        }
        
        highlightState = State.PLAYING;
        
        Debug.Log("highlightElement");

        Animator addedAnim;
        if(myImage.GetComponent<Animator>()==null){
            addedAnim = myImage.AddComponent<Animator>();
        } else {
            addedAnim = myImage.GetComponent<Animator>();
        }
        addedAnim.runtimeAnimatorController = myAnim;
        addedAnim.SetBool(highlightPurpose, true);
        yield return new WaitForSeconds(duration);
        addedAnim.SetBool(highlightPurpose, false);
        
        highlightState = State.IDLE;
    }

    AudioSource playingAud;

    public IEnumerator playOnlyOneAudio(AudioSource audSource){
        playingAud = audSource;
        while(audioState==State.PLAYING){
            yield return null;
        }
        audioState = State.PLAYING;
        playingAud.loop = false;
        if(playingAud.clip!=null){
            playingAud.Play();
            float audioLength = playingAud.clip.length;
            yield return new WaitForSeconds(audioLength);
        }
        if(audioState==State.PLAYING){//didnt get stopped
            audioState = State.IDLE;
        }
    }

    public void stopAudio(){
        if(playingAud!=null){
            playingAud.Stop();
            audioState = State.IDLE;
        }
    }
    
    public enum Duration{
        SHORT, MEDIUM
    }

    public IEnumerator playAudioWhileHighlight(GameObject gO, string highlightPurpose, Duration duration){
        while(aHState==State.PLAYING){
            yield return null;
        }
        aHState = State.PLAYING;
        while(audioState==State.PLAYING || highlightState==State.PLAYING){
            yield return null;
        }
        
        float durationTime = 1f;
        if(duration==Duration.SHORT){
            durationTime = 1f;
        }
        if(duration==Duration.MEDIUM){
            durationTime = 2f;
        }

        StartCoroutine(highlightOnlyOneElement(gO.GetComponent<Image>(), highlightPurpose, 4, durationTime));
        StartCoroutine(playOnlyOneAudio(gO.GetComponent<AudioSource>()));
        while(audioState==State.PLAYING || highlightState==State.PLAYING){
            yield return null;
        }
        
        aHState = State.IDLE;
    }

    public void setAllStateIDLE()
    {
        aHState = State.IDLE;
        audioState = State.IDLE;
        highlightState = State.IDLE;
    }
}

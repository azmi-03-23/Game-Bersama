using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReflectionController : MonoBehaviour
{
    [SerializeField]
    AudioSource characterAudio;
    [SerializeField]
    List<AudioClip> starsReflection = new List<AudioClip>();
    [SerializeField]
    UtilClass myUtilityClass;

    private Show laShow;
    public IEnumerator setActivity(AudioClip learningActivity){
        laShow = Show.SHOW;
        Debug.Log("playing reflection on learning activity");
        characterAudio.clip = learningActivity;
        StartCoroutine(myUtilityClass.playOnlyOneAudio(characterAudio));
        while(myUtilityClass.getAudioState()==UtilClass.State.PLAYING){
            yield return null;
        }
        laShow = Show.IDLE;
    }

    [SerializeField]
    GameObject starPrefab;
    GameObject star;
    [SerializeField]
    List<Transform> starPos = new List<Transform>();
    public IEnumerator setStars(int score, int wrongAttempt){
        Debug.Log("WrongAttempt: "+wrongAttempt+" questionCount: "+score);
        while(laShow==Show.SHOW){
            yield return null;
        }
        Debug.Log("is ending");
        if(wrongAttempt>score*2)
        {
            //Kamu sudah berusaha! Hmmm, apakah kamu mau mengulang lagi?
            //Satu bintang
            characterAudio.clip = starsReflection[0];
            StartCoroutine(showStar(1));
            Debug.Log(1);
        }
        else {
			if(wrongAttempt>=score){
			    //  dua bintang
                characterAudio.clip = starsReflection[1];
                StartCoroutine(showStar(2));
                Debug.Log(2);
			} else {
                // "100 poin untuk kamu!!"
                // Tiga bintang
                characterAudio.clip = starsReflection[2];
                StartCoroutine(showStar(3));
                Debug.Log(3);
			}
		}
        while(starShow==Show.SHOW){
            yield return null;
        }
        StartCoroutine(myUtilityClass.playOnlyOneAudio(characterAudio));
        Debug.Log("playing reflections audio");
    }

    void OnDisable(){
        StopAllCoroutines();
        myUtilityClass.setAllStateIDLE();
        laShow = Show.IDLE;
        starShow = Show.IDLE;
    }

    public Show starShow;

    public enum Show{
        SHOW, IDLE
    }

    private const string SHOW = "Show";

    private IEnumerator showStar(int starCount)
    {
        starShow = Show.SHOW;
        for(int i=0;i<starCount;i++){
            star = Instantiate(starPrefab);
            star.transform.SetParent(starPos[i], false);
            Image starImage = star.GetComponent<Image>();
            starImage.enabled = false;
            star.GetComponent<Animator>().SetBool(SHOW, true);
            yield return new WaitForSeconds(1f);
            star.GetComponent<Animator>().SetBool(SHOW, false);
            starImage.enabled = true;
        }
        starShow = Show.IDLE;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
	[SerializeField]
    Slider sliderSE, sliderNS;
    AudioSource sEAudSource, nSAudSource;
	public void setSENSAudio(AudioSource SE, AudioSource NS){
		sEAudSource = SE;
		nSAudSource = NS;
        sliderSE.value = sEAudSource.volume;
        sliderNS.value = nSAudSource.volume;
		sliderSE.onValueChanged.AddListener(delegate{sESliderChanged();});
		sliderNS.onValueChanged.AddListener(delegate{nSSliderChanged();});
	}
	[SerializeField]
	public Button closeButton;
	private void sESliderChanged(){
		sEAudSource.volume = sliderSE.value;
	}
	private void nSSliderChanged(){
		nSAudSource.volume = sliderNS.value;
	}
	void OnDisable(){
		sliderSE.onValueChanged.RemoveAllListeners();
		sliderNS.onValueChanged.RemoveAllListeners();
	}

}

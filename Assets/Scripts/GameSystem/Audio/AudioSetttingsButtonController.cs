using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetttingsButtonController : MonoBehaviour
{
    [SerializeField]
    AudioSource SE, NS;
    [SerializeField]
    GameObject audSettingsPrefab;
    GameObject audSettingsGO = null;
    AudioSettingsController myAudSetCtrl;
    [SerializeField]
    Transform canvas;

    public void audButtonClicked(){
        if(audSettingsGO==null){
            audSettingsGO = Instantiate(audSettingsPrefab);
            myAudSetCtrl = audSettingsGO.GetComponent<AudioSettingsController>();
            myAudSetCtrl.setSENSAudio(SE, NS);
            myAudSetCtrl.closeButton.onClick.AddListener(closeAudioSettingsGO);
            audSettingsGO.transform.SetParent(canvas, false);
        }
    }
    private void closeAudioSettingsGO(){
        myAudSetCtrl.closeButton.onClick.RemoveAllListeners();
        Destroy(audSettingsGO);
    }
}

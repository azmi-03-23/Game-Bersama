using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressLoader : MonoBehaviour
{
    [SerializeField]
    Slider progressBar;
    AsyncOperation operation;
    bool showProgress = false;

    public void setOperation(AsyncOperation theOperation){
        operation = theOperation;
        showProgress = true;
    }

    void OnDisable(){
        progressBar.value = 0f;
        showProgress = false;
    }

    void Update(){
        if(showProgress)
            progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);
    }
    
}

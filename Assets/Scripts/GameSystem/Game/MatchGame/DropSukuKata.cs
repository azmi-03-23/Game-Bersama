using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSukuKata : MonoBehaviour, IDropHandler
{
    public delegate void correctAnswerE(GameObject gO);
    public static event correctAnswerE correctAnswerEvent;

    public delegate void wrongAnswerE(GameObject gO);
    public static event wrongAnswerE wrongAnswerEvent;

    GameObject chosenSLObject;
    string chosenSL;

    void IDropHandler.OnDrop(PointerEventData eventData){
        Debug.Log("DropContainer = on drop");
        chosenSLObject = eventData.pointerDrag;
        chosenSL = chosenSLObject.GetComponentInChildren<TextMeshProUGUI>().text;
        if(chosenSL.Equals(gameObject.name)){
            setWrong(false);
            chosenSLObject.transform.SetParent(gameObject.transform, false);
            correctAnswerEvent?.Invoke(chosenSLObject);
        } else {
            setWrong(true);
            wrongAnswerEvent?.Invoke(chosenSLObject);
        }
	}

    bool wrong = false;
    private void setWrong(bool yes){
        wrong = yes;
    }
    
    public bool isWrong(){
        if(wrong is false){
            return false;
        } else {
            wrong = false;
            return true;
        }
    }


}

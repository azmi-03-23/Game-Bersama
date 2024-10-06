using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAnswer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool locked = false;
    Vector3 beginningPos;
    Canvas myCanvas;
    CanvasGroup cG;
    RectTransform rT;
    void Awake(){        
        rT = GetComponent<RectTransform>();
        myCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData){
        Debug.Log("On begin Drag");
        beginningPos = transform.position;
        cG = GetComponent<CanvasGroup>();
        cG.blocksRaycasts = false;
        DropSukuKata.correctAnswerEvent += lockPos;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData){
        Debug.Log("On End Drag");
        cG.blocksRaycasts = true;
        if(locked is false){
            backToBeginPos();
        } else {
            this.enabled = false;
        }
    }

    private void backToBeginPos(){
        Debug.Log("lockpos");
        transform.position = beginningPos;
    }

    private void lockPos(GameObject pos){
        Debug.Log("lockpos");
        pos.GetComponent<DragAnswer>().locked = true;
        pos.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rT.anchoredPosition += eventData.delta/myCanvas.scaleFactor;        
    }

    void OnDisable(){
        locked = false;
        DropSukuKata.correctAnswerEvent -=lockPos;
    }
}

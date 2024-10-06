using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class WordGameController : MatchGameController
{
    [SerializeField]
    GameObject opsiSLPrefab;
    List<GameObject> opsiSLGOs = new List<GameObject>();
    int additionalAnswerIndex = 1; //awalnya cuma 2 jawaban
    GameObject currentAssigningAnswerSL;
    const int CURRENT_DEFAULT_SL_COUNT = 2;
    private void setPrefab(int addition)
    {
        for (int i = 0; i < addition; i++)
        {
            currentAssigningAnswerSL = Instantiate(opsiSLPrefab);
            opsiSLGOs.Add(currentAssigningAnswerSL);
            additionalAnswerIndex++;
            setAdditionalQNA();
        }

        resizeAnsTempNChoices(addition);
    }

    private void resizeAnsTempNChoices(int addition)
    {
        RectTransform tempRT = rightChoiceObject.transform.GetChild(0).GetComponent<RectTransform>();

        defaultSize = tempRT.sizeDelta;
        anchorMax = tempRT.anchorMax;
        anchorMin = tempRT.anchorMin;

        int amount = addition + CURRENT_DEFAULT_SL_COUNT;

        float width = (jawabanTemp.GetComponent<RectTransform>().sizeDelta.x - 10 * (amount + 1)) / amount;

        Debug.Log("Width of SKOption is " + width);

        for (int i = 0; i < rightChoiceObject.transform.childCount; i++)
        {
            tempRT = rightChoiceObject.transform.GetChild(i).GetComponent<RectTransform>();
            tempRT.sizeDelta = new Vector2(width, tempRT.sizeDelta.y);
        }
        for (int i = 0; i < leftChoiceObject.transform.childCount; i++)
        {
            tempRT = leftChoiceObject.transform.GetChild(i).GetComponent<RectTransform>();
            tempRT.sizeDelta = new Vector2(width, tempRT.sizeDelta.y);
        }
    }

    [SerializeField]
    GameObject jawabanSLPrefab;
    [SerializeField]
    Transform jawabanTemp;
    private void setAdditionalQNA(){
        GameObject newSLGO = Instantiate(jawabanSLPrefab);
        newSLGO.name = currentQuestion.answers[additionalAnswerIndex].teksSukuKata;
        newSLGO.GetComponent<AudioSource>().clip = currentQuestion.answers[additionalAnswerIndex].audioSukuKata;

        newSLGO.transform.SetParent(jawabanTemp, false);

        Debug.Log("rightChoices.Count = " + rightChoices.Count.ToString());
        Debug.Log("Adding 1 more answer");
        Transform parentTr = null;
        currentAssigningAnswerSL.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[additionalAnswerIndex].teksSukuKata;
        currentAssigningAnswerSL.GetComponent<AudioSource>().clip = currentQuestion.answers[additionalAnswerIndex].audioSukuKata;
        switch(additionalAnswerIndex){
            case 2:
                parentTr = leftChoiceObject.transform;
                break;
            case 3:
                parentTr = rightChoiceObject.transform;
                break;
            default:
                Debug.Log("more than max. Maximum suku kata = 4");
                break;
        }
        currentAssigningAnswerSL.transform.SetParent(parentTr, false);
        choicesDA.Add(currentAssigningAnswerSL.GetComponent<DragAnswer>());
    }

    List<DragAnswer> choicesDA = new List<DragAnswer>();

    GameDetailKata myGameDetailKata;
    protected override void Awake(){
        base.Awake();
        myGameDetailKata = (GameDetailKata)myGameDetail;
    }
    protected override void startGame(){
        setQNA();
    }
    
    [SerializeField]
    GameObject bendaPos, sukuKata1, sukuKata2;

    [SerializeField]
    AudioSource bendaAudio, sK1Audio, sK2Audio;
    [SerializeField]
    TextMeshProUGUI bendaPosText;
    [SerializeField]
    GameObject leftChoiceObject, rightChoiceObject;
    [SerializeField]
    List<GameObject> leftChoices = new List<GameObject>(), rightChoices = new List<GameObject>();
    int leftAnswerIndex, rightAnswerIndex;
    GameDetailKata.SLQuestion currentQuestion;
    private void setQNA(){
        /*
        Menampilkan gambar (komponen dari layar)
        Menampilkan berbagai pilihan suku kata dari benda (komponen dari layar)
        Memainkan audio benda
        */
        Debug.Log("WGCtrl.setQNA");
        currentQuestion = myGameDetailKata.questions[currentQAIndex];
        SLCount = currentQuestion.answers.Count;
        Debug.Log("This is the question = " + currentQuestion.word.teksWord);
        Debug.Log("Jumlah suku kata dari kata tsb adalah " + SLCount);

        bendaAudio.clip = currentQuestion.word.audioWord;
        questionImage.sprite = currentQuestion.word.spriteWord;
        bendaPosText.enabled = false;
        bendaPosText.text = currentQuestion.word.teksWord;

        bendaPos.SetActive(true);
        
        StartCoroutine(myUtilityClass.playAudioWhileHighlight(bendaPos, highlightDefault, UtilClass.Duration.SHORT));
        
        Debug.Log("Count of nswers of suku kata dari " + currentQuestion.word.teksWord + " is " + currentQuestion.answers.Count.ToString());
        sukuKata1.name = currentQuestion.answers[0].teksSukuKata;
        sK1Audio.clip = currentQuestion.answers[0].audioSukuKata;
        sukuKata2.name = currentQuestion.answers[1].teksSukuKata;
        sK2Audio.clip = currentQuestion.answers[1].audioSukuKata;

        Debug.Log("leftChoices.Count = " + leftChoices.Count.ToString());
        leftAnswerIndex = Random.Range(0,2);
        leftChoices[leftAnswerIndex].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[0].teksSukuKata;
        leftChoices[leftAnswerIndex].GetComponent<AudioSource>().clip = currentQuestion.answers[0].audioSukuKata;
        fillOtherChoices(leftChoices, leftAnswerIndex, currentQuestion.otherChoices1);
        
        Debug.Log("rightChoices.Count = " + rightChoices.Count.ToString());
        rightAnswerIndex = Random.Range(0,2);
        rightChoices[rightAnswerIndex].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[1].teksSukuKata;
        rightChoices[rightAnswerIndex].GetComponent<AudioSource>().clip = currentQuestion.answers[1].audioSukuKata;
        fillOtherChoices(rightChoices, rightAnswerIndex, currentQuestion.otherChoices2);  
        

        if(SLCount>2){
            setPrefab(SLCount-CURRENT_DEFAULT_SL_COUNT);
        }

        leftChoiceObject.SetActive(true);
        rightChoiceObject.SetActive(true);

        for(int i=0;i<choicesDA.Count;i++){
            choicesDA[i].enabled = true;
            if(!choicesDA[i].isActiveAndEnabled){
                Debug.LogError("drag answer is not enabled");
            }
        }
        choicesDA.Clear();
        
    }


    private void fillOtherChoices(List<GameObject> posChoices, int answerIndex, List<GameDetailKata.SukuKata> otherChoiceList){
        int otherChoiceListIndex = 0;
        for(int i=0;i<posChoices.Count;i++){
            if(i==answerIndex)
                continue;
            posChoices[i].GetComponentInChildren<TextMeshProUGUI>().text = otherChoiceList[otherChoiceListIndex].teksSukuKata;
            posChoices[i].GetComponent<AudioSource>().clip = otherChoiceList[otherChoiceListIndex].audioSukuKata;
            otherChoiceListIndex++;
        }
    }
    int correctSLPerQuest = 0;
    /*
    Jika benar suku kata:
    5. Memberikan audio sambil menghighlight suku kata tersebut.
    Jika sudah lengkap:
    6. menghighlight masing-masing suku kata sesuai urutan sambil memainkan audio suku kata.
    7. Memainkan audio benda sambil menampilkan teks kata benda.
    8. Memainkan audio benar sambil menampilkan tanda benar.
    9. Menampilkan reward di bagian reward.
    Jika sudah benda terakhir:
    10. Menyimpan progress game
    11. Use case Melihat reward
    Jika bukan benda terakhir:
    11. Mengulang ke langkah 1.
    Jika salah suku kata:
    5. Mengembalikan suku kata tersebut ke tempat awal sambil menampilkan tanda salah dan memainkan audio salah. (DropSukuKata)
    6. Mengulang audio benda sambil menghighlight benda
    7. Menghighlight suku kata sambil memainkan audio suku kata.
    8. Kembali ke 4.
    Jika belum lengkap suku kata:
    6. Kembali menunggu (langkah 4)
    */

    int SLCount = 2;

    Vector2 defaultSize, anchorMin, anchorMax;
    protected override void correctAnswer(GameObject correctAnswer){        
        RectTransform tempRT = correctAnswer.GetComponent<RectTransform>();
        Vector2 temp = Vector2.one * 0.5f;
        tempRT.anchorMax = temp;
        tempRT.anchorMin = temp;
        StartCoroutine(myUtilityClass.playAudioWhileHighlight(correctAnswer, highlightCorrect, UtilClass.Duration.SHORT));
        
        correctSLPerQuest++;
        if(correctSLPerQuest==SLCount)
        {            
            StartCoroutine(explainBendaThenProceedForReward());
        }
    }
    GameObject ceklisGO;
    IEnumerator explainBendaThenProceedForReward()
    {
        StartCoroutine(myUtilityClass.playAudioWhileHighlight(bendaPos, highlightDefault, UtilClass.Duration.SHORT));
        bendaPosText.enabled = true;
        while (myUtilityClass.getAHState() == UtilClass.State.PLAYING)
        {
            yield return null;
        }

        correctSLPerQuest = 0;
        score++;
        // showCorrectSign();
        if (score == 1 && restart is false)
        {
            ceklisGO = Instantiate(ceklisPrefab);
            ceklisWord();
            ceklisGO.transform.SetParent(bendaPos.transform, false);
        }
        else
        {
            ceklisGO.SetActive(true);
        }
        StartCoroutine(myUtilityClass.playOnlyOneAudio(ceklisGO.GetComponent<AudioSource>()));
        base.correctAnswer(null);
        while (myUtilityClass.getAudioState() == UtilClass.State.PLAYING)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        ceklisGO.SetActive(false);

        showReward();

        nextQuestion();
    }

    private void ceklisWord()
    {
        RectTransform rT = ceklisGO.GetComponent<RectTransform>();
        Vector2 tempV2 = Vector2.one * 0.5f;
        rT.anchorMax = tempV2;
        rT.anchorMin = tempV2;
        rT.pivot = tempV2;
        ceklisGO.GetComponent<Image>().SetNativeSize();
    }

    private void nextQuestion(){
        currentQAIndex++;
        if(myGameDetailKata.questions.Count>currentQAIndex){
            choicesBack();
            hideTemporary();
            StartCoroutine(waitNShowNext());
        } else {
            saveGameNShowEndingReward();
        }
    }

    IEnumerator waitNShowNext()
    {
        yield return new WaitForSeconds(2f);
        setQNA();
    }

    protected override void choicesBack()
    {
        Debug.Log("choices is BACK");
        
        if(SLCount>2){
            for(int i=0;i<(SLCount-CURRENT_DEFAULT_SL_COUNT);i++){
                Destroy(opsiSLGOs[i]);
                Destroy(jawabanTemp.GetChild(CURRENT_DEFAULT_SL_COUNT+i).gameObject);
            }
            opsiSLGOs.Clear();
            additionalAnswerIndex = 1;
        }

        leftChoices[leftAnswerIndex].transform.SetParent(leftChoiceObject.transform, false);
        choicesDA.Add(leftChoices[leftAnswerIndex].GetComponent<DragAnswer>());
        rightChoices[rightAnswerIndex].transform.SetParent(rightChoiceObject.transform, false);
        choicesDA.Add(rightChoices[rightAnswerIndex].GetComponent<DragAnswer>());

        if(SLCount>CURRENT_DEFAULT_SL_COUNT){
            Transform tempT = rightChoiceObject.transform;
            for(int i=0;i<tempT.childCount;i++){
                backToDefaultSize(tempT.GetChild(i).GetComponent<RectTransform>());
            }
            tempT = leftChoiceObject.transform;
            for(int i=0;i<tempT.childCount;i++){
                backToDefaultSize(tempT.GetChild(i).GetComponent<RectTransform>());
            }
        }
    }

    private void backToDefaultSize(RectTransform rT){
        rT.sizeDelta = defaultSize;
        rT.anchorMin = anchorMin;
        rT.anchorMax = anchorMax;
    }

    protected override void hideTemporary(){
        leftChoiceObject.SetActive(false);
        rightChoiceObject.SetActive(false);
        bendaPos.SetActive(false);
    }

    protected override void showFeedback(GameObject wrongAnswer){
        wrongAttemptMade();
        GameObject wrongContainer = wrongAnswer;
        Transform tempT;
        for(int i=0;i<SLCount;i++){
            tempT = jawabanTemp.GetChild(i);
            if(tempT.GetComponent<DropSukuKata>().isWrong()){
                wrongContainer = tempT.gameObject;
                break;
            };
        }
        float highlightDuration = 1f;
        StartCoroutine(myUtilityClass.justHighlight(wrongContainer.GetComponent<Image>(), highlightWrong, 4, highlightDuration));
        StartCoroutine(myUtilityClass.justHighlight(wrongAnswer.GetComponent<Image>(), highlightWrong, 4, highlightDuration));
        StartCoroutine(myUtilityClass.playOnlyOneAudio(salahAud));
        
        StartCoroutine(myUtilityClass.playAudioWhileHighlight(bendaPos, highlightDefault, UtilClass.Duration.SHORT));

        StartCoroutine(myUtilityClass.playAudioWhileHighlight(wrongAnswer, highlightDefault, UtilClass.Duration.SHORT));
        base.showFeedback(null);
    }

}
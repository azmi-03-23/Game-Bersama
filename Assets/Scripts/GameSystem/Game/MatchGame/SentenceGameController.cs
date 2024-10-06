using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = System.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/*
pertama benerin resize untuk apply ke seluruh opsi
    -baik ketika wordCount==DEFAULT atau lebih
    -commit ketika sudah benar
    -merge
    -push before-final after-miscom
terus, jangan lupa apply stash@{0} untuk ngebenerin error reposisi
*/

public class SentenceGameController : MatchGameController
{
     int additionalAnswerQuestionIndex = 2; //awalnya cuma 3 kata
    GameObject currentAssigningChoiceWord;
    List<GameObject> additionalChoiceGOs = new List<GameObject>();
    const int CURRENT_DEFAULT_WORD_COUNT = 3;
    Vector2 defaultSize;
    private void setPrefab(int addition)
    {
        for (int i = 0; i < addition; i++)
        {
            currentAssigningChoiceWord = Instantiate(defaultChoices[0]);
            additionalChoiceGOs.Add(currentAssigningChoiceWord);
            additionalAnswerQuestionIndex++;
            setAdditionalQNA();
        }
    }

    Vector2 defaultAnsParentSize;

    IEnumerator resizeAnsParentNChoices()
    {
        defaultSize = defaultChoices[0].GetComponent<RectTransform>().sizeDelta;

        int max = 0, temp, maxIndex = 0;
        for (int i = 0; i < wordCount; i++)
        {
            temp = currentQuestion.answers[i].teksKata.Length;
            if (temp > max)
            {
                max = temp;
                maxIndex = i;
            }
        }

        yield return new WaitForEndOfFrame();

        Transform choiceParentTr = choiceParent.transform;
        choiceCSF = choiceParentTr.GetChild(maxIndex).gameObject.AddComponent<ContentSizeFitter>();
        choiceCSF.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        
        yield return new WaitForEndOfFrame();
        choiceCSF.SetLayoutHorizontal();
        HorizontalLayoutGroup cSFHLG = choiceCSF.GetComponent<HorizontalLayoutGroup>();
        cSFHLG.CalculateLayoutInputHorizontal();
        cSFHLG.SetLayoutHorizontal();

        RectTransform tempRt;
        float maxLength = choiceCSF.gameObject.GetComponent<HorizontalLayoutGroup>().preferredWidth;
        for (int i = 0; i < wordCount; i++)
        {
            if (i==maxIndex)
            {
                continue;
            }
            tempRt = choiceParentTr.GetChild(i).gameObject.GetComponent<RectTransform>();
            tempRt.sizeDelta = new Vector2(maxLength, tempRt.sizeDelta.y);
        }

        parentHLG.enabled = true;
        yield return new WaitForEndOfFrame();
        parentHLG.CalculateLayoutInputHorizontal();
        parentHLG.SetLayoutHorizontal();

        defaultAnsParentSize = answerParentTr.GetComponent<RectTransform>().sizeDelta;

        float width = maxLength*wordCount + answerParentTr.gameObject.GetComponent<HorizontalLayoutGroup>().spacing*(wordCount+1);
        RectTransform ansParentRt = answerParentTr.gameObject.GetComponent<RectTransform>();
        ansParentRt.sizeDelta = new Vector2(width, ansParentRt.sizeDelta.y);

        randomizeChoicePositions();

        yield break;
    }
    [SerializeField]
    HorizontalLayoutGroup parentHLG;

    [SerializeField]
    Transform answerParentTr;
    ContentSizeFitter choiceCSF;
    private void setAdditionalQNA(){
        GameObject newWordGO = Instantiate(answers[0]);
        newWordGO.name = currentQuestion.answers[additionalAnswerQuestionIndex].teksKata;

        newWordGO.transform.SetParent(answerParentTr, false);

        Debug.Log("Adding 1 more answer");
        
        currentAssigningChoiceWord.GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[additionalAnswerQuestionIndex].teksKata;
        currentAssigningChoiceWord.transform.SetParent(choiceParent.transform, false);
        choicesDA.Add(currentAssigningChoiceWord.GetComponent<DragAnswer>());
    }
    
    GameDetailKalimat myGameDetailKalimat;

    protected override void Awake(){
        base.Awake();
        myGameDetailKalimat = (GameDetailKalimat)myGameDetail;
    }
    protected override void startGame(){
        setImageNWords();
    }

    [SerializeField]
    GameObject gambarPos;
    [SerializeField]
    List<GameObject> answers;
    [SerializeField]
    AudioSource gambarAudio;
    [SerializeField]
    GameObject choiceParent;
    [SerializeField]
    List<GameObject> defaultChoices = new List<GameObject>();
    GameDetailKalimat.Kalimat currentQuestion;

    private void setImageNWords(){
        /*
        1. Menampilkan gambar dan template jawaban (dari layar)
        2. Menampilkan kalimat yang katanya di acak
        */
        Debug.Log("WGCtrl.setImageNWords");
        currentQuestion = myGameDetailKalimat.questions[currentQAIndex];

        gambarAudio.clip = currentQuestion.audioKalimat;
        questionImage.sprite = currentQuestion.gambarKalimat;
        
        gambarPos.SetActive(true);
        
        Debug.Log("Count of answers of kata dari currentQuestion is " + currentQuestion.answers.Count.ToString());
        wordCount = currentQuestion.answers.Count;
        
        for(int i=0;i<CURRENT_DEFAULT_WORD_COUNT;i++){
            answers[i].name = currentQuestion.answers[i].teksKata;
            defaultChoices[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i].teksKata;
        }

        if(currentQAIndex==0 && restart is false){
            DragAnswer tempDA;
            for(int i=0;i<defaultChoices.Count;i++){
                tempDA = defaultChoices[i].GetComponent<DragAnswer>();
                choicesDA.Add(tempDA);
                if(!tempDA.isActiveAndEnabled){
                    Debug.LogError("drag answer is of choice no "+i+" is not enabled");
                }
            }
        }

        if(wordCount>CURRENT_DEFAULT_WORD_COUNT){
            setPrefab(wordCount-CURRENT_DEFAULT_WORD_COUNT);
        }
        
        StartCoroutine(resizeAnsParentNChoices());

        // randomizeChoicePositions();
        
        choiceParent.SetActive(true);

        if(restart || currentQAIndex>0){
            for(int i=0;i<choicesDA.Count;i++){
                choicesDA[i].enabled = true;
                if(!choicesDA[i].isActiveAndEnabled){
                    Debug.LogError("drag answer is of choice no "+i+" is not enabled");
                }
            }
        }


    }

    List<DragAnswer> choicesDA = new List<DragAnswer>();

    int wordCount = 3;

    private void randomizeChoicePositions(){
        int randomNewPosIndex;
        Random random = new Random();

        // Vector2 temp;
        // RectTransform changing, toward;
        // Transform choiceParentTr = choiceParent.transform;    
        // for(int i=0;i<wordCount/2;i++){
        //     randomNewPosIndex = random.Next(i, wordCount);
        //     Debug.Log("Index tujuan dari choice ke "+i+" adalah "+randomNewPosIndex);
        //     changing = choiceParentTr.GetChild(i).GetComponent<RectTransform>();
        //     toward = choiceParentTr.GetChild(randomNewPosIndex).GetComponent<RectTransform>();
        //     temp = changing.anchoredPosition;
        //     changing.anchoredPosition = toward.anchoredPosition;
        //     toward.anchoredPosition = temp;
        //     Debug.Log("choice no " + i + " in anchoresPos = " + changing.anchoredPosition);
        // }
        Transform choiceParentTr = choiceParent.transform;    
        for(int i=0;i<wordCount/2;i++){
            randomNewPosIndex = random.Next(i, wordCount);
            choiceParentTr.GetChild(i).SetSiblingIndex(randomNewPosIndex);
        }
    }

    int correctWordPerQuest = 0;
    /*
    Jika benar kata:
    4. Menempatkan kata tersebut di urutan yang benar dan memainkan audio kata tersebut
    Jika sudah lengkap:
    5. Memainkan audio kalimat sambil menghighlight masing-masing kata
    Jika belum lengkap:
    5. Kembali menunggu (langkah 3)
    Jika salah kata:
    4. Mengembalikan kata tersebut ke tempat awal sambil menampilkan tanda salah dan memainkan audio salah.
    5. Kembali menunggu (langkah 3)
    Jika sudah kalimat terakhir:
    6. Menyimpan progress game
    7. Use case Melihat reward
    Jika bukan kalimat terakhir:
    6. Lanjut ke langkah 1 untuk kalimat selanjutnya.
    */
    protected override void correctAnswer(GameObject correctAnswer){
        RectTransform tempRT = correctAnswer.GetComponent<RectTransform>();
        Vector2 temp = Vector2.one * 0.5f;
        tempRT.anchorMax = temp;
        tempRT.anchorMin = temp;
        StartCoroutine(myUtilityClass.justHighlight(correctAnswer.GetComponent<Image>(), highlightCorrect, 1, 1f));
        correctWordPerQuest++;
        if(correctWordPerQuest==wordCount)
        {
            StartCoroutine(explainKalimatThenProceedForReward());
        }
    }

    GameObject ceklisGO;

    IEnumerator explainKalimatThenProceedForReward()
    {
        StartCoroutine(myUtilityClass.playAudioWhileHighlight(gambarPos, highlightDefault, UtilClass.Duration.MEDIUM));
        while (myUtilityClass.getAHState() == UtilClass.State.PLAYING)
        {
            yield return null;
        }

        correctWordPerQuest = 0;
        score++;
        // showCorrectSign();
        if (score == 1){
            ceklisGO = Instantiate(ceklisPrefab);
            ceklisGO.transform.SetParent(gambarPos.transform, false);
        } else{
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
    
    private void nextQuestion(){
        currentQAIndex++;
        if(myGameDetailKalimat.questions.Count>currentQAIndex){
            hideTemporary();
            choicesBack();
            StartCoroutine(waitNShowNext());
        } else {
            saveGameNShowEndingReward();
        }
    }

    
    IEnumerator waitNShowNext()
    {
        yield return new WaitForSeconds(2f);
        setImageNWords();
    }

    protected override void choicesBack()
    {
        Debug.Log("choices is BACK");

        for(int i=0;i<defaultChoices.Count;i++){
            defaultChoices[i].transform.SetParent(choiceParent.transform, false);
            if(wordCount>CURRENT_DEFAULT_WORD_COUNT){
                defaultChoices[i].GetComponent<RectTransform>().sizeDelta = defaultSize;
            }
        }
        
        Destroy(choiceCSF);
        parentHLG.enabled = false;

        if(wordCount>CURRENT_DEFAULT_WORD_COUNT){
            for(int i=0;i<(wordCount-CURRENT_DEFAULT_WORD_COUNT);i++){
                Destroy(additionalChoiceGOs[i]);
                Destroy(answerParentTr.GetChild(CURRENT_DEFAULT_WORD_COUNT+i).gameObject);
            }
            choicesDA.RemoveRange(CURRENT_DEFAULT_WORD_COUNT, wordCount-CURRENT_DEFAULT_WORD_COUNT);
            additionalChoiceGOs.Clear();
            additionalAnswerQuestionIndex = 2;
            defaultChoices[1].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            answerParentTr.GetComponent<RectTransform>().sizeDelta = defaultAnsParentSize;    
        }
    }

    protected override void hideTemporary(){
        choiceParent.SetActive(false);
        gambarPos.SetActive(false);
    }
    
    protected override void showFeedback(GameObject wrongAnswer){
        wrongAttemptMade();
        GameObject wrongContainer = wrongAnswer;
        Transform tempT;
        for(int i=0;i<wordCount;i++){
            tempT = answerParentTr.GetChild(i);
            if(tempT.GetComponent<DropSukuKata>().isWrong()){
                wrongContainer = tempT.gameObject;
                break;
            };
        }
        float highlightDuration = 1f;
        StartCoroutine(myUtilityClass.justHighlight(wrongContainer.GetComponent<Image>(), highlightWrong, 4, highlightDuration));
        StartCoroutine(myUtilityClass.justHighlight(wrongAnswer.GetComponent<Image>(), highlightWrong, 4, highlightDuration));
        StartCoroutine(myUtilityClass.playOnlyOneAudio(salahAud));
        base.showFeedback(null);
    }


}

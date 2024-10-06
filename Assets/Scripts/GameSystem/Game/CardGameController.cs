using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using ColorUtility = UnityEngine.ColorUtility;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CardGameController : GameplayController// , IGameMechanism
{
    const float waitTime = 1f;
    List<Color> colors = new List<Color>{
        Color.blue,
        Color.cyan,
        Color.magenta
    };

    List<string> colorString = new List<string>{
        "#FF9999",
        "#FFFF99",
        "#99FF99",
        "#99FFFF",
        "#CC99FF",
        "#FF99CC"
    };

    List<GameObject> myAlphaCards = new List<GameObject>();
    List<GameObject> myWordCards = new List<GameObject>();
    [SerializeField]
    GameObject myCardParent, alphaCardPrefab, wordCardPrefab;
    GameDetailCard myGameDetailCard;

    protected override void Awake(){
        base.Awake();
        myGameDetailCard = (GameDetailCard)myGameDetail;
    }

    /*
    1. Menampilkan beberapa pasangan kartu benda dengan huruf awalan
    2. Memainkan masing-masing audio dari benda dan huruf
        Misal: "Apel (highlight kartu apel) diawali dengan huruf a (highlight huruf a)"
    3. Mengacak kartu
    4. Memberikan instruksi "hafalkan posisi kartu dengan pasangan huruf awalnya" sambil menghihglight pasangan kartu
    5. Memberikan waktu beberapa detik
    6. Membalik / menutup kartu
    7. Memberikan instruksi untuk memilih salah satu kartu
    */
    protected override void startGame(){
        if(restart is false){
            setCardPrefab();
            setPartnerOfCards();
        } else {
            for(int i=0;i<myAlphaCards.Count;i++){
                setOrganizedPos(i);
            }
        }
        StartCoroutine(explainThenRandomizeNCover());
    }

    IEnumerator explainThenRandomizeNCover()
    {
        yield return new WaitForSeconds(waitTime);

        explainCoroutine = explain();
        StartCoroutine(explainCoroutine);
        skipButtonAnimator.SetBool(HIGHLIGHT, true);
        while(explainState==State.PLAYING){
            yield return null;
        }
        if(skipButton.isActiveAndEnabled){
            disableSkipButton();
        }

        yield return new WaitForSeconds(waitTime);

        randomizePosition();

        //hafalkan
        myCardParent.GetComponent<AudioSource>().clip = myGameDetailCard.instructions[1];
        StartCoroutine(myUtilityClass.playOnlyOneAudio(myCardParent.GetComponent<AudioSource>()));
        while (myUtilityClass.getAudioState() == UtilClass.State.PLAYING)
        {
            yield return null;
        }
        int colorIndex = 0;
        for (int i = 0; i < myGameDetailCard.partners.Count; i++)
        {
            float duration = 2f;
            StartCoroutine(myUtilityClass.highlightOnlyOneElement(myWordCards[i].GetComponent<Image>(), highlightDefault, 5, duration));
            StartCoroutine(myUtilityClass.highlightOnlyOneElement(myAlphaCards[i].GetComponent<Image>(), highlightDefault, 5, duration));
            while (myUtilityClass.getHighlightState() == UtilClass.State.PLAYING)
            {
                yield return null;
            }
            yield return new WaitForSeconds(waitTime);
            colorIndex++;
            if (colorIndex == 3)
            {
                colorIndex = 0;
            }
        }

        float waitToMemorize = 5f;
        yield return new WaitForSeconds(waitToMemorize);

        coverAllCards();

        playChooseFirstCardInstruction();
    }

    State explainState = State.IDLE;

    IEnumerator explain()
    {
        explainState = State.PLAYING;
        myCardParent.GetComponent<AudioSource>().clip = myGameDetailCard.instructions[0];
        int colorIndex = 0;
        for (int i = 0; i < myGameDetailCard.partners.Count; i++)
        {
            playingUtilCoroutine = myUtilityClass.playAudioWhileHighlight(myWordCards[i], highlightDefault, UtilClass.Duration.MEDIUM);
            StartCoroutine(playingUtilCoroutine);

            playingUtilCoroutine = myUtilityClass.playOnlyOneAudio(myCardParent.GetComponent<AudioSource>());
            StartCoroutine(playingUtilCoroutine);
            while (myUtilityClass.getAudioState() == UtilClass.State.PLAYING)
            {
                yield return null;
            }

            playingUtilCoroutine = myUtilityClass.playAudioWhileHighlight(myAlphaCards[i], highlightDefault, UtilClass.Duration.SHORT);
            StartCoroutine(playingUtilCoroutine);

            while (myUtilityClass.getAHState() == UtilClass.State.PLAYING)
            {
                yield return null;
            }

            colorIndex++;
            if (colorIndex == 3)
            {
                colorIndex = 0;
            }
        }
        explainState = State.IDLE;
    }

    private void setCardPrefab(){
        for(int i = 0;i<myGameDetailCard.partners.Count;i++){
            myAlphaCards.Add(Instantiate(alphaCardPrefab));
            myWordCards.Add(Instantiate(wordCardPrefab));
        }
    }

    Transform bottomT, topT;

    private void setPartnerOfCards(){
        List<GameDetailCard.Partner> partners = myGameDetailCard.partners;
        bottomT = myCardParent.transform.Find("Bottom");
        topT = myCardParent.transform.Find("Top");
        //Pasangan alphabet dan word
        int colorIndex = 0;
        Color color;
        for(int i=0;i<partners.Count;i++)
        {
            myAlphaCards[i].name = i.ToString();
            Image alpha = myAlphaCards[i].transform.Find("Alphabet").GetComponent<Image>();
            alpha.sprite = partners[i].alphabet.spriteAlpha;
            myAlphaCards[i].GetComponent<AudioSource>().clip = partners[i].alphabet.audioAlpha;
            cover(myAlphaCards[i].transform, false);

            myWordCards[i].name = i.ToString();
            Transform word = myWordCards[i].transform.Find("Word");
            word.GetComponentInChildren<Image>().sprite = partners[i].word.spriteWord;
            word.GetComponentInChildren<TextMeshProUGUI>().text = partners[i].word.teksWord;
            myWordCards[i].GetComponent<AudioSource>().clip = partners[i].word.audioWord;
            cover(myWordCards[i].transform, false);

            if(ColorUtility.TryParseHtmlString(colorString[i], out color)){
                myAlphaCards[i].GetComponent<Image>().color = color;
                myWordCards[i].GetComponent<Image>().color = color;
            } else {
                myAlphaCards[i].GetComponent<Image>().color = colors[colorIndex];
                myWordCards[i].GetComponent<Image>().color = colors[colorIndex];
            }

            setOrganizedPos(i);

            colorIndex++;
            if(colorIndex==3){
                colorIndex = 0;
            }
        }
        if(partners.Count>=6){
            topT.gameObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
            bottomT.gameObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        }
        if(partners.Count==0){
            topT.gameObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = false;
            bottomT.gameObject.GetComponent<HorizontalLayoutGroup>().childControlWidth = false;
        }
        showCards();
    }

    private void setOrganizedPos(int i)
    {
        myWordCards[i].transform.SetParent(topT, false);
        myAlphaCards[i].transform.SetParent(bottomT, false);
        
        myWordCards[i].transform.SetSiblingIndex(i);
        myAlphaCards[i].transform.SetSiblingIndex(i);
    }
    private void showCards(){
        myCardParent.transform.SetParent(aRFTransform, false);
    }

    private void randomizePosition(){
        partnerCount = myGameDetailCard.partners.Count;
        for(int i=0;i<partnerCount;i++){
            setRandomPosition(myAlphaCards[i]);
            setRandomPosition(myWordCards[i]);
        }
    }

    int top = 0, bottom = 0;
    int partnerCount;
    private void setRandomPosition(GameObject gO){
        int tempTopBottom = Random.Range(0,2);
        Debug.Log("TopBottom : " + top + bottom);
        Debug.Log("tempTopBottom : " + tempTopBottom);
        if(tempTopBottom==0){
            if(top==partnerCount){
                gO.transform.SetParent(bottomT, false);
                bottom++;
                Debug.Log("top already filled : " + top);
            } else{
                gO.transform.SetParent(topT, false);
                top++;
            }
        } else{
            if(bottom==partnerCount){
                gO.transform.SetParent(topT, false);
                top++;
                Debug.Log("bottom already filled : " + bottom);
            } else {
                gO.transform.SetParent(bottomT, false);
                bottom++;
            }
        }
        int tempHorizontal = Random.Range(0, partnerCount);
        gO.transform.SetSiblingIndex(tempHorizontal);
        if(top==partnerCount && bottom==partnerCount){
            top = 0;
            bottom = 0;
        }
    }





    private void playChooseFirstCardInstruction(){
        AudioSource myInstruction = myCardParent.GetComponent<AudioSource>();
        myInstruction.clip = myGameDetailCard.instructions[2];
        StartCoroutine(myUtilityClass.playOnlyOneAudio(myInstruction));
    }

    private void coverAllCards(){
        int i0, i1, i2, i3, i4, i5;
        for(int i=0;i<myGameDetailCard.partners.Count;i++){
            cover(myAlphaCards[i].transform, true);
            cover(myWordCards[i].transform, true);
            switch(i){
                case 0:
                    i0 = i;
                    myWordCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverWordCard(i0);});
                    myAlphaCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverAlphaCard(i0);});
                    break;
                case 1:
                    i1 = i;
                    myWordCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverWordCard(i1);});
                    myAlphaCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverAlphaCard(i1);});
                    break;
                case 2:
                    i2 = i;
                    myWordCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverWordCard(i2);});
                    myAlphaCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverAlphaCard(i2);});
                    break;
                case 3:
                    i3 = i;
                    myWordCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverWordCard(i3);});
                    myAlphaCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverAlphaCard(i3);});
                    break;
                case 4:
                    i4 = i;
                    myWordCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverWordCard(i4);});
                    myAlphaCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverAlphaCard(i4);});
                    break;
                case 5:
                    i5 = i;
                    myWordCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverWordCard(i5);});
                    myAlphaCards[i].GetComponent<Button>().onClick.AddListener(delegate{uncoverAlphaCard(i5);});
                    break;
            }
        }
    }

    List<GameObject> chosenCard = new List<GameObject>();
    
    /*
    8. Menampilkan dan memainkan audio kartu yang dipilih.
    9. Memberikan instruksi untuk memilih kartu pasangannya.
    11. Menampilkan dan memainkan audio kartu yang dipilih.
    12. Mengulang langkah 8 untuk kartu pertama tadi.
    */

    private void stopAudio(){
        if(myUtilityClass.getAudioState()==UtilClass.State.PLAYING){
            myUtilityClass.stopAudio();
        }
    }

    private void cover(Transform transformGO, bool cover){
        transformGO.Find("Cover").gameObject.SetActive(cover);
    }

    private void uncoverAlphaCard(int alphaIndex){
        alphaChosenCardInd = alphaIndex;
        Debug.Log("cgCtrl.uncoverAlphaCard with index " + alphaIndex.ToString());
        stopAudio();
        cover(myAlphaCards[alphaIndex].transform, false);
        StartCoroutine(myUtilityClass.playOnlyOneAudio(myAlphaCards[alphaIndex].GetComponent<AudioSource>()));
        chosenCard.Add(myAlphaCards[alphaIndex]);
        checkAmountCardToBeChecked();
    }

    private void uncoverWordCard(int wordIndex){
        wordChosenCardInd = wordIndex;
        Debug.Log("cgCtrl.uncoverWordCard with index " + wordIndex.ToString());
        stopAudio();
        cover(myWordCards[wordIndex].transform, false);
        StartCoroutine(myUtilityClass.playOnlyOneAudio(myWordCards[wordIndex].GetComponent<AudioSource>()));
        chosenCard.Add(myWordCards[wordIndex]);
        checkAmountCardToBeChecked();
    }

    private void checkAmountCardToBeChecked(){
        if(chosenCard.Count==2){
            enableOtherUnchosenCard(false);
            StartCoroutine(myUtilityClass.playOnlyOneAudio(chosenCard[0].GetComponent<AudioSource>()));
            correctOrWrong();
        }
        if(chosenCard.Count==1){
            playChooseSecondCardInstruction();
        }
    }

    private void enableOtherUnchosenCard(bool yON)
    {
        for(int i=0;i<myAlphaCards.Count;i++){
            if(i!=alphaChosenCardInd)
                myAlphaCards[i].GetComponent<Button>().interactable = yON;
            if(i!=wordChosenCardInd)
                myWordCards[i].GetComponent<Button>().interactable = yON;
        }
    }

    private void playChooseSecondCardInstruction(){
        AudioSource myInstruction = myCardParent.GetComponent<AudioSource>();
        myInstruction.clip = myGameDetailCard.instructions[3];
        StartCoroutine(myUtilityClass.playOnlyOneAudio(myInstruction));
    }

    /*
    Jika cocok(benar):
    13. Menampilkan ceklis pada kedua kartu dan memainkan audio benar.
    14. Menampilkan kartu tadi di bagian reward.
    Jika tidak cocok(salah):
    13. Menampilkan x (tanda salah) pada kedua kartu dan memainkan audio salah.
    14. Membalik / menutup kedua kartu
    15. Kembali ke 7.
    */
    private void correctOrWrong(){
        //to confirm
        if(chosenCard.Count==2){
            if(chosenCard[0].name.Equals(chosenCard[1].name)){
                removeButton();
                StartCoroutine(showCorrectHint());
                StartCoroutine(showRewardThenProceed());
            } else {
                StartCoroutine(showWrongHint());
                StartCoroutine(showFeedbackThenCoverAgain());
            }
        }
    }

     int wordChosenCardInd = 0, alphaChosenCardInd = 1;

    private void removeButton(){
        for(int i=0;i<chosenCard.Count;i++){
            chosenCard[i].GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
   
    protected override void restartGame(){
        restart = true;
        skipButton.gameObject.SetActive(true);
        skipButton.enabled = true;
        foreach(GameObject ceklis in ceklisGOs){
            ceklis.SetActive(false);
        }
        base.restartGame();
    }
    
    protected override void showReward(){
        for(int i=0;i<chosenCard.Count;i++){
            acquiredReward.Add(Instantiate(rewardPrefab));
            SetAtRewardList(acquiredReward[score*2-2+i], rewardListTemp);
        }
        if(acquiredReward.Count>=((score*2)-2+1)){
            acquiredReward[score*2-2].GetComponent<Image>().sprite = myAlphaCards[alphaChosenCardInd].transform.Find("Alphabet").GetComponent<Image>().sprite;
            acquiredReward[score*2-1].GetComponent<Image>().sprite = myWordCards[wordChosenCardInd].transform.Find("Word").GetComponentInChildren<Image>().sprite;
        } else {
            Debug.Log("Error: CGCtrl.showReward");
        }
        if(score==4){
            rewardListTempOrgSize = rewardListTemp.GetComponent<RectTransform>().sizeDelta;
            rewardListTemp.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.MinSize;
        }
    }
    List<GameObject> ceklisGOs = new List<GameObject>();

    IEnumerator showRewardThenProceed(){
        score++;
        // showCorrectSign();
        if(restart is false){
            for(int i=0;i<chosenCard.Count;i++){
                ceklisGOs.Add(Instantiate(ceklisPrefab));
                ceklisGOs[score*2-2+i].transform.SetParent(chosenCard[i].transform, false);
            }
        } else {
            for(int i=chosenCard.Count-2;i<chosenCard.Count;i++){
                chosenCard[i].transform.Find("Ceklis(Clone)").gameObject.SetActive(true);
            }
        }
        StartCoroutine(myUtilityClass.playOnlyOneAudio(ceklisGOs[0].GetComponent<AudioSource>()));
        while (myUtilityClass.getAudioState() == UtilClass.State.PLAYING)
        {
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);

        showReward();
        clear();
        enableOtherUnchosenCard(true);
        anyCardNext();
    }

    private void clear(){
        chosenCard.Clear();
    }

    /*
    Jika bukan kartu terakhir:
    15. Lanjut ke 7 lagi untuk pasangan kartu selanjutnya.
    Jika kartu terakhir:
    15. Menyimpan progress game
    16. Use case Melihat reward.
    */
    private void anyCardNext(){
        if((partnerCount-score)==0){
            saveGameNShowEndingReward();
        }
    }
    [SerializeField]
    GameObject silangPrefab;
    List<GameObject> silangGOs = new List<GameObject>();
    IEnumerator showFeedbackThenCoverAgain(){
        wrongAttemptMade();
        for(int i=0;i<chosenCard.Count;i++){
            silangGOs.Add(Instantiate(silangPrefab));
            silangGOs[i].transform.SetParent(chosenCard[i].transform, false);
            silangGOs[i].transform.SetAsLastSibling();
        }
        StartCoroutine(myUtilityClass.playOnlyOneAudio(silangGOs[0].GetComponent<AudioSource>()));
        while(myUtilityClass.getAudioState()==UtilClass.State.PLAYING){
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        
        foreach(GameObject silang in silangGOs){
            Destroy(silang);
        }
        silangGOs.Clear();
        
        coverAgain();

        clear();

        enableOtherUnchosenCard(true);
    }

    private void coverAgain(){
        foreach(GameObject card in chosenCard){
            cover(card.transform, true);
        }
    }
    private IEnumerator playingUtilCoroutine, explainCoroutine;
    [SerializeField]
    Animator skipButtonAnimator;
    [SerializeField]
    Button skipButton;

    const string HIGHLIGHT = "Highlight";

    public void SkipExplanation()
    {
        skipButtonAnimator.SetBool(HIGHLIGHT, false);
        disableSkipButton();
        StartCoroutine(checkNStopExplainRoutine());
    }

    private IEnumerator checkNStopExplainRoutine()
    {
        while(explainCoroutine==null){
            yield return null;
        }
        StopCoroutine(explainCoroutine);
        explainState = State.IDLE;
        if(playingUtilCoroutine!=null)
            StopCoroutine(playingUtilCoroutine);
        myUtilityClass.setAllStateIDLE();
    }

    private void disableSkipButton()
    {
        skipButton.enabled = false;
        skipButton.gameObject.SetActive(false);
    }
}

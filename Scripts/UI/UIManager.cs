using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
//*****************************************
//创建人： 马俊�? 
//功能说明：UI管理�?
//***************************************** 
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private float leftTime;
    private int maxcontentNum=4;
    private int currentBoardNum;
    private int currentCardPosID;
    public Text energyText;
    public Text leftTimeText;
    public Slider energySlider;
    private List<int> cardIDList=new List<int>();
    public GameObject cardGo;
    public Transform nextCardT;
    public Sprite[] cardSprites;
    public Sprite[] cardDisSprites;
    public Transform[] boardCardsT;// card cells Positions
    public Transform boardTrans;

    public GameObject winPanelGo;
    public GameObject losePanelGo;
    public GameObject startPanelGo;
    public GameObject endPanelGo;
    public GameObject maskPanelGo;
    public AudioClip winClip;
    public AudioClip loseClip;

    private void Awake()
    {
        Instance = this;
        leftTime = 180;
    }
    void Start()
    {
        CreateNewCard();
        DOTween.To(()=> Camera.main.orthographicSize, x=>Camera.main.orthographicSize=x, 9f, 2)
            .OnComplete(() => { startPanelGo.SetActive(false); });       
    }
    void Update()
    {
        DecreaseTime();
    }
    public void SetEnergySliderValue()
    {
        energyText.text = (GameController.Instance.energyValue).ToString("F1");
        energySlider.value = GameController.Instance.energyValue / 10;
    }
    private void DecreaseTime()
    {
        leftTime -= Time.deltaTime;
        int min = (int)leftTime / 60;
        int sec = (int)leftTime % 60;
        leftTimeText.text = min.ToString() + ":" + sec.ToString();
    }
    private void CreateNewCard()
    {
        if (currentBoardNum > maxcontentNum) return; // no longer instantiate a new card
        GameObject go = Instantiate(cardGo, nextCardT); // set card to nextCard position
        go.transform.localPosition = Vector3.zero; // set position 0 relative to parent position
 
        int randomNum=Random.Range(1,11); // card from 1 to 10
        while (cardIDList.Contains(randomNum))//if list contains it, recreate until a different one
        {
            randomNum = Random.Range(1, 11); //From 1 to 10
        }
        cardIDList.Add(randomNum);
        go.GetComponent<Card>().id = randomNum;

        // set card sprite when able
        Image image = go.transform.GetChild(0).GetComponent<Image>(); // child 0 is the button
        image.sprite = cardSprites[randomNum-1];

        // set card sprite when disable
        Button button = go.transform.GetChild(0).GetComponent<Button>();
        SpriteState ss = button.spriteState;
        ss.disabledSprite = cardDisSprites[randomNum - 1];
        button.spriteState = ss;

        // four cards will be moved to board
        if (currentBoardNum < maxcontentNum)
        {
            MoveCardToBoard(currentBoardNum);
        }
    }
    private void MoveCardToBoard(int posID)
    {
        Transform rt = nextCardT.GetChild(0);
        rt.SetParent(boardTrans);
        
        Card card = rt.GetComponent<Card>();
        card.posID = posID; // remember its posID so when used, next card will replenish this position
        rt.DOScale(Vector3.one * 1f, 0.2f); // scale up the card, default scale is 0.8
        rt.DOLocalMove(boardCardsT[posID].localPosition, 0.2f).OnComplete// move next card after this move is finished
            (() => 
            {
                currentBoardNum++;
                //rt.GetChild(0).GetComponent<Button>().interactable = true;
                card.SetInitPos();
                CreateNewCard();           
            }
            );
    }
    public void UseCard(int posID)// posID from 1 to 4
    {
        currentBoardNum--;
        MoveCardToBoard(posID);// move next card to the place we used last card
    }
    public void RemoveCardIDInList(int id)
    {
        cardIDList.Remove(id);
    }
    public void GameOver(bool win)
    {
        // gradually make camera farther away
        DOTween.To(() => Camera.main.orthographicSize, x => Camera.main.orthographicSize = x, 9f, 0.5f)
    .OnComplete(() => { OpenGameOverPanel(win); });        
    }
    private void OpenGameOverPanel(bool win)
    {
        Time.timeScale = 0; // stop timer it means stop the game
        endPanelGo.SetActive(true);
        if (win)
        {
            GameManager.Instance.PlayMusic(winClip);
            winPanelGo.SetActive(true);
        }
        else
        {
            GameManager.Instance.PlayMusic(loseClip);
            losePanelGo.SetActive(true);
        }
    }
    public void ShowOrHideMaskPanel(bool show)
    {
        maskPanelGo.SetActive(show);
        Invoke("CloseMaskPanel",0.5f);
    }
    private void CloseMaskPanel()
    {
        maskPanelGo.SetActive(false);
    }
    public void Replay()
    {
        Time.timeScale = 1;
        StopAllCoroutines();
        SceneManager.LoadScene(2);
    }
}

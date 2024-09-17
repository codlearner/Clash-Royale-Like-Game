using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
//*****************************************
//创建人： 马俊�? 
//功能说明�?
//***************************************** 
public class Card : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public int id; // this card's id
    public int posID; // this card's posID 1-4
    private bool isDraging; // draging the card or not
    private bool showCharacter; // show the character or card
    public Button button; // card button
    public Vector3 initPos; // card's initial position
    private Tweener tweener;
    public Transform btnCardT; // btn card Transform
    public GameObject characterShowGo; 
    public GameObject[] modelGos; // which Model of the character to show
    private Camera cam; // main camera
    public Text cardText; // name
    public GameObject magicCircleGo; // 
    public Transform imgEnergyT; // energy image
    public bool canCreateAnywhere;
    public AudioClip useCardSound;
    void Start()
    {
        cam = Camera.main;
        button.interactable = false;

        for (int i = 0; i < modelGos.Length ; i++)
        {
            modelGos[i].SetActive(false);
        }
        if (id<=8) // above 8 is magic card no model
        {
            modelGos[id - 1].SetActive(true);
            //animator = modelGos[id - 1].GetComponentInChildren<Animator>();
        }
        magicCircleGo.SetActive(false);
        canCreateAnywhere = GameController.Instance.unitInfos[id - 1].canCreateAnywhere;
    }

    void Update()
    {
        button.interactable = GameController.Instance.CanUse(id);
    }
    public void OnPointerEnter(PointerEventData eventData) // move up when pointer is on card
    {
        if (!button.interactable) return; // return when button is not active
        if (!isDraging) // useful when not draging card
        {
            tweener = transform.DOLocalMove(initPos + new Vector3(0, 50, 0), 0.1f);
        }
    }
    public void OnPointerExit(PointerEventData eventData) // move down when pointer leave card
    {
        if (!button.interactable) return;
        if (!isDraging)
        {
            tweener.Pause();
            transform.localPosition = initPos;
        }
    }
    public void SetInitPos()
    {
        initPos = transform.localPosition;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!button.interactable) return;
        tweener.Pause();
        isDraging = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!button.interactable) return;
        Vector2 mousePos;
        // transfer the screen pos of mouse to a specific object RectTransform (Emp_Board) local position so that this object will follow the mouse
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent.GetComponent<RectTransform>(), 
            Input.mousePosition, null, out mousePos);
        transform.localPosition = mousePos; 
        if (showCharacter) // showing model
        {
            // calculate the dis between init and now, make scale from 0 to 1 when distance from 250 to 500
            float scale = Mathf.Clamp((Vector3.Distance(transform.localPosition, initPos)-250) / 250, 0, 1);
            // change the scale of the model
            characterShowGo.transform.position = ScreenPointToWorldPoint(transform.position, 10f);
            characterShowGo.transform.localScale = Vector3.one * scale;
            if (characterShowGo.transform.localScale.x<=0f) // means character is invisible so show card
            {
                // nonmagic card
                showCharacter = false;
                btnCardT.gameObject.SetActive(true);
                characterShowGo.SetActive(false);
                cardText.gameObject.SetActive(false);
                if (id > 8) // id > 8 are all magic cards
                {
                    magicCircleGo.SetActive(false);
                }
            }
        }
        else // showing card
        {
            // calculate the dis between init and now, make scale from 1 to 0 when distance from 0 to 250
            float scale = Mathf.Clamp((250 - Vector3.Distance(transform.localPosition, initPos)) / 250, 0, 1);
            // change the scale of the card
            btnCardT.localScale = Vector3.one * scale;
            if (btnCardT.localScale.x<=0f) // means card is invisible so show character
            {
                // show character
                showCharacter = true;
                btnCardT.gameObject.SetActive(false);
                characterShowGo.SetActive(true);
                cardText.gameObject.SetActive(true);
                // show character's name below the card
                cardText.text = GameController.Instance.unitInfos[id - 1].unitName;
                if (id>8) // id > 8 are all magic cards
                {
                    magicCircleGo.SetActive(true);
                }
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!button.interactable) return;
        btnCardT.localScale = Vector3.one; // back to original scale (1)
        if (showCharacter)
        {
            imgEnergyT.gameObject.SetActive(true); // imgEnergy movement
            // detect if card is in available area
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits=Physics.RaycastAll(ray);
            if (!canCreateAnywhere && hits.Length > 0 && hits[0].collider.tag == "CantClick")
            {
                ReturnToInit();
                UIManager.Instance.ShowOrHideMaskPanel(true);
                return;
            }
            GameManager.Instance.PlaySound(useCardSound); // play use card sound
            // energy flow up
            imgEnergyT.DOLocalMove(imgEnergyT.localPosition + new Vector3(0, 50, 0),0.5f).OnComplete
            (
                () =>
                {
                    GameController.Instance.DecreaseEnergyValue(GameController.Instance.unitInfos[id-1].cost);
                    for (int i = 0; i < hits.Length; i++)
                    {
                        RaycastHit hit = hits[i];
                        if (hit.collider != null && hit.collider.tag == "Plane")
                        {
                            // create an unit on target position
                            Vector3 targetPos = hit.point;
                            GameController.Instance.CreateUnit(id, targetPos);
                            // Remove card from board and move next card to that place
                            UIManager.Instance.UseCard(posID);
                            // remove card id from list so it can be generated later
                            UIManager.Instance.RemoveCardIDInList(id);
                            // destory this card
                            Destroy(gameObject);
                        }
                    }
                }
            );
        }
        else
        {
            ReturnToInit(); // back to original state
        }
    }
    private void ReturnToInit()
    {
        characterShowGo.SetActive(false);
        btnCardT.gameObject.SetActive(true);
        cardText.gameObject.SetActive(false);
        imgEnergyT.gameObject.SetActive(false);
        tweener = transform.DOLocalMove(initPos, 0.2f).OnComplete(() => { isDraging = false; });
    }
    /// <summary>
    /// 屏幕坐标转换为世界坐�?
    /// </summary>
    /// <param name="screenPoint">屏幕坐标</param>
    /// <param name="planeZ">距离摄像�? Z 平面的距�?</param>
    /// <returns></returns>
    private Vector3 ScreenPointToWorldPoint(Vector2 screenPoint, float planeZ)
    {
        return cam.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, planeZ));
    }
}

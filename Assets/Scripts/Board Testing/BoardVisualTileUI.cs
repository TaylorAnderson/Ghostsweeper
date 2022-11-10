using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class BoardVisualTileUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Image mine;
    [SerializeField]
    private SuperTextMesh text;
    [SerializeField]
    private Image flag;
    [SerializeField]
    private Image bg;
    [SerializeField]
    private Image block;

    [SerializeField]
    private Image debug;
    public int gridX;
    public int gridY;
    private bool isRevealed;


    public UnityEvent<BoardVisualTileUI> onClicked;
    public UnityEvent<BoardVisualTileUI> onRightClicked;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRevealed(bool revealed) {
        block.gameObject.SetActive(!revealed);
    }
    public void SetMine(bool isMine) {
        mine.gameObject.SetActive(isMine); 
    }
    public void SetNumber(int num) {
        text.text = num.ToString();
        if (num == 0) {
            text.text = "";
        }
    }
    public void SetFlagged(bool flagged) {
        flag.gameObject.SetActive(flagged);
    }

    public void SetDebug(bool isDebug) {
        debug.gameObject.SetActive(isDebug);
    }

    public void OnPointerClick(PointerEventData data) {
        if (data.button == PointerEventData.InputButton.Left) {
            OnClicked();
        }   
        if (data.button == PointerEventData.InputButton.Right) {
            OnRightClicked();
        }
    }

    
    public void OnClicked() {
        onClicked.Invoke(this);
    }
    public void OnRightClicked() {
        onRightClicked.Invoke(this);
    }
}

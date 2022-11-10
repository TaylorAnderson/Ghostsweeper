using System.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoardVisualTile : MonoBehaviour
{
    public List<Sprite> numberSprites;
    public GameObject mist;
    public GameObject mistBurst;
    public GameObject mine;
    public SpriteRenderer numberSprite;
    public int value = 0;
    [HideInInspector] public int gridX = 0;
    [HideInInspector] public int gridY = 0;
    public bool currentlyRevealed = false;
    public UnityEvent<BoardVisualTile> OnCleared;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
    }

    public void SetRevealed(bool revealed) {
        if (revealed && !currentlyRevealed && mistBurst) {
            mistBurst.SetActive(true);
            mistBurst.GetComponent<Animator>().Play("FogExplode");
        }
        currentlyRevealed = revealed;
        mist.SetActive(!revealed);
    }
    public bool GetRevealed() {
        return currentlyRevealed;
    }
    public void SetValue(int value) {
        this.value = value;
        numberSprite.sprite = numberSprites[value-1];
    }

    public void SetValueShown(bool shown) {
        numberSprite.gameObject.SetActive(shown);
    }

    public void SetMine(bool hasMine) {
        this.mine.SetActive(hasMine && currentlyRevealed);
    }

    public void OnClear(Vector2 dir) {
        OnCleared.Invoke(this);

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistTile : MonoBehaviour
{
    public Sprite topLeft;
    public Sprite topRight;
    public Sprite bottomRight;
    public Sprite bottomLeft;
    public Sprite left;
    public Sprite right;
    public Sprite up;
    public Sprite down;
    public Sprite individual;
    public Sprite bottomLeftTopRight;
    public Sprite bottomRightTopLeft;
    public Sprite middle;
    public Sprite upEnd;
    public Sprite downEnd;
    public Sprite leftEnd;
    public Sprite rightEnd;
    public Sprite leftRight;
    public Sprite upDown;

    public Gradient colorGradient;

    public LayerMask mask;

    public BoardVisualTile boardTile;

    private SpriteRenderer spriteRenderer;

    private float colorOffset;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        colorOffset = Random.Range(0, 20);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVisual();

    }

    void UpdateVisual() {
        if (boardTile.currentlyRevealed) return;

        var checkLeft = Check(Vector2.left);
        var checkRight = Check(Vector2.right);
        var checkDown = Check(Vector2.down);
        var checkUp = Check(Vector2.up);
        var checkLeftDown = Check(Vector2.down + Vector2.left);
        var checkRightDown = Check(Vector2.down + Vector2.right);
        var checkLeftUp = Check(Vector2.up + Vector2.left);
        var checkRightUp = Check(Vector2.up + Vector2.right);

        spriteRenderer.sprite = null;

        if (checkRight && checkDown && !checkLeft && !checkUp) {
            spriteRenderer.sprite = topLeft;
        }
        if (checkLeft && checkDown && !checkRight && !checkUp) {
            spriteRenderer.sprite = topRight;
        }
        if (checkRight && checkUp && !checkLeft && !checkDown) {
            spriteRenderer.sprite = bottomLeft;
        }
        if (checkLeft && checkUp && !checkRight && !checkDown) {
            spriteRenderer.sprite = bottomRight;
        }

        if (checkLeft && !checkRight && checkUp && checkDown && checkLeftUp && checkLeftDown) {
            spriteRenderer.sprite = right;
        }
        if (checkRight && !checkLeft && checkUp && checkDown && checkRightUp && checkRightDown) {
            spriteRenderer.sprite = left;
        }
        if (checkUp && !checkDown && checkLeft && checkRight && checkLeftUp && checkRightUp) {
            spriteRenderer.sprite = down;
        }
        if (checkDown && !checkUp && checkLeft && checkRight && checkLeftDown && checkRightDown) {
            spriteRenderer.sprite = up;
        }

        if (checkUp && checkDown && !checkLeft && !checkRight) {
            spriteRenderer.sprite = upDown;
        }

        if (checkLeft && checkRight && !checkUp && !checkDown) {
            spriteRenderer.sprite = leftRight;
        }

        if (checkLeft && checkRight && checkUp && checkDown) {
            spriteRenderer.sprite = middle;
        }

        if (checkDown && !checkUp && !checkLeft && !checkRight) {
            spriteRenderer.sprite = upEnd;
        }

        if (checkLeft && !checkRight && !checkUp && !checkDown) {
            spriteRenderer.sprite = rightEnd;
        }

        if (checkRight && !checkLeft && !checkUp && !checkDown) {
            spriteRenderer.sprite = leftEnd;
        }

        if (checkUp && !checkRight && !checkDown && !checkLeft) {
            spriteRenderer.sprite = downEnd;
        }

        if (spriteRenderer.sprite == null) {
            spriteRenderer.sprite = individual;
        }


    }
    bool Check(Vector2 dir) {
        var hit = Physics2D.RaycastAll(transform.position, dir, 1, mask);
        return hit.Length > 1 ? true : false;
    }
}

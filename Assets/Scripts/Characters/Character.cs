using System.Linq.Expressions;
using System.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
public class Character : MonoBehaviour
{
    protected Vector2 dest;
    protected List<Vector2> dirs = new List<Vector2>() {
        Vector2.left, 
        Vector2.right,
        Vector2.up,
        Vector2.down
    };
    private float destAngle = 0;
    public LayerMask wallMask;
    public LayerMask mistMask;
    public float maxSightRange = 5;
    public float midSightRange = 3;
    public float minSightRange = 2;

    public UnityEvent<Character, Vector3> OnMove;
    // Start is called before the first frame update
    public virtual void Start()
    {
        dest = transform.position;
        destAngle = transform.eulerAngles.z;
        DoMoveLogic(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, dest, 0.3f);
    }
    public void OnMovePressed(InputAction.CallbackContext ctx) {
        if (ctx.started) {
          transform.position = dest;
            var move = ctx.ReadValue<Vector2>();
            var cardinalDir = GetCardinalDir(move);
            Vector2 dist = Vector2.zero;
            var wallHit = Physics2D.Raycast(dest, cardinalDir, 1, wallMask);
            if (wallHit) {
                //do nothing
            }
            else {
                dist = cardinalDir;
            }
            if (dist.magnitude > 0) {
              DoMoveLogic(dist + dest);
            }
            
        }
    }   
    protected virtual void DoMoveLogic(Vector2 moveDest) {
        var mistHit = Physics2D.Raycast(moveDest, Vector2.one, 0.1f, mistMask);
        if (mistHit) {
            var boardTile = mistHit.collider.GetComponentInParent<BoardVisualTile>();
            if (boardTile.mistDamaged) {
              boardTile.OnClear();
            }
            else {
              boardTile.mistDamaged = true;
              moveDest = transform.position;
            }
        }
        dest = moveDest;
        OnMove.Invoke(this, moveDest);
    }
    protected virtual void DoAbility() {

    }
    public void OnLook(InputAction.CallbackContext ctx) {
        if (ctx.started) {
            var cardinalDir = GetCardinalDir(ctx.ReadValue<Vector2>());
            destAngle = Mathf.Rad2Deg * Mathf.Atan2(cardinalDir.y, cardinalDir.x) - 90;
        }
    }
    public void OnUseAbility(InputAction.CallbackContext ctx) {
      if (ctx.performed) {
        DoAbility();
      }
    }

    TileBase CheckTile(Tilemap tilemap, Vector2 position) {
        var cellPos = BoardManager.instance.grid.WorldToCell(position);
        print(cellPos);
        var tile = tilemap.GetTile(cellPos);
        return tile;
    }

    private Vector2 GetCardinalDir(Vector2 vec) {
        Vector2 closestDir = Vector2.zero;
        float closestAngle = 1000;
        for (int i = 0; i < dirs.Count; i++) {
            var angleDiff = Vector2.Angle(dirs[i], vec);
            if (angleDiff < closestAngle) {
                closestAngle = angleDiff;
                closestDir = dirs[i];
            }
        }
        return closestDir;
    }
}

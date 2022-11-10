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
    private Vector2 dest;
    private List<Vector2> dirs = new List<Vector2>() {
        Vector2.left, 
        Vector2.right,
        Vector2.up,
        Vector2.down
    };
    private float destAngle = 0;
    public GameObject flashlight;
    public LayerMask wallMask;
    public LayerMask mistMask;
    public float maxSightRange = 5;
    public float midSightRange = 3;
    public float minSightRange = 2;
    public bool isMoving = false;

    public UnityEvent<Character, Vector3> OnMove;
    // Start is called before the first frame update
    void Start()
    {
        dest = transform.position;
        destAngle = transform.eulerAngles.z;
        OnMove.Invoke(this, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, dest, 0.1f);
        isMoving = Vector2.Distance(transform.position, dest) > 0.01f;
    }
    public void OnMovePressed(InputAction.CallbackContext ctx) {
        if (ctx.started) {
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

            Debug.DrawLine(dest, dest + dist, Color.red, 0.5f);
            var mistHit = Physics2D.Raycast(dest + dist, dist, dist.magnitude * 0.1f, mistMask);
            if (mistHit) {
                mistHit.collider.GetComponentInParent<BoardVisualTile>().OnClear(cardinalDir);
            }
            if (dist.magnitude > 0) {
                OnMove.Invoke(this, dest + dist);
            }
            dest += dist;
        }
    }   
    public void OnLook(InputAction.CallbackContext ctx) {
        if (ctx.started) {
            var cardinalDir = GetCardinalDir(ctx.ReadValue<Vector2>());
            destAngle = Mathf.Rad2Deg * Mathf.Atan2(cardinalDir.y, cardinalDir.x) - 90;
        }
    }
    public void OnUseAbility(InputAction.CallbackContext ctx) {

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

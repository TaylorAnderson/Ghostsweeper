using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TechieState {
  FLAGGING,
  MOVING
}
public class TechieCharacter : Character
{
    public int emfDist = 5;
    public GameObject flagPrefab;
    private TechieState state = TechieState.MOVING;
    private List<GameObject> currentFlags = new List<GameObject>();
    private List<GameObject> placedFlags = new List<GameObject>();

    public override void Start() {
        base.Start();
    }

    protected override void DoMoveLogic(Vector2 moveDest) {
        if (state == TechieState.MOVING) {
          base.DoMoveLogic(moveDest);
          BoardManager.instance.ShowNumberOnTiles(moveDest, emfDist);
        }
        if (state == TechieState.FLAGGING) {
          var mistHit = Physics2D.Raycast(moveDest, Vector2.one, 0.1f, mistMask);
          for (int i = 0; i < currentFlags.Count; i++) {
            var techFlag = currentFlags[i].GetComponent<TechieFlag>();
            var flagDir = (techFlag.dest - techFlag.basePos);
            var moveDir = (Vector3)moveDest - transform.position;
            
            if (flagDir != moveDir || !mistHit) {
              Destroy(currentFlags[i]);
            }
            else {
              GameObject foundPlacedFlag = null;
              for (int j = 0; j < placedFlags.Count; j++) {
                print(Vector3.Distance(placedFlags[j].transform.position, (techFlag.basePos + techFlag.dest)));
                if (placedFlags[j].transform.position == (techFlag.dest)) {
                  foundPlacedFlag = placedFlags[j];
                }
              }
              if (!foundPlacedFlag) { 
                placedFlags.Add(currentFlags[i]);
              }
              else {
                Destroy(foundPlacedFlag);
                placedFlags.Remove(foundPlacedFlag);
                Destroy(currentFlags[i]);
              }
            }
          }
          state = TechieState.MOVING;
        }
    }

    protected override void DoAbility() {
      if (state == TechieState.FLAGGING) return;
      transform.position = dest;
      currentFlags = new List<GameObject>();
      for (int i = 0; i < 4; i++) {
        var flag = Instantiate(flagPrefab);
        flag.transform.position = transform.position;
        flag.GetComponent<TechieFlag>().basePos = transform.position;
        flag.GetComponent<TechieFlag>().dest = transform.position + (Vector3)dirs[i];
        currentFlags.Add(flag);
      }
      state = TechieState.FLAGGING;
    }
}

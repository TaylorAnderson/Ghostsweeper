using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechieCharacter : Character
{
    public int emfDist = 5;

    public override void Start() {
        base.Start();
    }

    protected override void DoMoveLogic(Vector2 moveDest) {
        base.DoMoveLogic(moveDest);
        BoardManager.instance.ShowNumberOnTiles(moveDest, emfDist);
    }
}

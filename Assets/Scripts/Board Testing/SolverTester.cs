using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SolverTester : MonoBehaviour
{
    public int iterations = 10;
    public int w = 16;
    public int h = 30;
    public int mines = 99;
    private Board board = new Board();
    private BoardSolver solver = new BoardSolver();

    private float fails = 0;
    private float wins = 0;
    // Start is called before the first frame update
    void Start() {
        board.InitGrid(w, h);
        StartCoroutine(TestTimeToSolvable());
    }
    IEnumerator DoTest() {
        for (int i = 0; i < iterations; i++) {
            board.Reset();
            board.FillGridByMinesRandom(mines);
            solver.SolveBoard(board, this);
            if (board.IsSolved()) {
                Debug.Log("Board is solved!");
                wins+=1;
            }
            else {
                Debug.Log("Board failed!");
                fails+=1;
            }
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("Win percentage: " + ((wins / iterations) * 100) + "%");   
        Debug.Log("Loss percentage: " + ((fails / iterations) * 100) + "%");   
    }
    IEnumerator TestTimeToSolvable() {
        float totalTime = 0;
        for (int i = 0; i < iterations; i++) {
            var value = TestOneTime();
            totalTime += value;
            var span = TimeSpan.FromSeconds(value);
            Debug.Log("Found solution in: " + span.ToString("ss\\:ff"));
            yield return new WaitForEndOfFrame();
        }
        var avgSpan = TimeSpan.FromSeconds(totalTime / iterations);
        Debug.Log("Average timespan is: " + avgSpan.ToString("ss\\:ff"));
    }
    float TestOneTime() {
        var foundWin = false;
        var start = Time.realtimeSinceStartup;
        while (!foundWin) {
            board.Reset();
            board.FillGridByMinesRandom(mines);
            solver.SolveBoard(board, this);
            if (board.IsSolved()) {
                foundWin = true;
            }
        }
        return Time.realtimeSinceStartup - start;
    }
}

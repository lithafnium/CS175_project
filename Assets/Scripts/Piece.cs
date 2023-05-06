using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameObject tb; 
    float lastFall = 0;
    public bool isGamePiece; 

    public Vector2 roundPosition(Vector2 pos) {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public bool isOccupied(TetrisBoard board, Vector2 pos) {
        return board.grid[(int) pos.y, (int) pos.x] != null && 
                board.grid[(int) pos.y, (int) pos.x].parent != transform;
    }

    public bool isOwned(TetrisBoard board, int i, int j) {
        return board.grid[i, j] != null &&
                    board.grid[i, j].parent == transform;
    }

    public bool checkBoardPosition() {
        TetrisBoard board = this.tb.GetComponent<TetrisBoard>();
        foreach (Transform child in transform) {
            Vector2 pos = child.position; 
            pos = new Vector2(pos.x + 4.5f, pos.y); 
            pos = roundPosition(pos); 
            if (!board.insideGrid(pos) || isOccupied(board, pos)) {
                return false; 
            }
        }

        return true; 
    }

    public void movePiece() {
        TetrisBoard board = this.tb.GetComponent<TetrisBoard>();
        for (int i = 0; i < 20; i++) {
            for (int j = 0; j < 10; j++) {
                if (isOwned(board, i, j)) {
                    board.grid[i, j] = null; 
                }
            }
        }
        foreach (Transform child in transform) {
            Vector2 pos = child.position; 
            pos = new Vector2(pos.x + 4.5f, pos.y); 
            pos = roundPosition(pos);
            board.grid[(int) pos.y, (int) pos.x] = child; 
        }
    }



    private void moveTetromino(Vector3 direction) {
        transform.localPosition += direction;
        if (checkBoardPosition()) {
            movePiece();
        } else {
            transform.localPosition -= direction;
        }
    }

    private void rotateTetromino(float angle) {
        transform.RotateAround(transform.GetChild(0).position, new Vector3(0, 0, 1), angle);

        // transform.Rotate(new Vector3(0, 0, angle));
        if (checkBoardPosition()) {
            movePiece();
        } else {
            transform.RotateAround(transform.GetChild(0).position, new Vector3(0, 0, 1), -angle);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        this.tb = GameObject.Find("TetrisBoard");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!this.isGamePiece) {
            return; 
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            moveTetromino(new Vector3(-1, 0, 0));
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            moveTetromino(new Vector3(1, 0, 0));
        } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
            rotateTetromino(90);
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= 1) {                
            transform.localPosition += new Vector3(0, -1, 0);
            if (checkBoardPosition()) {
                // Debug.Log("CALLING UPDATE"); 
                movePiece();
            } else {
                transform.localPosition += new Vector3(0, 1, 0);
                
                TetrisBoard board = this.tb.GetComponent<TetrisBoard>(); // Getting the rigidbody from the player.
                board.updateGrid(); 

                board.spawnTetrominoe(false);
                enabled = false; 
            }


            lastFall = Time.time;
        }

    }

}

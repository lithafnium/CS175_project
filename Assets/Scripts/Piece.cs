using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameObject tb; 
    float lastFall = 0;
    public bool isGamePiece; 
    public int id; 

    public float arr;  // 1 second
    public float das; 
    public float timePassed;
    public float totalTimePassed; 
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

    public void moveToBottom(){
        // I don't know why this works and a while loop doesn't
        for(int i=0;i<20;i++){
            moveTetromino(new Vector3(0, -1, 0));
        }
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

    public void movePieceDown() {
        transform.localPosition += new Vector3(0, -1, 0);
        if (checkBoardPosition()) {
            // Debug.Log("CALLING UPDATE"); 
            movePiece();
        } else {
            transform.localPosition += new Vector3(0, 1, 0);
            
            Lock();
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
        this.arr = 0.025f; 
        this.das = 0.25f; 
        this.timePassed = 0f; 
        this.totalTimePassed = 0f; 
        this.tb = GameObject.Find("TetrisBoard");
    }

    void Lock()
    {
        TetrisBoard board = this.tb.GetComponent<TetrisBoard>(); // Getting the rigidbody from the player.
        board.updateGrid(); 

        board.spawnTetrominoe(false, null, -1, true);
        enabled = false; 
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        totalTimePassed += Time.deltaTime; 
        if (!this.isGamePiece) {
            return; 
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            totalTimePassed = 0;
            moveTetromino(new Vector3(-1, 0, 0));
        } else if (Input.GetKey(KeyCode.LeftArrow) && timePassed >= arr && totalTimePassed >= das) {
            moveTetromino(new Vector3(-1, 0, 0));
            timePassed = 0f;
        } 
        
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            totalTimePassed = 0;
            Debug.Log(Time.deltaTime);
            moveTetromino(new Vector3(1, 0, 0));
        } else if (Input.GetKey(KeyCode.RightArrow) && timePassed >= arr && totalTimePassed >= das) {
            moveTetromino(new Vector3(1, 0, 0));
            timePassed = 0f;
        } 
        
        // hi
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            rotateTetromino(90);
        } 
        
        if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= 1) {  
            totalTimePassed = 0;
            movePieceDown(); 
            
            lastFall = Time.time;
        } else if (Input.GetKey(KeyCode.DownArrow) && timePassed >= arr && totalTimePassed >= das) {
            movePieceDown(); 
            lastFall = Time.time;
            timePassed = 0f;
        } else if (Input.GetKeyDown(KeyCode.Space)){
            // Move the piece to the bottom of the board
            moveToBottom();
            Lock();
            

        } 

        if (Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.RightArrow) ||
            Input.GetKeyUp(KeyCode.DownArrow)) {
            totalTimePassed = 0; 
        }

    }

}

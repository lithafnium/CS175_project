using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    public Vector3 LEFT = new Vector3(-1, 0, 0); 
    public Vector3 RIGHT = new Vector3(1, 0, 0); 
    public Vector3 DOWN = new Vector3(0, -1, 0); 

    public GameObject tb; 
    float lastFall = 0;
    public bool isGamePiece; 
    public int id; 

    public float arr;
    public float das; 
    public float timePassed;
    public float totalTimePassed; 

    public int numRotations; 

    public Dictionary<int, Vector3[]> jlstzWallKicks = new Dictionary<int, Vector3[]>() {
        {1, new Vector3[]{new Vector3(0, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 1, 0), new Vector3(0, -2, 0), new Vector3(-1, -2, 0)}},
        {2, new Vector3[]{new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, -1, 0), new Vector3(0, 2, 0), new Vector3(1, 2, 0)}},
        {3, new Vector3[]{new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, -1, 0), new Vector3(0, 2, 0), new Vector3(1, 2, 0)}},
        {0, new Vector3[]{new Vector3(0, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 1, 0), new Vector3(0, -2, 0), new Vector3(-1, -2, 0)}}
    };

    public Dictionary<int, Vector3[]> iWallKicks = new Dictionary<int, Vector3[]>() {
        {1, new Vector3[]{new Vector3(0, 0, 0), new Vector3(-2, 0, 0), new Vector3(1, 0, 0), new Vector3(-2, -1, 0), new Vector3(1, 2, 0)}},
        {2, new Vector3[]{new Vector3(0, 0, 0), new Vector3(2, 0, 0), new Vector3(-1, 0, 0), new Vector3(2, 1, 0), new Vector3(-1, -2, 0)}},
        {3, new Vector3[]{new Vector3(0, 0, 0), new Vector3(-1, 0, 0), new Vector3(2, 0, 0), new Vector3(-1, 2, 0), new Vector3(2, -1, 0)}},
        {0, new Vector3[]{new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(-2, 0, 0), new Vector3(1, -2, 0), new Vector3(-2, 1, 0)}}
    };

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

    public bool checkBoardPosition(Vector3 direction) {
        TetrisBoard board = this.tb.GetComponent<TetrisBoard>();
        foreach (Transform child in transform) {
            Vector2 pos = child.position; 
            pos += new Vector2(direction.x, direction.y); 
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
        for(int i=0;i<22;i++){
            moveTetromino(DOWN);
        }
    }
    public void movePiece() {
        TetrisBoard board = this.tb.GetComponent<TetrisBoard>();
        for (int i = 0; i < 22; i++) {
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
        if (checkBoardPosition(DOWN)) {
            transform.localPosition += DOWN;
            movePiece();
        } else {
            movePiece(); 
            Lock();
        }
    }

    private void moveTetromino(Vector3 direction) {
        if (checkBoardPosition(direction)) {
            transform.localPosition += direction;
            movePiece();
        } 
    }

    private void rotateTetromino(float angle) {
        transform.RotateAround(transform.GetChild(0).position, new Vector3(0, 0, 1), angle);
        Vector3[] kicks;
        if (this.id == 6){
            kicks = iWallKicks[this.numRotations];
        }else {
            kicks = jlstzWallKicks[this.numRotations];
        }

        bool canMove = false; 
        
        foreach (Vector3 kick in kicks) {
            if (checkBoardPosition(kick)) {
                transform.position += kick; 
                movePiece(); 
                canMove = true; 
                break; 
            }
        }

        if (!canMove) {
            transform.RotateAround(transform.GetChild(0).position, new Vector3(0, 0, 1), -angle);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        this.arr = 0.025f; 
        this.das = 0.15f; 
        this.timePassed = 0f; 
        this.totalTimePassed = 0f; 
        this.numRotations = 0; 
        this.tb = GameObject.Find("TetrisBoard");
        TetrisBoard board = this.tb.GetComponent<TetrisBoard>(); // Getting the rigidbody from the player.

        if (this.isGamePiece){
            if (!checkBoardPosition(new Vector3(0, 0, 0))) {
                Debug.Log("GAME OVER");
                this.enabled = false;
                this.isGamePiece = false;
            }
        }
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
            moveTetromino(LEFT);
        } else if (Input.GetKey(KeyCode.LeftArrow) && timePassed >= arr && totalTimePassed >= das) {
            moveTetromino(LEFT);
            timePassed = 0f;
        } 
        
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            totalTimePassed = 0;
            moveTetromino(RIGHT);
        } else if (Input.GetKey(KeyCode.RightArrow) && timePassed >= arr && totalTimePassed >= das) {
            moveTetromino(RIGHT);
            timePassed = 0f;
        } 
        
        if (Input.GetKeyDown(KeyCode.UpArrow) && this.id != 3) {
            this.numRotations = (numRotations + 1) % 4; 
            rotateTetromino(-90);
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

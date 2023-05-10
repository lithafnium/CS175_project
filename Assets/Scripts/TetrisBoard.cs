using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBoard : MonoBehaviour
{
    public GameObject[] tetrominoes; 
    public GameObject[] outlineTetrominoes;
    public GameObject[] upcoming; 
    public int[] upcomingIds; 
    public GameObject hold; 
    public GameObject current; 
    public GameObject preview; 

    public static int w = 10; 
    public static int h = 22; 
    public Transform[,] grid = new Transform[h, w]; 

    public List<GameObject> spawned; 

    public bool insideGrid(Vector2 pos) {
        return pos.y >= 1.0f && pos.x >= 0 && pos.x <= 9;
    }

    public Vector2 roundPosition(Vector2 pos) {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    public void updateGrid() {
        List<int> rowsToDestroy = new List<int>();  
        for (int i = 0; i < 20; i++) {
            bool filled = true; 
            for (int j = 0; j < 10; j++) {
                if (grid[i, j] == null) {
                    filled = false; 
                    break;
                }
            }
            if (filled) {
                rowsToDestroy.Add(i); 
            }
        }
        int offset = 0;
        for (int i = 0; i < rowsToDestroy.Count; i++) {
            int row = rowsToDestroy[i] - offset; 
            for (int j = 0; j < w; j++) {
                var parent = grid[row, j].transform.parent.gameObject;
                Destroy(grid[row, j].gameObject); 
                grid[row, j] = null; 
            }

            // decrase all rows above i 
            for (int z = row + 1; z < h; z++) {
                for(int x = 0; x < w; x++) {
                    if (grid[z, x] != null) {
                        grid[z-1, x] = grid[z, x]; 
                        grid[z-1, x].position += new Vector3(0, -1, 0); 
                        grid[z, x] = null; 
                    }
                }
            }
            offset += 1; 
        }
    }

    public (GameObject obj, int index) getRandomPiece() {
        int index = Random.Range(0, tetrominoes.Length); 
        return (tetrominoes[index], index); 
    }

    public GameObject instantiatePiece(GameObject piece, bool isGamePiece, Vector3 pos, int id) {
        var piece_prefab = Instantiate(piece,
            pos,
            Quaternion.identity);
        
        var pieceObj = piece_prefab.GetComponent<Piece>(); 
        pieceObj.isGamePiece = isGamePiece;
        pieceObj.id = id; 

        return piece_prefab; 
    }

    public void drawUpcoming(bool drawNew) {
        for (int i = 0; i < 5; i++) {
            if (drawNew) {
                var (randomPiece, index) = getRandomPiece(); 
                GameObject piece_prefab = instantiatePiece(
                    randomPiece, false, new Vector3(9.5f, 18 + -3 * i, 0), index);
                this.upcoming[i] = piece_prefab; 
                this.upcomingIds[i] = index;
            } else {
                if (i == 0) {
                    Destroy(this.upcoming[0]);
                }
                else  {
                    // Destroy(this.upcoming[i - 1]);
                    this.upcoming[i-1] = this.upcoming[i]; 
                    this.upcomingIds[i-1] = this.upcomingIds[i];
                    this.upcoming[i - 1].transform.localPosition += new Vector3(0, 3, 0);
                }
            }
        }

        if (!drawNew) {
            // Destroy(this.upcoming[4]); 

            var (randomPiece, index) = getRandomPiece(); 
            GameObject piece_prefab = instantiatePiece(randomPiece, 
                false, 
                new Vector3(9.5f, 18 + -3 * 4, 0), index);

            this.upcoming[4] = piece_prefab; 
            this.upcomingIds[4] = index; 
        }
    }

    public int getLowestPossiblePosition(Piece piece){
        TetrisBoard board = this;
        Debug.Log("FINDING LOWEST POSSIBLE for "+piece);

        int startingY = (int) piece.transform.localPosition.y;
        for(int i=startingY-1;i>=0;i--){
            foreach (Transform child in piece.transform) {
                Vector2 pos = child.position; 
                pos = new Vector2(pos.x + 4.5f, i+(pos.y - piece.transform.localPosition.y)); 
                pos = piece.roundPosition(pos); 
                if (!board.insideGrid(pos) || piece.isOccupied(board, pos)) {
                    return (i+1);
                }
            }
        }
        return 1;
    }


    public void spawnTetrominoe(bool first, GameObject spawn, int id, bool drawUpcomingPiece) {

        if (spawn == null) {
            if (first) {
                var (piece, index) = getRandomPiece(); 
                spawn = piece; 
                id = index; 
            } else {
                spawn = this.upcoming[0]; 
                id = this.upcomingIds[0];
                // shiftUpcoming(); 
            }
        }
        Debug.Log("Spawning: " + spawn); 
        this.current = instantiatePiece(spawn, true, new Vector3(-0.5f, 18, 0), id);
        var pieceObj = this.current.GetComponent<Piece>();

        // Create shadow piece
        int previewY = getLowestPossiblePosition(pieceObj);
        this.preview = instantiatePiece(outlineTetrominoes[id], false, new Vector3(-0.5f, previewY, 0), id);
        pieceObj.shadow = this.preview;
        
        spawned.Add(this.current); 

        if (drawUpcomingPiece) drawUpcoming(false);

    }

    public void beginGame() {
        foreach (GameObject g in this.spawned) {
            Destroy(g); 
        }

        foreach (GameObject g in this.upcoming) {
            Destroy(g); 
        }

        Destroy(this.preview); 

        this.spawned = new List<GameObject>(); 
        this.upcoming = new GameObject[5];
        this.upcomingIds = new int[5]; 
        drawUpcoming(true);
        spawnTetrominoe(true, null, -1, true);
    }
    // Start is called before the first frame update
    void Start()
    {
        beginGame(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q)) {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            bool isNull = this.hold == null;

            GameObject piece = this.current;

            foreach (Transform child in piece.transform) {
                Vector2 pos = child.position; 
                pos = new Vector2(pos.x + 4.5f, pos.y); 
                pos = roundPosition(pos);
                grid[(int) pos.y, (int) pos.x] = null; 
            }

            GameObject newHold; 
            var pieceObj = piece.GetComponent<Piece>(); 

            int id = pieceObj.id; 

            if (id == 3) {
                newHold = instantiatePiece(tetrominoes[id], false, new Vector3(-12.25f, 14.5f, 0), id);
            } else if (id == 6) {
                newHold = instantiatePiece(tetrominoes[id], false, new Vector3(-12.25f, 15f, 0), id);
            } else {
                newHold = instantiatePiece(tetrominoes[id], false, new Vector3(-11.5f, 14.5f, 0), id);
            }

            
            if (!isNull) {
                // we want to swap here
                var holdObj = this.hold.GetComponent<Piece>(); 
                Destroy(this.hold); 
                Destroy(this.current); 
                Destroy(this.preview); 
                this.hold = newHold; 
                spawnTetrominoe(false, tetrominoes[holdObj.id], holdObj.id, false); 
                
            } else {
                this.hold = newHold; 
                Destroy(this.current); 
                Destroy(this.preview); 
                spawnTetrominoe(false, null, -1, true); 
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBoard : MonoBehaviour
{
    public GameObject[] tetrominoes; 
    public GameObject[] upcoming; 
    public int[] upcomingIds; 
    public GameObject hold; 
    public GameObject current; 

    public static int w = 10; 
    public static int h = 20; 
    public Transform[,] grid = new Transform[h, w]; 

    public bool insideGrid(Vector2 pos) {
        return pos.y >= 1.0f && pos.x >= 0 && pos.x <= 9;
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
            for (int j = 0; j < 10; j++) {
                Destroy(grid[row, j].gameObject); 
                grid[row, j] = null; 
            }

            // decrase all rows above i 
            for (int z = row + 1; z < 20; z++) {
                for(int x = 0; x < 10; x++) {
                    if (grid[z, x] != null) {
                        grid[z-1, x] = grid[z, x]; 
                        grid[z, x] = null; 
                        grid[z-1, x].position += new Vector3(0, -1, 0); 
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
        if (drawUpcomingPiece) drawUpcoming(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        this.upcoming = new GameObject[5];
        this.upcomingIds = new int[5]; 
        drawUpcoming(true);
        spawnTetrominoe(true, null, -1, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            bool isNull = this.hold == null;

            GameObject piece = this.current;
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

            Debug.Log("Holding: " + this.hold); 
            
            if (!isNull) {
                // we want to swap here
                var holdObj = this.hold.GetComponent<Piece>(); 
                Debug.Log("hold id: " + holdObj.id); 
                Destroy(this.hold); 
                Destroy(this.current); 

                this.hold = newHold; 
                spawnTetrominoe(false, tetrominoes[holdObj.id], holdObj.id, false); 
                
            } else {
                this.hold = newHold; 
                Destroy(this.current); 

                spawnTetrominoe(false, null, -1, true); 
            }
        }
    }
}

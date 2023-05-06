using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBoard : MonoBehaviour
{
    public GameObject[] tetrominoes; 
    public GameObject[] upcoming; 
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

    public GameObject getRandomPiece() {
        int index = Random.Range(0, tetrominoes.Length); 
        return tetrominoes[index]; 
    }

    public GameObject instantiatePiece(GameObject piece, bool isGamePiece, Vector3 pos) {
        var piece_prefab = Instantiate(piece,
            pos,
            Quaternion.identity);
        
        var pieceObj = piece_prefab.GetComponent<Piece>(); 
        pieceObj.isGamePiece = isGamePiece;

        return piece_prefab; 
    }

    public void drawUpcoming(bool drawNew) {
        for (int i = 0; i < 5; i++) {
            if (drawNew) {
                GameObject randomPiece = getRandomPiece(); 
                GameObject piece_prefab = instantiatePiece(randomPiece, false, new Vector3(9.5f, 18 + -3 * i, 0));
                this.upcoming[i] = piece_prefab; 
            } else {
                if (i == 0) {
                    Destroy(this.upcoming[0]);
                }
                else  {
                    // Destroy(this.upcoming[i - 1]);
                    this.upcoming[i-1] = this.upcoming[i]; 
                    Debug.Log("upcoming: " + this.upcoming[i]);
                    this.upcoming[i - 1].transform.localPosition += new Vector3(0, 3, 0);
                }
            }
        }

        if (!drawNew) {
            // Destroy(this.upcoming[4]); 

            GameObject randomPiece = getRandomPiece(); 
            GameObject piece_prefab = instantiatePiece(randomPiece, 
                false, 
                new Vector3(9.5f, 18 + -3 * 4, 0));

            this.upcoming[4] = piece_prefab; 
        }
    }

    public void spawnTetrominoe(bool first) {
        GameObject spawn; 
        if (first) {
            spawn = getRandomPiece(); 
        } else {
            spawn = this.upcoming[0]; 
            // shiftUpcoming(); 
        }
        
        Debug.Log("Spawning: " + spawn); 
        instantiatePiece(spawn, true, new Vector3(-0.5f, 18, 0));
        drawUpcoming(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        this.upcoming = new GameObject[5];
        drawUpcoming(true);
        spawnTetrominoe(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

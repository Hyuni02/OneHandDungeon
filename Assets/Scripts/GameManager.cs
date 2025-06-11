using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Field")]
    public Vector2Int fieldSize = Vector2Int.zero;
    public int enemyCount;
    private Cell[,] field;

    private void Start() {
        CreateField();
    }

    private void CreateField() {
        field = new Cell[fieldSize.x, fieldSize.y];
        PlaceEnemy();
    }

    private void PlaceEnemy() {
        List<Vector2> placed = new List<Vector2>();
    
        for (int i = 0; i < enemyCount; i++) {
            while (true) {
                Vector2Int xy = new Vector2Int(Random.Range(0, fieldSize.x - 1), Random.Range(0, fieldSize.y - 1));
                if (placed.Contains(xy)) continue;
                placed.Add(xy);
                field[xy.x, xy.y].enemy = true;
                break;
            }
        }
        
        Visualizer.instance.VisualizeField(field);
    }
}

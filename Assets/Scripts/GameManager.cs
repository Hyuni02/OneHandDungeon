using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [Header("Field")]
    public Vector2Int fieldSize = Vector2Int.zero;
    public int enemyCount;
    private Cell[,] field;

    [Header("Entity")]
    public Player player;
    public List<Entity> lst_entity = new List<Entity>();

    private void Start() {
        CreateField();
    }

    private void CreateField() {
        field = new Cell[fieldSize.x, fieldSize.y];

        List<Vector2Int> placed = new List<Vector2Int>();
        Vector2Int xy = Vector2Int.zero;

        //Spawn Enemies
        for (int i = 0; i < enemyCount; i++) {
            SelectCell(out xy, ref placed);
            GameObject enemy = Instantiate(PrefabContainer.instance.pref_chicken);
            lst_entity.Add(enemy.GetComponent<Entity>());
            field[xy.x, xy.y].entity = enemy;
        }

        //Spawn Player
        SelectCell(out xy, ref placed);
        GameObject _player = Instantiate(PrefabContainer.instance.pref_player);
        player = _player.GetComponent<Player>();
        field[xy.x, xy.y].entity = _player;

        //Spanw Items

        Visualizer.instance.VisualizeField(field);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Move(player.gameObject, player.range);
        }
    }

    private void Move(GameObject target, int range = 1) {
        Vector2Int newPos;
        Vector2Int curPos = FindPos(target);

        do {
            newPos = new Vector2Int(curPos.x + Random.Range(-range, range + 1), curPos.y + Random.Range(-range, range + 1));
        } while (!IsValidPos(newPos));
        
        print($"from {curPos.x}, {curPos.y}\nto {newPos.x}, {newPos.y}");

        field[newPos.x, newPos.y].entity = target;
        field[curPos.x, curPos.y].entity = null;

        Visualizer.instance.VisualizeField(field);
    }

    private bool IsValidPos(Vector2Int targetPos) {
        bool valid = targetPos.x >= 0
            && targetPos.x < field.GetLength(1)
            && targetPos.y >= 0
            && targetPos.y < field.GetLength(0)
            && !field[targetPos.x, targetPos.y].entity;
        return valid;
    }

    private Vector2Int FindPos(GameObject target) {
        for (int x = 0; x < field.GetLength(0); x++) {
            for (int y = 0; y < field.GetLength(1); y++) {
                if (field[x, y].entity == target) {
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.LogError($"Can't find target : {target.name}");
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// placed에 없는 x y 값을 생성
    /// </summary>
    /// <param name="xy">새로생성 된 좌표 값</param>
    /// <param name="placed">중복 좌표를 저장하는 리스트</param>
    private void SelectCell(out Vector2Int xy, ref List<Vector2Int> placed) {
        while (true) {
            xy = new Vector2Int(Random.Range(0, fieldSize.x - 1), Random.Range(0, fieldSize.y - 1));
            if (placed.Contains(xy)) continue;
            placed.Add(xy);
            break;
        }
    }
}

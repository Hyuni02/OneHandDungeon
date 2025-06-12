using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    
    [Header("Field")]
    public Vector2Int fieldSize = Vector2Int.zero;
    public int enemyCount;
    public int itemCount;
    private Cell[,] field;

    [Header("Entity")]
    public Player player;
    public List<Entity> lst_entity = new List<Entity>();
    public List<Obj> lst_obj = new List<Obj>();

    [Header("UI")]
    public Button btn_move;
    public Button btn_attack;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    
    private void Start() {
        btn_attack.interactable = false;
        
        CreateField();
    }

    private void CreateField() {
        field = new Cell[fieldSize.x, fieldSize.y];
        for (int x = 0; x < fieldSize.x; x++) {
            for (int y = 0; y < fieldSize.y; y++) {
                field[x, y] = new Cell();
            }
        }

        List<Vector2Int> placed = new List<Vector2Int>();
        Vector2Int xy;

        //Spawn Enemies
        for (int i = 0; i < enemyCount; i++) {
            SelectCell(out xy, ref placed);
            Entity enemy = new Animal("chicken");
            lst_entity.Add(enemy);
            PlaceEntity(xy, enemy);
        }

        //Spawn Player
        SelectCell(out xy, ref placed);
        Player _player = new Player("Player");
        player = _player;
        PlaceEntity(xy, _player);

        //Spawn Items
        for (int i = 0; i < itemCount; i++) {
            SelectCell(out xy, ref placed); 
            Obj item = new Obj("chicken");
            lst_obj.Add(item);
            Obj[] items = new[] { item };
            PlaceItem(xy, items);
        }

        Visualizer.instance.VisualizeField(field);
    }

    public void PlaceEntity(Vector2Int pos, Entity entity) {
        field[pos.x, pos.y].entity = entity;
    }

    public void PlaceItem(Vector2Int pos, Obj[] items) {
        field[pos.x, pos.y].obj.AddRange(items);
    }

    public void Move_Player() {
        Move(player, player.range);
        FindNearBy();
    }

    public void Attack_Player() {
        Attack(player, player.lst_nearEntity[0]);
    }

    private void Attack(Entity from, Entity to) {
        from.Attack(to);
        Visualizer.instance.VisualizeField(field);
    }

    private void Move(Entity target, int range = 1) {
        Vector2Int newPos;
        Vector2Int curPos = FindPos(target);

        do {
            newPos = new Vector2Int(curPos.x + Random.Range(-range, range + 1), curPos.y + Random.Range(-range, range + 1));
        } while (!IsValidPos(newPos, true));

        // print($"from {curPos.x}, {curPos.y}\nto {newPos.x}, {newPos.y}");

        field[newPos.x, newPos.y].entity = target;
        field[curPos.x, curPos.y].entity = null;

        Visualizer.instance.VisualizeField(field);
    }

    private void FindNearBy() {
        Vector2Int playerPos = FindPos(player);
        player.lst_nearEntity.Clear();
        player.lst_nearObejct.Clear();
        btn_attack.interactable = false;
        for (int x = playerPos.x - player.range; x <= playerPos.x + player.range; x++) {
            for (int y = playerPos.y - player.range; y <= playerPos.y + player.range; y++) {
                Vector2Int newPos = new Vector2Int(x, y);
                if (!IsValidPos(newPos)) continue;
                if (newPos == playerPos) continue;
                if (field[x, y].entity != null) {
                    player.lst_nearEntity.Add(field[x, y].entity);
                    btn_attack.interactable = true;
                }
                if (field[x, y].obj != null) {
                    player.lst_nearObejct.AddRange(field[x, y].obj);
                }
            }
        }

        // print(player.lst_nearEntity.Count);
    }

    private bool IsValidPos(Vector2Int targetPos, bool checkEntity = false) {
        if (targetPos.x < 0 || targetPos.x >= field.GetLength(1) ||
            targetPos.y < 0 || targetPos.y >= field.GetLength(0))
            return false;

        return !checkEntity || field[targetPos.x, targetPos.y].entity == null;
    }

    public Vector2Int FindPos(Entity target) {
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

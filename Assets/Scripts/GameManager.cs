using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Button btn_pick;
    public Button btn_search;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    
    private void Start() {
        //init buttons
        btn_move.gameObject.SetActive(true);
        btn_search.gameObject.SetActive(true);
        btn_attack.gameObject.SetActive(false);
        btn_pick.gameObject.SetActive(false);
        
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
            Obj item = new Obj("Meat");
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
        player.target = FindNearBy(player, true);
        ShowNearBy(player.target);
    }

    public void Attack_Player() {
        Attack(player, (Entity)player.target);
    }

    public void Pick_Player() {
        Pick(player, (Obj)player.target);
    }

    public void Search_Player() {
        player.target = FindNearBy(player, true);
        ShowNearBy(player.target);
    }

    private void Pick(Entity from, Obj item) {
        print($"{from.name} - Pick : {item.name}");
        from.inventory.Add(item);
        Vector2Int itemPos = FindPos(item);
        field[itemPos.x, itemPos.y].obj.Remove(item);
        ShowNearBy(FindNearBy(from));
        Visualizer.instance.VisualizeField(field);
    }

    private void Attack(Entity from, Entity to) {
        print($"{from.name} - Attack : {to.name}");
        from.Attack(to);
        Visualizer.instance.VisualizeField(field);
    }

    private void Move(Entity target, int range = 1) {
        Vector2Int newPos;
        Vector2Int curPos = FindPos(target);

        do {
            newPos = new Vector2Int(curPos.x + Random.Range(-range, range + 1), curPos.y + Random.Range(-range, range + 1));
        } while (!IsValidPos(newPos, true));

        field[newPos.x, newPos.y].entity = target;
        field[curPos.x, curPos.y].entity = null;

        Visualizer.instance.VisualizeField(field);
    }

    public void ShowNearBy(object target) {
        switch (target) {
            case null:
                btn_attack.gameObject.SetActive(false);
                btn_pick.gameObject.SetActive(false);
                return;
            case Entity entity:
                print($"Entity : {entity.name}");
                btn_attack.gameObject.SetActive(true);
                btn_pick.gameObject.SetActive(false);
                break;
            case Obj obj:
                print($"Obj : {obj.name}");
                btn_attack.gameObject.SetActive(false);
                btn_pick.gameObject.SetActive(true);
                break;
        }

    }
    
    public object FindNearBy(Entity from, bool select = false) {
        Vector2Int pos = FindPos(from);
        from.lst_nearEntity.Clear();
        from.lst_nearObejct.Clear();
        for (int x = pos.x - from.range; x <= pos.x + from.range; x++) {
            for (int y = pos.y - from.range; y <= pos.y + from.range; y++) {
                Vector2Int newPos = new Vector2Int(x, y);
                if (!IsValidPos(newPos)) continue;
                if (newPos == pos) continue;
                if (field[x, y].entity != null) {
                    from.lst_nearEntity.Add(field[x, y].entity);
                }
                if (field[x, y].obj != null) {
                    from.lst_nearObejct.AddRange(field[x, y].obj);
                }
            }
        }

        if (from.lst_nearEntity.Count == 0 && from.lst_nearObejct.Count == 0) return null;

        if (select) {
            int totalCount = from.lst_nearEntity.Count + from.lst_nearObejct.Count;
            int index = Random.Range(0, totalCount);

            if (index < from.lst_nearEntity.Count) {
                return from.lst_nearEntity[index];
            }
            else {
                return from.lst_nearObejct[index - from.lst_nearEntity.Count];
            }
        }
        else {
            return null;
        }
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

        Debug.LogError($"Can't find entity : {target.name}");
        return new Vector2Int(-1, -1);
    }

    public Vector2Int FindPos(Obj target) {
        for (int x = 0; x < field.GetLength(0); x++) {
            for (int y = 0; y < field.GetLength(1); y++) {
                if (field[x, y].obj.Contains(target)) {
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.LogError($"Can't find obj : {target.name}");
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

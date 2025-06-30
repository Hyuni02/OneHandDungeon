using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    [Header("Field")]
    public int depth = 1;
    public int fieldSize = 0;
    public int enemyCount;
    public int itemCount;
    private Cell[,] field;
    private bool knowExit = false;

    [Header("Entity")]
    public Player player;
    public List<Entity> lst_entity = new List<Entity>();
    public List<Obj> lst_obj = new List<Obj>();

    [Header("UI")]
    public Button btn_move;
    public Button btn_attack;
    public Button btn_pick;
    public Button btn_search;
    public Button btn_open;
    public Button btn_exit;
    public Button btn_toexit;
    public GameObject pnl_container;
    public Transform content_container;

    [Header("UI")]
    public TMP_Text target;

    [Header("Prefab")]
    public GameObject prefab_btn_item;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        
        
    }

    private void Start() {
        init(1);
    }

    private void init(int _depth) {
        print(_depth + "층 진입");
        //init buttons
        btn_move.gameObject.SetActive(true);
        btn_search.gameObject.SetActive(true);
        knowExit = false;

        SetUI();

        CreateField();
    }

    private void CreateField() {
        //필드 생성
        field = new Cell[fieldSize, fieldSize];
        for (int x = 0; x < fieldSize; x++) {
            for (int y = 0; y < fieldSize; y++) {
                field[x, y] = new Cell();
            }
        }

        List<Vector2Int> placed = new List<Vector2Int>();
        Vector2Int xy;

        //Spawn Player - (0,0)
        xy = new Vector2Int(0, 0);
        placed.Add(xy);
        if (player == null) {
            Nicky _player = new Nicky("Nicky");
            player = _player;
            PlaceEntity(xy, _player);
        }
        else {
            PlaceEntity(xy, player);
        }

        //Spawn Exit - (n,n)
        xy = new Vector2Int(fieldSize - 1, fieldSize - 1);
        placed.Add(xy);
        field[xy.x, xy.y].floor.type = FloorType.exit;

        //Spawn Enemies
        for (int i = 0; i < enemyCount; i++) {
            SelectCell(out xy, ref placed);
            Entity enemy = new Animal("chicken");
            lst_entity.Add(enemy);
            PlaceEntity(xy, enemy);
        }

        //Spawn Items
        for (int i = 0; i < itemCount; i++) {
            SelectCell(out xy, ref placed);
            Item item = new Item("Meat");
            lst_obj.Add(item);
            PlaceObj(xy, item);
        }

        Visualizer.instance.VisualizeField(field);
    }

    public void PlaceEntity(Vector2Int pos, Entity entity) {
        field[pos.x, pos.y].entity = entity;
    }

    public void PlaceObj(Vector2Int pos, Obj[] objs) {
        field[pos.x, pos.y].obj.AddRange(objs);
    }

    public void PlaceObj(Vector2Int pos, Obj obj) {
        field[pos.x, pos.y].obj.Add(obj);
    }

    public void Move_Player() {
        if (pnl_container.activeSelf) {
            Close_Player();
        }
        
        Move(player, player.range);
        player.target = FindNearBy(player, true);
        ShowNearBy(player.target);
    }

    public void Open_Player() {
        Body body = (Body)player.target;
        
        pnl_container.SetActive(true);
        btn_open.interactable = false;
        
        foreach (var item in body.content) {
            GameObject obj = Instantiate(prefab_btn_item, content_container);
            obj.GetComponent<btn_item>().init(body, item);
        }
    }

    public void Close_Player() {
        ClearChild(content_container);
        pnl_container.SetActive(false);
        btn_open.interactable = true;
    }

    public void Attack_Player() {
        Attack(player, (Entity)player.target);
    }

    public void Pick_Player() {
        Pick(player, (Item)player.target);
    }

    public void Search_Player() {
        if (pnl_container.activeSelf) {
            Close_Player();
        }
        
        player.target = FindNearBy(player, true);
        ShowNearBy(player.target);
    }

    public void Exit_Player() {
        depth++;
        init(depth);
    }

    public void ToExit_Player() {
        if (pnl_container.activeSelf) {
            Close_Player();
        }
        
        Vector2Int newPos;
        Vector2Int curPos = FindPos(player);

        do {
            newPos = new Vector2Int(fieldSize - 1 + Random.Range(-1, 2), fieldSize - 1 + Random.Range(-1, 2));
        } while (!IsValidPos(newPos, true));

        field[newPos.x, newPos.y].entity = player;
        field[curPos.x, curPos.y].entity = null;

        Visualizer.instance.VisualizeField(field);

        player.target = FindNearBy(player, true);
        ShowNearBy(player.target);
    }

    private void Pick(Entity from, Item item) {
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

    private void Move(Entity _target, int range = 1) {
        Vector2Int newPos;
        Vector2Int curPos = FindPos(_target);

        do {
            newPos = new Vector2Int(curPos.x + Random.Range(-range, range + 1), curPos.y + Random.Range(-range, range + 1));
        } while (!IsValidPos(newPos, true));

        field[newPos.x, newPos.y].entity = _target;
        field[curPos.x, curPos.y].entity = null;

        Visualizer.instance.VisualizeField(field);
    }

    private void SetUI(string type = null, string text = null) {
        target.transform.parent.gameObject.SetActive(type != null);
        btn_attack.gameObject.SetActive(type == "attack");
        btn_pick.gameObject.SetActive(type == "pick");
        btn_exit.gameObject.SetActive(type == "exit");
        btn_open.gameObject.SetActive(type == "open");
        btn_toexit.gameObject.SetActive(knowExit);
        pnl_container.SetActive(type == "container");
        target.text = text;
    }

    public void ShowNearBy(object _target) {
        switch (_target) {
            case null:
                SetUI();
                return;
            case Entity entity:
                print($"Entity : {entity.name}");
                SetUI("attack", entity.name);
                break;
            case Body body:
                print($"Body : {body.name}");
                SetUI("open", body.name);
                break;
            case Obj obj:
                print($"Obj : {obj.name}");
                SetUI("pick", obj.name);
                break;
            case Floor floor:
                print($"Find Exit");
                knowExit = false;
                SetUI("exit", "Exit");
                knowExit = true;
                break;
        }
    }

    public object FindNearBy(Entity from, bool select = false) {
        Vector2Int pos = FindPos(from);
        from.lst_nearEntity.Clear();
        from.lst_nearObejct.Clear();
        Floor exit = null;
        for (int x = pos.x - from.range; x <= pos.x + from.range; x++) {
            for (int y = pos.y - from.range; y <= pos.y + from.range; y++) {
                Vector2Int newPos = new Vector2Int(x, y);
                if (!IsValidPos(newPos)) continue;
                if (newPos != pos) {
                    if (field[x, y].entity != null) {
                        from.lst_nearEntity.Add(field[x, y].entity);
                    }
                }
                if (field[x, y].obj != null) {
                    from.lst_nearObejct.AddRange(field[x, y].obj);
                }
                if (field[x, y].floor.type == FloorType.exit) {
                    //탈출구에 처음 접근하면 무조건 발견
                    if (!knowExit) {
                        return field[x, y].floor;
                    }
                    exit = field[x, y].floor;
                }
            }
        }

        if (from.lst_nearEntity.Count == 0 && from.lst_nearObejct.Count == 0 && exit == null) return null;

        if (select) {
            int totalCount = from.lst_nearEntity.Count + from.lst_nearObejct.Count + (exit != null ? 1 : 0);
            int index = Random.Range(0, totalCount);

            if (index < from.lst_nearEntity.Count) {
                return from.lst_nearEntity[index];
            }
            else if (exit != null && index == totalCount - 1) {  // exit은 마지막 인덱스
                return exit;
            }
            else {
                int objectIndex = index - from.lst_nearEntity.Count;
                if (exit != null) objectIndex--;  // exit이 있으면 object 인덱스를 1 감소
                return from.lst_nearObejct[objectIndex];
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

    public Vector2Int FindPos(Entity _target) {
        for (int x = 0; x < field.GetLength(0); x++) {
            for (int y = 0; y < field.GetLength(1); y++) {
                if (field[x, y].entity == _target) {
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.LogError($"Can't find entity : {_target.name}");
        return new Vector2Int(-1, -1);
    }

    private Vector2Int FindPos(Obj _target) {
        for (int x = 0; x < field.GetLength(0); x++) {
            for (int y = 0; y < field.GetLength(1); y++) {
                if (field[x, y].obj.Contains(_target)) {
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.LogError($"Can't find obj : {_target.name}");
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// placed에 없는 x y 값을 생성
    /// </summary>
    /// <param name="xy">새로생성 된 좌표 값</param>
    /// <param name="placed">중복 좌표를 저장하는 리스트</param>
    private void SelectCell(out Vector2Int xy, ref List<Vector2Int> placed) {
        while (true) {
            xy = new Vector2Int(Random.Range(0, fieldSize - 1), Random.Range(0, fieldSize - 1));
            if (placed.Contains(xy)) continue;
            placed.Add(xy);
            break;
        }
    }

    private void ClearChild(Transform parent){
        for(int i = parent.childCount - 1; i>=0; i--){
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public static bool meet = false; 
    
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

    [Header("UI Button")]
    public Button btn_move;
    public Button btn_attack;
    public Button btn_pick;
    public Button btn_search;
    public Button btn_opencontainer;
    public Button btn_exit;
    public Button btn_toexit;
    public Button btn_inventory;

    [Header("Container")]
    public GameObject pnl_inventory;
    public GameObject pnl_container;
    public Transform content_container;
    public Transform content_inventory;

    [Header("Popup")]
    public GameObject pnl_popup;
    public Transform content_popup;

    [Header("UI")]
    public TMP_Text target;

    [Header("Prefab")]
    public GameObject prefab_btn_item;
    public GameObject prefab_popupaction;
    
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
            Entity enemy = new Animal("dog");
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

    public void UI_Move() {
        if (pnl_container.activeSelf) {
            UI_CloseContainer();
        }
        
        Move(player, player.range);
        player.target = FindNearBy(player, true);
        ShowNearBy(player.target);
    }

    public void UI_OpenContainer() {
        Body body = (Body)player.target;
        
        pnl_container.SetActive(true);
        btn_opencontainer.interactable = false;
        
        foreach (var item in body.content) {
            GameObject obj = Instantiate(prefab_btn_item, content_container);
            obj.GetComponent<btn_item>().init(body, item);
        }
    }

    public void UI_CloseContainer() {
        ClearChild(content_container);
        pnl_container.SetActive(false);
        btn_opencontainer.interactable = true;
    }

    public void UI_Attack() {
        Attack(player, (Entity)player.target);
    }

    public void UI_Pick() {
        Pick(player, (Item)player.target);
    }

    public void UI_Search() {
        if (pnl_container.activeSelf) {
            UI_CloseContainer();
        }
        
        player.target = FindNearBy(player, true);
        ShowNearBy(player.target);
    }

    public void UI_Exit() {
        depth++;
        init(depth);
    }
    //탈출구 바로가기
    public void UI_ToExit() {
        if (pnl_container.activeSelf) {
            UI_CloseContainer();
        }
        
        Vector2Int newPos;
        Vector2Int curPos = FindPos(player);

        //탈출구 주변 이동 가능한 셀 선택
        do {
            newPos = new Vector2Int(fieldSize - 1 + Random.Range(-1, 2), fieldSize - 1 + Random.Range(-1, 2));
        } while (!IsValidPos(newPos, true));

        //플레이어 이동
        field[newPos.x, newPos.y].entity = player;
        field[curPos.x, curPos.y].entity = null;

        Visualizer.instance.VisualizeField(field);

        player.target = FindNearBy(player, true);
        ShowNearBy(player.target);
    }

    public void UI_OpenInventory() {
        pnl_inventory.SetActive(true);
        btn_inventory.interactable = false;

        foreach (var item in player.inventory) {
            GameObject obj = Instantiate(prefab_btn_item, content_inventory);
            obj.GetComponent<btn_item>().init(new Body(player.name, player.inventory), item, "use");
        }
    }
    
    public void UI_CloseInventory() {
        ClearChild(content_inventory);
        pnl_inventory.SetActive(false);
        btn_inventory.interactable = true;
    }

    public void UI_ClosePopup() {
        ClearChild(content_popup);
        pnl_popup.SetActive(false);
    }

    public void OpenPopup() {
        pnl_popup.SetActive(true);
        //컨텍스트 위치 조절
        RectTransform rect_context = content_popup.GetComponent<RectTransform>();
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect_context.parent as RectTransform, // 부모 기준
            Input.mousePosition,
            null, // 카메라 (Screen Space Overlay일 때는 null)
            out pos
        );
        rect_context.anchoredPosition = pos;
        throw new NotImplementedException("우클릭 메뉴 생성 미구현");
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
        from.Attack(to);
        Visualizer.instance.VisualizeField(field);
    }
    /// <summary>
    /// 대상을 범위만큼 이동
    /// </summary>
    /// <param name="_target">이동 할 대상</param>
    /// <param name="range">이동 거리</param>
    private void Move(Entity _target, int range = 1) {
        Vector2Int newPos;
        Vector2Int curPos = FindPos(_target);
        //현재위치 기준 범위 내 이동 가능한 셀 선택
        do {
            newPos = new Vector2Int(curPos.x + Random.Range(-range, range + 1), curPos.y + Random.Range(-range, range + 1));
        } while (!IsValidPos(newPos, true));
        //대상 이동
        field[newPos.x, newPos.y].entity = _target;
        field[curPos.x, curPos.y].entity = null;

        Visualizer.instance.VisualizeField(field);
    }

    /// <summary>
    /// 유형에 따른 UI 세팅
    /// </summary>
    /// <param name="type">유형</param>
    /// <param name="text">조우 대상 이름</param>
    private void SetUI(string type = null, string text = null) {
        target.transform.parent.gameObject.SetActive(type != null);
        btn_attack.gameObject.SetActive(type == "attack");
        btn_pick.gameObject.SetActive(type == "pick");
        btn_exit.gameObject.SetActive(type == "exit");
        btn_opencontainer.gameObject.SetActive(type == "open");
        btn_toexit.gameObject.SetActive(knowExit);
        pnl_container.SetActive(type == "container");
        pnl_inventory.SetActive(type == "inventory");
        pnl_popup.SetActive(type == "popup");
        target.text = text;
    }

    public void ShowNearBy(object _target) {
        meet = _target == null;
        switch (_target) {
            case null:
                SetUI();
                return;
            case Entity entity:
                print($"Meet Entity : {entity.name}");
                SetUI("attack", entity.name);
                break;
            case Body body:
                print($"Meet Body : {body.name}");
                SetUI("open", body.name);
                break;
            case Obj obj:
                print($"Meet Obj : {obj.name}");
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

    /// <summary>
    /// 대상 주위를 범위 만큼 탐색
    /// </summary>
    /// <param name="from">탐색 원점(대상)</param>
    /// <param name="select">발견 대상 반환 여부</param>
    /// <returns>발견 대상</returns>
    public object FindNearBy(Entity from, bool select = false) {
        Vector2Int pos = FindPos(from); //from의 위치 획득
        //탐색 대상 초기화
        from.lst_nearEntity.Clear();
        from.lst_nearObejct.Clear();
        Floor exit = null;
        //필드 순회
        for (int x = pos.x - from.range; x <= pos.x + from.range; x++) {
            for (int y = pos.y - from.range; y <= pos.y + from.range; y++) {
                Vector2Int newPos = new Vector2Int(x, y);
                //필드를 벗어난 셀은 스킵
                if (!IsValidPos(newPos)) continue;
                //자신 제외
                if (newPos != pos) {
                    //엔티티 발견, 대상으로 추가
                    if (field[x, y].entity != null) {
                        from.lst_nearEntity.Add(field[x, y].entity);
                    }
                }
                //아이템 발견, 대상으로 추가
                if (field[x, y].obj != null) {
                    from.lst_nearObejct.AddRange(field[x, y].obj);
                }
                //탈출구 발견
                if (field[x, y].floor.type == FloorType.exit) {
                    //탈출구에 처음 접근하면 무조건 발견
                    if (!knowExit) {
                        return field[x, y].floor;
                    }
                    //탈출구 위치 저장
                    exit = field[x, y].floor;
                }
            }
        }
        //주변에 아무것도 없음
        if (from.lst_nearEntity.Count == 0 && from.lst_nearObejct.Count == 0 && exit == null) return null;
        //발견 대상 중 무작위 반환
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

    /// <summary>
    /// 목표 지점이 유효한지 확인
    /// </summary>
    /// <param name="targetPos">목표 지점</param>
    /// <param name="checkEntity">엔티티 체크 여부</param>
    /// <returns>목표 지점 내부, 셀에 엔티티가 없을 때 true</returns>
    private bool IsValidPos(Vector2Int targetPos, bool checkEntity = false) {
        //목표 지점이 필드 내부인지 확인
        if (targetPos.x < 0 || targetPos.x >= field.GetLength(1) ||
            targetPos.y < 0 || targetPos.y >= field.GetLength(0))
            return false; //필드 외부임

        //
        return !checkEntity || field[targetPos.x, targetPos.y].entity == null;
    }

    /// <summary>
    /// 필드 중 대상의 위치 찾기
    /// </summary>
    /// <param name="_target">위치를 찾을 엔티티</param>
    /// <returns>대상의 위치</returns>
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
    /// <summary>
    /// 필드 중 대상의 위치 찾기
    /// </summary>
    /// <param name="_target">위치를 찾을 아이템</param>
    /// <returns>대상의 위치</returns>
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

    /// <summary>
    /// 대상의 모든 자식 제거
    /// </summary>
    /// <param name="parent">자식을 제거할 대상</param>
    private void ClearChild(Transform parent){
        for(int i = parent.childCount - 1; i>=0; i--){
            DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour {
    public static Visualizer instance;

    [Header("Field UI")]
    public Transform ui_field;
    
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void VisualizeField(Cell[,] field) {
        for (int i = ui_field.childCount - 1; i >= 0; i--) {
            Destroy(ui_field.GetChild(i).gameObject);
        }
        
        Vector2 cellSize = ui_field.GetComponent<GridLayoutGroup>().cellSize;
        ui_field.GetComponent<RectTransform>().sizeDelta = new Vector2(field.GetLength(0) * cellSize.x, field.GetLength(1) * cellSize.y);

        foreach (var cell in field) {
            GameObject obj = new GameObject("cell");
            Image img = obj.AddComponent<Image>();
            obj.transform.SetParent(ui_field);
            
            if (cell.entity != null) {
                img.color = Color.red;
                if (cell.entity.name == "Player") {
                    img.color = Color.green;
                }
            }
            else if (cell.obj.Count != 0) {
                img.color = Color.yellow;
            }
            else {
                img.color = Color.gray;
            }
        }
    }
}

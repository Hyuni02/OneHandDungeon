using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class btn_item : MonoBehaviour {
    public Body body;
    public Obj obj;

    public void init(Body _body, Obj _obj) {
        obj = _obj;
        body = _body;
        
        transform.GetChild(0).GetComponent<TMP_Text>().text = obj.name;
        
        GetComponent<Button>().onClick.AddListener(click_Item);
    }
    
    private void click_Item() {
        if (GameManager.instance.player.GetItem(obj)) {
            body.content.Remove(obj);
            gameObject.SetActive(false);
        }
    }   
    //todo 호버 시 아이템 정보 표시 구현
}

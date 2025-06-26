using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class btn_item : MonoBehaviour {
    public Body body;
    public Item item;

    public void init(Body _body, Item _item) {
        item = _item;
        body = _body;
        
        transform.GetChild(0).GetComponent<TMP_Text>().text = item.name;
        
        GetComponent<Button>().onClick.AddListener(click_Item);
    }
    
    private void click_Item() {
        if (GameManager.instance.player.GetItem(item)) {
            body.content.Remove(item);
            gameObject.SetActive(false);
        }
    }   
    //todo 호버 시 아이템 정보 표시 구현
}

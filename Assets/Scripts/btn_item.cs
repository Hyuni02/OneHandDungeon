using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class btn_item : MonoBehaviour, IPointerClickHandler {
    public Body body;
    public Item item;
    private string defaultAction;

    public void init(Body _body, Item _item, string _defaultAction = "get") {
        item = _item;
        body = _body;

        transform.GetChild(0).GetComponent<TMP_Text>().text = item.name;
        Sprite loadedSprite = Resources.Load<Sprite>($"img/{item.name}");
        if (loadedSprite != null) {
            transform.GetComponent<Image>().sprite = loadedSprite;
        }
        else {
            transform.GetComponent<Image>().sprite = Resources.Load<Sprite>($"img/noimg");
            Debug.LogError($"No Img : {item.name}");
        }
        defaultAction = _defaultAction;
        GetComponent<Button>().onClick.AddListener(click_Item);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if(eventData.button == PointerEventData.InputButton.Right) {
            GameManager.instance.OpenPopup();
        }
    }

    private void click_Item() {
        switch (defaultAction) {
            case "get":
                if (GameManager.instance.player.GetItem(item)) {
                    body.content.Remove(item);
                    gameObject.SetActive(false);
                }
                break;
            case "use":
                item.Use(GameManager.instance.player);
                break;
            case "discard":
                throw new NotImplementedException("discard 미구현");
                break;
        }
    }
    //todo 호버 시 아이템 정보 표시 구현
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class btn_item : MonoBehaviour {
    public Obj obj;

    public void init(Obj _obj) {
        obj = _obj;

        transform.GetChild(0).GetComponent<TMP_Text>().text = obj.name;
    }
    
    //todo 클릭 시 아이템 획득 구현
    
    //todo 호버 시 아이템 정보 표시 구현
}

using System.Collections.Generic;
using UnityEngine;

public abstract class Entity {
    protected Entity(string _name) {
        name = _name;
        
        //이름으로 엔티티 스텟 불러오기
        Dictionary<string, object> data = DataLoader.GetData(name, "entity");
        maxHP = (int)data["hp"];
        curHP = maxHP;
        dmg = (int)data["damage"];
      
        string[] itemStrings = ((string)data["inventory"]).Split(',');
        if (itemStrings[0] == "") return;
        foreach (var itemString in itemStrings) {
            string[] itemData = itemString.Split(':'); // 0:item name, 1:item probability
            if (Random.Range(0, 100) < float.Parse(itemData[1]) * 100) {
                Obj obj = new Obj(itemData[0]);
                inventory.Add(obj);
                Debug.Log($"{obj.name}");
            }
        }
    }
    
    public string name { get; protected set; }
    public int maxHP { get; protected set; }
    public int curHP { get; protected set; }
    public int dmg { get; protected set; }
    public List<Obj> inventory = new List<Obj>();
    
    public int range = 1;
    public List<Entity> lst_nearEntity = new List<Entity>();
    public List<Obj> lst_nearObejct = new List<Obj>();
    public object target;
    
    public virtual void Attack(Entity _target) {
        _target.GetDmg(this, dmg);
    }
    
    public virtual void GetDmg(Entity from, int _dmg) {
        curHP -= _dmg;
        if (curHP <= 0) {
            curHP = 0;
            Die(from);
        }
    }

    public abstract void Die(Entity from);
}

#region Player

public class Player : Entity { 
    public Player(string _name) : base(_name) {
    }
    
    public override void Die(Entity from) {
        throw new System.NotImplementedException();
    }

    public virtual bool GetItem(Obj obj) {
        inventory.Add(obj);
        Debug.Log($"Get Item : {obj.name}");
        return true;
    }
}

#endregion

#region Enemy

public class Animal : Entity {
    public Animal(string _name) : base(_name) {
    }
    
    public override void Die(Entity from) {
        Debug.Log($"Die : {name}");
        //현재 위치에서 자신을 제거
        Vector2Int curPos = GameManager.instance.FindPos(this);
        GameManager.instance.PlaceEntity(curPos, null);
        GameManager.instance.lst_entity.Remove(this);
        //드랍탬을 담는 시체 생성
        Body body = new Body(name + " body", inventory);
        if (inventory.Count != 0) {
            GameManager.instance.PlaceItem(curPos, body);
            GameManager.instance.lst_obj.Add(body);
        }
        from.target = body;
        //공격자(플레이어)에게 시체를 보여줌
        GameManager.instance.ShowNearBy(from.target);
    }
}

#endregion

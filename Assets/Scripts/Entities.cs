using System.Collections.Generic;
using UnityEngine;

public abstract class Entity {
    protected Entity(string _name) {
        name = _name;
        
        //todo 이름으로 엔티티 스텟 불러오기
        //todo 이름으로 드랍탬 불러오기
        
        //임시
        maxHP = 100;
        curHP = 100;
        dmg = 50;
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

public class Player : Entity { 
    public Player(string _name) : base(_name) {
    }
    
    public override void Die(Entity from) {
        throw new System.NotImplementedException();
    }
}

public class Animal : Entity {
    public Animal(string _name) : base(_name) {
        inventory.Add(new Obj("Meat"));
        inventory.Add(new Obj("Leather"));
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
        from.target = GameManager.instance.FindNearBy(from, true);
        //공격자(플레이어)에게 시체를 보여줌
        GameManager.instance.ShowNearBy(from.target);
    }
}

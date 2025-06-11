using System.Collections.Generic;
using UnityEngine;

public class Entity {
    protected Entity(string _name) {
        name = _name;
    }
    
    public string name { get; protected set; }
    public int maxHP;
    public int curHP;
    public List<Obj> inventory = new List<Obj>();
}

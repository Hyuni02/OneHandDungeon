using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class Obj {
    public string name;
    
    protected Obj(string _name) {
        name = _name;
    }
}

public class Item : Obj {
    public readonly Dictionary<string, int> property = new Dictionary<string, int>();
    public readonly string description;
    
    public Item(string _name) : base(_name) {
        Dictionary<string, object> data = DataLoader.GetData(_name, "obj");
        description = data["description"].ToString();
        string[] propertyString = data["property"].ToString().Split(',');
        if (propertyString[0] == "") return;
        foreach (string prop in propertyString) {
            string[] p = prop.Split(':');
            property.Add(p[0], int.Parse(p[1]));
        }
    }

    public void ShowProperty() {
        throw new NotImplementedException("아이템 속성 표시 미구현");
    }

    public void Use(Entity caster) {
        throw new NotImplementedException($"아이템 사용 미구현 - {name}");
    }
}

public class Body : Obj {
    public List<Item> content;

    public Body(string _name, List<Item> items) : base(_name) {
        content = items;
    }
}



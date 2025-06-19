using System.Collections.Generic;
public class Obj {
    public string name;

    public Obj(string _name) {
        name = _name;
    }
}

public class Body : Obj {
    public List<Obj> content;

    public Body(string _name, List<Obj> items) : base(_name) {
        content = items;
    }
}



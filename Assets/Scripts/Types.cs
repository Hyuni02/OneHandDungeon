using System.Collections.Generic;
public struct Cell {
    public Entity entity;
    public List<Obj> obj;
    public Cell(Entity _entity, List<Obj> _obj) {
        entity = null;
        obj = new List<Obj>();
    }
}



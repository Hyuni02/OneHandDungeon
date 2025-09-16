using System.Collections.Generic;
public class Floor {
    public FloorType type = FloorType.none;
}

public class Cell {
    public Entity entity = null;
    public List<Obj> obj = new List<Obj>();
    public Floor floor = new Floor();
}
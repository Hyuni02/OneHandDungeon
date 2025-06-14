using System.Collections.Generic;

public class Cell {
    public Entity entity = null;
    public List<Obj> obj = new List<Obj>();
    public Floor floor = new Floor();
}

public enum FloorType {
    none,
    exit
}
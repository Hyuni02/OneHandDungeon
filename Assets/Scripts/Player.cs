using System.Collections.Generic;
public class Player : Entity { 
    public Player(string _name) : base(_name) {
    }
    
    public int range = 1;
    public List<Entity> lst_nearEntity = new List<Entity>();
    public List<Obj> lst_nearObejct = new List<Obj>();

   
}

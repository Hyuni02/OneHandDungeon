using UnityEngine;

public class PrefabContainer : MonoBehaviour {
    public static PrefabContainer instance;

    [Header("Entity")]
    public GameObject pref_player;
    public GameObject pref_chicken;
    
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
}

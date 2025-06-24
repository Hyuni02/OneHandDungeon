using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class DataLoader : MonoBehaviour {
    private static List<Dictionary<string, object>> entityData;
    private static List<Dictionary<string, object>> objData;

    private void Awake() {
        LoadEntityData();
        // LoadObjData();
    }
    
    private void LoadEntityData() {
        entityData = CSVReader.Read("EntityData");
    }

    private void LoadObjData() {
        objData = CSVReader.Read("ObjData");
    }

    public static Dictionary<string, object> GetData(string _name, string type) {
        switch (type) {
            case "entity":
                foreach (var data in entityData) {
                    if ((string)data["name"] == _name) {
                        return data;
                    }
                }
                break;
            case "obj":
                foreach (var data in objData) {
                    if ((string)data["name"] == _name) {
                        return data;
                    }
                }
                break;
            default:
                Debug.LogError("Wrong type");
                return null;
        }
        Debug.LogError($"Can't Find : {_name} in {type}");
        return null;
    }
}

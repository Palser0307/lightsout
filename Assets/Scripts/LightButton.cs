using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightButton : MonoBehaviour{
    [System.NonSerialized]
    public int row;
    [System.NonSerialized]
    public int col;

    LightsOut main;

    void Start(){
        main = GameObject.FindGameObjectWithTag("GameController").GetComponent<LightsOut>();
    }

    public void OnClick(){
        main.SwitchLights(row, col, false);
        Debug.Log("Lights \"OnClick\": [ "+row+", "+col+"] is clicked.");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeDropdown : MonoBehaviour{
    [SerializeField]
    LightsOut main;

    Dropdown dropDown_menu;

    void Start(){
        if(this.TryGetComponent(out this.dropDown_menu) == false){
            Debug.LogError("Can't found: \"DropDown\" Component at this object.");
            return;
        }

        // プルダウンメニューで別の値が選択された際に叩かれる関数を
        // こっちでも叩いてゲームを始める
        OnValueChanged();
    }

    // プルダウンメニューの値が選択された際に叩かれる関数
    public void OnValueChanged(){
        main.ClearLights();

        // 各値に通し番号(0-3)が割り当てられている
        // そのため，+4すれば一辺の長さが得られる って寸法
        main.CreateLights(dropDown_menu.value + 4);
    }
}
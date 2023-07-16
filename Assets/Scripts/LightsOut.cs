using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ゲームのメインスクリプト
public class LightsOut : MonoBehaviour{
    // 問題生成時の乱数に使う上限，下限値
    int min_Size = 3, max_Size = 9;

    // 問題生成時の乱数生成の回数
    [SerializeField]
    int random_Count = 5;

    [SerializeField]
    GameObject light_Prefab;

    [SerializeField]
    Transform light_Parent;

    [SerializeField]
    GridLayoutGroup grid_Object;

    // ボタンの色 ON状態
    [SerializeField]
    Color onButtonColor, onButtonHighlightedColor;
    
    // ボタンの色 OFF状態
    [SerializeField]
    Color offButtonColor, offButtonHighlightedColor;

    // CLEAR!! のTextObjへのアクセサ
    // 正直Text型じゃなくても良い
    [SerializeField]
    Text gameClear_Text;

    // ボタン(light)のON/OFFフラグ
    bool[,] lightStatus;

    // ボタン(light)のオブジェクトへのアクセサ
    GameObject[,] lightObjects;

    // ランダムに問題を生成する
    public void CreateProblem(){
        // CLEAR!!のテキストを非表示に
        gameClear_Text.enabled = false;

        // 各ボタンの機能を有効化
        foreach(GameObject light in lightObjects){
            Button button;
            if(!light.TryGetComponent(out button)){
                Debug.LogError("Can't get \"button\" Component");
                return;
            }else{
                // これが有効化部分
                button.interactable = true;
            }
        }

        // グリッドの各ライトの状態を保存S
        lightStatus = new bool[lightStatus.GetLength(0), lightStatus.GetLength(1)];

        // 全ライトの分だけループする
        int roopCount = Random.Range(2, lightStatus.Length);

        // random_Countが少なくとも1以上になるように
        random_Count = Mathf.Max(random_Count, 1);

        // ランダムでライトを選んで反転させる
        for(int i = 0; i < roopCount; i++){
            int choosedRow = 0, choosedCol = 0;

            // 乱数に「コク」を加える ここから
            for(int j=0; j<random_Count; j++){
                choosedRow += Random.Range(0, lightStatus.GetLength(0));
                choosedCol += Random.Range(0, lightStatus.GetLength(1));
            }

            choosedRow /= random_Count;
            choosedCol /= random_Count;
            // 乱数に「コク」を加える ここまで

            // 選択された位置とその隣接ライトを反転
            // この関数を細分化すればもう少し問題生成の処理が軽くなると思われる
            SwitchLights(choosedRow, choosedCol, true);
        }

        // ライトの色設定
        SetLightColor();

        Debug.Log("Method \"CreateProblem\": Successful Completion");
    }

    // ライトを生成する処理
    // int size : 正方形グリッドの一辺の長さ size*sizeのグリッド，ボタンを生成する
    public void CreateLights(int size){
        // Mathf.Clamp(num, min, max);
        // 引数numの値をmin以上max以下の範囲の値に圧縮する
        // 例1: Mathf.Clamp( 5, 1, 4) -> 4
        // 例2: Mathf.Clamp(-1, 1, 4) -> 1
        size = Mathf.Clamp(size, min_Size, max_Size);

        gameClear_Text.enabled = false;
        lightStatus = new bool[size, size];
        lightObjects = new GameObject[size, size];
        grid_Object.constraintCount = size;

        for(int i=0; i<size; i++){
            for(int j=0; j<size; j++){
                GameObject button = Instantiate(light_Prefab);
                button.transform.SetParent(light_Parent);
                button.transform.localScale = transform.lossyScale;

                button.transform.position = light_Parent.transform.position;

                LightButton light = button.GetComponent<LightButton>();
                light.row = i;
                light.col = j;

                lightObjects[i,j] = button;
            }
        }
        CreateProblem();
    }

    public void ClearLights(){
        if(lightObjects == null || lightObjects.Length <= 0){
            return;
        }
        foreach(GameObject buttonObj in lightObjects){
            Destroy(buttonObj);
        }
    }

    // ライトを押したときの処理
    public void SwitchLights(int row, int col, bool createMode){
        if(lightStatus == null || lightStatus.Length <= 0){
            return;
        }

        row = Mathf.Clamp(row, 0, lightStatus.GetLength(0));
        col = Mathf.Clamp(col, 0, lightStatus.GetLength(1));

        // その要素自身
        lightStatus[row, col] = !lightStatus[row, col];

        // 上
        if(row - 1 >= 0){
            lightStatus[row - 1, col] = !lightStatus[row - 1, col];
        }

        // 下
        if(row + 1 < lightStatus.GetLength(0)){
            lightStatus[row + 1, col] = !lightStatus[row + 1, col];
        }

        // 左
        if(col - 1 >= 0){
            lightStatus[row, col - 1] = !lightStatus[row, col - 1];
        }

        // 右
        if(col + 1 < lightStatus.GetLength(1)){
            lightStatus[row, col + 1] = !lightStatus[row, col + 1];
        }

        if(!createMode){
            SetLightColor();
            CheckClear();
        }
    }

    // クリア判定
    void CheckClear(){
        if(lightStatus == null || lightStatus.Length <= 0){
            return;
        }

        foreach(bool status in lightStatus){
            if(status == true){
                return;
            }
        }

        foreach(GameObject light in lightObjects){
            Button button = light.GetComponent<Button>();
            button.interactable = false;
        }
        gameClear_Text.enabled = true;
    }

    // ライトの色設定処理
    void SetLightColor(){
        if(lightStatus == null || lightStatus.Length <= 0){
            return;
        }

        Button button;

        for(int i = 0; i < lightStatus.GetLength(0); i++){
            for(int j = 0; j < lightStatus.GetLongLength(1); j++){
                button = lightObjects[i, j].GetComponent<Button>();
                ColorBlock colorBlock = button.colors;

                if(lightStatus[i, j]){
                    colorBlock.normalColor = onButtonColor;
                    colorBlock.pressedColor = onButtonColor;
                    colorBlock.selectedColor = onButtonColor;
                    colorBlock.highlightedColor = onButtonHighlightedColor;
                }else{
                    colorBlock.normalColor = offButtonColor;
                    colorBlock.pressedColor = offButtonColor;
                    colorBlock.selectedColor = offButtonColor;
                    colorBlock.highlightedColor = offButtonHighlightedColor;
                }

                button.colors = colorBlock;
                button.interactable = true;
            }
        }
    }
}

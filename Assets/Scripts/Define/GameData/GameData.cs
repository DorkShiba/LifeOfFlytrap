using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Game Data", menuName = "GameData/GameData", order = 1)]
public class GameData : ScriptableObject
{
    private static GameData _instance;
    public static GameData Instance
    {
        get
        {
            if (_instance == null)
            {
                // Resources/GameData 폴더 안에 있는 "GameData"라는 이름의 SO를 찾아서 불러옵니다.
                _instance = Resources.Load<GameData>("GameData/GameData");

                if (_instance == null)
                    Debug.LogError("Resources/GameData 폴더 안에 GameData 파일이 없습니다!");
            }
            return _instance;
        }
    }

    public float MapWidth = 11.25f * 2;  // 맵의 가로 크기
    public float MapHeight = 7.5f * 2; // 맵의 세로

    public List<int> ClearConstraints = new List<int>{
        1000, 1000, 1000,
        1000, 1000, 1000,
        1000, 1000, 1000,
        1000, 1000, 1000
    };
}

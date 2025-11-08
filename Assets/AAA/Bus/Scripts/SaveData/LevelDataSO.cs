using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "GameData/Level Data")]
public class LevelDataSO : ScriptableObject
{
    public List<GameColors> bookerColorList = new List<GameColors>();// DO NOT CHANGE NAME!!

    public List<VehicleData> VehicleColorMap = new List<VehicleData>();
}

[Serializable]
public class VehicleData
{
    public Vector3 position;
    public Quaternion rotation;
    public GameColors gameColors;
}

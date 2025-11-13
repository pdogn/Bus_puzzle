using UnityEngine;
using System.Data;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class SaveData : MonoBehaviour
{
    public LevelDataSO dataAsset;

    private Dictionary<GameColors, Vehicle> DictType;
    [Header("Bus prefabs")]
    [SerializeField] private BusContainer busContainer;

    private void Start()
    {
        //SetDictTypeOfBus();
    }

    void SetDictTypeOfBus()
    {
        DictType = new Dictionary<GameColors, Vehicle>();
        foreach (var x in busContainer.busCatalog)
        {
            DictType[x.busColor] = x.bus;
        }
    }

#if UNITY_EDITOR
    public  void SaveNew()
    {
        string folderPath = "Assets/AAA/Bus/GameData/LevelData";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        int index = 1;
        string assetPath;
        do
        {
            assetPath = $"{folderPath}/Level_{index}.asset";
            index++;
        }
        while (File.Exists(assetPath));

        LevelDataSO newDataAsset = ScriptableObject.CreateInstance<LevelDataSO>();

        // Ghi laij
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Vehicle>() == null) continue;

            Vehicle vehicle = child.GetComponent<Vehicle>();
            VehicleData tData = new VehicleData();
            tData.position = child.position;
            tData.rotation = child.rotation;
            tData.gameColors = vehicle.Attributes.VehicleColor;
            tData.maxSizeCount = vehicle.maxSize;

            newDataAsset.VehicleColorMap.Add(tData);
        }

        // Lưu asset
        AssetDatabase.CreateAsset(newDataAsset, assetPath);
        EditorUtility.SetDirty(newDataAsset);
        AssetDatabase.SaveAssets();

        dataAsset = newDataAsset;

        Debug.Log($"Đã lưu {dataAsset.VehicleColorMap.Count} object con vào {dataAsset.name}");
    }

    public void SaveOverwrite()
    {
        if (dataAsset == null)
        {
            Debug.LogWarning("Chưa gán ScriptableObject để ghi đè!");
            return;
        }

        dataAsset.VehicleColorMap.Clear();

        foreach (Transform child in transform)
        {
            if (child.GetComponent<Vehicle>() == null) continue;

            Vehicle vehicle = child.GetComponent<Vehicle>();
            VehicleData tData = new VehicleData();
            tData.position = child.position;
            tData.rotation = child.rotation;
            tData.gameColors = vehicle.Attributes.VehicleColor;
            tData.maxSizeCount = vehicle.maxSize;

            dataAsset.VehicleColorMap.Add(tData);
        }

        EditorUtility.SetDirty(dataAsset);
        AssetDatabase.SaveAssets();
        Debug.Log($"Đã ghi đè dữ liệu vào {AssetDatabase.GetAssetPath(dataAsset)} ({dataAsset.VehicleColorMap.Count} object)");
    }

    public void LoadLevel()
    {
        SetDictTypeOfBus();

        foreach (var v in dataAsset.VehicleColorMap)
        {
            Vehicle newVehicle;
            var initialPos = v.position;
            var initialRotation = v.rotation;
            var initialColor = v.gameColors;
            int _maxSize = v.maxSizeCount;
            if (_maxSize == 0) _maxSize = 4;

            newVehicle = Instantiate(DictType[initialColor], initialPos, initialRotation, transform);
            newVehicle.maxSize = _maxSize;
            newVehicle.SetTypeVehicle(_maxSize);
        }
    }

    public void Clearr()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
#endif
}

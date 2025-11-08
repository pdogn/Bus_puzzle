using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    [Header("Bus prefabs")]
    [SerializeField] private BusContainer busContainer;

    public Transform top;
    public Transform bottom;
    public Transform left;
    public Transform right;

    private Dictionary<GameColors, Vehicle> DictType;

    public List<Vehicle> vehicleList = new List<Vehicle>();

    private LevelDataSO _levelDataSo;

    public static VehicleManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetDictTypeOfBus();
        //InitializeLevel();
    }

    void SetDictTypeOfBus()
    {
        DictType = new Dictionary<GameColors, Vehicle>();
        foreach (var x in busContainer.busCatalog)
        {
            DictType[x.busColor] = x.bus;
        }
    }

    public void InitializeLevel()
    {
        GenerateVehicle();
    }

    public void SetLevelData(LevelDataSO levelData) { _levelDataSo = levelData; }

    private void GenerateVehicle()
    {
        foreach(var v in _levelDataSo.VehicleColorMap)
        {
            Vehicle newVehicle;
            var initialPos = v.position;
            var initialRotation = v.rotation;
            var initialColor = v.gameColors;
            newVehicle = Instantiate(DictType[initialColor], initialPos, initialRotation, transform);

            newVehicle.OnReach += VehicleLineManager.Instance.HandleOnVehicleReach;
        }
    }

}

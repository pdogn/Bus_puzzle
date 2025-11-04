using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    [Header("Bus prefabs")]
    [SerializeField] private BusContainer busContainer;
    private Dictionary<GameColors, Vehicle> DictType;

    public List<Vehicle> vehicleList = new List<Vehicle>();

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
        InitializeLevel();
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

    private void GenerateVehicle()
    {
        for(int i=0; i< 7; i++)
        {
            Vehicle newVehicle;

            var initialPos = new Vector3(6 - i * 3, -15, -25);
            newVehicle = Instantiate(DictType[GameColors.LILAC], initialPos, Quaternion.identity, transform);

            newVehicle.OnReach += VehicleLineManager.Instance.HandleOnVehicleReach;

            vehicleList.Add(newVehicle);
        }
    }

}

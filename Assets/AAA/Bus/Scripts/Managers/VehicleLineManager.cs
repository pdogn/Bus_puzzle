using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleLineManager : MonoBehaviour
{
    //position vehicleStop
    //[SerializeField] private List<Transform> vehicleStops;

    [SerializeField] private List<PlacePoint> vehicleStopPoints;

    [SerializeField] private PlacePoint nextStopPoints;
    public PlacePoint NextStopPoints => nextStopPoints;

    ////vehicle in line
    //[SerializeField] private Vehicle[] vehicles;
    //public Vehicle[] Vehicles => vehicles;

    [SerializeField] private List<Vehicle> vehicles2;
    public List<Vehicle> Vehicles2 => vehicles2;

    public bool isVehicleReaching;

    public static VehicleLineManager Instance;

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
        //vehicles = new Vehicle[vehicleStops.Count];
    }

    //public Vector3 GetStopPositionEmpty()
    //{
    //    Vector3 _position = Vector3.zero;
    //    for(int i=0; i< Vehicles.Length; i++)
    //    {
    //        if(Vehicles[i] == null)
    //        {
    //            _position = vehicleStops[i].position;
    //            break;
    //        }
    //    }

    //    return _position;
    //}

    public bool IsAvaiableStopPointEmpty()
    {
        foreach(var child in vehicleStopPoints)
        {
            if (!child.hasVehicle)
            {
                nextStopPoints = child;
                return true;
            }
        }
        return false;
    }

    //public int GetIndexVehicleNull()
    //{
    //    int idx = 0;
    //    for (int i = 0; i < Vehicles.Length; i++)
    //    {
    //        if (Vehicles[i] == null)
    //        {
    //            idx = i;
    //            break;
    //        }
    //    }
    //    return idx;
    //}


    //public void AddVehicleToLine(Vehicle vehicle)
    //{
    //    int index = GetIndexVehicleNull();
    //    Vehicles[index] = vehicle;
    //}

    public void AddVehicleToLine2(Vehicle vehicle)
    {
        vehicles2.Add(vehicle);
    }

    public void HandleOnVehicleReach(Vehicle vehicle)
    {
        if (isVehicleReaching) return;

        isVehicleReaching = true;

        var booker = BookerLineManager.Instance.firstbooker;

        if(ColorControl(vehicle, booker))
        {
            booker.MoveBookerToBus(vehicle);
        }
        else
        {
            isVehicleReaching = false;
        }
    }

    private bool ColorControl(Vehicle vehicle, Booker booker)
    {
        return booker.Attributes.bookerColor == vehicle.Attributes.VehicleColor;
    }
}

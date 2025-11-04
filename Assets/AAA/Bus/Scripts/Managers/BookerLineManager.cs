using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookerLineManager : MonoBehaviour
{
    [SerializeField] private List<Transform> standPoints;

    public List<Transform> StandPoints => standPoints;

    //public bool isFindedBus;

    public Booker firstbooker;

    public static BookerLineManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    //public void HandleBookerReach(Booker booker)
    //{
    //    booker.OnReach -= HandleBookerReach;
    //    //if (!isFindedBus) return;
    //    //booker.MoveBookerToBus();

    //    foreach(var c in VehicleLineManager.Instance.Vehicles)
    //    {
    //        if (c == null || c.bookerCount == 3) continue;

    //        if(c.bookerCount != 3)
    //        {
    //            booker.MoveBookerToBus(c);
    //        }
    //    }
    //}

    public void HandleBookerReach(Booker booker)
    {
        booker.OnReach -= HandleBookerReach;
        //if (!isFindedBus) return;
        //booker.MoveBookerToBus();

        //foreach (var c in VehicleLineManager.Instance.Vehicles)
        //{
        //    if (c == null || c.bookerCount == 3) continue;

        //    if (c.bookerCount != 3)
        //    {
        //        booker.MoveBookerToBus(c);
        //    }
        //}
        if (!BookerFindedVehicle(booker))
        {
            VehicleLineManager.Instance.isVehicleReaching = false;
            return;
        }

        foreach (var vehicle in VehicleLineManager.Instance.Vehicles2)
        {
            if (vehicle == null || vehicle.bookerCount == 3 || !ColorControl(vehicle, booker)) continue;

            if (vehicle.bookerCount != 3 && ColorControl(vehicle, booker))
            {
                booker.MoveBookerToBus(vehicle);
            }
        }

        //if (!BookerFindedVehicle(booker))
        //{
        //    VehicleLineManager.Instance.isVehicleReaching = false;
        //}
    }

    bool BookerFindedVehicle(Booker booker)
    {
        bool result = false;
        foreach (var vehicle in VehicleLineManager.Instance.Vehicles2)
        {
            result = false;
            if (ColorControl(vehicle, booker) && vehicle.bookerCount != 3)
            {
                result = true;
                break;
            }
        }
        return result;
    }
    public void AllCharacterMoveInLine()
    {
        foreach (var c in BookerManager.Instance.bookers)
        {
            //if (c.CrrIndex == 0) continue;
            //c.CrrIndex = c.CrrIndex - 1;
            c.MoveCharacterInLine();
        }
    }

    private bool ColorControl(Vehicle vehicle, Booker booker)
    {
        return booker.Attributes.bookerColor == vehicle.Attributes.VehicleColor;
    }
}

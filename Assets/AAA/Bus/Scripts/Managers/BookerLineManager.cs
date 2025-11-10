using DG.Tweening;
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

    public void HandleBookerReach(Booker booker)
    {
        booker.OnReach -= HandleBookerReach;

        Vehicle foundVehicle = null;

        foreach (var vehicle in VehicleLineManager.Instance.Vehicles)
        {
            if (vehicle == null || vehicle.bookerCount >= vehicle.maxSize) continue;
            if (ColorControl(vehicle, booker))
            {
                foundVehicle = vehicle;
                break;
            }
        }

        if (foundVehicle == null)
        {
            VehicleLineManager.Instance.isVehicleReaching = false;
            return;
        }

        booker.MoveBookerToBus(foundVehicle);
    }

    bool BookerFindedVehicle(Booker booker)
    {
        foreach (var vehicle in VehicleLineManager.Instance.Vehicles)
        {
            if (ColorControl(vehicle, booker) && vehicle.bookerCount < vehicle.maxSize)
            {
                return true;
            }
        }
        return false;
    }

    public void AllCharacterMoveInLine()
    {
        foreach (var c in BookerManager.Instance.bookers)
        {
            c.MoveCharacterInLine();
        }
    }

    public void AddBookerToLastLine(Booker booker)
    {
        booker.transform.position = standPoints[19].position;
        BookerManager.Instance.bookers.Add(booker);
        booker.crrIndex = 19;
    }

    private bool ColorControl(Vehicle vehicle, Booker booker)
    {
        return booker.Attributes.bookerColor == vehicle.Attributes.VehicleColor;
    }
}

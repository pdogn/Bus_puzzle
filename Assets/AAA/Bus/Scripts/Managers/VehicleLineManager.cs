using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VehicleLineManager : MonoBehaviour
{
    //position vehicleStop
    //[SerializeField] private List<Transform> vehicleStops;

    [SerializeField] private List<PlacePoint> vehicleStopPoints;
    [SerializeField] private Transform exitPoint;
    public Transform ExitPoint => exitPoint;

    [SerializeField] private PlacePoint nextStopPoints;
    public PlacePoint NextStopPoints => nextStopPoints;

    ////vehicle in line
    //[SerializeField] private Vehicle[] vehicles;
    //public Vehicle[] Vehicles => vehicles;

    [SerializeField] private List<Vehicle> vehicles;
    public List<Vehicle> Vehicles => vehicles;

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


    public void AddVehicleToLine2(Vehicle vehicle)
    {
        vehicles.Add(vehicle);
    }

    public void HandleOnVehicleReach(Vehicle vehicle)
    {
        if (isVehicleReaching) return;

        Debug.Log("Xe reach");
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

    //public void MoveToExit(Vehicle vehicle)
    //{
    //    DOTween.Kill(vehicle.transform);
    //    Vector3 origin = vehicle.transform.position;
    //    Vector3 target = vehicle.targetPlacePoint.subPoint1.position;
    //    float distance = Vector3.Distance(origin, target);
    //    float duration = distance / 45f;
    //    vehicle.transform.DOMove(target, duration).SetEase(Ease.Linear).OnComplete(() =>
    //    {
    //        Vector3 direction = (target - transform.position).normalized;
    //        if (direction != Vector3.zero)
    //        {
    //            vehicle.transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90f, 0);
    //        }
    //        target = vehicle.targetPlacePoint.subPoint2.position;
    //        distance = Vector3.Distance(origin, target);
    //        duration = distance / 45f;

    //        vehicle.transform.DOMove(target, duration).SetEase(Ease.Linear).OnComplete(() =>
    //        {
    //            target = exitPoint.position;
    //            distance = Vector3.Distance(origin, target);
    //            duration = distance / 45f;

    //            vehicle.transform.DOMove(target, duration).SetEase(Ease.Linear);
    //        });
    //    });
    //}

    public void RemoveVehicleInLine(Vehicle vehicle)
    {
        Vehicles.Remove(vehicle);
    }

    public void ClearLevel()
    {
        //vehicles.Clear();
        isVehicleReaching = false;
    }
}

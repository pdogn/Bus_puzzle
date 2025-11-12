using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePoint : MonoBehaviour
{
    public Transform subPoint1;
    public Transform subPoint2;

    public bool hadVehicle;

    public Vector3 Position;

    private void Start()
    {
        Position = this.transform.position;
    }
}

using BookerNamespace;
using DG.Tweening;
using System;
using UnityEngine;

public class Booker : MonoBehaviour
{
    [SerializeField] private BookerAttributes bookerAttributes;
    [SerializeField] private Animator bookerAnim;

    //[SerializeField] private BoxCollider boxCollider;

    public int crrIndex;

    public Vehicle targetVehicle;
    public Vehicle TargetVehicle => targetVehicle;

    public BookerAttributes Attributes => bookerAttributes;

    public event Action<Booker> OnReach;
    public event Action<Booker> OnExit;
    public event Action<Booker> OnEnd;

    private static readonly int Running = Animator.StringToHash("Running");

    public bool booked;

    public void MoveCharacterInLine()
    {
        //if (crrIndex == 0) return;

        //var nextPos = BookerLineManager.Instance.StandPoints[crrIndex -1].position;
        //transform.LookAt(nextPos);

        //bookerAnim.SetBool(Running, true);

        //transform.DOMove(nextPos, .2f).OnComplete(() =>
        //{
        //    crrIndex = crrIndex - 1;
        //    bookerAnim.SetBool(Running, false);
        //    if (crrIndex == 0)
        //    {
        //        BookerLineManager.Instance.firstbooker = this;
        //        OnReach?.Invoke(this);
        //    }
        //});

        if (crrIndex == 0) return;
        crrIndex--;

        var nextPos = BookerLineManager.Instance.StandPoints[crrIndex].position;
        transform.LookAt(nextPos);

        bookerAnim.SetBool(Running, true);

        transform.DOKill();

        transform.DOMove(nextPos, .3f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                bookerAnim.SetBool(Running, false);
                if (crrIndex == 0)
                {
                    BookerLineManager.Instance.firstbooker = this;
                    BookerOnReach();

                }
            });
    }

    public void RunAnim()
    {
        bookerAnim.SetBool(Running, true);
    }
    public void IdleAnim()
    {
        bookerAnim.SetBool(Running, false);
    }

    public void BookerOnReach()
    {
        OnReach?.Invoke(this);
    }

    public void MoveBookerToBusOld( Vehicle vehilcle)
    {
        if(targetVehicle == null)
        {
            targetVehicle = vehilcle;
        }
        var vehiclePos = targetVehicle.transform.position;
        Vector3 dir = (vehiclePos - this.transform.position).normalized;
        vehiclePos -= dir * .35f; //lùi lại 0.35f so với xe
                               
        transform.LookAt(vehiclePos);

        vehilcle.bookerCount++;
        bookerAnim.SetBool(Running, true);

        BookerLineManager.Instance.AllCharacterMoveInLine();

        //booker ra khỏi hàng
        BookerManager.Instance.RemoveBookerInLine(this);
        //Lấy booker trong pool
        Booker _booker = BookerManager.Instance.GetBookerInPool();
        //Thêm booker và0 cuối hàng
        BookerLineManager.Instance.AddBookerToLastLine(_booker);

        transform.DOMove(vehiclePos, 1f).OnComplete(() =>
        {
            //BookerLineManager.Instance.AllCharacterMoveInLine();
            //DOTween.Kill(vehilcle.transform);
            //vehilcle.bookerCount++;
            if(vehilcle.bookerSittingCount < vehilcle.bookerCount)
            {
                vehilcle.bookerSittingCount++;
            }

            vehilcle.transform.DOShakePosition(0.5f, 0.2f, 10, 90, false, true).OnComplete(() =>
            {
                if(vehilcle.bookerCount == vehilcle.maxSize && vehilcle.bookerSittingCount == vehilcle.bookerCount)
                {
                    //VehicleLineManager.Instance.MoveToExit(vehilcle);
                    //vehilcle.MoveToExit();
                    vehilcle.OnVehicleExit();
                }
            });
            //vehilcle.transform.DOPunchRotation(new Vector3(0, 0, 10), 0.4f, 10, 1);
            this.gameObject.SetActive(false);
            bookerAnim.SetBool(Running, false);
            Debug.Log("Return Pool");
            //Thêm vào pool
            BookerManager.Instance.ReturnToPool(this);
        });
    }

    public void MoveBookerToBus(Vehicle vehicle)
    {
        if (targetVehicle == null)
            targetVehicle = vehicle;

        Vector3 vehiclePos = targetVehicle.transform.position;
        Vector3 dir = (vehiclePos - transform.position).normalized;
        Vector3 targetPos = vehiclePos - dir * 0.35f; // cách xe 0.35f

        transform.DOKill();
        transform.DOLookAt(targetPos, 0.2f);

        targetVehicle.bookerCount++;
        bookerAnim.SetBool(Running, true);

        // Di chuyển đến xe
        transform.DOMove(targetPos, 0.6f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                bookerAnim.SetBool(Running, false);
                BookerOnReachBus(vehicle); // callback riêng khi tới xe
            });

        // Khi 1 booker đi, cả hàng tiến lên
        BookerLineManager.Instance.AllCharacterMoveInLine();
        //booker ra khỏi hàng
        BookerManager.Instance.RemoveBookerInLine(this);
        //Lấy booker trong pool
        Booker _booker = BookerManager.Instance.GetBookerInPool();
        //Thêm booker và0 cuối hàng
        BookerLineManager.Instance.AddBookerToLastLine(_booker);
    }

    private void BookerOnReachBus(Vehicle vehicle)
    {
        if (vehicle == null) return;

        vehicle.transform.DOKill();
        vehicle.bookerSittingCount++;

        // Rung xe nhẹ khi có người lên
        vehicle.transform.DOShakePosition(0.5f, 0.2f, 10, 90, false, true).OnComplete(() =>
        {
            // Khi xe đầy và mọi booker đã ngồi
            if (vehicle.bookerSittingCount >= vehicle.maxSize && !vehicle.isLeaving)
            {
                vehicle.isLeaving = true;

                DOVirtual.DelayedCall(.5f, () =>
                {
                    vehicle.transform.DOKill();
                    vehicle.OnVehicleExit();
                });
            }
        });

        // Tắt anim, chuẩn bị trả về pool
        transform.DOKill();
        bookerAnim.SetBool(Running, false);

        DOVirtual.DelayedCall(0.1f, () =>
        {
            gameObject.SetActive(false);
            Debug.Log("Return Pool");
            BookerManager.Instance.ReturnToPool(this);
        });
    }

}

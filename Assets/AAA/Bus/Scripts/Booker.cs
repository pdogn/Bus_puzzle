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

    public BookerAttributes Attributes => bookerAttributes;
    //public int CrrIndex
    //{
    //    get => crrIndex;
    //    set
    //    {
    //        if(value != crrIndex)
    //        {
    //            crrIndex = value;
    //            transform.DOMove(BookerLineManager.Instance.StandPoints[crrIndex].position, .5f).OnComplete(() =>
    //            {
    //                //if(crrIndex == 0)
    //                //{
    //                //    this.MoveToBus();
    //                //}
    //            });

    //        }
    //    }
    //}

    public event Action<Booker> OnReach;
    public event Action<Booker> OnExit;
    public event Action<Booker> OnEnd;

    private static readonly int Running = Animator.StringToHash("Running");

    public bool booked;

    public void MoveCharacterInLine()
    {
        if (crrIndex == 0) return;

        var nextPos = BookerLineManager.Instance.StandPoints[crrIndex -1].position;
        transform.LookAt(nextPos);

        bookerAnim.SetBool(Running, true);

        transform.DOMove(nextPos, .2f).OnComplete(() =>
        {
            crrIndex = crrIndex - 1;
            bookerAnim.SetBool(Running, false);
            if (crrIndex == 0)
            {
                BookerLineManager.Instance.firstbooker = this;
                OnReach?.Invoke(this);
            }
        });

    }

    //public void MoveCharacterToNextPoint(int index)
    //{
    //    var nextPos = BookerLineManager.Instance.StandPoints[index].position;

    //    transform.DOMove(nextPos, 1f).OnComplete(() =>
    //    {
    //        if (index == 0)
    //        {
    //            OnReach?.Invoke(this);
    //        }
    //    });
    //}

    public void MoveBookerToBus( Vehicle vehilcle)
    {
        ////booker ra khỏi hàng
        //BookerManager.Instance.RemoveBookerInLine(this);
        ////Lấy booker trong pool
        //Booker _booker = BookerManager.Instance.GetBookerInPool();
        ////Thêm booker và0 cuối hàng
        ////BookerLineManager.Instance.AddBookerToLastLine(_booker);

        var vehiclePos = vehilcle.transform.position;
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

        transform.DOMove(vehiclePos, .5f).OnComplete(() =>
        {
            DOTween.Kill(vehilcle.transform);

            vehilcle.transform.DOShakePosition(0.5f, 0.2f, 10, 90, false, true).OnComplete(() =>
            {
                if(vehilcle.bookerCount == 3)
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
}

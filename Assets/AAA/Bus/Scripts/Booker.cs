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

        transform.DOMove(nextPos, .2f).OnComplete(() =>
        {
            crrIndex = crrIndex - 1;
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
        var targetPos = Vector3.zero;
        targetPos = vehilcle.transform.position;
        transform.LookAt(targetPos);

        vehilcle.bookerCount++;

        BookerLineManager.Instance.AllCharacterMoveInLine();

        transform.DOMove(targetPos, .5f).OnComplete(() =>
        {
            Debug.Log("Return Pool");
            //CrrIndex = 19;
        });
    }
}

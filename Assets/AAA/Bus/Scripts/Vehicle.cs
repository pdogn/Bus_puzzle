using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Vehicle : MonoBehaviour
{
    [SerializeField] private BusAttributes busAttributes;
    [SerializeField] private Animator bookerAnim;

    [SerializeField] private BoxCollider boxCollider;

    public bool Clicked;
    public int bookerCount = 0;

    [Header("Move properties")]
    public float speed = 45f;            // tốc độ (đơn vị / giây)
    //public Vector3 direction = Vector3.right; // hướng ban đầu
    public float moveStep = 1f;         // quãng đường mỗi tween (càng nhỏ càng mượt)

    private Camera cam;
    private Tween moveTween;

    public PlacePoint targetPlacePoint;

    public BusAttributes Attributes => busAttributes;

    public event Action<Vehicle> OnReach;
    public event Action<Vehicle> OnExit;
    public event Action<Vehicle> OnEnd;


    private void Start()
    {
        cam = Camera.main;
    }


    public bool FindAPath()
    {
        if (VehicleLineManager.Instance.IsAvaiableStopPointEmpty())
        {
            MoveVehicleWithPath();
            return true;
        }
        return false;
    }

    private void MoveVehicleWithPath()
    {
        Clicked = true;

        PlacePoint p = VehicleLineManager.Instance.NextStopPoints;
        p.hasVehicle = true;
        //VehicleLineManager.Instance.AddVehicleToLine2(this);
        //MoveVehicle(p);
        MoveToScreenEdge(p);
    }

    void MoveToScreenEdge(PlacePoint p)
    {
        //// Lấy biên màn hình theo World (camera nhìn từ trên xuống)
        //float camHeight = Camera.main.transform.position.y;
        //Vector3 screenMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camHeight));
        //Vector3 screenMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camHeight));

        //// Lấy hướng của xe (chỉ tính theo XZ)
        //Vector3 dir = transform.right;
        //dir.y = 0f;
        //dir.Normalize();

        //Vector3 pos = transform.position;

        //// Tính khoảng cách đến biên theo X và Z
        //float tX = dir.x > 0 ? (screenMax.x - pos.x) / dir.x :
        //            dir.x < 0 ? (screenMin.x - pos.x) / dir.x : Mathf.Infinity;

        //float tZ = dir.z > 0 ? (screenMax.z - pos.z) / dir.z :
        //            dir.z < 0 ? (screenMin.z - pos.z) / dir.z : Mathf.Infinity;

        //// Lấy biên gần nhất
        //float tMin = Mathf.Min(Mathf.Abs(tX), Mathf.Abs(tZ));
        //Vector3 targetPos = pos + dir * tMin;

        //// Di chuyển đến vị trí biên
        //float distance = Vector3.Distance(pos, targetPos);
        //float duration = distance / speed;

        //if (targetPos.z < VehicleLineManager.Instance.bottomMounth.position.z)
        //{
        //    float t = (VehicleLineManager.Instance.bottomMounth.position.z - transform.position.z) / (targetPos.z - transform.position.z);
        //    // Tính vị trí tương ứng theo t
        //    Vector3 tgPos = Vector3.Lerp(transform.position, targetPos, t);
        //    transform.DOMove(tgPos, duration).SetEase(Ease.Linear).OnComplete(() =>
        //    {

        //    });
        //}
        //else
        //{
        //    transform.DOMove(targetPos, duration).SetEase(Ease.Linear).OnComplete(() =>
        //    {
        //        if (targetPos.z < p.subPoint1.position.z)
        //        {
        //            //di chuyển theo viền
        //        targetPos.z = p.subPoint1.position.z;
        //            MoveTo(targetPos, () =>
        //            {
        //            //tới subPoint1
        //            MoveTo(p.subPoint1.position, () =>
        //                    {
        //                    // tới điểm đỗ xe
        //                    MoveTo(p.Position, () =>
        //                        {
        //                        //VehicleLineManager.Instance.AddVehicleToLine2(this);
        //                        OnReach?.Invoke(this);
        //                            VehicleLineManager.Instance.AddVehicleToLine2(this);
        //                        });
        //                    });
        //            });
        //        }
        //    });
        //}

        Vector3 phai = GetPositionAtX(transform.position, transform.right, VehicleManager.Instance.right.transform.position.x);Debug.Log("OOOO : " + phai);
        Vector3 trai = GetPositionAtX(transform.position, transform.right, VehicleManager.Instance.left.transform.position.x); Debug.Log("OOOO : " + trai);
        Vector3 tren = GetPositionAtZ(transform.position, transform.right, VehicleManager.Instance.top.transform.position.z); Debug.Log("OOOO : " + tren);
        Vector3 duoi = GetPositionAtZ(transform.position, transform.right, VehicleManager.Instance.bottom.transform.position.z); Debug.Log("OOOO : " + duoi);

        if (phai.z < VehicleManager.Instance.top.transform.position.z && phai.z > VehicleManager.Instance.bottom.transform.position.z)
        {
            Vector3 tg = phai;
            transform.DOMove(tg, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (tg.z < p.subPoint1.position.z)
                {
                    //di chuyển theo viền
                    tg.z = p.subPoint1.position.z;
                    MoveTo(tg, () =>
                    {
                    //tới subPoint1
                    MoveTo(p.subPoint1.position, () =>
                            {
                            // tới điểm đỗ xe
                            MoveTo(p.Position, () =>
                                {
                                //VehicleLineManager.Instance.AddVehicleToLine2(this);
                                OnReach?.Invoke(this);
                                    VehicleLineManager.Instance.AddVehicleToLine2(this);
                                });
                            });
                    });
                }
            });
        }

        if (duoi.x > VehicleManager.Instance.left.transform.position.x && duoi.x < VehicleManager.Instance.right.transform.position.x)
        {
            Vector3 tg = duoi;
            transform.DOMove(tg, 1).SetEase(Ease.Linear).OnComplete(() =>
            {
                if(tg.x < VehicleManager.Instance.bottom.transform.position.x)
                {
                    tg.x = VehicleManager.Instance.left.transform.position.x;
                }
                else
                {
                    tg.x = VehicleManager.Instance.right.transform.position.x;
                }
                MoveTo(tg, () =>
                {
                    //di chuyển theo viền
                    tg.z = p.subPoint1.position.z;
                    MoveTo(tg, () =>
                    {
                        //tới subPoint1
                        MoveTo(p.subPoint1.position, () =>
                        {
                            // tới điểm đỗ xe
                            MoveTo(p.Position, () =>
                            {
                                //VehicleLineManager.Instance.AddVehicleToLine2(this);
                                OnReach?.Invoke(this);
                                VehicleLineManager.Instance.AddVehicleToLine2(this);
                            });
                        });
                    });
                });
            });
        }
    }

    private void MoveTo(Vector3 destination, TweenCallback onComplete)
    {
        Vector3 direction = (destination - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90f, 0);
        }
        //destination.y = transform.position.y; 
        float distance = Vector3.Distance(transform.position, destination);
        float duration = distance / speed;
        transform.DOMove(destination, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    //void MoveVehicle(PlacePoint _targetPlacePoint)
    //{
    //    moveTween?.Kill();

    //    // Di chuyển theo hướng hiện tại trong một khoảng ngắn
    //    Vector3 targetPos = transform.position + direction * moveStep;
    //    if (targetPos.z < _targetPlacePoint.subPoint1.position.z)
    //    {
    //        float duration = moveStep / speed;

    //        moveTween = transform.DOMove(targetPos, duration)
    //            .SetEase(Ease.Linear)
    //            .OnComplete(() =>
    //            {
    //                CheckBounds();
    //                MoveVehicle(_targetPlacePoint); // lặp lại
    //        });
    //    }
    //    else
    //    {
    //        targetPos = _targetPlacePoint.subPoint1.position;
    //        moveTween = transform.DOMove(targetPos, .5f)
    //            .SetEase(Ease.Linear)
    //            .OnComplete(() =>
    //            {
    //                targetPos = _targetPlacePoint.Position;
    //                moveTween = transform.DOMove(targetPos, .5f)
    //                    .SetEase(Ease.Linear)
    //                    .OnComplete(() =>
    //                    {

    //                    });
    //            });
    //    }
    //}

    //void CheckBounds()
    //{
    //    Vector3 viewPos = cam.WorldToViewportPoint(transform.position);

    //    // Chạm viền trái hoặc phải
    //    if (viewPos.x <= 0f && direction.x < 0)
    //    {
    //        //direction.x = Mathf.Abs(direction.x); // đổi hướng sang phải
    //        direction = Vector3.forward;
    //    }
    //    else if (viewPos.x >= 1f && direction.x > 0)
    //    {
    //        //direction.x = -Mathf.Abs(direction.x); // đổi hướng sang trái
    //        direction = Vector3.forward;
    //    }

    //    //// Chạm viền trên hoặc dưới
    //    //if (viewPos.y <= 0f && direction.y < 0)
    //    //{
    //    //    direction.y = Mathf.Abs(direction.y); // đi lên
    //    //}
    //    //else if (viewPos.y >= 1f && direction.y > 0)
    //    //{
    //    //    direction.y = -Mathf.Abs(direction.y); // đi xuống
    //    //}

    //    // Xoay xe theo hướng mới (nếu muốn)
    //    if (direction != Vector3.zero)
    //    {
    //        //transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.5f);
    //        Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90f, 0);
    //        transform.DORotateQuaternion(targetRotation, 0.5f);
    //    }
    //}

    Vector3 GetPositionAtZ(Vector3 currentPos, Vector3 direction, float targetZ)
    {
        // Tránh chia cho 0 nếu direction song song trục X
        if (Mathf.Abs(direction.z) < 0.0001f)
            return currentPos;

        float t = (targetZ - currentPos.z) / direction.z;
        Vector3 result = currentPos + direction.normalized * t;

        return result;
    }

    Vector3 GetPositionAtX(Vector3 currentPos, Vector3 direction, float targetX)
    {
        // Tránh chia cho 0 nếu direction song song trục X
        if (Mathf.Abs(direction.x) < 0.0001f)
            return currentPos;

        float t = (targetX - currentPos.x) / direction.x;
        Vector3 result = currentPos + direction.normalized * t;

        return result;
    }


}

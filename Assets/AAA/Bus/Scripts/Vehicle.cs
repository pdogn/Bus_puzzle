using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Burst.CompilerServices;

public class Vehicle : MonoBehaviour
{
    [SerializeField] private BusAttributes busAttributes;
    [SerializeField] private Animator bookerAnim;

    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private Transform Vehicle_4;
    [SerializeField] private Transform Vehicle_6;
    [SerializeField] private Transform Vehicle_8;

    public bool Clicked;
    public int bookerCount = 0;
    public int bookerSittingCount = 0;
    [SerializeField] public int maxSize = 3;

    [Header("Move properties")]
    public float speed = 45f;            // tốc độ (đơn vị / giây)
    //public Vector3 direction = Vector3.right; // hướng ban đầu
    public float moveStep = 1f;         // quãng đường mỗi tween (càng nhỏ càng mượt)

    private Tween moveTween;
    public float stopOffset = 0.2f;
    //public LayerMask obstacleLayer;

    public PlacePoint targetPlacePoint;

    public bool isLeaving;

    public BusAttributes Attributes => busAttributes;

    public event Action<Vehicle> OnReach;
    public event Action<Vehicle> OnExit;
    public event Action<Vehicle> OnEnd;

    
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

        targetPlacePoint = VehicleLineManager.Instance.NextStopPoints;
        //p.hasVehicle = true;
        //VehicleLineManager.Instance.AddVehicleToLine2(this);
        //MoveVehicle(p);
        //MoveToScreenEdge(p);
        MoveVehicle(targetPlacePoint);
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

    public void MoveVehicle(PlacePoint p)
    {
        if (moveTween != null && moveTween.IsActive()) moveTween.Kill();

        Vector3 startPos = transform.position;

        Vector3 dir = transform.right;
        // 🔹 Nhân với scale thật
        Vector3 halfExtents = Vector3.Scale(boxCollider.size * 0.5f, transform.lossyScale);

        Vector3 origin = transform.position + boxCollider.center/* - dir * 0.1f*/;
        origin.y = transform.position.y;

        float maxDistance = 50f;
        //LayerMask mask = LayerMask.GetMask("Default"); 

        RaycastHit[] hits = Physics.BoxCastAll(origin, halfExtents, dir, transform.rotation, maxDistance);

        if (hits.Length > 0)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var h in hits)
            {
                if (h.collider.gameObject == gameObject) continue;

                GameObject obstacle = h.collider.gameObject;

                // 🚗 Nếu trúng xe khác
                if (obstacle.CompareTag("Vehicle"))
                {
                    // Khoảng cách thật sự đến xe trước (theo hướng dir)
                    float hitDistance = h.distance;

                    // Giữ khoảng cách an toàn giữa 2 xe
                    float stopOffset = 0.3f;
                    float stopDistance = Mathf.Max(hitDistance - stopOffset, 0f);

                    // Vị trí dừng được tính từ vị trí ban đầu + hướng * khoảng cách
                    Vector3 hitStopPos = transform.position + dir * stopDistance;
                    hitStopPos.y = transform.position.y; // giữ nguyên độ cao

                    float duration = stopDistance / speed;

                    moveTween = transform.DOMove(hitStopPos, duration)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            ImpactOtherVehicle(obstacle, 10f);
                            DOVirtual.DelayedCall(0.2f, () =>  // dừng 0.2 giây
                            {
                                // Quay về vị trí cũ
                                transform.DOMove(startPos, duration)
                                .SetEase(Ease.InQuad).OnComplete(() => Clicked = false);
                            });
                        });

                    Debug.DrawLine(transform.position, hitStopPos, Color.red, 2f);
                    return;
                }

                // 🧱 Nếu trúng tường
                if (obstacle.CompareTag("Wall"))
                {
                    p.hadVehicle = true;

                    Vector3 stopPos = h.point - dir * 0.05f;
                    float distance = Vector3.Distance(transform.position, stopPos);
                    float duration = distance / speed;

                    moveTween = transform.DOMove(stopPos, duration)
                        .SetEase(Ease.Linear)
                        .OnComplete(() => 
                        {
                            if(obstacle.name == "right" || obstacle.name == "left" || obstacle.name == "Top")
                            {
                                Vector3 target = stopPos;
                                //di chuyển theo viền
                                target.z = p.subPoint1.position.z;
                                MoveTo(target, () =>
                                {
                                    //tới subPoint1
                                    MoveTo(p.subPoint1.position, () =>
                                    {
                                        // tới điểm đỗ xe
                                        MoveTo(p.Position, () =>
                                        {
                                            Debug.Log("NHan khack");
                                            //VehicleLineManager.Instance.AddVehicleToLine2(this);
                                            OnReach?.Invoke(this);
                                            VehicleLineManager.Instance.AddVehicleToLine2(this);
                                        });
                                    });
                                });
                            }

                            if (obstacle.name == "bot")
                            {
                                Vector3 target = stopPos;
                                //di chuyển theo viền
                                if (target.x < VehicleManager.Instance.bottom.position.x)
                                {
                                    target.x = VehicleManager.Instance.left.position.x;
                                }
                                else
                                {
                                    target.x = VehicleManager.Instance.right.position.x;
                                }
                                MoveTo(target, () =>
                                {
                                    target.z = p.subPoint1.position.z;
                                    MoveTo(target, () =>
                                    {
                                        //tới subPoint1
                                        MoveTo(p.subPoint1.position, () =>
                                        {
                                            // tới điểm đỗ xe
                                            MoveTo(p.Position, () =>
                                            {
                                                //VehicleLineManager.Instance.AddVehicleToLine2(this);
                                                Debug.Log("NHan khack");
                                                OnReach?.Invoke(this);Debug.Log("NHan khack2");
                                                VehicleLineManager.Instance.AddVehicleToLine2(this);
                                            });
                                        });
                                    });
                                });
                            }
                        });

                    Debug.DrawLine(transform.position, stopPos, Color.red, 2f);
                    return;
                }
            }
        }
        else
        {
            Debug.Log("Không có vật cản nào trước mặt.");
        }
    }

    //Xe rời đi
    public void MoveToExit()
    {
        DOTween.Kill(this.transform);
        MoveTo(targetPlacePoint.subPoint1.position, () =>
        {
            MoveTo(VehicleLineManager.Instance.ExitPoint.position, () =>
            {
                //Số xe còn lại chưa chở khách đi -1
                ManagerController.Instance.VehiclRemaining--;
            });
        });
    }

    void ImpactOtherVehicle(GameObject other, float punchStrength)
    {
        Vector3 impactDir = (this.transform.position - other.transform.position).normalized;
        Vector3 punchRotation = new Vector3(-impactDir.z, 0, impactDir.x) * punchStrength;
        other.transform.DOPunchRotation(punchRotation, 0.4f, 10, 1);
    }

    public void OnVehicleExit()
    {
        MoveToExit();
        VehicleLineManager.Instance.RemoveVehicleInLine(this);
        targetPlacePoint.hadVehicle = false;
    }

    public void SetTypeVehicle(int seatCount)
    {
        if (Vehicle_4 == null || Vehicle_6 == null || Vehicle_8 == null) return;

        switch (seatCount)
        {
            case 4://xe 4 chỗ
                Vehicle_4.gameObject.SetActive(true);
                Vehicle_6.gameObject.SetActive(false);
                Vehicle_8.gameObject.SetActive(false);
                boxCollider = Vehicle_4.gameObject.GetComponent<BoxCollider>();
                break;
            case 6://xe 6 chỗ
                Vehicle_4.gameObject.SetActive(false);
                Vehicle_6.gameObject.SetActive(true);
                Vehicle_8.gameObject.SetActive(false);
                boxCollider = Vehicle_6.gameObject.GetComponent<BoxCollider>();
                break;
            case 8://xe 8 chỗ
                Vehicle_4.gameObject.SetActive(false);
                Vehicle_6.gameObject.SetActive(false);
                Vehicle_8.gameObject.SetActive(true);
                boxCollider = Vehicle_8.gameObject.GetComponent<BoxCollider>();
                break;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();

        Vector3 dir = transform.right;
        Vector3 halfExtents = Vector3.Scale(boxCollider.size * 0.5f, transform.lossyScale);
        Vector3 origin = transform.position + boxCollider.center/* - dir * 0.1f*/;
        origin.y = transform.position.y;
        float maxDistance = 50f;

        Gizmos.color = new Color(0, 1, 1, 0.25f);
        Matrix4x4 matrix = Matrix4x4.TRS(origin, transform.rotation, Vector3.one);
        Gizmos.matrix = matrix;
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + dir * maxDistance);

        // Vẽ hộp tại vị trí chạm
        RaycastHit[] hits = Physics.BoxCastAll(origin, halfExtents, dir, transform.rotation, maxDistance);
        foreach (var h in hits)
        {
            Vector3 hitCenter = origin + dir * h.distance;
            Matrix4x4 hitMatrix = Matrix4x4.TRS(hitCenter, transform.rotation, Vector3.one);
            Gizmos.matrix = hitMatrix;
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2f);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.DrawSphere(h.point, 0.05f);
        }
    }
#endif

}

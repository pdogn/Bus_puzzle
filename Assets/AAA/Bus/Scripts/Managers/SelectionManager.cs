using DG.Tweening;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Transform line;
    
    [SerializeField] private Camera mainCamera;
    public LayerMask selectionMask;

    [SerializeField] private AudioSource audioSource;
    

    [SerializeField] private short clickedBookerCount;
    
    public static SelectionManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);

        else
            Instance = this;
    }

    public short ClickedBookerCount
    {
        get => clickedBookerCount;
        set => clickedBookerCount = value;
    }

    public void HandleClick(Vector3 clickPos)
    {
        GameObject result;
        
        if (!FindTarget(clickPos, out result)) return;
        
        var vehicle = result.GetComponent<Vehicle>();
        //if(LineManager.Instance.LinePlaces.Count == clickedBookerCount) return;

        //Vibrate 
        Vibration.Vibrate(100);

        if (vehicle.Clicked == true) return;
        //if (!vehicle.FindAPath())
        //{
        //    Debug.Log("Not find path");
        //    vehicle.transform.DOShakeRotation(0.2f,
        //        new Vector3(0f, 30f, 0f), 1, 0);
        //}
        //else
        //{
        //    Debug.Log("Goooo");
        //}
        //vehicle.MoveVehicleWithPath();

        if (!vehicle.FindAPath())
        {
            Debug.Log("Not find path");
            vehicle.transform.DOShakeRotation(0.2f,
                new Vector3(0f, 30f, 0f), 1, 0);
        }
        else
        {
            Debug.Log("Goooo");
        }
    }

    private bool FindTarget(Vector3 mousePosition, out GameObject result)
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out hit, 100, selectionMask))
        {
            result = hit.collider.gameObject;
            return true; 
        }

        result = null;
        return false;
    }

    //public void BookerEndsItsPath(Booker booker)
    //{
    //    clickedBookerCount--;
    //    booker.OnExit -= BookerEndsItsPath;
    //}
    
    
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerController : MonoBehaviour
{
    [SerializeField] private LevelCatalogSO levelCatalog;

    private LevelDataSO currentLevel;

    //Số người còn lại
    private int bookerRemaining;
    public int BookerRemaining
    {
        get { return bookerRemaining; }
        set
        {
            if (bookerRemaining != value)
            {
                bookerRemaining = value;
            }
        }
    }

    //Số xe còn lại
    private int vehicleRemaining;
    public int VehiclRemaining
    {
        get { return vehicleRemaining; }
        set
        {
            if (vehicleRemaining != value)
            {
                vehicleRemaining = value;
                if (value == 0)
                {
                    Debug.Log("Win game");
                    UIManager.Instance.ShowWinUI();
                }
            }
        }
    }

    public static ManagerController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void LoadGameData()
    {
        ClearLevel();

        //var level = levelCatalog.gameLevels[lv];
        var level = levelCatalog.gameLevels[LevelManager.Instance.CurrentLevel];
        currentLevel = level;
        //LevelCount.SetText((LevelManager.Instance.CurrentLevel + 1).ToString());

        BookerManager.Instance.SetLevelData(level);
        VehicleManager.Instance.SetLevelData(level);
        SelectionManager.Instance.ClickedBookerCount = 0;

        InitializeLevels();
    }

    private void InitializeLevels()
    {
        BookerManager.Instance.InitializeLevel();
        VehicleManager.Instance.InitializeLevel();

        BookerRemaining = currentLevel.bookerColorList.Count;
        VehiclRemaining = currentLevel.VehicleColorMap.Count;
    }

    private void ClearLevel()
    {
        BookerLineManager.Instance.ClearLevel();
        VehicleLineManager.Instance.ClearLevel();
        VehicleManager.Instance.ClearVehicle();
        BookerManager.Instance.ClearBookers();
        //ClearTiles();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerController : MonoBehaviour
{
    [SerializeField] private LevelCatalogSO levelCatalog;

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
    }

    private void ClearLevel()
    {
        //LineManager.Instance.ClearLevel();
        //VehicleManager.Instance.ClearVehicle();
        //BookerManager.Instance.ClearBookers();
        //ClearTiles();

    }
}

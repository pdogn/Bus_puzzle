using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookerManager : MonoBehaviour
{
    [Header("Booker prefabs")]
    [SerializeField] private BookerContainer bookerContainer;
    private Dictionary<GameColors, Booker> DictType;

    public List<Booker> bookers = new List<Booker>();

    private LevelDataSO _levelDataSo;

    public static BookerManager Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
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
        SetDictTypeOfBooker();

    }

    private void Update()
    {
        
    }

    public void InitializeLevel()
    {
        GenerateBooker();
    }

    public void SetLevelData(LevelDataSO levelData) { _levelDataSo = levelData; }

    private void GenerateBooker()
    {
        for(int i=0; i<20; i++)
        {
            Booker newBooker;
            var initialPos = BookerLineManager.Instance.StandPoints[i].position;
            newBooker = Instantiate(DictType[GameColors.LILAC], initialPos, Quaternion.identity, transform);

            if (i == 0)
            {
                BookerLineManager.Instance.firstbooker = newBooker;
            }

            bookers.Add(newBooker);
            newBooker.crrIndex = i;
            newBooker.OnReach += BookerLineManager.Instance.HandleBookerReach;
        }
    }

    void SetDictTypeOfBooker()
    {
        DictType = new Dictionary<GameColors, Booker>();
        foreach (var x in bookerContainer.bookerCatalog)
        {
            DictType[x.bookerColor] = x.booker;
        }
    }
}

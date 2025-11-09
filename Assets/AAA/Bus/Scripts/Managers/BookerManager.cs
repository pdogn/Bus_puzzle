using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BookerManager : MonoBehaviour
{
    [Header("Booker prefabs")]
    [SerializeField] private BookerContainer bookerContainer;
    private Dictionary<GameColors, Booker> DictType;

    public List<Booker> bookers = new List<Booker>();

    private LevelDataSO _levelDataSo;

    private Queue<Booker> poolBooker = new Queue<Booker>();
    [SerializeField] private Transform _pooling;

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
        InitPool(10);
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

    public void RemoveBookerInLine(Booker booker)
    {
        bookers.Remove(booker);
    }

    public void AddBookerToLine(Booker booker)
    {
        bookers.Add(booker);
    }

    void InitPool(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            Booker newBooker = Instantiate(DictType[GameColors.BLUE], _pooling);
            newBooker.gameObject.SetActive(false);
            newBooker.OnReach += BookerLineManager.Instance.HandleBookerReach;
            poolBooker.Enqueue(newBooker);
        }
    }

    public Booker GetBookerInPool()
    {
        if (poolBooker.Count > 0)
        {
            Booker booker = poolBooker.Dequeue();
            booker.gameObject.SetActive(true);
            booker.gameObject.transform.parent = this.transform;
            return booker;
        }
        Booker newBooker = Instantiate(DictType[GameColors.BLUE], this.transform);
        return newBooker;
    }

    public void ReturnToPool(Booker booker)
    {
        booker.gameObject.SetActive(false);
        booker.gameObject.transform.parent = _pooling;
        poolBooker.Enqueue(booker);
    }

}

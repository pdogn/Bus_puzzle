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

    private Queue<GameColors> _bookerColorsQueue = new Queue<GameColors>();

    private List<Booker> poolBooker = new List<Booker>();
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
        //InitPool(10);
    }

    private void Update()
    {
        
    }

    public void InitializeLevel()
    {
        InitBookerColorsQueue();
        GenerateBooker();
        InitPool(10);
    }

    public void SetLevelData(LevelDataSO levelData) { _levelDataSo = levelData; }


    private void InitBookerColorsQueue()
    {
        foreach(var b in _levelDataSo.bookerColorList)
        {
            _bookerColorsQueue.Enqueue(b);
        }
    }

    private void GenerateBooker()
    {
        for(int i=0; i<20; i++)
        {
            Booker newBooker;
            var initialPos = BookerLineManager.Instance.StandPoints[i].position;
            GameColors bookerColor = _bookerColorsQueue.Dequeue();
            newBooker = Instantiate(DictType[bookerColor], initialPos, Quaternion.identity, transform);

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

    public bool BookerColorQueueNotEmpty()
    {
        return _bookerColorsQueue.Count > 0;
    }

    public GameColors GetNextColorInQueue()
    {
        GameColors cl = _bookerColorsQueue.Dequeue();
        Debug.Log("oclor: " + cl);
        return cl;
    }

    void InitPool(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            Booker newBooker = Instantiate(DictType[GameColors.BLUE], _pooling);
            newBooker.gameObject.SetActive(false);
            newBooker.OnReach += BookerLineManager.Instance.HandleBookerReach;
            poolBooker.Add(newBooker);
        }
    }

    public Booker GetBookerInPool(GameColors color)
    {
        //if (poolBooker.Count > 0)
        //{
        //    //Booker booker = poolBooker.Dequeue();
        //    //booker.gameObject.SetActive(true);
        //    //booker.gameObject.transform.parent = this.transform;
        //    //return booker;
        //    foreach(var booker in poolBooker)
        //    {
        //        if(color == booker.Attributes.bookerColor)
        //            return booker;
        //    }
        //}
        foreach (var booker in poolBooker)
        {
            if (color == booker.Attributes.bookerColor)
                return booker;
        }
        Booker newBooker = Instantiate(DictType[color], this.transform);
        newBooker.OnReach += BookerLineManager.Instance.HandleBookerReach;
        return newBooker;
    }

    public void ReturnToPool(Booker booker)
    {
        booker.gameObject.SetActive(false);
        booker.gameObject.transform.parent = _pooling;
        poolBooker.Add(booker);
    }


    public void ClearBookers()
    {
        _bookerColorsQueue.Clear();
        
        foreach(var b in bookers)
        {
            Destroy(b.gameObject);
        }
        bookers?.Clear();

        foreach(var b in poolBooker)
        {
            Destroy(b.gameObject);
        }
        poolBooker?.Clear();
    }
}

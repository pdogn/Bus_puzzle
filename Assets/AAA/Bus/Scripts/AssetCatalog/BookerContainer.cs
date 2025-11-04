using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Booker Catalog", menuName = "ScriptableObjects/Catalogs/Booker")]
public class BookerContainer : ScriptableObject
{
    public List<BookerDes> bookerCatalog;
}

[Serializable]
public class BookerDes
{
    public GameColors bookerColor;
    public Booker booker;
}

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bus Catalog", menuName = "ScriptableObjects/Catalogs/Bus")]
public class BusContainer : ScriptableObject
{
    public List<BusDes> busCatalog;
}

[Serializable]
public class BusDes
{
    public GameColors busColor;
    public Vehicle bus;
}

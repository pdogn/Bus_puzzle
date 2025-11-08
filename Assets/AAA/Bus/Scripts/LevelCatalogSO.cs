using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelCatalog", menuName = "ScriptableObjects/LevelCatalog")]
public class LevelCatalogSO : ScriptableObject
{
    public List<LevelDataSO> gameLevels;
}

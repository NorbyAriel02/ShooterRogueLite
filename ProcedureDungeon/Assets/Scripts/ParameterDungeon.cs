using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data Dungeon", menuName = "Dungeon/Data Dungeon", order = 1)]
public class ParameterDungeon : ScriptableObject
{
    public int index = 5;
    public int count = 0;
    public List<GameObject> roomList;
}

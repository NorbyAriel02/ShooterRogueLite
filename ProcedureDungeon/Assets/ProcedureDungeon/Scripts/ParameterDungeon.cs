using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Data Dungeon", menuName = "Dungeon/Data Dungeon", order = 1)]
public class ParameterDungeon : ScriptableObject
{
    public int index = 5;
    public int count = 0;
    public List<GameObject> roomList;
    public List<GameObject> doorList;
    [Header("End Dungeon")]
    public GameObject EndDungeon;
    public float offPos = 2f;
    public bool ending = false;
    public GameObject KeyDungeon;
    public bool IsKeySpawned;
    public GameObject parentRooms;
    public GameObject parentDoors;
    public GameObject prefabDebug;
    public float deltaCenter = 10;
    public float sizeBoxCast = 10;
}

using System;
using UnityEngine;

[Serializable]
public class FacilityData
{
    public string id;
    public ItemType type;
    public Vector3 position;
    public bool isBuilding;
    public string finishedBuildingTime;

    public FacilityData()
    {
    }

    public FacilityData(string id, ItemType type, Vector3 position,
        bool isBuilding, string finishedBuildingTime)
    {
        this.id = id;
        this.type = type;
        this.position = position;
        this.isBuilding = isBuilding;
        this.finishedBuildingTime = finishedBuildingTime;
    }
}
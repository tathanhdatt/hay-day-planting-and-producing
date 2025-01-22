using System;
using System.Collections.Generic;

[Serializable]
public class ProducibleFacilityData : FacilityData
{
    public string finishedProducingTime;
    public List<string> productNames = new List<string>(5);
    public List<string> collectibleProductNames = new List<string>(5);

    public ProducibleFacilityData()
    {
    }

    public ProducibleFacilityData(FacilityData facilityData)
    {
        this.id = facilityData.id;
        this.type = facilityData.type;
        this.position = facilityData.position;
        this.isBuilding = facilityData.isBuilding;
        this.finishedBuildingTime = facilityData.finishedBuildingTime;
    }
}
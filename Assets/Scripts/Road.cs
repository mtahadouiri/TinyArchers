using UnityEngine;

[System.Serializable]
public class Road {
    
    public GameObject RoadGO;
    public GameObject RoadCameraPivotGO;
    public int RoadIndex;

    public Road(GameObject roadGO, GameObject roadCameraPivotGO, int roadIndex)
    {
        RoadGO = roadGO;
        RoadCameraPivotGO = roadCameraPivotGO;
        RoadIndex = roadIndex;
    }
}

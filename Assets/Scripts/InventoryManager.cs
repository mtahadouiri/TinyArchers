using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance = null;
    Dictionary<ArrowType,int> arrowsDictionnary;


    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
        arrowsDictionnary = new Dictionary<ArrowType, int>();
    }

    void Start()
    {
        LoadFromList();
    }

    public void AddArrow(ArrowType arrowType)
    {
        Debug.Log("Adding arrow to inventory");
        SaveToJson();
    }

    public void RemoveArrow(ArrowType arrowType)
    {
        Debug.Log("Removing arrow from inventory");
        int current = GetArrowCount(arrowType);
        current--;
        arrowsDictionnary[arrowType] = current;
        SaveToJson();
    }

    public void SaveToJson()
    {
        Debug.Log("Saving inventory to json");

    }

    public void LoadFromJson()
    {
        Debug.Log("Loading inventory from json");
        arrowsDictionnary = new Dictionary<ArrowType, int>();
    }
    public void LoadFromList()
    {
        Debug.Log("Loading inventory from list");
        arrowsDictionnary = new Dictionary<ArrowType, int>
        {
            { ArrowType.NoramlArrow, 50 },
            { ArrowType.PoisonArrow, 1 },
            { ArrowType.FireArrow, 1 },
            { ArrowType.IceArrow, 1 }
        };
    }

    public int GetArrowCount(ArrowType arrowType)
    {
        int res;
        if (arrowsDictionnary.TryGetValue(arrowType, out res))
        {
            return res;
        }
        return 0;
    }

    public int GetArrowCount()
    {
        return arrowsDictionnary.Values.Count;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum SpawnType
{
    Time, Death
}
public enum ControlMode
{
    Joystick, Buttons
}
public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    [SerializeField]
    TextAsset lvlFile;
    public int lvlNumber;
    [SerializeField]
    Wave wave;
    public GameObject[] roads;
    public GameObject[] roadPivots;
    public GameObject[] roadIndicators;

    public List<GameObject> roadPivotsList;
    public List<GameObject> roadIndicatorsList;
    public List<GameObject> newRoads;
    public GameObject[] roadsSpawn;
    public GameObject[] roadsQueue;
    public Transform poolSpawn;
    public GameObject cameraGO;
    public GameObject activeRoad;
    //public int waveCount;
    public int targetIndex;
    public GameObject minionPF;
    public GameObject giantPF;
    public List<GameObject> enemies;
    public GameObject player;
    public ArrowType selectedArrow = ArrowType.NoramlArrow;
    //public GameObject bow;
    public GameObject towerTarget;
    public GameObject controlButtons;
    public GameObject joystickPanel;
    public float _sensitivity;
    public ControlMode controlMode;
    public Joystick joystick;
    //Vector3 _rotation;
    public bool spawnReady;
    public int enemyIndex;
    float minDistance = 100;
    public SpawnType spawnType;
    public float spawnInterval;
    bool aimReady;
    public bool gameOver;
    public int enemiesRemain;

    public LaunchArcRenderer launchArcRenderer;


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
    }
    // Use this for initialization
    void Start()
    {
        /*wave = new Wave(10, new List<string>());
        wave.Enemies.Add("Taha");
        wave.Enemies.Add("Sata");
        string json = JsonUtility.ToJson(wave);
        WriteDataToFile(json);*/
        string pathToLevel = Application.dataPath + "/Levels/"+lvlNumber+".json";
        StreamReader reader = new StreamReader(pathToLevel); 
        wave = JsonUtility.FromJson<Wave>(reader.ReadToEnd());


        _sensitivity = 0.4f;
        targetIndex = -1;
        aimReady = true;
        //_rotation = Vector3.zero;
        enemies = new List<GameObject>();
        roadsQueue = new GameObject[wave.WaveCapacity];
        enemiesRemain = wave.WaveCapacity;



        switch (controlMode)
        {
            case ControlMode.Buttons:
                ShowControlButtons();
                DeActivateJoystick();
                break;
            case ControlMode.Joystick:
                HideControlButtons();
                ActivateJoystick();
                break;
        }

        PrepareRoads();
        PrepareSpawnWave();

    }

    // Update is called once per frame
    void Update()
    {
        gameOver |= enemiesRemain <= 0;
        if (!gameOver)
        {
            if (spawnReady && spawnType == SpawnType.Death)
            {
                PlaceEnemy(enemyIndex);
                enemyIndex++;
                spawnReady = false;
            }

        }
    }

    void PrepareSpawnWave()
    {

        for (int i = 0; i < wave.WaveCapacity; i++)
        {
            GameObject enemy;
            int road = UnityEngine.Random.Range(0, roadsSpawn.Length);
            switch (wave.Enemies[i])
            {
                case "minion":
                    enemy = Instantiate(minionPF, poolSpawn.position, Quaternion.identity);
                    enemy.GetComponent<Enemy>().spawn = roadsSpawn[road];
                    enemy.GetComponent<Enemy>().index = i;
                    enemy.GetComponent<Enemy>().road = new Road(roadsSpawn[road], roadPivotsList[road], road);
                    enemy.SetActive(false);
                    enemies.Add(enemy);
                    break;
                case "giant":
                    enemy = Instantiate(giantPF, poolSpawn.position, Quaternion.identity);
                    enemy.GetComponent<Enemy>().spawn = roadsSpawn[road];
                    enemy.GetComponent<Enemy>().index = i;
                    enemy.GetComponent<Enemy>().road = new Road(roadsSpawn[road], roadPivotsList[road], road);
                    enemy.SetActive(false);
                    enemies.Add(enemy);
                    break;
            }

        }

        if (spawnType == SpawnType.Time)
        {
            aimReady = true;
            StartCoroutine(Spawn(spawnInterval));
        }
        else
            spawnReady = true;

    }

    void PlaceEnemy(int index)
    {
        enemies[index].transform.position = enemies[index].GetComponent<Enemy>().spawn.transform.position;
        enemies[index].SetActive(true);
        if (spawnType == SpawnType.Death)
            activeRoad = enemies[index].GetComponent<Enemy>().spawn;
        else
        {
            roadsQueue[index] = enemies[index].GetComponent<Enemy>().spawn;
            activeRoad = roadsQueue[0];
        }
        ActivateRoadIndicator(enemies[index].GetComponent<Enemy>().road.RoadIndex);
        cameraGO.GetComponent<CameraScript>().GoToPivot(enemies[index].GetComponent<Enemy>().road.RoadCameraPivotGO);
        spawnReady = false;
    }

    void ActivateRoadIndicator(int roadIndex)
    {
        for (int i = 0; i < roadIndicators.Length; i++)
        {
            if (i != roadIndex)
                roadIndicators[i].SetActive(false);
            else
                roadIndicators[i].SetActive(true);
        }
    }

    public void AimAtNextIndex()
    {
        targetIndex++;
        if (targetIndex <= roadsQueue.Length)
            activeRoad = roadsQueue[targetIndex];
    }

    public void AimAtClosest()
    {
        foreach (GameObject item in enemies)
        {
            float dist = Vector3.Distance(item.transform.position, transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                activeRoad = item.GetComponent<Enemy>().spawn;
            }
        }
    }


    public void RemoveEnemyFromArray(int index)
    {
        enemies.RemoveAt(index);
    }

    IEnumerator Spawn(float waitTime)
    {
        while (enemyIndex < enemies.Count)
        {
            if (aimReady)
            {
                AimAtClosest();
                aimReady = false;
            }
            PlaceEnemy(enemyIndex);
            enemyIndex++;
            yield return new WaitForSeconds(waitTime);
        }
    }
    public void HideControlButtons()
    {
        controlButtons.SetActive(false);
    }

    public void ShowControlButtons()
    {
        controlButtons.SetActive(true);
    }

    public void ActivateJoystick()
    {
        joystickPanel.SetActive(true);
    }

    public void DeActivateJoystick()
    {
        joystickPanel.SetActive(false);
    }

    public void PrepareRoads()
    {
        int i = 0;
        newRoads = new List<GameObject>();
        roadPivotsList = new List<GameObject>();
        roadIndicatorsList = new List<GameObject>();

        if ((PlayerPrefs.GetInt("road1")) != 0)
        {
            roads[0].SetActive(true);
            newRoads.Add(roads[0]);
            roadIndicatorsList.Add(roadIndicators[0]);
            roadPivotsList.Add(roadPivots[0]);
        }
        if ((PlayerPrefs.GetInt("road2")) != 0)
        {
            roads[1].SetActive(true);
            newRoads.Add(roads[1]);
            roadIndicatorsList.Add(roadIndicators[1]);
            roadPivotsList.Add(roadPivots[1]);

        }
        if ((PlayerPrefs.GetInt("road3")) != 0)
        {
            roads[2].SetActive(true);
            newRoads.Add(roads[2]);
            roadIndicatorsList.Add(roadIndicators[2]);
            roadPivotsList.Add(roadPivots[2]);
        }

        roadsSpawn = new GameObject[newRoads.Count];
        roadIndicators = new GameObject[roadIndicatorsList.Count];

        foreach (GameObject item in newRoads)
        {
            roadsSpawn[i] = item.transform.Find("Spawn").gameObject;
            i++;
        }
        i = 0;
        foreach (GameObject item in roadIndicatorsList)
        {
            roadIndicators[i] = item;
            i++;
        }
        enemyIndex = 0;
    }

    public void JoystickRelease()
    {
        player.GetComponent<PlayerScript>().ShootArrow(selectedArrow);
    }

    public void NormalArrowClicked(){
        selectedArrow = ArrowType.NoramlArrow;
    }

    public void FireArrowClicked()
    {
        selectedArrow = ArrowType.FireArrow;
    }

    public void PoisonArrowClicked()
    {
        selectedArrow = ArrowType.PoisonArrow;
    }

    public void IceArrowClicked()
    {
        selectedArrow = ArrowType.IceArrow;
    }

    public static void WriteDataToFile(string jsonString)
    {
        string path = Application.dataPath + "/LevelSample.json";
        Debug.Log("AssetPath:" + jsonString);
        File.WriteAllText(path, jsonString);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

}

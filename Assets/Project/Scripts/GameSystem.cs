using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum FoodType
{
    Food1,
    Food2,
    Food3
}

public enum RemoveType
{
    Destroy,
    Drop,
    Finish,
    Oven
}

[System.Serializable]
public class FoodValues
{
    public FoodType foodType;
    public GameObject foodObject;
}

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance;
    [SerializeField] private FoodValues[] foodValues;
    [SerializeField] private GameObject[] plateObjects;
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform mapParent;
    [SerializeField] private GameObject obstacleObject;
    [SerializeField] private GameObject gateObject;
    [SerializeField] private Transform startObject;
    [SerializeField] private Transform finishObject;
    private int[] collectedPlates = {0,0,0};

    void Awake()
    {
        Application.targetFrameRate = 30;
        Instance = this;
    }
    void Start()
    {
        //InvokeRepeating("SpawnFood",0,Globals.GetSpawnRate());
        CreateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateLevel()
    {
        foreach(Transform tf in mapParent)
        {
            if(!tf.name.Contains("Player"))
            {
                Destroy(tf.gameObject);
            }
        }

        string level = Resources.Load<TextAsset>($"Levels/{Globals.GetCurrentLevel()+1}").ToString();

        string[] levelValues = level.Split('\n');

        foreach(string item in levelValues)
        {
            if(item.Length > 3)
            {
                StringToObject(item);
            }
            
            //StringToObject(reader.ReadLine());
        }

    }

    void StringToObject(string val)
    {
        string objName = val.Split(';')[0];
        Vector3 objPos = new Vector3(float.Parse(val.Split(';')[1]),float.Parse(val.Split(';')[2]),float.Parse(val.Split(';')[3]));

        if(objName == "start")
        {
            startObject.position = objPos;

        }
        else if(objName == "finish")
        {
            finishObject.position = objPos;
        }
        else if(objName.Contains("o"))
        {
            int obstacleNo = int.Parse(objName[1].ToString());
            GameObject cloneObstacle = Instantiate(obstacleObject.gameObject, objPos, Quaternion.identity);

            cloneObstacle.GetComponent<ObstacleController>().Init((ObstacleType)obstacleNo);

            cloneObstacle.transform.SetParent(mapParent);
 
        }
        else if(objName.Contains("f"))
        {
            int foodNo = int.Parse(objName[1].ToString());
            SpawnFood(objPos,foodNo);
        }
        else if(objName.Contains("g"))
        {
            int gateNo = int.Parse(objName[1].ToString());
            GameObject cloneGate = Instantiate(gateObject.gameObject, objPos, Quaternion.identity);

            for(int i=0; i<cloneGate.transform.childCount; i++)
            {
                bool status = i == gateNo;
                cloneGate.transform.GetChild(i).gameObject.SetActive(status);
            }

            cloneGate.transform.SetParent(mapParent);
        }
    }

    void SpawnFood(Vector3 pos, int foodLevel)
    {
        Vector3 foodPos = pos;

        GameObject cloneFood = (GameObject)Instantiate(GetFoodObject(),foodPos,Quaternion.identity);
        cloneFood.GetComponent<FoodStackItem>().Init(0,transform, Vector3.zero,true,foodLevel);
        cloneFood.transform.SetParent(mapParent);


    }

    public PlayerController GetPlayer()
    {
        return player;
    }

    public GameObject GetFoodObject()
    {
        GameObject foodObject = foodValues[0].foodObject;

        foreach(FoodValues values in foodValues)
        {
            if(values.foodType == Globals.GetCurrentFoodType()) foodObject = values.foodObject;
        }
        return foodObject;
    }



    public void AddPlate(int level)
    {
        collectedPlates[level]++;
    }

    public void Finish()
    {
        StartCoroutine(FinishEffect());
    }

    IEnumerator FinishEffect()
    {
        int totalPlate = 0;
        for(int plate=0; plate<3; plate++)
        {
            for(int i=0; i<collectedPlates[plate]; i++)
            {
                player.transform.position += new Vector3(0,Globals.GetPlateSize(),0);
                Vector3 platePos = player.transform.position;
                platePos.y = totalPlate * Globals.GetPlateSize();
                GameObject clonePlate = Instantiate(plateObjects[plate],platePos,plateObjects[plate].transform.rotation);
                yield return new WaitForSeconds(0.15f);

                totalPlate++;
                

            }
            player.transform.position += new Vector3(0,Globals.GetPlateSize(),0) / 2;
        }
    }


}



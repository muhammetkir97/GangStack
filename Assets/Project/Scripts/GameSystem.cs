using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private DragZone dragZone;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private TextMeshProUGUI txtScore;
    [SerializeField] private Button btnPause;
    [SerializeField] private GameObject fingerIcon;


    
    

    [Header("Map Objects")]
    [SerializeField] private Transform mapParent;
    [SerializeField] private Transform startObject;
    [SerializeField] private Transform finishObject;
    [SerializeField] private GameObject obstacleObject;
    [SerializeField] private GameObject gateObject;

    [Header("Finish Score Board")]
    [SerializeField] private Transform finishScoreMultiplierParent;
    [SerializeField] private Color[] gradientColors;

    private int collectedPlates = 0;
    private bool isStarted = false;

    void Awake()
    {
        Application.targetFrameRate = 30;
        Instance = this;
    }

    
    void Start()
    {
        //InvokeRepeating("SpawnFood",0,Globals.GetSpawnRate());
        CreateLevel();

        btnPause.onClick.AddListener(clickPuaseGame);
        txtLevel.text = $"Level {(Globals.GetCurrentLevel() + 1).ToString()}";
        iTween.MoveBy(fingerIcon,iTween.Hash("amount",Vector3.right * 50,"time", 1f, "easetype", iTween.EaseType.linear,"looptype",iTween.LoopType.pingPong));


    }

    // Update is called once per frame
    void Update()
    {
        if(!isStarted && dragZone.GetDragDelta().magnitude > 0) StartGame();
    }

    void StartGame()
    {
        isStarted = true;
        iTween.Stop(fingerIcon);
        iTween.ScaleTo(fingerIcon,Vector3.zero,0.3f);
        player.StartCharacter();
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
            finishScoreMultiplierParent.position = objPos + (Vector3.forward * 10f);
            CreateFinishBoard();

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

    void CreateFinishBoard()
    {
        GameObject boardObject = finishScoreMultiplierParent.GetChild(0).gameObject;
        int boardCount = Globals.GetFinishScoreBoardCount();
        float boardScore = 1.1f;
        for(int i=0; i<boardCount; i++)
        {
            Vector3 pos = new Vector3(0,2 * (i+1),0);
            GameObject clone = Instantiate(boardObject,finishScoreMultiplierParent);
            clone.transform.localPosition = pos;
            string boardText = string.Format("{0:0.0}", boardScore) + " X";
            boardText = boardText.Replace(',','.');
            clone.transform.GetChild(1).GetComponent<TextMeshPro>().text = boardText;

            boardScore += 0.1f;
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
        //collectedPlates += level+1;
        collectedPlates += 1;
        txtScore.text = collectedPlates.ToString();
    }

    public void Finish()
    {
        //iTween.MoveBy(Camera.main.transform.parent.gameObject,iTween.Hash("amount", Vector3.forward * -5,"time", 5f,"islocal",true));
        StartCoroutine(FinishEffect());
    }

    IEnumerator FinishEffect()
    {
        int cnt = 0;
        float totalPlayerHeight = Globals.GetPlateSize() / 2;
        int totalPlate = 0;
        int lastSelectedBoard = 0;
        iTween.MoveBy(finishScoreMultiplierParent.transform.GetChild(0).gameObject,Vector3.forward * -1.5f ,0.3f);
        int colorNumber = 1;

        finishScoreMultiplierParent.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.color = Color.Lerp(gradientColors[colorNumber-1], gradientColors[colorNumber], 0);
        player.transform.position += new Vector3(0,Globals.GetPlateSize(),0) / 2;
        //iTween.MoveBy(player.gameObject,iTween.Hash("amount",new Vector3(0,Globals.GetPlateSize() * collectedPlates,0), "time",0.15f * collectedPlates,"easetype",iTween.EaseType.easeOutSine));
        iTween.MoveBy(player.gameObject,iTween.Hash("amount",new Vector3(0,Globals.GetPlateSize() * collectedPlates,0), "time", 0.05f * collectedPlates,"easetype",iTween.EaseType.easeOutSine));    
        while(totalPlate < collectedPlates)
        {
            if(totalPlate * Globals.GetPlateSize() < player.transform.position.y - Globals.GetPlateSize())
            {
                //iTween.MoveBy(player.gameObject, new Vector3(0,Globals.GetPlateSize(),0), 0.1f);
                totalPlayerHeight += Globals.GetPlateSize();

                int newBoard = Mathf.RoundToInt((totalPlayerHeight - (totalPlayerHeight % 2f)) / 2);
                if(newBoard != lastSelectedBoard)
                {
                    iTween.MoveBy(finishScoreMultiplierParent.transform.GetChild(lastSelectedBoard).gameObject,Vector3.forward * 1.5f,0.3f);
                    lastSelectedBoard = newBoard;
                    iTween.MoveBy(finishScoreMultiplierParent.transform.GetChild(lastSelectedBoard).gameObject,Vector3.forward * -1.5f,0.3f);

                    colorNumber = Mathf.RoundToInt((float)(lastSelectedBoard - (lastSelectedBoard%10)) / 10) + 1;
                    finishScoreMultiplierParent.transform.GetChild(lastSelectedBoard).GetChild(0).GetComponent<MeshRenderer>().material.color = Color.Lerp(gradientColors[colorNumber-1], gradientColors[colorNumber], (float)(lastSelectedBoard%10)/10);
                }


                Vector3 platePos = player.transform.position;
                platePos.y = totalPlate * Globals.GetPlateSize();
                GameObject clonePlate = Instantiate(plateObjects[0],platePos,plateObjects[0].transform.rotation);
                clonePlate.transform.localScale = Vector3.zero;
                iTween.ScaleTo(clonePlate,Vector3.one * 0.11f, 0.3f);

                totalPlate++;
            }
            cnt++;
            if(cnt > 6)
            {
                cnt = 0;
                
//yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
            /*
            for(int i=0; i<collectedPlates; i++)
            {
                //iTween.MoveBy(player.gameObject, new Vector3(0,Globals.GetPlateSize(),0), 0.1f);
                totalPlayerHeight += Globals.GetPlateSize();

                int newBoard = Mathf.RoundToInt((totalPlayerHeight - (totalPlayerHeight % 2f)) / 2);
                if(newBoard != lastSelectedBoard)
                {
                    iTween.MoveBy(finishScoreMultiplierParent.transform.GetChild(lastSelectedBoard).gameObject,Vector3.forward * 1.5f,0.3f);
                    lastSelectedBoard = newBoard;
                    iTween.MoveBy(finishScoreMultiplierParent.transform.GetChild(lastSelectedBoard).gameObject,Vector3.forward * -1.5f,0.3f);

                    colorNumber = Mathf.RoundToInt((float)(lastSelectedBoard - (lastSelectedBoard%10)) / 10) + 1;
                    finishScoreMultiplierParent.transform.GetChild(lastSelectedBoard).GetChild(0).GetComponent<MeshRenderer>().material.color = Color.Lerp(gradientColors[colorNumber-1], gradientColors[colorNumber], (float)(lastSelectedBoard%10)/10);

                }

                Vector3 platePos = player.transform.position;
                platePos.y = totalPlate * Globals.GetPlateSize();
                GameObject clonePlate = Instantiate(plateObjects[0],platePos,plateObjects[0].transform.rotation);
                yield return new WaitForSeconds(0.14f);

                totalPlate++;
            }
            */
            //player.transform.position += new Vector3(0,Globals.GetPlateSize(),0) / 2;
    }


    void clickPuaseGame()
    {
        SceneManager.LoadScene("GameScene",LoadSceneMode.Single);
    }




}



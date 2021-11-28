using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class LevelEditor : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    [Header("UI")]
    [SerializeField] private Button btnFood1;
    [SerializeField] private Button btnFood2;
    [SerializeField] private Button btnFood3;

    [SerializeField] private Button btnObstacle1;
    [SerializeField] private Button btnObstacle2;
    [SerializeField] private Button btnObstacle3;
    [SerializeField] private Button btnObstacle4;

    [SerializeField] private Button btnGate1;
    [SerializeField] private Button btnGate2;

    [SerializeField] private Button btnDelete;
    [SerializeField] private Button btnSave;
    [SerializeField] private TMP_Dropdown levelList;

    [Header("Objects")]
    [SerializeField] private Transform startObject;
    [SerializeField] private Transform finishObject;
    [SerializeField] private Transform foodObject;
    [SerializeField] private Transform gateObject;
    [SerializeField] private Transform obstacleObject;
    [SerializeField] private Transform mapParent;

    private Transform selectedObject;
    int screenWidth = 0;
    int levelCount = 0;
    int selectedLevel = 0;
    int lastSelectedLevel = -1;

    void Awake()
    {
        screenWidth = Screen.width;

        btnFood1.onClick.AddListener(() => clickAddFood(0));
        btnFood2.onClick.AddListener(() => clickAddFood(1));
        btnFood3.onClick.AddListener(() => clickAddFood(2));

        btnObstacle1.onClick.AddListener(() => clickAddObstacle(0));
        btnObstacle2.onClick.AddListener(() => clickAddObstacle(1));
        btnObstacle3.onClick.AddListener(() => clickAddObstacle(2));
        btnObstacle4.onClick.AddListener(() => clickAddObstacle(3));

        btnGate1.onClick.AddListener(() => clickAddGate(0));
        btnGate2.onClick.AddListener(() => clickAddGate(1));

        levelList.onValueChanged.AddListener(LevelSelected);

        btnDelete.onClick.AddListener(ClickDelete);

        btnSave.onClick.AddListener(SaveLevel);

        Object[] levels = Resources.LoadAll("Levels", typeof(TextAsset));
        levelCount = levels.Length;

        List<string> levelNameList = new List<string>();
        for(int i=0; i<levelCount; i++)
        {
            levelNameList.Add($"Level {i+1}");
        }

        levelList.AddOptions(levelNameList);
        selectedLevel = 0;
        LoadLevel();

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            float moveSpeed = Input.mousePosition.x - (screenWidth/2);
            cameraTransform.Translate(Vector3.forward * moveSpeed * Time.deltaTime * 0.2f ,Space.World);

        }

        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit)) 
            {
                if(hit.transform.tag == "Editable")
                {
                    if(selectedObject != null) SetOutlineStatus(false);
                    selectedObject = hit.transform;
                    SetOutlineStatus(true);
                }
                else
                {
                    if(selectedObject != null) SetOutlineStatus(false);
                    selectedObject = null;
                }   

            }
            else
            {
                if(selectedObject != null) SetOutlineStatus(false);
                selectedObject = null;
            }
        }

        if(Input.GetKeyDown(KeyCode.W))
        {
            MoveSelected(-1,0);
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            MoveSelected(1,0);
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            MoveSelected(0,-1);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            MoveSelected(0,1);
        }
    }

    void SetOutlineStatus(bool status)
    {
        foreach(Transform tf in selectedObject)
        {
            tf.GetComponent<Outline>().enabled = status;
        }
    }

    void MoveSelected(int ver, int hor)
    {
        if(selectedObject != null)
        {
            selectedObject.Translate((Vector3.right * ver * 1) + (Vector3.forward * hor * 1));
        }
    }

    public void clickAddFood(int level)
    {
        Vector3 clonePos = cameraTransform.position;
        clonePos.x = 0;
        clonePos.y = 0;
        clonePos.z = Mathf.Round(clonePos.z);
        GameObject cloneFood = Instantiate(foodObject.gameObject, clonePos, Quaternion.identity);
        cloneFood.transform.SetParent(mapParent);
        for(int i=0; i<cloneFood.transform.childCount; i++)
        {
            bool status = i == level;
            cloneFood.transform.GetChild(i).gameObject.SetActive(status);
        }
    }

    public void clickAddObstacle(int obstacleType)
    {
        Vector3 clonePos = cameraTransform.position;
        clonePos.x = 0;
        clonePos.y = 0;
        clonePos.z = Mathf.Round(clonePos.z);
        GameObject cloneObstacle = Instantiate(obstacleObject.gameObject, clonePos, Quaternion.identity);
        cloneObstacle.transform.SetParent(mapParent);
        for(int i=0; i<cloneObstacle.transform.childCount; i++)
        {
            bool status = i == obstacleType;
            cloneObstacle.transform.GetChild(i).gameObject.SetActive(status);
        }  
    }

    public void clickAddGate(int gateType)
    {
        Vector3 clonePos = cameraTransform.position;
        clonePos.x = 0;
        clonePos.y = 0;
        clonePos.z = Mathf.Round(clonePos.z);
        GameObject cloneGate = Instantiate(gateObject.gameObject, clonePos, Quaternion.identity);
        cloneGate.transform.SetParent(mapParent);
        for(int i=0; i<cloneGate.transform.childCount; i++)
        {
            bool status = i == gateType;
            cloneGate.transform.GetChild(i).gameObject.SetActive(status);
        }
    }

    void ClickDelete()
    {
        if(selectedObject != null) Destroy(selectedObject.gameObject);
    }

    void LevelSelected(int selection)
    {
        SaveLevel();
        Debug.Log(selection);
        selectedLevel = selection;
        LoadLevel();
    }

    void LoadLevel()
    {
        foreach(Transform tf in mapParent)
        {
            if(!tf.name.Contains("Player"))
            {
                Destroy(tf.gameObject);
            }
        }
        string filePath = $"{Application.dataPath}/Resources/Levels/{selectedLevel+1}.txt";
        StreamReader reader = new StreamReader(filePath);
        while(!reader.EndOfStream)
        {
            StringToObject(reader.ReadLine());
        }
        reader.Close();
    }

    void SaveLevel()
    {
        string filePath = $"{Application.dataPath}/Resources/Levels/{selectedLevel+1}.txt";
        StreamWriter outStream = System.IO.File.CreateText(filePath);

        foreach(Transform mapObj in mapParent)
        {
            outStream.WriteLine(ObjectValuesToString(mapObj.gameObject));
        }
        
        outStream.Close();
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

            for(int i=0; i<cloneObstacle.transform.childCount; i++)
            {
                bool status = i == obstacleNo;
                cloneObstacle.transform.GetChild(i).gameObject.SetActive(status);
            }

            cloneObstacle.transform.SetParent(mapParent);
 
        }
        else if(objName.Contains("f"))
        {
            int foodNo = int.Parse(objName[1].ToString());
            GameObject cloneFood = Instantiate(foodObject.gameObject, objPos, Quaternion.identity);

            for(int i=0; i<cloneFood.transform.childCount; i++)
            {
                bool status = i == foodNo;
                cloneFood.transform.GetChild(i).gameObject.SetActive(status);
            }

            cloneFood.transform.SetParent(mapParent);
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

    string ObjectValuesToString(GameObject obj)
    {
        string val = "";

        if(obj.name == "PlayerStart")
        {
            val = $"start;{PositionToString(obj.transform.position)};";

        }
        else if(obj.name == "PlayerFinish")
        {
            val = $"finish;{PositionToString(obj.transform.position)};";
        }
        else if(obj.name.Contains("Obstacle"))
        {
            int obstacleNo = 0;
            for(int i=0; i<obj.transform.childCount; i++)
            {
                if(obj.transform.GetChild(i).gameObject.activeSelf) obstacleNo = i;
            }
            val = $"o{obstacleNo};{PositionToString(obj.transform.position)};";
        }
        else if(obj.name.Contains("FoodObject"))
        {
            int foodNo = 0;
            for(int i=0; i<obj.transform.childCount; i++)
            {
                if(obj.transform.GetChild(i).gameObject.activeSelf) foodNo = i;
            }
            val = $"f{foodNo};{PositionToString(obj.transform.position)};";
        }
        else if(obj.name.Contains("GateObject"))
        {
            int gateNo = 0;
            for(int i=0; i<obj.transform.childCount; i++)
            {
                if(obj.transform.GetChild(i).gameObject.activeSelf) gateNo = i;
            }
            val = $"g{gateNo};{PositionToString(obj.transform.position)};";
        }


        return val;
    }

    string PositionToString(Vector3 pos)
    {
        string val = "";

        val = $"{pos.x};{pos.y};{pos.z}";
        return val;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStackItem : MonoBehaviour
{
    private int stackOrder = 0;
    private Transform targetObject;
    private Vector3 targetPosition;
    private float stackDistance;
    private bool isMove = false;
    private bool isTaken = false;
    private bool isDropped = false;
    private Vector3 dropPosition;
    private FoodType foodType = FoodType.Food1;
    private bool isUpgraded = false;
    private int foodLevel = 0;
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(isMove) MoveWithPlayer();

        if(isDropped) DropMove();

    }



    void MoveWithPlayer()
    {
        Vector3 newPos = transform.position;
        newPos.z = targetObject.transform.position.z + (stackOrder * stackDistance);
        transform.position = newPos;

        targetPosition = transform.position;
        targetPosition.x = targetObject.position.x;


        transform.position = Vector3.Lerp(transform.position,targetPosition,0.5f / stackOrder);
    }

    void DropMove()
    {
        transform.position = Vector3.Lerp(transform.position, dropPosition,Time.deltaTime * Globals.GetDropSmooth());
    }

    void LateUpdate()
    {


    }

    public void SetLevel(int level)
    {
        foodLevel = level;

        for(int i=0; i<transform.childCount; i++)
        {
            bool status = i == level;
            transform.GetChild(i).gameObject.SetActive(status);
        }
    }

    public void SetTakenStatus(bool status)
    {
        isTaken = status;
    }

    public bool GetTakenStatus()
    {
        return isTaken;
    }


    public void Init(int order, Transform target, Vector3 lastPosition, bool newSpawn,int level)
    {
        if(newSpawn)
        {

            SetTakenStatus(false);
            SetLevel(level);
        }
        else
        {
            stackDistance = Globals.GetModelDistance();
            stackOrder = order;
            targetObject = target;
            isMove = true;
            SetTakenStatus(true);


            Vector3 newPos = targetObject.position;
            newPos.z += stackOrder * stackDistance;
            newPos.x = lastPosition.x;
            transform.position = newPos;
            SetLevel(level);
        }

    }

    public void UpgradeFood()
    {
        if(!isUpgraded && foodLevel < Globals.GetFoodLevelCount()-1)
        {
            isUpgraded = true;
            Invoke("DelayUpgradeStatus",0.5f);
            foodLevel++;
            SetLevel(foodLevel);
        }

    }

    void DelayUpgradeStatus()
    {
        isUpgraded = false;
    }

    public void AddNewEfect()
    {
        iTween.ScaleTo(gameObject,iTween.Hash("scale",Vector3.one * 1.2f,"time",0.1f));
        iTween.ScaleTo(gameObject,iTween.Hash("scale",Vector3.one,"time",0.1f,"delay",0.1f));
        
    }

    void OnCollisionEnter(Collision col)
    {
        string colliderName = col.transform.name;
        if(isTaken)
        {
            if(colliderName.Contains("Obstacle"))
            {
                //Debug.Log("obstacle");
                targetObject.GetComponent<PlayerController>().ObstacleHit(stackOrder,col.GetContact(0).point);
            }
            else if(colliderName.Contains("Finish"))
            {
                targetObject.GetComponent<PlayerController>().RemoveFromStack(stackOrder-1,RemoveType.Finish,Vector3.zero);
                GameSystem.Instance.AddPlate(foodLevel);
            }
            else if(colliderName.Contains("Oven"))
            {
                targetObject.GetComponent<PlayerController>().ObstacleHit(stackOrder+1,col.GetContact(0).point);
                targetObject.GetComponent<PlayerController>().RemoveFromStack(stackOrder-1,RemoveType.Oven,Vector3.zero);
                GameSystem.Instance.AddPlate(foodLevel);
            }


        }
        else
        {
            if(colliderName.Contains("Food"))
            {
                FoodStackItem colliderItem = col.transform.GetComponent<FoodStackItem>();
        
                if(colliderItem.GetTakenStatus())
                {
                    AddToPlayer();
                }
            }
            else if(colliderName.Contains("Player"))
            {
                AddToPlayer();
            }

        }

    }

    void AddToPlayer()
    {
        isDropped = false;
        isTaken = true;
        GameSystem.Instance.GetPlayer().AddToStack(foodType, foodLevel);
        iTween.ScaleTo(gameObject,Vector3.zero,0.2f);
        Debug.Log("taken");
    }


    public void DropEffect(Vector3 dropPos)
    {
        Vector3 dropSpacing = Globals.GetDropSpacing();

        float xVal = Random.Range(-dropSpacing.x, dropSpacing.x);
        float yVal = dropSpacing.y;
        float zVal = Random.Range(dropPos.z - dropSpacing.z, dropPos.z + dropSpacing.z);

        dropPosition = new Vector3(xVal, yVal, zVal);
        isTaken = false;
        isMove = false;
        isDropped = true;
 
    }

    public void DestroyEffect()
    {
        Destroy(gameObject);
    }

    public void FinishEffect()
    {
        transform.GetChild(foodLevel).GetChild(0).gameObject.SetActive(false);
        transform.GetChild(foodLevel).GetChild(1).gameObject.SetActive(true);
        iTween.MoveTo(gameObject, iTween.Hash("position",transform.position + (Vector3.right * 15),"time",2.0f,"easetype",iTween.EaseType.linear));
        isMove = false;
        //Destroy(gameObject);
    }

    public void OvenEffect()
    {
        Destroy(gameObject);
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform stackParent;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform playerModelParent;
    [SerializeField] private Transform playerHatParent;
    private List<Transform> stackTransforms = new List<Transform>();

    int currentStackCount = 0;
    bool isFinished = false;
    bool isStarted = false;


    void Start()
    {
        SetCharacterModel();
        //StartCharacter();
    }

    void StartCharacter()
    {
        playerAnimator.SetTrigger("Walk");
        isStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S) && !isStarted) StartCharacter();
    }

    void FixedUpdate()
    {
        if(!isFinished && isStarted) Move();
    }

    void Move()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 10,Space.World);
        transform.Translate(Vector3.right * Input.GetAxis("Horizontal")* Time.deltaTime * 15,Space.World);
    }

    public void AddToStack(FoodType foodType, int level)
    {
        currentStackCount++;
        GameObject clone = Instantiate(GameSystem.Instance.GetFoodObject(),stackParent);
        
        Vector3 clonePos = transform.position;
        clonePos.z += currentStackCount * Globals.GetModelDistance();
        clone.transform.position = clonePos;

        Vector3 firstPos = transform.position;
        if(stackTransforms.Count > 0) firstPos = stackTransforms[stackTransforms.Count-1].position;
        clone.GetComponent<FoodStackItem>().Init(currentStackCount, transform, firstPos, false, level);
        stackTransforms.Add(clone.transform);

        StartCoroutine(DelayEffect());
        

    }

    IEnumerator DelayEffect()
    {

        for(int i=0; i<stackTransforms.Count; i++)
        {
            yield return new WaitForSeconds(0.05f);
            stackTransforms[stackTransforms.Count - i - 1].GetComponent<FoodStackItem>().AddNewEfect();
        }
    }



    public void RemoveFromStack(int stackNumber,RemoveType removeType,Vector3 dropPos)
    {
        StopAllCoroutines();
        Debug.Log($"{stackNumber} - {currentStackCount}");
        FoodStackItem stackItem =  stackTransforms[stackNumber].GetComponent<FoodStackItem>();

        switch(removeType)
        {
            case RemoveType.Destroy:
                stackItem.DestroyEffect();
                break;
            case RemoveType.Drop:
                dropPos.z += 5;
                stackItem.DropEffect(dropPos);
                break;
            case RemoveType.Finish:
                stackItem.FinishEffect();
                break;
            case RemoveType.Oven:
                stackItem.OvenEffect();
                break;

        }
     
        stackTransforms.Remove(stackTransforms[stackNumber]);
        currentStackCount--;
    }

    public void ObstacleHit(int stackNumber, Vector3 hitPosition)
    {
        StopAllCoroutines();
        for(int i=stackTransforms.Count-1; i> stackNumber-2; i--)
        {
            Debug.Log($"{i} - {stackNumber}");
            RemoveType removeType = RemoveType.Drop;
            if(i == stackNumber-1) removeType = RemoveType.Destroy;

            RemoveFromStack(i, removeType,hitPosition);
            
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.transform.name.Contains("Finish"))
        {
            transform.position = new Vector3(0,transform.position.y,transform.position.z); 
            isFinished = true;
            GameSystem.Instance.Finish();
            playerAnimator.SetTrigger("Dance");
            
            playerModelParent.rotation = Quaternion.Euler(0,-180,0);
        }
    }



    void SetCharacterModel()
    {
        int characterNo = Globals.GetCurrentCharacterModel();
        int childCount = playerModelParent.childCount;

        for(int i=0; i<childCount-1; i++)
        {
            bool status = i == characterNo;

            playerModelParent.GetChild(i).gameObject.SetActive(status);
            playerHatParent.GetChild(i).gameObject.SetActive(status);
            
        }


    }


}

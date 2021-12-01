using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ObstacleType
{
    Spike,
    Mouse,
    Slider,
    SwingBlade
}
public class ObstacleController : MonoBehaviour
{
    private ObstacleType obstacleType;
    private bool isInitialized = false;
    private GameObject activeObject;
    void Start()
    {
    
    }   

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    void ObstacleMovement()
    {
        switch(obstacleType)
        {
            case ObstacleType.Spike:
                break;
            case ObstacleType.Mouse:
                Vector3 mouseDirection = transform.right;
                if(transform.position.x > 0) mouseDirection = -transform.right;
                iTween.MoveTo(gameObject,iTween.Hash("position",transform.position + (mouseDirection * 20),"time",1f,"easetype",iTween.EaseType.easeOutQuad,"looptype",iTween.LoopType.pingPong,"delay",1f,"occomplete","MouseEatAnim","oncompletetarget",gameObject));
                InvokeRepeating("MouseEatAnim",0f,1f);
                break;
            case ObstacleType.Slider:
                Vector3 sliderDirection = transform.position.x < 0 ? transform.right : -transform.right;
                iTween.MoveTo(gameObject,iTween.Hash("position",transform.position + (sliderDirection * 13),"time",1f,"easetype",iTween.EaseType.easeOutQuad,"looptype",iTween.LoopType.pingPong,"delay",0.5f));
                break;
            case ObstacleType.SwingBlade:
                iTween.RotateTo(transform.GetChild(3).gameObject,iTween.Hash("rotation",new Vector3(0,0,-90) ,"time",1f,"easetype",iTween.EaseType.easeOutQuad,"looptype",iTween.LoopType.pingPong,"delay",0.5f));
                break;
        }
    }

    void MouseEatAnim()
    {
        Debug.Log("mouse");
        activeObject.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Eat");
    }

    public void Init(ObstacleType obstacle)
    {
        obstacleType = obstacle;

        SetModel();
        isInitialized = true;
        ObstacleMovement();

    }

    void SetModel()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            bool status = i == (int)obstacleType;
            transform.GetChild(i).gameObject.SetActive(status);
            if(status) activeObject = transform.GetChild(i).gameObject;

        }
    }


}

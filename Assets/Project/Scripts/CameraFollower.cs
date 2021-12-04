using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform targetObject;

    [Header("Position Follow")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private float positionFollowSpeed = 1;
    [SerializeField] private bool followXPosition = false;
    [SerializeField] private bool followYPosition = false;
    [SerializeField] private bool followZPosition = false;

    [Header("Rotation")]
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] private bool lookToObject = false;


    private Vector3 targetPosition;
    private Quaternion targetAngle;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        targetPosition.x = followXPosition ? targetObject.position.x + positionOffset.x : positionOffset.x;
        targetPosition.y = followYPosition ? targetObject.position.y + positionOffset.y : positionOffset.y;
        targetPosition.z = followZPosition ? targetObject.position.z + positionOffset.z : positionOffset.z;

        transform.position = Vector3.Lerp(transform.position, targetPosition,Time.deltaTime * positionFollowSpeed);

        targetAngle = lookToObject ? (Quaternion.LookRotation(targetObject.position) * Quaternion.Euler(rotationOffset)) : Quaternion.Euler(rotationOffset);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetAngle, Time.deltaTime * positionFollowSpeed);
        


    }

    public void ChangePositionOffset(Vector3 changeValue)
    {
        positionOffset += changeValue;
    }

    public void ChangeRotationOffset(Vector3 changeValue)
    {
        rotationOffset += changeValue;
    }
}

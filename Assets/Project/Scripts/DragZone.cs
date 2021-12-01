using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class DragZone : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    private RectTransform canvas;
    private Vector2 dragDelta = Vector2.zero;
  
    public UnityAction OnDragAction;
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(OnDragAction != null) OnDragAction();
        dragDelta = eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragDelta = Vector2.zero;
    }

    public Vector2 GetDragDelta()
    {
        return dragDelta / canvas.localScale.x;
    }

    void Awake()
    {
        canvas = transform.parent.GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        //dragDelta = Vector2.zero;
    }
}

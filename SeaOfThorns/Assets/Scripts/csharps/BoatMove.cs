using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoatMove : BaseMove
{
    public Transform PickMark;
    [HideInInspector]
    public float weight = 0f;
    
    public int MaxWeigh = 10;
    private int currentWeigh = 0;
    public int CurrentWeigh{
        get{
            return currentWeigh;
        }
        set{
            currentWeigh = value;
            weight = 1 - (float)currentWeigh / MaxWeigh;
            GameingState.Instance.UpdateWeight(weight);
        }
    }

    public float PickRange = 10;
    private Transform cloestItem;
    private float minDisToItem;

    public float detectIntervalTime = 2.0f;
    private float detectTime = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

    }



    IEnumerator EmphasizeItem(Collider[] colliders)
    {

        foreach(var col in colliders)
        {
            MeshRenderer render = col.gameObject.GetComponent<MeshRenderer>();
            if(render != null)
            {
                render.material = new Material(Shader.Find("Unlit/ItemPick"));
            }
        }
        yield return new WaitForSeconds(1.0f);
        foreach(var col in colliders)
        {
            if(col != null)
            {
                MeshRenderer render = col.gameObject.GetComponent<MeshRenderer>();
                if(render != null)
                {
                    render.material = new Material(Shader.Find("Unlit/Item"));
                }
            }


        }
    }

    void PickItem(Transform go)
    {
        if(go == null)return;
        Transform place = this.transform.Find("Place");
        foreach(Transform p in place)
        {
            if(p.childCount <= 0)
            {
                go.parent = p;
                go.localPosition = Vector3.zero;
                go.rotation = Quaternion.identity;
                go.gameObject.layer = LayerMask.GetMask("Default");
                MeshRenderer render = go.gameObject.GetComponent<MeshRenderer>();
                if(render != null)
                {
                    render.material = new Material(Shader.Find("Unlit/Item"));
                }
                Destroy(go.GetComponent<Collider>());
                Destroy(go.GetComponent<BaseMove>());
                Destroy(go.GetComponent<Rigidbody>());
                CurrentWeigh += GameingState.Instance.GetItemConfig(go.name).weight;
                break;
            }
        }
    }

    void DropItem()
    {
        Transform place = this.transform.Find("Place");
        foreach(Transform p in place)
        {
            if(p.childCount > 0)
            {
            }
        }
    }

    void Gaming()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, PickRange, LayerMask.GetMask("Interactive"));
        if(colliders.Length > 0)
        {
            minDisToItem = Vector3.Magnitude(colliders[0].transform.position - this.transform.position);
            cloestItem = colliders[0].transform;
        }else
        {
            cloestItem = null;
        }
        foreach(var col in colliders)
        {            
            float dis = Vector3.Magnitude(col.transform.position - this.transform.position);
            if(dis < minDisToItem)
            {
                minDisToItem = dis;
                cloestItem = col.transform;
            }
        }
        if(cloestItem != null)
            PickMark.position = Camera.main.WorldToScreenPoint(cloestItem.transform.position);
        else
            PickMark.position = new Vector3(Screen.width + 200, Screen.height + 200, 0);

        //拾取
        if(Input.GetKeyDown(KeyCode.E))
        {
            PickItem(cloestItem);
        }

        //间隔检测
        detectTime += Time.deltaTime;
        if(detectTime > detectIntervalTime)
        {
            StartCoroutine(EmphasizeItem(colliders));
            detectTime = 0;
        }



        Vector3 BoatPosOnScreen = Camera.main.WorldToScreenPoint(this.transform.position);
        transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime);
        Vector3 temp = Input.mousePosition - BoatPosOnScreen;
        direction = new Vector3(
            Mathf.Clamp(temp.x /(Screen.resolutions[0].width * 1f), -1f, 1f), 
            0, 
            Mathf.Clamp(temp.y / (Screen.resolutions[0].height * 1f),-1f, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        if(GameingState.Instance.CurrentState == GState.GAMING)
        {
            Gaming();
        }
        
        base.Update();
    }

    void FixedUpdate()
    {
        Camera.main.transform.position = Vector3.Lerp(
            Camera.main.transform.position,
            //根据相机的角度保持一个偏移
            Util.KeepY(transform.position, Camera.main.transform.position.y) 
            - new Vector3(0,0,8),
            .2f * Time.deltaTime);
    }
}

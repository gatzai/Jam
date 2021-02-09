using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class BaseMove : MonoBehaviour
{
    Rigidbody rigidbody;

    protected Vector3 direction;
    float LimitY = .0f;

    // Start is called before the first frame update
    protected void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        if(rigidbody == null)
        rigidbody = gameObject.AddComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected void Update()
    {
        rigidbody.velocity = Util.KeepY(direction * Time.deltaTime * 50f * 10, rigidbody.velocity.y);


        //transform.Translate(delta * Time.deltaTime);
        if(transform.position.y <= 0.5f)
        {
           rigidbody.useGravity = false;
        }
        else
        {
            rigidbody.useGravity = true;
        }

        if(transform.position.y <= LimitY)
        { 
            transform.position = new Vector3(transform.position.x, 0.02f, transform.position.z);
            //rigidbody.AddForce(Vector3.up * Mathf.Abs((transform.position.y - LimitY) / sensity), ForceMode.Impulse);
        }
    }

}

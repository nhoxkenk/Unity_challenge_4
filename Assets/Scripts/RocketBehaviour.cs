using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{

    private Transform target;
    private float speed = 15.0f;
    private bool homing = false;

    private float rocketStrength = 15.0f;
    private float aliveTimer = 3.0f;

    public void Fire(Transform target)
    {
        this.target = target;
        homing = true;
        Destroy(gameObject, aliveTimer);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(homing && target != null)
        {
            Vector3 moveDirection = (target.transform.position - gameObject.transform.position).normalized;
            transform.position += moveDirection * speed * Time.deltaTime;
            transform.LookAt(target);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(target != null)
        {
            if (collision.gameObject.CompareTag(this.target.tag))
            {
                Rigidbody targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 away = -collision.contacts[0].normal;
                targetRigidbody.AddForce(away * rocketStrength, ForceMode.Impulse);
                Destroy(gameObject);
            }
        }
    }
}

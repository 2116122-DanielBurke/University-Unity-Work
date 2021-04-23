using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    GameObject ogre;
    float distance;
    void Start()
    {
        ogre = GameObject.Find("Ogre_Transform");
    }

    
    void Update()
    {
        distance = Vector3.Distance(ogre.transform.position, transform.position);
        transform.position = transform.position + speed * transform.right * Time.deltaTime; ;

        if (distance <= 0.1)
        {
            Destroy(gameObject);
        }
    }
}

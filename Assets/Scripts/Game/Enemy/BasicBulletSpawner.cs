using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBulletSpawner : MonoBehaviour
{
    [SerializeField] GameObject bulletType;
    [SerializeField] Vector3 Offset = new Vector3(0, -1, 0);

    [SerializeField] float ShootInterval = 0.1f;

    float timeSinceShot = 0f;

    void Start()
    {
        
    }

    // Start is called before the first frame update
    private void Shoot()
    {
        timeSinceShot -= ShootInterval;
        
        var newBullet = Instantiate(bulletType,
            transform.position,
            transform.rotation);
        
        newBullet.transform.Translate(Offset);

        //this keeps bullets aligned time-wise
        newBullet.GetComponent<BulletMoveBase>().initialOffsetTime = timeSinceShot;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceShot += Time.deltaTime;
        if(timeSinceShot >= ShootInterval)
        {
            Shoot();
        }
    }
}

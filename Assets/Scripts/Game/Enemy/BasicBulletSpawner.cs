using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBulletSpawner : MonoBehaviour
{
    [SerializeField] GameObject bulletType;
    [SerializeField] Vector3 Offset = new Vector3(0, -1, 0);

    [SerializeField] float ShootInterval = 0.1f;
	
	private GameObject player = null;

    float timeSinceShot = 0f;
	const float activateDistance = 30.0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    private void Shoot()
    {
        timeSinceShot -= ShootInterval;
        
        var newBullet = Instantiate(bulletType,
            transform.position,
            transform.rotation,
			transform.parent);
        
        newBullet.transform.Translate(Offset);

        //this keeps bullets aligned time-wise
        newBullet.GetComponent<BulletMoveBase>().initialOffsetTime = timeSinceShot;
    }

    // Update is called once per frame
    void Update()
    {
		if(Vector3.Distance(transform.position, player.transform.position) < activateDistance)
		{
			timeSinceShot += Time.deltaTime;
			if(timeSinceShot >= ShootInterval)
			{
				Shoot();
			}
		}
    }
}

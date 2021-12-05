using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScroller : MonoBehaviour
{
	[SerializeField] private Vector3 scrollDir = new Vector3(0, 1, 0);
	[SerializeField] private float scrollSpeed = 1.0f;
	
    // Start is called before the first frame update
    void Start()
    {
        scrollDir = scrollDir.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(scrollDir * scrollSpeed * Time.deltaTime);
    }
}

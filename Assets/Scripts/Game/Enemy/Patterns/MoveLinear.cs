using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLinear : BulletMoveBase
{
    public float speedMultiplier = 10f;
    public Vector2 moveSpeed = new Vector2(0, -1);

    private void AdvancePosition(float dt)
    {
        transform.Translate(moveSpeed * dt);
    }

    // Start is called before the first frame update
    void Start()
    {
        AdvancePosition(speedMultiplier * initialOffsetTime);
    }

    // Update is called once per frame
    void Update()
    {
        AdvancePosition(speedMultiplier * Time.deltaTime);
    }
}

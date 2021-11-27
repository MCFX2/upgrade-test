using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    Transform m_transform;

    //move scaling speed. X = left, Y = right.
    [SerializeField] Vector2 horizontalMoveSpeed = new Vector2(0.2f, 0.2f);
    //move scaling speed. X = up, Y = down
    [SerializeField] Vector2 verticalMoveSpeed = new Vector2(0.2f, 0.1f);
    enum MovementInterpType { ConstrainHigh, ConstrainLow };

    //Constrain High: Favor maximum player travel speed in all cases, but
    //make movement harder to predict for the player.
    //Constrain Low: Player will always move in the most predictable way, but
    //will move sub-optimally within the constraints given for most cases.
    [SerializeField] MovementInterpType interpType = MovementInterpType.ConstrainLow;

    // Start is called before the first frame update
    void Start()
    {
        m_transform = GetComponent<Transform>();
    }

    private float GetXConstraint(float xDelta)
    {
        if(xDelta < 0)
        {
            return horizontalMoveSpeed.x;
        }
        return horizontalMoveSpeed.y;
    }

    private float GetYConstraint(float yDelta)
    {
        if(yDelta < 0)
        {
            return verticalMoveSpeed.y;
        }
        return verticalMoveSpeed.x;
    }

    //
    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 currentPos = m_transform.position;
        Vector3 targetDelta = targetPos - currentPos;
        //clear Z coordinate, Z coordinate is only used for draw order
        targetDelta.z = 0;


        if (interpType == MovementInterpType.ConstrainLow)
        {
            if(targetDelta.x == 0)
            {
                targetDelta.x = 0.0000001f;
            }
            //step 1: figure out which item is constraining us
            float targetRatio = Mathf.Abs(targetDelta.y / targetDelta.x);
            float actualXLimit, actualYLimit;

            actualXLimit = GetXConstraint(targetDelta.x);
            actualYLimit = GetYConstraint(targetDelta.y);

            //figure out which bias constrain ratio uses (vert or horiz)
            if (actualYLimit / actualXLimit < targetRatio)
            { //bias is vertical, constrain along horizontal
                //constrain X
                actualYLimit = actualXLimit * targetRatio;
            }
            else
            {
                //constrain Y
                actualXLimit = actualYLimit / targetRatio;
            }


            targetDelta.x *= actualXLimit;
            targetDelta.y *= actualYLimit;
        }
        else
        {
            //scale delta by user-specified lerp values
            targetDelta.x *= GetXConstraint(targetDelta.x);
            targetDelta.y *= GetYConstraint(targetDelta.y);
        }




        m_transform.position += targetDelta;
    }
}

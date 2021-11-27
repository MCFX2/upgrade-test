using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
[RequireComponent(typeof(Rigidbody))]
public class FollowMousePhysics : MonoBehaviour
{


    Transform m_transform;
    Rigidbody m_rigidbody;
    [SerializeField] float MouseFarDistance = 0.4f;

    [SerializeField] float Speed = 40f;

    [SerializeField] float LeftSpeed = 1f;
    [SerializeField] float RightSpeed = 1f;
    [SerializeField] float UpSpeed = 1f;
    [SerializeField] float DownSpeed = 1f;

    [SerializeField] float MaxLeftSpeed = 10f;
    [SerializeField] float MaxRightSpeed = 10f;
    [SerializeField] float MaxUpSpeed = 10f;
    [SerializeField] float MaxDownSpeed = 10f;

    [SerializeField] Vector2 DecelerationFactor = new Vector2(0.8f, 0.8f);

    [SerializeField] float MouseSnapRadius = 0.07f;

    Vector2 m_lastMousePos = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        m_transform = GetComponent<Transform>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void CheckMouseSnap()
    {
        //skip if mouse moved
        if (Vector2.Distance(m_lastMousePos, new Vector2(m_transform.position.x, m_transform.position.y)) <= MouseSnapRadius)
        {
            m_transform.position = new Vector3(m_lastMousePos.x, m_lastMousePos.y, m_transform.position.z);
            m_rigidbody.velocity = new Vector3(0, 0, m_rigidbody.velocity.z);
        }
    }

    //returns the target position, assuming mouse input
    private Vector2 GetMouseDelta()
    {
        Vector3 fullMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        m_lastMousePos.x = fullMousePos.x;
        m_lastMousePos.y = fullMousePos.y;

        float targetDistance = Vector2.Distance(
            new Vector2(m_transform.position.x, m_transform.position.y),
            m_lastMousePos);

        //Player acceleration increases as the mouse cursor gets further away
        //but only to a certain distance limit- this clamp figures out what % along this line we are.
        float distanceRatio = Mathf.Clamp(targetDistance / MouseFarDistance, 0, 1);

        Vector2 velocity = Vector2.zero;

        if(m_transform.position.x > fullMousePos.x)
        {
            velocity.x -= LeftSpeed * Speed * distanceRatio;
        }
        else
        {
            velocity.x += RightSpeed * Speed * distanceRatio;
        }

        if(m_transform.position.y > fullMousePos.y)
        {
            velocity.y -= DownSpeed * Speed * distanceRatio;
        }
        else
        {
            velocity.y += UpSpeed * Speed * distanceRatio;
        }

        return velocity;
    }

    private void TickDeceleration()
    {
        ///Decelerate X
        if(Mathf.Abs(m_rigidbody.velocity.x) < DecelerationFactor.x)
        { //if speed is very small, we set it to zero. This keeps mouse-snapping from breaking.
            m_rigidbody.velocity = new Vector3(0, m_rigidbody.velocity.y, m_rigidbody.velocity.z);
        }
        else if(m_rigidbody.velocity.x > 0)
        { //accelerate towards 0
            m_rigidbody.velocity -= Time.deltaTime * new Vector3(DecelerationFactor.x, 0, 0);
        }
        else
        {
            m_rigidbody.velocity += Time.deltaTime * new Vector3(DecelerationFactor.x, 0, 0);
        }

        ///Decelerate Y
        if(Mathf.Abs(m_rigidbody.velocity.y) < DecelerationFactor.y)
        {
            m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
        }
        else if (m_rigidbody.velocity.y > 0)
        { //accelerate towards 0
            m_rigidbody.velocity -= Time.deltaTime * new Vector3(0, DecelerationFactor.y, 0);
        }
        else
        {
            m_rigidbody.velocity += Time.deltaTime * new Vector3(0, DecelerationFactor.y, 0);
        }
    }

    private void TickAcceleration()
    {
        Vector2 velocity = GetMouseDelta();

        m_rigidbody.velocity += new Vector3(velocity.x, velocity.y, 0);
        //immediately apply deceleration so velocity is always correct after this function call
        TickDeceleration();
    }




    private void ClampVelocity()
    {

        //factor to correct offset from mouse position
        //(used to adjust for screen border locking)
        Vector2 correctiveFactor = new Vector2(
            Mathf.Abs(m_transform.position.x - m_lastMousePos.x),
            Mathf.Abs(m_transform.position.y - m_lastMousePos.y));
        correctiveFactor.Normalize();


        m_rigidbody.velocity = new Vector3(
                correctiveFactor.x *
                    Mathf.Clamp(m_rigidbody.velocity.x, -MaxLeftSpeed, MaxRightSpeed),
                correctiveFactor.y *
                    Mathf.Clamp(m_rigidbody.velocity.y, -MaxDownSpeed, MaxUpSpeed),
                0);
    }

    // Update is called once per frame
    void Update()
    {
        TickAcceleration();
        ClampVelocity();

        CheckMouseSnap();
    }
}

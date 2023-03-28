using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WalkingObject : MonoBehaviour
{
    public float maxStepHeight = 0.2f;
    public float stepSearchOvershoot = 0.01f;
    public float maxSlopeAngle = 0.3f;
    

    private List<ContactPoint> m_ContactPoints;
    protected Rigidbody m_Rigidbody;

    protected virtual void Start()
    {
        m_ContactPoints = new List<ContactPoint>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    protected void FixedUpdate()
    {
        m_ContactPoints.Clear();
    }

    void OnCollisionEnter(Collision collision)
    {
        m_ContactPoints.AddRange(collision.contacts);
    }

    void OnCollisionStay(Collision collision)
    {
        m_ContactPoints.AddRange(collision.contacts);
    }

    bool FindGround(out ContactPoint groundContactPoint)
    {
        groundContactPoint = default(ContactPoint);
        bool found = false;
        foreach(ContactPoint point in m_ContactPoints) {
            if (
                point.normal.y > 0.0001f 
                && (
                    found == false 
                    || point.normal.y > groundContactPoint.normal.y
                )
            ) {
                groundContactPoint = point;
                found = true;
            }
        }

        return found;
    }

    protected ContactPoint? FindStep(out RaycastHit hitInfo)
    {

        ContactPoint groundContactPoint = default(ContactPoint);
        bool grounded = FindGround(out groundContactPoint);

        foreach(ContactPoint point in m_ContactPoints) {
            bool test = IsSteppableContact(point, groundContactPoint, out hitInfo);
            if (test) return point;
        }

        hitInfo = default(RaycastHit);
        return null;
    }

    bool IsSteppableContact(ContactPoint stepTestContactPoint, ContactPoint groundContactPoint, out RaycastHit hitInfo)
    {
        hitInfo = default(RaycastHit);

        Collider stepCollider = stepTestContactPoint.otherCollider;
        if (Mathf.Abs(stepTestContactPoint.normal.y) >= 0.2f) return false;
        if (stepTestContactPoint.point.y - groundContactPoint.point.y > maxStepHeight) return false;
        
        float stepHeight = groundContactPoint.point.y + maxStepHeight + 0.001f;
        Vector3 stepTestInvDir = new Vector3(-stepTestContactPoint.normal.x, 0, -stepTestContactPoint.normal.z).normalized;
        Vector3 origin = new Vector3(stepTestContactPoint.point.x, stepHeight, stepTestContactPoint.point.z) + (stepTestInvDir * stepSearchOvershoot);
        Vector3 direction = Vector3.down;
        if ( !(stepCollider.Raycast(new Ray(origin, direction), out hitInfo, maxStepHeight)) ) {
            return false;
        }

        return true;
    }

    protected bool IsOnSlope(out Vector3 slopeNormal)
    {
        RaycastHit slopeHitInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHitInfo, 10)) {
            float angle = Vector3.Angle(Vector3.up, slopeHitInfo.normal);
            slopeNormal = slopeHitInfo.normal;
            return angle < maxSlopeAngle && angle != 0;
        }

        slopeNormal = default(Vector3);
        return false;
    }

    protected abstract bool IsMoving();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : WalkingObject
{
    public float walkSpeed = 5.0f;
    public GameObject sword;
    public GameObject hand;
    public int health = 5;

    private Animator m_Animator;
    private Vector2 m_MovementInput;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();

        m_Animator = GetComponent<Animator>();

        // sword.transform.parent = hand.transform;
        // sword.transform.position = Vector3.zero;
    }

    void Update()
    {
        if (IsMoving()) {
            float angle = Vector2.SignedAngle(
                Vector2.right,
                new Vector2(m_MovementInput.x, -m_MovementInput.y)
            );
            transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));

            Vector3 moveDirection = new Vector3(m_MovementInput.x, 0, m_MovementInput.y);

            RaycastHit hitInfo;
            ContactPoint? test = FindStep(out hitInfo);
            if (test != null) {
                ContactPoint resolved = test.GetValueOrDefault();
                moveDirection = Vector3.ProjectOnPlane(moveDirection, hitInfo.normal).normalized * walkSpeed;
            }
            
            m_Rigidbody.velocity = moveDirection * walkSpeed;
        } else {
            m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
        }
    }
    
    void OnMove(InputValue movementValue)
    {
        m_MovementInput = movementValue.Get<Vector2>();

        bool isMoving = m_MovementInput.x != 0 || m_MovementInput.y != 0;
        m_Animator.SetBool("IsWalking", isMoving);
    }

    void OnFire()
    {
        m_Animator.SetTrigger("AttackTrigger");
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("health is " + health);

        if (health == 0) {
            Destroy(gameObject);
        }
    }

    protected override bool IsMoving()
    {
        return m_MovementInput.x != 0 || m_MovementInput.y != 0;
    }
}

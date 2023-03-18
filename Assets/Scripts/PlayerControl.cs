using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public GameObject sword;
    public GameObject hand;
    public int health = 5;

    private Animator m_Animator;
    private Vector2 m_MovementInput;
    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        // sword.transform.parent = hand.transform;
        // sword.transform.position = Vector3.zero;
    }

    void Update()
    {
        if (m_MovementInput.x != 0 || m_MovementInput.y != 0) {

            // TODO: work more with physics, figure out why it's falling over
            // m_Rigidbody.velocity = new Vector2(m_MovementInput.x * walkSpeed, m_MovementInput.y * walkSpeed);

            float angle = Vector2.SignedAngle(
                Vector2.right,
                new Vector2(m_MovementInput.x, -m_MovementInput.y)
            );
            transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));

            transform.Translate(Vector3.right * Time.deltaTime * walkSpeed);
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
}

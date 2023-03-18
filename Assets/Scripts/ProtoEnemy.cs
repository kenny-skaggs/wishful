using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProtoEnemy : MonoBehaviour
{
    public float walkSpeed = 2.0f;
    public float attackRange;

    private Animator m_Animator;
    private Collider m_Collider;
    private bool m_IsAttacking;
    private bool m_IsPlayerInRange = false;
    private NavMeshAgent m_NavMeshAgent;
    private GameObject m_Player;

    void Start()
    {
        m_Player = GameObject.Find("player");
        m_NavMeshAgent = GetComponent<NavMeshAgent>();

        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();


        m_NavMeshAgent.updateRotation = false;
    }

    void Update()
    {
        if (m_IsPlayerInRange) {
            m_NavMeshAgent.destination = transform.position;
            Attack();
        } else if (!m_IsAttacking) {
            transform.LookAt(m_Player.transform);
            transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
            // transform.Translate(
            //     new Vector3(-walkSpeed * Time.deltaTime, 0.0f, 0.0f)
            // );
            m_NavMeshAgent.destination = m_Player.transform.position;
        }
    }

    void FixedUpdate()
    {
        m_IsPlayerInRange = IsPlayerInAttackRange();
    }

    void Attack()
    {
        if (!m_IsAttacking) {
            m_Animator.SetTrigger("AttackTrigger");
            m_IsAttacking = true;
        }
    }

    void OnAttackEnd()
    {
        m_IsAttacking = false;
    }

    void OnInflictDamage()
    {
        PlayerControl target = GetObjectInAttackRange();
        if (target != null) {
            target.TakeDamage(1);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "weapon") {
            // Destroy(gameObject);
        }
    }

    bool IsPlayerInAttackRange()
    {
        PlayerControl attackTarget = GetObjectInAttackRange();
        return attackTarget != null;
    }

    PlayerControl GetObjectInAttackRange()
    {
        RaycastHit hit;
        bool hitDetected = Physics.BoxCast(
            m_Collider.bounds.center,
            transform.localScale,
            -transform.right,
            out hit,
            transform.rotation,
            attackRange
        );
        if (hitDetected && hit.collider.CompareTag("Player")) {
            return hit.collider.gameObject.GetComponent<PlayerControl>();
        } else {
            return null;
        };
    }
}

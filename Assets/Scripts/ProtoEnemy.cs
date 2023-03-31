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
    private bool m_IsRendering = false;
    private NavMeshAgent m_NavMeshAgent;
    private GameObject m_Player;

    void Start()
    {
        m_Player = GameObject.Find("player");

        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();

        m_NavMeshAgent.updateRotation = false;
        ToggleRender(false);
    }

    void Update()
    {
        m_IsPlayerInRange = IsPlayerInAttackRange();

        bool shouldBeVisible = IsVisibleToPlayer();
        if (shouldBeVisible != m_IsRendering) {
            m_IsRendering = shouldBeVisible;
            ToggleRender(m_IsRendering);
        }

        if (m_IsPlayerInRange) {
            Attack();
        } else if (!m_IsAttacking) {
            m_NavMeshAgent.destination = m_Player.transform.position;

            transform.LookAt(m_Player.transform, Vector3.up);
            transform.Rotate(new Vector3(0, 90, 0), Space.Self);
        }
    }

    void LookAt(Vector3 location)
    {
        transform.LookAt(
            new Vector3(
                location.x,
                transform.position.y,
                location.z
            )
        );
        transform.Rotate(new Vector3(0.0f, 90.0f, 0.0f));
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

    protected void ToggleRender(bool shouldRender)
    {
        foreach (Transform childTransform in transform) {
            GameObject child = childTransform.gameObject;
            SkinnedMeshRenderer renderer = child.GetComponent<SkinnedMeshRenderer>();
            if (renderer != null) {
                renderer.enabled = shouldRender;
            }
        }
    }

    protected bool IsVisibleToPlayer()
    {
        return (m_Player.transform.position - transform.position).magnitude < 15;
    }
}

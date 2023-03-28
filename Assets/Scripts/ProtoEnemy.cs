using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ProtoEnemy : WalkingObject
{
    public float walkSpeed = 2.0f;
    public float attackRange;

    private Animator m_Animator;
    private Collider m_Collider;
    private bool m_IsAttacking;
    private bool m_IsPlayerInRange = false;
    private GameObject m_Player;

    private float elapsed = 0;
    private NavMeshPath path;

    private Queue<Vector3> pathToGoal;

    override protected void Start()
    {
        base.Start();

        m_Player = GameObject.Find("player");

        m_Animator = GetComponent<Animator>();
        m_Collider = GetComponent<Collider>();

        path = new NavMeshPath();
        pathToGoal = new Queue<Vector3>();
    }

    void Update()
    {
        m_IsPlayerInRange = IsPlayerInAttackRange();

        if (m_IsPlayerInRange) {
            Attack();
        } else if (!m_IsAttacking) {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = m_Player.transform.position;

            // elapsed += Time.deltaTime;
            // if (elapsed > 2.0f)
            // {
            //     elapsed = 0;
            //     RecalculatePath();
            // }

            // if (pathToGoal.Count > 0) {
            //     CheckAtCheckpoint();
            //     Vector3 firstCorner = GetNextCorner();
            //     LookAt(firstCorner);
            //     Debug.DrawRay(firstCorner, Vector3.up, Color.yellow, 3);
            //     Vector3 movementDirection = firstCorner - transform.position;

            //     Vector3 newVelocity = new Vector3(movementDirection.x, m_Rigidbody.velocity.y, movementDirection.z).normalized * walkSpeed;
            //     m_Rigidbody.velocity = newVelocity;
            // }
        }
    }

    void RecalculatePath()
    {
        NavMesh.CalculatePath(transform.position, m_Player.transform.position, NavMesh.AllAreas, path);
        pathToGoal.Clear();

        foreach(Vector3 corner in path.corners) {
            pathToGoal.Enqueue(corner);
        }
    }

    Vector3 GetNextCorner()
    {
        // int index = 0;
        
        // Vector3 nextCorner = corners[index];
        // while ((nextCorner - transform.position).magnitude < walkSpeed) {
        //     index += 1;
        //     nextCorner = corners[index];
        // }

        // return nextCorner;

        return pathToGoal.Peek();
    }

    void CheckAtCheckpoint()
    {
        Vector3 nextCorner = GetNextCorner();
        Vector3 distanceTo = nextCorner - transform.position;
        if (distanceTo.magnitude < attackRange) {
            Debug.Log("deque " + nextCorner);
            pathToGoal.Dequeue();
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

    protected override bool IsMoving()
    {
        return m_Rigidbody.velocity.x != 0 || m_Rigidbody.velocity.y != 0;
    }
}

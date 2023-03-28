using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Vector3 offset;
    
    private GameObject m_Player;

    void Start()
    {
        m_Player = GameObject.Find("player");

        transform.position = m_Player.transform.position + offset;
        transform.LookAt(m_Player.transform.position, Vector3.up);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = m_Player.transform.position + offset;
    }
}

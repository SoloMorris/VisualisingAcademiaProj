using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxsize : MonoBehaviour
{
    BoxCollider m_Collider;
    float m_ScaleX;
    float rectSize;

    // a small script to resize the box collider to fit with the rect transform width
    void Start()
    {
        m_Collider = GetComponent<BoxCollider>();

    }

    void Update()
    {
        rectSize = GetComponent<RectTransform>().rect.width;

        if (rectSize != m_Collider.size.x)
        {
            m_ScaleX = rectSize;
            m_Collider.size = new Vector3(m_ScaleX, m_Collider.size.y, m_Collider.size.z);
        }

        Debug.Log("Current Rect Size : " + rectSize);
        Debug.Log("Current BoxCollider Size : " + m_Collider.size);
    }
}

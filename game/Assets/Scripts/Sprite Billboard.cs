using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteBillboard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
}

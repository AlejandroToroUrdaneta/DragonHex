using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform actualPlayerUnit;
    public CameraControl control;
    void LateUpdate()
    {
        actualPlayerUnit.transform.position = control.player.transform.position;
        Vector3 newPosition = actualPlayerUnit.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}

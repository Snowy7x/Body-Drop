using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    void Update()
    {
        Aim();
    }

    void Aim()
    {
        Vector3 pozycja = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float AngleRad = Mathf.Atan2(pozycja.y - transform.position.y, pozycja.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        this.transform.rotation = Quaternion.Euler(0, 0, AngleDeg-35);
    }

    public void Shoot()
    {
        // TODO: Shooting
    }
}

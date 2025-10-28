using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class LaserFX : MonoBehaviour
{
    LineRenderer lr;

    void Awake() { lr = GetComponent<LineRenderer>(); lr.enabled = false; }

    public void Zap(Vector3 from, Vector3 to)
    {
       GameManager.I?.PlayLaser(from);
        StopAllCoroutines();
        StartCoroutine(DoZap(from, to));
    }

    IEnumerator DoZap(Vector3 a, Vector3 b)
    {
        lr.enabled = true;
        lr.positionCount = 2;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);
        yield return new WaitForSeconds(0.05f);
        lr.enabled = false;
    }
    

}

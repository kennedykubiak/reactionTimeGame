using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    Transform target;
    float speed;
    bool alive = true;

    public void Init(Transform t)
    {
        target = t;
        speed = GameManager.I.CurrentEnemySpeed();
        FaceTarget();
    }

    void FaceTarget()
    {
        if (!target) return;
        Vector2 dir = (target.position - transform.position).normalized;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(ang - 90f, Vector3.forward);
    }

    void Update()
    {
        if (GameManager.I.state != GameState.Playing || !alive) return;

        speed = GameManager.I.CurrentEnemySpeed();
        transform.position = Vector3.MoveTowards(
            transform.position, target.position, speed * Time.deltaTime);

        if (GameManager.I.FireButtonDown())
        {
            var cam = Camera.main;
            Vector3 m = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 p = new Vector2(m.x, m.y);
            var hit = Physics2D.OverlapPoint(p);
            if (hit && hit.gameObject == this.gameObject)
            {
                Blast();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!alive) return;
        if (other.GetComponent<CenterBase>() != null)
        {
            alive = false;
            GameManager.I.LoseLife();
            Destroy(gameObject);
        }
    }

    void Blast()
    {
        if (!alive) return;
        alive = false;

        GameManager.I.laserFx?.Zap(GameManager.I.centerTarget.position, transform.position);
        GameManager.I.PlayBlast(transform.position);
        GameManager.I.AddScore(1);
        Destroy(gameObject);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StandardEnemyAIBehaviour;
using UnityEngine.XR;

public class JoshEnemy : StandardRangedEnemy
{
    private void OnEnable()
    {
        OnStateUpdate += UpdateState;
        StartCoroutine(PointTowardsPlayer());
    }

    public IEnumerator Attack()
    {
        while (isTargetOnViewRange)
        {
            yield return StartCoroutine(ShootProjectileAnim());
            
        }
        if (!isTargetOnViewRange) State = EnemyState.Idle;
    }


    private void Start()
    {
        UpdateState();
        StartCoroutine(IsOnViewRange());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (player == null) return;
        Gizmos.DrawRay(transform.position, player.position - transform.position);
    }

    public IEnumerator Idle()
    {
        while (State == EnemyState.Idle)
        {
            if (isTargetOnViewRange)
            {
                State = EnemyState.Attacking;
            }
            yield return null;
        }
    }


    public void InstantiateProjectile()
    {
        GameObject pr = Instantiate(projectile, projectileOrigin.position, Quaternion.LookRotation(player.position - transform.position));
        projectileAmount.Add(pr);
        StartCoroutine(ProjectileBehaviour(pr));
    }
    public IEnumerator ShootProjectileAnim()
    {
        anim.SetTrigger("attack");
        yield return null;
        yield return new WaitForSeconds(stats.attackSpeed);
    }
    private IEnumerator PointTowardsPlayer()
    {
        while (true)
        {
            yield return new WaitUntil(() => isTargetOnViewRange == true);
            Physics.Raycast(transform.position, player.position - transform.position, out RaycastHit hit, Mathf.Infinity, -1, QueryTriggerInteraction.Ignore);
            Vector3 point = hit.point;
            point.y = transform.position.y;
            Vector3 dir = point - transform.position;
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            yield return null;
        }

    }

    public override IEnumerator TakeHitboxDamage(HitBox hitBox)
    {
        if (isDead) yield break;
        CurrentHealth -= hitBox.value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, MinHealth, MaxHealth);
        Debug.Log($"Enemy: {gameObject}  Health: {CurrentHealth}");
        if (CurrentHealth == 0) 
        {
            StartCoroutine(Death());
        }
    }

    public override IEnumerator Death()
    {
        yield return null;
    }

    public IEnumerator IsOnViewRange()
    {
        while (true)
        {
            Physics.Raycast(transform.position, player.position - transform.position, out RaycastHit hit, stats.detectionRange, LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
            if(hit.collider != null) isTargetOnViewRange = hit.collider.CompareTag("Player");
            yield return null;
        }
    }
    public override void UpdateState() 
    {
        switch (State)
        {
            case EnemyState.Idle:
                StartCoroutine(Idle());
                break;
            case EnemyState.Attacking:
                StartCoroutine(Attack());
                break;
            case EnemyState.Dead:
                PlayerOutOfDetectionRangeEvent();
                break;
        }
    }

}

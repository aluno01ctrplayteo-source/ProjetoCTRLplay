using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StandardEnemyAIBehaviour;
using UnityEngine.XR;
using System.Threading.Tasks;
using System.Threading;

public class JoshEnemy : StandardRangedEnemy
{

    public IEnumerator AttackState()
    {
        int Attacking = 0;

        void PointTowards(RaycastHit h, out bool pointingtowards)
        {
            Vector3 point = h.point;
            point.y = transform.position.y;
            Vector3 dir = point - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(dir, transform.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            pointingtowards = angle < 15f;
        }   

        GameObject InstantiateProjectile()
        {
            GameObject pr = Instantiate(projectile, projectileOrigin.position, Quaternion.LookRotation(player.position - transform.position));
            HitBox prhb = pr.GetComponent<HitBox>();
            projectileAmount.Add(pr);
            StartCoroutine(ProjectileBehaviour(pr, prhb));
            return pr;
        }

        IEnumerator ShootProjectile()
        {
            if (Interlocked.Exchange(ref Attacking, 1) == 1) yield break;
            anim.SetTrigger("attack");
            yield return new WaitForSeconds(.3f);
            InstantiateProjectile();
            yield return new WaitForSeconds(stats.attackSpeed);
            Interlocked.Exchange(ref Attacking, 0);
        }

        while (State == EnemyState.Attacking)
        {
            bool canSeePlayer = CanSeePlayer(out RaycastHit hit);
            PointTowards(hit, out bool b);
            if (!canSeePlayer) 
            {
                ChangeState(EnemyState.Idle);
                yield break;
            }
            if (b)
            {
                StartCoroutine(ShootProjectile());
            }
            yield return null;
        }
    }


    private void Start()
    {
        ChangeState(EnemyState.Idle);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (player != null) 
        {
            Gizmos.DrawRay(transform.position, transform.position - player.position); 
        }
    }

    public IEnumerator IdleState()
    {
        while (State == EnemyState.Idle)
        {
            if (CanSeePlayer())
            {
                ChangeState(EnemyState.Attacking);
                yield break;
            }
            yield return null;
        }
    }


    public override IEnumerator TakeHitboxDamage(HitBox hitBox)
    {
        CurrentHealth -= hitBox.value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, MinHealth, MaxHealth);
        Debug.Log($"Enemy: {gameObject}  Health: {CurrentHealth}");
        if (CurrentHealth == 0) 
        {
            ChangeState(EnemyState.Dead);
        }
        yield return null;
    }

    public bool CanSeePlayer()
    {
      Physics.Raycast(transform.position, player.position - transform.position, out RaycastHit ray, stats.detectionRange, LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
      return ray.collider != null ? ray.collider.CompareTag("Player") : false;
    }

    public bool CanSeePlayer(out RaycastHit hit)
    {
        Physics.Raycast(transform.position, player.position - transform.position, out hit, stats.detectionRange, LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);
        return hit.collider != null ? hit.collider.CompareTag("Player") : false;
    }

    public override void ChangeState(EnemyState newState)
    {
        if (currentStateRoutine != null) StopCoroutine(currentStateRoutine);

        State = newState;

        switch (State)
        {
            case EnemyState.Idle:
                currentStateRoutine = StartCoroutine(IdleState());
                break;
            case EnemyState.Attacking:
                currentStateRoutine = StartCoroutine(AttackState());
                break;
            case EnemyState.Dead:
                StartCoroutine(DeathState());
                break;
        }
    }

    public override IEnumerator DeathState()
    {
        yield return null;
    }
}

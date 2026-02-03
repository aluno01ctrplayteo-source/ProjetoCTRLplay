
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using StandardEnemyAIBehaviour;

public class JohnEnemy : StandardMeleeEnemy
{
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.forward, .5f);
    }
    
    private void OnEnable()
    {
        gameManager.OnEnemyDeath += () => { gameManager.killCount++; Debug.Log("Killed an enemy"); };
        OnPlayerDetection += () => { _detectionRange = _defaultDetectionRange * 1.1f; };
        OnPlayerOutOfDetectionRange += () => { _detectionRange = _defaultDetectionRange; };
        OnTakeDamage += () => { StartCoroutine(ProcDamageAnim()); };
    }



    public override IEnumerator Attack()
    {
        isAttacking = true;
        canMove = false;

        yield return new WaitForSeconds(1f);

        if (!IsAttackTriggerOverriding("Player", "Player")) { isAttacking = false; canMove = true; yield break; }
        anim.SetTrigger("isAttacking");
        AnimatorStateInfo currentAnim = anim.GetCurrentAnimatorStateInfo(0);
        while (!(currentAnim.IsName("RockRig_Attack")))
        {
            yield return null;
            currentAnim = anim.GetCurrentAnimatorStateInfo(0);
        }
        anim.ResetTrigger("isAttacking");
        yield return new WaitForSeconds(currentAnim.length);
        yield return new WaitForSeconds(.1f);
        

        isAttacking = false;
        canMove = true;

    }
    public bool IsAttackTriggerOverriding(string tag, string layer)
    {
        Collider[] hit = new Collider[10];
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position + transform.forward, .5f, hit, LayerMask.GetMask(layer), QueryTriggerInteraction.Ignore);
        if (hitCount == 0) return false;
        
        for (int i = 0; i < hitCount; i++)
        {
            if (hit[i].transform != null && hit[i].transform.CompareTag(tag))
            {
                return true;
            }
        }
           
        return false;
    }
    public override IEnumerator DeathState()
    {
        isDead = true;
        gameManager.RaiseEnemyDeathEvent();
        anim.SetTrigger("isDying");

        yield return new WaitForSeconds(3f);

        StartCoroutine(DeathDisappear());
        Instantiate(dropPrefab, transform.position, Quaternion.identity);

    }
    IEnumerator DeathDisappear()
    {
        while (true) 
        {
            if (gameManager.isPaused) { yield return null; continue; }
            if (transform.localScale.magnitude <= 0.05f) { Destroy(gameObject); }


            transform.localScale += Vector3.one * -.005f;
            yield return null;
            
        }
    }
    public bool IsOnViewRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        return distanceToPlayer <= _detectionRange;
    }

    public void FollowPlayer()
    {
        
        if (!IsOnViewRange() || !canMove || isDead) { agent.isStopped = true; anim.SetFloat("movement", Mathf.Lerp(anim.GetFloat("movement"), 0, 10f * Time.deltaTime)); PlayerOutOfDetectionRangeEvent(); return; }
        PlayerDetectionEvent();
        agent.isStopped = false;
        agent.SetDestination(player.position);
        anim.SetFloat("movement", Mathf.Lerp(anim.GetFloat("movement"), 1, 10f * Time.deltaTime));
    }
    void Update()
    {
        if (isDead)
        {
            canMove = false;
            isAttacking = false;
        }
        FollowPlayer();
        if (IsAttackTriggerOverriding("Player", "Player") && !isAttacking) StartCoroutine(Attack());

    }

    public override void Heal(int amount)
    {
        CurrentHealth += amount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, MinHealth, MaxHealth);
    }
        
    public override IEnumerator TakeDirectDamage(HitBox hitbox)
    {
        if (hitbox.type != HitboxType.Damage) yield break;
        TakeDamageEvent();
        CurrentHealth -= hitbox.value;
        CurrentHealth = Mathf.Clamp(CurrentHealth, MinHealth, MaxHealth);
        Debug.Log($"Enemy: {gameObject}  Health: {CurrentHealth}");
        if (CurrentHealth < 0.1f) StartCoroutine(DeathState());
    }
}
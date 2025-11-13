using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected LivingRockEnemyAI enemyAI;
    protected EnemyStateMachine stateMachine;

    public EnemyState(LivingRockEnemyAI enemyAI, EnemyStateMachine stateMachine)
    {
        this.enemyAI = enemyAI;
        this.stateMachine = stateMachine;
    }
}

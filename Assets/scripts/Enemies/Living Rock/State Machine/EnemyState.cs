using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected EnemyAI enemyAI;
    protected EnemyStateMachine stateMachine;

    public EnemyState(EnemyAI enemyAI, EnemyStateMachine stateMachine)
    {
        this.enemyAI = enemyAI;
        this.stateMachine = stateMachine;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveableEnemy
{
    void FollowPlayer();
    void ApplyGravity();
}

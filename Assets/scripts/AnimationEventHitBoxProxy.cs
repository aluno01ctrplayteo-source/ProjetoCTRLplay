using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AnimationEventHitBoxProxy : MonoBehaviour
{
    public HitBox hitbox;


    public void CallFunc()
    {
        hitbox.SetActive();
    }
}

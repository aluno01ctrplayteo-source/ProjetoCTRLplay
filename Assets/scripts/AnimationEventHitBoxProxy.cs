using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
public class AnimationEventHitBoxProxy : MonoBehaviour
{
    public HitBox hitbox;


    public void Activate()
    {
        hitbox.SetActive(true);
    }
    public void DeActivate()
    {
        hitbox.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo
{
    public int DamageAmount;
    public float CritMultiplier = 1;
    public GameObject DamageSource;
}

public interface IDamagable
{
    public void Damage(DamageInfo info);
}


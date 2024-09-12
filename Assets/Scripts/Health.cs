using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class Health : NetworkBehaviour
{
    public int maxHealth = 100;
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public int NetworkedHealth { get; set; } = 100;



    public override void Spawned()
    {
        NetworkedHealth = maxHealth;
    }
    void OnHealthChanged(){
        Debug.Log("Health changed");
    }
    void OnKillCountChange(){
        Debug.Log("Kill count changed");
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void TakeDamageRpc(int damage){
        if(HasStateAuthority){
            TakeDamage(damage);
        }
    }
    void TakeDamage(int damage){
        NetworkedHealth -= damage;
        if (NetworkedHealth < 0)
        {
            Runner.Despawn(Object);
        }
    }
}

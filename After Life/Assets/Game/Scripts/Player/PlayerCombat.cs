using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{

    public Animator animator;
    [SerializeField] Animator vfx;
    public PlayerMovement pM;
    public CharacterController2D cc2D;

    [SerializeField] Transform attackPoint;
    [SerializeField] float attackRange = 0.5f;
    [SerializeField] LayerMask enemyLayers;
    
    public void EnAtk1(){

        pM.m_Attacking = true;
        Attack("One", attackRange, 25);
    }
    public void ExAtk1(){

        pM.m_Attacking = false;
    }

    public void EnAtk2(){

        pM.m_Attacking = false;
        Attack("Two", attackRange, 25, false);
    }

    float par = 0;
    public void EnAtk3(){

        pM.m_Attacking = true;
        cc2D.m_Attacking04 = true;
        par = attackRange;
        attackRange = 3f;
        attackPoint.localPosition = new Vector3(1, attackPoint.localPosition.y, attackPoint.localPosition.z);
        Attack("Three", 3f, 50);
    }
    public void ExAtk3(){

        attackRange = par;
        attackPoint.localPosition = new Vector3(2, attackPoint.localPosition.y, attackPoint.localPosition.z);
        pM.m_Attacking = false;
        cc2D.m_Attacking04 = false;
    }

    public void EnAtk4(){

        pM.m_Attacking = true;
        cc2D.m_Attacking04 = true;
        Attack("Four", attackRange, 50);
    }
    public void ExAtk4(){

        pM.m_Attacking = false;
        cc2D.m_Attacking04 = false;
    }

    void Attack(string type, float radius, int damage, bool animatePlayer = true){
        
        vfx.transform.position = transform.position;
        vfx.transform.localScale = transform.localScale;
        
        // Play attack animation
        if(animatePlayer)
            animator.Play($"Attack {type}");
        else
            animator.Play("Player_Idle");
        
        // if(animateVfx)
        //     vfx.SetTrigger($"Attack-{type}");

        // Detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, radius, enemyLayers);

        // Damage them
        foreach(Collider2D enemy in hitEnemies){

            Debug.Log($"we hit {enemy.name}");
            if(enemy.name == "VS757"){
                enemy.GetComponent<VS757>().TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected(){

        if(attackPoint == null){return;}

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

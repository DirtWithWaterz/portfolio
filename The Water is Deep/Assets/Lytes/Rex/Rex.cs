using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Rex : MonoBehaviour
{
    [SerializeField] float speed;
    Player player;
    Rigidbody2D rb;
    [SerializeField] DeathMessages deathMessages;
    bool once;
    Vector3 randomPoint;
    private void Start() {
        player = FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate(){
        if(!player.rb.velocity.Equals(new Vector2(0, 0))){
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.fixedDeltaTime);
        } else{
            if(!once){
                randomPoint = new Vector3(Mathf.RoundToInt(Random.Range(-78, 78)), Mathf.RoundToInt(Random.Range(-78, 78)), 0);
                StartCoroutine(MoveRandDir());
                once = true;
            } else if(randomPoint != null){
                transform.position = Vector3.MoveTowards(transform.position, randomPoint, speed * Time.deltaTime);
            } else{
                once = false;
            }

            
        }
    }

    IEnumerator MoveRandDir(){
        transform.position = Vector3.MoveTowards(transform.position, randomPoint, speed * Time.deltaTime);
        yield return new WaitForSeconds(Random.Range(3, 20));
        once = false;
    }













    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            other.gameObject.SetActive(false);
            deathMessages.playerDied = true;
            
        }
    }
}

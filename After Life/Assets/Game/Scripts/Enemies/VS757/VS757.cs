using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VS757 : MonoBehaviour
{

    public float ID;

    public int maxHealth = 100;
    public int currentHealth;

    public bool takingDamage;

    public bool HitSurprised;

    void Start(){

        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage){

        takingDamage = true;
        currentHealth -= damage;

        // Play hurt anim
        if(firstNotice)
            vsa.Play("Hit");
        else{

            firstNotice = true;
            surprised = true;
            HitSurprised = true;
            vsa.Play("HitSurprised");
        }
    }

    public void Die(){

        takingDamage = true;
        // Play die anim
        vsa.Play("Death");
    }

    #region Pathing

    public Rigidbody2D vsrb;

    public Animator vsa;

    public Animator vsvfxa;

    [SerializeField] Transform flamer;
    
    [SerializeField] Transform origin;
    [SerializeField] int rayAmount = 3;
    [SerializeField] float range = 10;

    public bool surprised;

    public bool firstNotice = false;

    public bool attack;

    public bool weHittinSmth;
    public bool weInRange;

    float lastHitDist = 5;
    float currHitDist = 5;

    bool turnedAlready = false;

    float rotation;
    [SerializeField] float rayDistance;

    [SerializeField] LayerMask visibleLayers;

    void Awake(){

        vsa = this.GetComponent<Animator>();
        vsvfxa = this.transform.parent.transform.GetChild(1).GetComponent<Animator>();
        vsrb = GetComponent<Rigidbody2D>();
        lastHitDist = currHitDist;
    }

    // Update is called once per frame
    void Update()
    {
        if(takingDamage || surprised)
            return;

        rotation = rayAmount/2;

        for(int i = 0; i < rayAmount; i++){

            Vector2 direction = new Vector2((rayDistance*5)*Mathf.Clamp(transform.localScale.x, -1, 1), (rotation*5)-5*i);

            RaycastHit2D hit = Physics2D.Raycast(origin.position, direction, range, visibleLayers);

            weHittinSmth = hit;

            Debug.DrawRay(origin.position, direction, Color.red);

            if(hit.transform != null){

                if(hit.transform.tag == "Player"){

                    if(!firstNotice){
                        surprised = true;
                    }
                    firstNotice = true;


                    Debug.Log($"I see the {hit.transform.name}");

                    weInRange = !takingDamage && Mathf.RoundToInt(Mathf.Abs(hit.point.x - transform.position.x)) >= 4 ? false : true;

                    Debug.Log(Mathf.RoundToInt(Mathf.Abs(hit.point.x - transform.position.x)));

                    if(!weInRange && !vsa.GetCurrentAnimatorStateInfo(0).IsName("AtkB") && !vsa.GetCurrentAnimatorStateInfo(0).IsName("AtkM") && !vsa.GetCurrentAnimatorStateInfo(0).IsName("AtkE")){

                        vsrb.velocity = new Vector2(Mathf.Clamp(direction.x, -3, 3), vsrb.velocity.y);
                        attack = false;
                    }
                    else{
                        
                        vsrb.velocity = new Vector2(0, vsrb.velocity.y);
                        flamer.localScale = transform.localScale;
                        flamer.position = transform.position;
                        if(!takingDamage)
                            attack = true;
                    }
                    currHitDist = Mathf.Abs(hit.point.x - transform.position.x);
                }
            }
            else{
                vsrb.velocity = new Vector2(0, vsrb.velocity.y);
                attack = false;
                lastHitDist = currHitDist;
            }
        }

        if(lastHitDist < 4 && !turnedAlready){

            Turn(1);
        }
    }

    IEnumerator TurnAround(float wait){

        turnedAlready = true;

        yield return new WaitForSeconds(wait);

        if(!attack)
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);

        lastHitDist = 5;

        turnedAlready = false;
    }

    public void Turn(float wait){

        StartCoroutine(TurnAround(wait));
    }

    #endregion
}

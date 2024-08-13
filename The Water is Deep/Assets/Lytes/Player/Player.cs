using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public float moveSpeed = 1f;

    [SerializeField] Light2D light2D;

    private Vector2 moveInput;
    public Rigidbody2D rb;

    float timeLeft;
    Color targetColor;

    float scalingRate = 1;
    float foodInnerRadius = 0.0225f;
    float foodOuterRadius = 0.235f;
    float playerInnerRadius = 0.045f;
    float playerOuterRadius = 0.65f;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate(){
        rb.AddForce(moveInput * moveSpeed * Time.fixedDeltaTime);
        if(rb.velocity.Equals(new Vector2(0, 0))){
            if (timeLeft <= Time.deltaTime)
            {
                // transition complete
                // assign the target color
                light2D.color = targetColor;
 
                // start a new transition
                targetColor = new Color(0, 0.1776958f, 1);
                timeLeft = 1.0f;
            }
            else
            {
                // transition in progress
                // calculate interpolated color
                light2D.color = Color.Lerp(light2D.color, targetColor, Time.deltaTime / timeLeft);
 
                // update the timer
                timeLeft -= Time.deltaTime;
            }

            light2D.pointLightInnerRadius = Mathf.Lerp(light2D.pointLightInnerRadius, foodInnerRadius, scalingRate * Time.deltaTime);
            light2D.pointLightOuterRadius = Mathf.Lerp(light2D.pointLightOuterRadius, foodOuterRadius, scalingRate * Time.deltaTime);

        } else{
            if (timeLeft <= Time.deltaTime)
            {
                // transition complete
                // assign the target color
                light2D.color = targetColor;
 
                // start a new transition
                targetColor = new Color(0, 0.8429375f, 1);
                timeLeft = 1.0f;
            }
            else
            {
                // transition in progress
                // calculate interpolated color
                light2D.color = Color.Lerp(light2D.color, targetColor, Time.deltaTime / timeLeft);
 
                // update the timer
                timeLeft -= Time.deltaTime;
            }

            light2D.pointLightInnerRadius = Mathf.Lerp(light2D.pointLightInnerRadius, playerInnerRadius, scalingRate * Time.deltaTime);
            light2D.pointLightOuterRadius = Mathf.Lerp(light2D.pointLightOuterRadius, playerOuterRadius, scalingRate * Time.deltaTime);

        }
    }

    void OnMove(InputValue value){
        rb.drag = 1;
        rb.mass = 1;
        moveInput = value.Get<Vector2>();
    }

    void OnStop(){
        rb.drag = 50;
        rb.mass = 50;
    }
}

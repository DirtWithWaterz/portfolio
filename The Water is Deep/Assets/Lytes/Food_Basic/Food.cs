using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    Counter counter;
    FoodValues foodValues;
    private void Start() {
        foodValues = FindObjectOfType<FoodValues>();
        counter = FindObjectOfType<Counter>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Player"){
            counter.foodLeft = counter.foodLeft - 1;
            foodValues.playerFood = foodValues.playerFood + 1;
            Destroy(this.gameObject);
        }
        if(other.tag == "Eater"){
            counter.foodLeft = counter.foodLeft - 1;
            foodValues.eaterFood = foodValues.eaterFood + 1;
            Destroy(this.gameObject);
        }
    }
}

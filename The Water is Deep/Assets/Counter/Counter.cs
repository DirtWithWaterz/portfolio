using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour
{
    [SerializeField] private TMP_Text counter;
    public int foodLeft;
    // Start is called before the first frame update
    void Start()
    {
        foodLeft = 27;
    }

    // Update is called once per frame
    void Update()
    {
        counter.text = foodLeft.ToString();
    }
}

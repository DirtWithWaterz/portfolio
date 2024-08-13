using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
    [SerializeField]
    int HydrogenAmount = 0;
    [SerializeField]
    int HeliumAmount = 0;
    [SerializeField]
    int AntimonyAmount = 0;
    [SerializeField]
    GameObject Atom;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < HydrogenAmount; i++)
        {
            var a = Instantiate(Atom, new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f)), Quaternion.identity);
            a.AddComponent<Hydrogen>();
            a.name = "Hydrogen Atom";
        }
        for (int i = 0; i < HeliumAmount; i++)
        {
            var a = Instantiate(Atom, new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f)), Quaternion.identity);
            a.AddComponent<Helium>();
            a.name = "Helium Atom";
        }
        for (int i = 0; i < AntimonyAmount; i++)
        {
            var a = Instantiate(Atom, new Vector2(Random.Range(-200f, 200f), Random.Range(-200f, 200f)), Quaternion.identity);
            a.AddComponent<Antimony>();
            a.name = "Antimony Atom";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

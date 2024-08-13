using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antimony : Atom
{

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Serialize()
    {
        elementGroup = ElementGroups.Metalloid;

        Element = "Sb";
        AtomicNumber = 51;
        AtomicMass = 121.76f;
        Protons = 51;
        Neutrons = 70;
        Electrons = 51;
        ValenceElectrons = 5;
    }
}

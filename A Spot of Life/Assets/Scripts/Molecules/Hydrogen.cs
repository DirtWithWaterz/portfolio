using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hydrogen : Atom
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
        elementGroup = ElementGroups.ReactiveNonmetal;

        Element = "H";
        AtomicNumber = 1;
        AtomicMass = 1.0078f;
        Protons = 1;
        Neutrons = 0;
        Electrons = 1;
        ValenceElectrons = 1;
    }
}

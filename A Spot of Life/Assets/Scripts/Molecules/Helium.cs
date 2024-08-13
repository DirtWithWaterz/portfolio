using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helium : Atom
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
        elementGroup = ElementGroups.NobleGas;

        Element = "He";
        AtomicNumber = 2;
        AtomicMass = 4.0026f;
        Protons = 2;
        Neutrons = 2;
        Electrons = 2;
        ValenceElectrons = 2;
    }
}

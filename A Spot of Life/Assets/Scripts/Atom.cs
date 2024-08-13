using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementGroups {

    AlkaliMetal = 1,
    AlkalineEarthMetal = 2,
    TransitionMetal = 3,
    PostTransitionMetal = 4,
    Metalloid = 5,
    ReactiveNonmetal = 6,
    NobleGas = 7,
    Lanthanide = 8,
    Actinide = 9,
    Unknown = 10
}

public class Atom : MonoBehaviour
{

    public ElementGroups elementGroup;

    [HideInInspector]
    public SpriteRenderer rend;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Collider2D c;

    public string Element;
    public int AtomicNumber;
    public float AtomicMass;
    public int Protons;
    public int Neutrons;
    public int Electrons;
    public int ValenceElectrons;
    int AvailableValenceElectrons;
    public Color32 Pigment;

    public Transform[] Bonds = null;

    // Start is called before the first frame update
    public virtual void Start()
    {
        Serialize();
        rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        c = GetComponent<Collider2D>();
        UpdatePigment();
        AvailableValenceElectrons = ValenceElectrons;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, 50, Vector2.up);
        foreach(RaycastHit2D a in results)
        {
            if(a.transform == null)
            {
                return;
            }

            float fx = 0;
            float fy = 0;


            float dx = transform.position.x - a.transform.position.x;
            float dy = transform.position.y - a.transform.position.y;

            float d = Mathf.Sqrt(dx * dx + dy * dy);

            float vx = 0;
            float vy = 0;

            float F = 0;

            float g = 0;

            Atom atom = a.transform.GetComponent<Atom>();

            if(atom == null) { return; }

            g = Bonds == a.transform ? 2 : AvailableValenceElectrons + atom.AvailableValenceElectrons == 8 ? 1.5f :
                AvailableValenceElectrons + atom.AvailableValenceElectrons == 7 ? 1 :
                AvailableValenceElectrons + atom.AvailableValenceElectrons == 6 ? 0.5f :
                AvailableValenceElectrons + atom.AvailableValenceElectrons == 5 ? -0.5f :
                AvailableValenceElectrons + atom.AvailableValenceElectrons == 4 ? -1 :
                AvailableValenceElectrons + atom.AvailableValenceElectrons == 3 ? -2 :
                AvailableValenceElectrons + atom.AvailableValenceElectrons == 2 ? 1.5f :
                -1;

            if (d > 0 && d < 100)
            {
                F = g * atom.AtomicMass / d;
                fx += (F * dx);
                fy += (F * dy);
            }
            vx = (vx + fx)*0.5f;
            vy = (vy + fy)*0.5f;

            rb.velocity += new Vector2(-vx, -vy);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -8, 8), Mathf.Clamp(rb.velocity.y, -8, 8));

            if(Bonds != null)
            {
                AvailableValenceElectrons = 0;
            }
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        Atom atom = collision.transform.GetComponent<Atom>();

        if(atom == null)
        {
            if(rb == null)
            {
                return;
            }

            if(collision.transform.name == "wall R")
            {
                rb.velocity = new Vector2(-rb.velocity.x*10, rb.velocity.y);
            }
            if (collision.transform.name == "wall U")
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y*10);
            }
            if (collision.transform.name == "wall D")
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y*10);
            }
            if (collision.transform.name == "wall L")
            {
                rb.velocity = new Vector2(-rb.velocity.x*10, rb.velocity.y);
            }
            return;
        }

        if(AvailableValenceElectrons + atom.AvailableValenceElectrons == 8)
        {
            AvailableValenceElectrons = 0;
            Bonds = atom.transform;
            atom.Bonds = transform;
        }
        else if(AvailableValenceElectrons + atom.AvailableValenceElectrons == 2 && Electrons == 1)
        {
            AvailableValenceElectrons = 0;
            Bonds = atom.transform;
            atom.Bonds = transform;
        }
        AvailableValenceElectrons -= atom.AvailableValenceElectrons;
        if(AvailableValenceElectrons <= 0)
        {
            AvailableValenceElectrons = 0;
        }
    }
    public virtual void OnCollisionExit2D(Collision2D collision)
    {
        Atom atom = collision.transform.GetComponent<Atom>();
        if(atom == null)
        {
            return;
        }
        AvailableValenceElectrons = ValenceElectrons;
        Bond = null;
        atom.Bond = null;
    }

    public virtual void Serialize()
    {
        elementGroup = ElementGroups.Unknown;
        Element = "null";
        AtomicNumber = 0;
        AtomicMass = 0;
        Protons = 0;
        Neutrons = 0;
        Electrons = 0;
        ValenceElectrons = 0;
    }

    void UpdatePigment()
    {
        switch (elementGroup)
        {
            case ElementGroups.AlkaliMetal:
                Pigment = new Color32(36, 77, 87, 255);
                break;
            case ElementGroups.AlkalineEarthMetal:
                Pigment = new Color32(98, 46, 57, 255);
                break;
            case ElementGroups.TransitionMetal:
                Pigment = new Color32(67, 60, 101, 255);
                break;
            case ElementGroups.PostTransitionMetal:
                Pigment = new Color32(47, 77, 71, 255);
                break;
            case ElementGroups.Metalloid:
                Pigment = new Color32(82, 62, 27, 255);
                break;
            case ElementGroups.ReactiveNonmetal:
                Pigment = new Color32(42, 65, 101, 255);
                break;
            case ElementGroups.NobleGas:
                Pigment = new Color32(98, 56, 66, 255);
                break;
            case ElementGroups.Lanthanide:
                Pigment = new Color32(0, 74, 119, 255);
                break;
            case ElementGroups.Actinide:
                Pigment = new Color32(97, 59, 40, 255);
                break;
            case ElementGroups.Unknown:
                Pigment = new Color32(70, 71, 76, 255);
                break;
            default:
                elementGroup = ElementGroups.Unknown;
                Pigment = new Color32(70, 71, 76, 255);
                break;
        }
        rend.color = Pigment;
    }
}

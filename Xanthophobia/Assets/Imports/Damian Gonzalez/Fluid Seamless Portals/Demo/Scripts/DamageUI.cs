
using UnityEngine;
using UnityEngine.UI;

namespace DamianGonzalez {
    public class DamageUI : MonoBehaviour {
        Image im;
        public static DamageUI inst;
        Color transparentWhite = new Color(1, 1, 1, 0);


        void Start() {
            im = GetComponent<Image>();
            inst = this;
        }


        void FixedUpdate() {
            im.color = Color.Lerp(im.color, transparentWhite, .03f);
        }

        public void DamageNow() {
            im.color = Color.white;
        }

    }
}
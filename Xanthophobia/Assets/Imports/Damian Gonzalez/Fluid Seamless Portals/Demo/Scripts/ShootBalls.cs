using System.Collections.Generic;
using UnityEngine;
using DamianGonzalez.Portals;

//this script is not part of the portals system, it's only for the demo scene,
//only to ilustrate how other object can (or can't) cross portals

namespace DamianGonzalez {

    public class ShootBalls : MonoBehaviour {
        private enum WhichButton { LeftClick, RightClick };
        [SerializeField] private WhichButton button;

        [SerializeField] private GameObject prefabBall;
        [SerializeField] private float shortThrowForce = 100f;
        [SerializeField] private int maxAmount = 10;


        private Transform recycleBin;
        private Queue<GameObject> ballsPool = new Queue<GameObject>();


        [SerializeField] private Transform objectFromWhichToShoot;
        [SerializeField] private Vector3 positionOffset = new Vector3(0, .5f, 1f);

        private void Start() {
            recycleBin = (GameObject.Find("recycle bin") ?? new GameObject("recycle bin")).transform;
            if (objectFromWhichToShoot == null) objectFromWhichToShoot = transform.parent;
        }

        void Update() {
        
            if (button == WhichButton.LeftClick && Input.GetMouseButtonDown(0)) ThrowProjectile();
            if (button == WhichButton.RightClick && Input.GetMouseButtonDown(1)) ThrowProjectile();


            //restart current scene
            if (Input.GetKeyDown(KeyCode.R)) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
            }

        }

        void ThrowProjectile() {
            //new projectile slightly in front of player
            GameObject projectile = NewBall();

            //reposition
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            projectile.SetActive(false);
            projectile.transform.position = objectFromWhichToShoot.TransformPoint(positionOffset);
            projectile.SetActive(true);

            //add force
            float force = shortThrowForce;
            if (Input.GetKey(KeyCode.LeftShift)) force *= 2.5f;
            if (Input.GetKey(KeyCode.LeftControl)) force *= .2f;
            rb.velocity = Vector3.zero;
            rb.AddForce(transform.forward * force, ForceMode.Impulse);


            //Both ball and character have gravity script? Make them match
            if (projectile.TryGetComponent(out CustomGravity ballGr)) {
                if (transform.parent.TryGetComponent(out CustomGravity playerGr)) {
                    ballGr.down = playerGr.down;
                    rb.useGravity = false;
                }
            }

        }

        GameObject NewBall() {
            GameObject ball;
            if (ballsPool.Count < maxAmount) {
                //instantiate
                ball = Instantiate(
                    prefabBall,
                    recycleBin
                );

                //random color
                ball.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();

                //and a name
                ball.name = "ball";
            } else {
                ball = ballsPool.Dequeue();
            }
            ballsPool.Enqueue(ball);
            return ball;
        }
    }
}
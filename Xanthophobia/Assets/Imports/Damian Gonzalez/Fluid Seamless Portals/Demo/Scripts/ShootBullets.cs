using System.Collections.Generic;
using UnityEngine;
using DamianGonzalez.Portals;

    //this script is not part of the portals system, it's only for the demo scene,
    //only to provide shooting in order to ilustrate how portals can "teleport" raycasts
    //and camera shake, just for dramatic effect when shooting

namespace DamianGonzalez {
    public class ShootBullets : MonoBehaviour {

        private enum WhichButton { LeftClick, RightClick };
        [SerializeField] private WhichButton button;
        [SerializeField] private Transform objectFromWhichToShoot;

        //shooting
        [SerializeField] private GameObject prefabShotSound;
        [SerializeField] private GameObject prefabBulletHole;
        Transform recycleBin;
        Material simpleMaterial;
        [SerializeField] private Vector3 startOffset = new Vector3(.01f, -.001f, .5f);
        [SerializeField] private Color colorNormalRay = Color.white;
        [SerializeField] private Color colorTeleportedRay = Color.yellow;
        [SerializeField] private LayerMask layerMask = 1; //just "default"

        //camera shake
        float shakeMagnitude = 0;
        float shakeSpeed = .95f;

        void Start() {
            recycleBin = (GameObject.Find("recycle bin") ?? new GameObject("recycle bin")).transform;
            simpleMaterial = new Material(Shader.Find("Unlit/Color"));
            if (objectFromWhichToShoot == null) objectFromWhichToShoot = transform;
        }

        void Update() {
            if (button == WhichButton.LeftClick && Input.GetMouseButtonDown(0))  Shoot();
            if (button == WhichButton.RightClick && Input.GetMouseButtonDown(1)) Shoot();

        }

        void FixedUpdate() {
            shakeMagnitude *= shakeSpeed;
        }
        void LateUpdate() { //after the update of FPS controller
            if (shakeMagnitude > 0.01f) {
                transform.Rotate(
                    Random.Range(-shakeMagnitude / 2, shakeMagnitude / 2),
                    Random.Range(-shakeMagnitude / 2, shakeMagnitude / 2),
                    Random.Range(-shakeMagnitude / 2, shakeMagnitude / 2)
                );
            }
        }


        void Shoot() {

            NewShootSound();

            CameraShake(.2f);


            //check if we hit something

            /* 
             * Instead of using Physics.Raycast,
             * we'll use portal's own method for raycasting, 
             * whose syntaxis is nearly identical
             * 
             * see documentation for more info: www.damian-gonzalez.com/portals
            */


            if (PortalRaycast.Raycast(
                objectFromWhichToShoot.TransformPoint(startOffset),      //from
                transform.forward,                                       //direction
                out RaycastHit lastHit,
                out List<PortalRaycast.RayInfo> subRays,
                layerMask
            )) {
                //and we can use lastHit the same as in Physics.Raycast

                PortalEvents.rayThroughPortals?.Invoke( transform.position, lastHit.point, subRays);

                Vector3 lastDirection = subRays[subRays.Count - 1].direction;
                
                //bullet hole
                GameObject hole = NewBulletHole();
                hole.transform.position = lastHit.point - lastDirection * 0.001f;
                hole.transform.rotation = Quaternion.FromToRotation(Vector3.forward, lastHit.normal);
                hole.transform.SetParent(lastHit.transform, true);


                bool rayTeleported = (subRays.Count == 1);

                //draw rays
                foreach (PortalRaycast.RayInfo subRay in subRays) {
                    LineRenderer lr = NewRayLine().GetComponent<LineRenderer>();
                    lr.SetPosition(0, subRay.from);
                    lr.SetPosition(1, subRay.hit.point);
                    lr.material.color = rayTeleported ? colorTeleportedRay : colorNormalRay;
                }

                //add force to the hit object (if has rigidbody)
                if (lastHit.transform.TryGetComponent(out Rigidbody rb)) {
                    rb.AddForceAtPosition(lastDirection * 3f, lastHit.point, ForceMode.Impulse);
                }

                //did player shoot himself? (those damn portals!)
                if (lastHit.transform == transform.parent || lastHit.transform.CompareTag("Player")) {
                    CameraShake(10f); //giant shake
                    DamageUI.inst.DamageNow(); //blood stains in camera
                }

            }
        }

        void CameraShake(float magnitude) {
            shakeMagnitude = magnitude;
        }

        #region pools management
        //____________________________________________ pool of ray traces
        Queue<GameObject> rayTraces = new Queue<GameObject>();
        int maxRayLinesAmount = 20;
        GameObject NewRayLine() {
            GameObject trace;
            if (rayTraces.Count < maxRayLinesAmount) {
                //instantiate
                trace = new GameObject("ray trace");
                trace.transform.SetParent(recycleBin);
                LineRenderer lr = trace.AddComponent<LineRenderer>();
                lr.startWidth = .01f; //.0005f;
                lr.endWidth = .01f;
                lr.startColor = Color.white;
                lr.endColor = Color.white;
                lr.material = simpleMaterial;
            } else {
                trace = rayTraces.Dequeue();
            }
            rayTraces.Enqueue(trace);
            return trace;
        }

        //____________________________________________ pool of bullet holes
        Queue<GameObject> bulletHoles = new Queue<GameObject>();
        int maxbulletHolesAmount = 20;
        GameObject NewBulletHole() {
            GameObject hole;
            if (bulletHoles.Count < maxbulletHolesAmount) {
                hole = Instantiate(prefabBulletHole);
                hole.name = "bullet hole";
            } else {
                hole = bulletHoles.Dequeue();
            }
            hole.transform.SetParent(null);
            bulletHoles.Enqueue(hole);
            return hole;
        }

        //____________________________________________ pool of sounds
        Queue<GameObject> shootSounds = new Queue<GameObject>();
        int maxShootSoundsAmount = 3;
        GameObject NewShootSound() {
            GameObject sound;
            if (shootSounds.Count < maxShootSoundsAmount) {
                sound = Instantiate(prefabShotSound, recycleBin); //auto play
                sound.name = "shoot sound";
            } else {
                sound = shootSounds.Dequeue();
                sound.GetComponent<AudioSource>().Play();
            }
            shootSounds.Enqueue(sound);
            return sound;
        }
        #endregion

    }
}
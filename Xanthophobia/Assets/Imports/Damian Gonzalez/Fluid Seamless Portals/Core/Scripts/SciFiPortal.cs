using UnityEngine;


namespace DamianGonzalez.Portals {
    public class SciFiPortal : MonoBehaviour {
        [HideInInspector] PortalSetup portalSet;
        [Header("Spinning")]
        [SerializeField] bool spinInnerRing;
        [SerializeField] float innerRingMaxSpeed;
        Transform innerRing;
        private float innerRingCurrentSpeed;

        [Header("Sparks")]
        [SerializeField] bool sparksWhenTurnOn = true;
        [SerializeField] bool sparksRepeatWhileOn = true;
        [SerializeField] bool sparksWhenTurnOff = true;
        ParticleSystem sparksOn, sparksOff;
        [SerializeField] float repeatSparksEvery_min = 1f;
        [SerializeField] float repeatSparksEvery_max = 5f;
        [SerializeField] bool ignoreSparksOnAwake = true;

        [Header("Circle Lights")]
        [SerializeField] bool circleLights = true;
        [SerializeField] float pulsePeriod = 3f;
        Transform ringLightsContainer;
        Material[] ringLightsMaterials = new Material[10];
        float ratio = 5.8f ;

        [System.Serializable]
        public class IntensitySetup {
            public Material material;
            public Color baseColor = Color.red;
            public float intensityOn = 2f;
            public float intensityOff = 0;
            public float speed = .01f;
            [HideInInspector] public float currentIntensity = 0;
            public void SetInitialValue(bool isOn) {
                currentIntensity = isOn ? intensityOn : intensityOff;
            }													
        }

        [SerializeField]  IntensitySetup setupForRing1;
        [SerializeField]  IntensitySetup setupForRing2;
        [SerializeField]  IntensitySetup setupForConnectors;
        [SerializeField]  IntensitySetup setupForSwitchLight;
        [SerializeField]  IntensitySetup setupForHallway;

        void Awake() {
            if (portalSet == null) portalSet = transform.GetComponentInParent<PortalSetup>();
            innerRing = transform.Find("Ring inner");

            //get sparks
            if (sparksWhenTurnOn && sparksOn == null) sparksOn = transform.Find("Sparks")?.GetComponent<ParticleSystem>();
            if (sparksWhenTurnOn && sparksOn == null) sparksOn = transform.Find("Sparks on")?.GetComponent<ParticleSystem>();
            
            if (sparksWhenTurnOff && sparksOff == null) sparksOff = transform.Find("Sparks")?.GetComponent<ParticleSystem>();
            if (sparksWhenTurnOff && sparksOff == null) sparksOff = transform.Find("Sparks off")?.GetComponent<ParticleSystem>();

            //get ring lights
            ringLightsContainer = transform.Find("Ring lights");
            for (int i = 0; i < 10; i++) {
                ringLightsMaterials[i] = ringLightsContainer.GetChild(i).GetComponent<MeshRenderer>().material;
            }

            //subscribe to public event, so this portal can react to the change
            PortalEvents.onOffStateChanged += StateChanged;

        }

        private void Start() {
            bool isOn = portalSet.onOffSwitch.initialState == PortalSetup.OnOffSwitch.OnOff.On;
            StateChanged("", portalSet, isOn);

            //initial values
            setupForRing1.SetInitialValue(isOn);
            setupForRing2.SetInitialValue(isOn);
            setupForSwitchLight.SetInitialValue(isOn);
            setupForHallway.SetInitialValue(isOn);
            setupForConnectors.SetInitialValue(isOn);

            if (isOn && sparksRepeatWhileOn) {
                ScheduleSparkRepetition();
            } 
        }


        void FixedUpdate() {
            if (spinInnerRing) {
                //speed of the ring
                innerRingCurrentSpeed = Mathf.Lerp(innerRingCurrentSpeed, portalSet.onOffSwitch.isOn ? innerRingMaxSpeed : 0f, 0.01f);

                //rotate ring
                innerRing.Rotate(0,innerRingCurrentSpeed,0);

            }

            //lights change
            ProcessMaterial(setupForRing1);
            ProcessMaterial(setupForRing2);
            ProcessMaterial(setupForSwitchLight);
            ProcessMaterial(setupForHallway);
            if (!circleLights) ProcessMaterial(setupForConnectors, true);


            //ring lights
            if (circleLights) {
                ProcessMaterial(setupForConnectors, false); //sets general intensity
                for (int i = 0; i < 10; i++) {

                    float time = Time.time + i * .1f * pulsePeriod ;
                    float sin = Mathf.Sin((time / pulsePeriod) * ratio ); // -1 ~ 1
                    float sin01 = sin / 2f + .5f; // 0 ~ 1

					//for built-in
                    ringLightsMaterials[i].SetColor("_EmissionColor", setupForConnectors.baseColor * setupForConnectors.currentIntensity * sin01);

					//for HDRP
                    ringLightsMaterials[i].SetColor("_EmissiveColor", setupForConnectors.baseColor * setupForConnectors.currentIntensity * sin01);																																																  

                }

            }
        }


        void ProcessMaterial(IntensitySetup setup, bool apply = true) { 
            setup.currentIntensity = Mathf.Lerp(
                setup.currentIntensity, 
                portalSet.onOffSwitch.isOn ? setup.intensityOn : setup.intensityOff,
                setup.speed
            );

            if (apply) {
                setup.material.SetColor("_EmissionColor", setup.baseColor * setup.currentIntensity); //built in
                setup.material.SetColor("_EmissiveColor", setup.baseColor * setup.currentIntensity); //hdrp
            }

        }

       void StateChanged(string groupId, PortalSetup _portalSet, bool newState) {
            if (_portalSet == portalSet) {
                if (!(ignoreSparksOnAwake && Time.time < .1f)) {
                    if (newState && sparksWhenTurnOn && sparksOn != null) {
                        sparksOn.Stop();
                        sparksOn.Play();
                    }
                    if (!newState && sparksWhenTurnOff && sparksOff != null) {
                        sparksOff.Stop();
                        sparksOff.Play();
                    }
                }


                if (newState && sparksRepeatWhileOn) {
                    ScheduleSparkRepetition();
                } else {
                    CancelInvoke(nameof(RepeatSparks));
                }
            }
        }

        void ScheduleSparkRepetition() {
            Invoke(nameof(RepeatSparks), Random.Range(repeatSparksEvery_min, repeatSparksEvery_max));
        }

        void RepeatSparks() {
            bool isOn = portalSet.onOffSwitch.isOn;

            sparksOn.Stop();
            sparksOn.Play();
            if (sparksRepeatWhileOn && isOn) {
                ScheduleSparkRepetition();
            }
        }
    }
}
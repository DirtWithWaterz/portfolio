#pragma warning disable 0618
using System.Collections;
using mudz;
using UnityEngine;
using UnityEngine.InputSystem;

public class LiDAR : MonoBehaviour
{
    [SerializeField] ParticleSystem liDAR;
    [SerializeField] Transform rayGunnnnnn;
    Camera cam;

    float choice;

    [SerializeField] GameObject line;
    private byte mode = 0;
    public int numberOfRays = 10;
    public float coneAngle = 45f;
    public float maxDistance = 10f;
    public LayerMask layerMask;

    private Coroutine lineScanCoroutine;
    Vector3 noVelocity = new Vector3(0, 0, 0);

    [SerializeField] private float lineScanMinAngleUD = 0f;
    [SerializeField] private float lineScanMaxAngleUD = 65f;

    [SerializeField] private float lineScanMinAngleLR = 0f;
    [SerializeField] private float lineScanMaxAngleLR = 70f;

    [SerializeField] private float lineScanSpeed = 10f;
    public int lineScanNumberOfRays = 10;
    public float lineScanConeAngleUD = 100f;
    public float lineScanConeAngleLR = 85f;

    void Awake(){
        cam = GetComponent<Camera>();
    }

    void Update(){
        choice = Random.Range(0, 11);
        // Debug.Log(choice);
        if (Mouse.current.leftButton.isPressed)
        {
            if (mode == 0) { ConeScan(); }
        }
        if(Mouse.current.leftButton.wasPressedThisFrame){
            if (mode == 1){ 
                if(lineScanCoroutine == null){
                    lineScanCoroutine = StartCoroutine(LineScan());
                }
            }
        } if(Mouse.current.leftButton.wasReleasedThisFrame){
            if(mode == 1){
                if(lineScanCoroutine != null){
                    StopCoroutine(lineScanCoroutine);
                    lineScanCoroutine = null;
                }
            }
        }

        if (UserInput.instance.FlashlightModePressed)
        {
            if (mode >= 1)
            {
                mode = 0;
            }
            else
            {
                mode++;
            }
        }
    }
    // void SpiralScan()
    // {
    //     Vector3 forward = cam.transform.forward;
    //     Quaternion startRotation = Quaternion.AngleAxis(-coneAngle / 2, transform.up);

    //     for (int i = 0; i < numberOfRays; i++)
    //     {
    //         float inclination = Mathf.Acos(1 - i / (numberOfRays - 1f) * (1 - Mathf.Cos(coneAngle * Mathf.Deg2Rad)));
    //         float azimuth = 2 * Mathf.PI * i / numberOfRays;

    //         float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
    //         float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
    //         float z = Mathf.Cos(inclination);

    //         Vector3 direction = new Vector3(x, y, z);
    //         direction = transform.rotation * startRotation * direction;

    //         Ray ray = new Ray(transform.position, direction);
    //         RaycastHit hit;

    //         if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
    //         {
    //             DrawLine(hit.point);
    //             Debug.DrawLine(ray.origin, hit.point, Color.green);
    //             PlaceDot(
    //                  hit.point, 
    //                  choice >= 5f && hit.transform.gameObject.tag == "Surroundings" ? Color.grey : 
    //                  choice <= 6f && hit.transform.gameObject.tag == "Surroundings" ? Color.white :
    //                  choice >= 5f && hit.transform.gameObject.tag == "Organics" ? Color.red : 
    //                  Color.red
    //              );
    //         }
    //         else
    //         {
    //             Debug.DrawRay(ray.origin, direction * maxDistance, Color.red);
    //         }
    //     }
    // }


private IEnumerator LineScan()
{
    while (true)
    {
        // Up to down movement
        for (float currentAngleUpDown = lineScanMinAngleUD; currentAngleUpDown <= lineScanMaxAngleUD; currentAngleUpDown += lineScanSpeed)
        {
            ScanAtAngle(currentAngleUpDown, true);
            yield return null;
        }

        // Left to right movement
        for (float currentAngleLeftRight = lineScanMinAngleLR; currentAngleLeftRight <= lineScanMaxAngleLR; currentAngleLeftRight += lineScanSpeed)
        {
            ScanAtAngle(currentAngleLeftRight, false);
            yield return null;
        }
    }
}

private void ScanAtAngle(float currentAngle, bool isUpDown){
    Vector3 forward = cam.transform.forward;
    Quaternion startRotationUpDown = Quaternion.AngleAxis(-lineScanConeAngleUD / 2, cam.transform.right);
    Quaternion startRotationLeftRight = Quaternion.AngleAxis(-lineScanConeAngleLR / 2, cam.transform.up);
    Quaternion currentRotationUpDown = Quaternion.AngleAxis(currentAngle, cam.transform.right);
    Quaternion currentRotationLeftRight = Quaternion.AngleAxis(currentAngle, cam.transform.up);

    for (int i = 0; i < lineScanNumberOfRays; i++){
        float angleStepUD = lineScanConeAngleUD / (lineScanNumberOfRays - 1);
        float angleStepLR = lineScanConeAngleLR / (lineScanNumberOfRays - 1);
        float randomOffset = Random.Range(-0.07f, 0.07f);
        angleStepUD += randomOffset;
        angleStepLR += randomOffset;
        Mathf.Clamp(angleStepLR, lineScanMinAngleLR, lineScanMaxAngleLR);
        Mathf.Clamp(angleStepUD, lineScanMinAngleUD, lineScanMaxAngleUD);

        Quaternion rayRotationUpDown = Quaternion.AngleAxis(angleStepUD * i - (lineScanConeAngleUD / 2), cam.transform.up) * currentRotationUpDown;
        Quaternion rayRotationLeftRight = Quaternion.AngleAxis(angleStepLR * i - (lineScanConeAngleLR / 2), cam.transform.right) * currentRotationLeftRight;
        Vector3 directionUpDown = rayRotationUpDown * startRotationUpDown * forward;
        Vector3 directionLeftRight = rayRotationLeftRight * startRotationLeftRight * forward;

        if (isUpDown){
            RaycastAndDraw(directionUpDown);
        } else{
            RaycastAndDraw(directionLeftRight);
        }
    }
}

private void RaycastAndDraw(Vector3 direction)
{
    Ray ray = new Ray(transform.position, direction);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
    {
        DrawLine(hit.point);
        Debug.DrawLine(ray.origin, hit.point, Color.green);
        PlaceDot(
            hit.point, 
            choice >= 5f && hit.transform.gameObject.tag == "Surroundings" ? Color.grey : 
            choice <= 6f && hit.transform.gameObject.tag == "Surroundings" ? Color.white :
            choice >= 5f && hit.transform.gameObject.tag == "Organics" ? Color.red : 
            Color.red
        );
    }
    else
    {
        Debug.DrawRay(ray.origin, direction * maxDistance, Color.red);
    }
}

    void ConeScan(){
        Vector3 forward = cam.transform.forward;

        for (int i = 0; i < numberOfRays; i++)
        {
            float randomInclination = Random.Range(0, coneAngle * Mathf.Deg2Rad);
            float randomAzimuth = Random.Range(0, 2 * Mathf.PI);

            float x = Mathf.Sin(randomInclination) * Mathf.Cos(randomAzimuth);
            float y = Mathf.Sin(randomInclination) * Mathf.Sin(randomAzimuth);
            float z = Mathf.Cos(randomInclination);

            Vector3 direction = new Vector3(x, y, z);
            direction = transform.rotation * direction;

            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                DrawLine(hit.point);
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                PlaceDot(
                    hit.point, 
                    choice >= 5f && hit.transform.gameObject.tag == "Surroundings" ? Color.grey : 
                    choice <= 6f && hit.transform.gameObject.tag == "Surroundings" ? Color.white :
                    choice >= 5f && hit.transform.gameObject.tag == "Organics" ? Color.red : 
                    Color.red
                );
            }
            else
            {
                Debug.DrawRay(ray.origin, direction * maxDistance, Color.red);
            }
        }
    }

    void DrawLine(Vector3 end)
    {
        GameObject lineObject = Instantiate(line, rayGunnnnnn);
        LineRenderer lineRend = lineObject.GetComponent<LineRenderer>();
        lineRend.SetPosition(0, rayGunnnnnn.position);
        lineRend.SetPosition(1, end);

        Destroy(lineObject, 0.008f);
    }
    [SerializeField] float particleSize = 0.006f;
    void PlaceDot(Vector3 pos, Color32 color){
        liDAR.Emit(pos, noVelocity, particleSize, 120, color);
    }
}
#pragma warning restore 0618

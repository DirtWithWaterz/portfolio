using UnityEngine;
using System.Collections;

public class EarthOptionsGUISimple : MonoBehaviour {

	[SerializeField] float earthRotationSpeed = 2.0f;


	void Update()
	{
		transform.Rotate(new Vector3(0, Time.deltaTime * earthRotationSpeed, 0));
	}

}

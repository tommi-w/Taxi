using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CageConnectorScript : MonoBehaviour {

    private DistanceJoint2D connectingJoint;

	// Use this for initialization
	void Awake () {
        connectingJoint = GetComponent<DistanceJoint2D>();
        connectingJoint.connectedBody = GameManager.instance.playerCurrentCharacter.GetComponent<Rigidbody2D>();
        connectingJoint.enabled = true;
	}
}

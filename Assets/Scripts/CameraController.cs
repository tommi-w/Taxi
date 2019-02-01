using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


	private GameObject player;

	private Vector3 offset;
	private Vector3 newPosition;

	void Start () {
        player = GameManager.instance.playerCurrentCharacter;
        newPosition = new Vector3(player.transform.position.x,player.transform.position.y,-10.0f);
		transform.position = newPosition;
	}

	// Update is called once per frame
	void Update () {
        if (player == null)
            player = GameManager.instance.playerCurrentCharacter;

		newPosition = player.transform.position;
		newPosition.z = -10.0f;
		transform.position = newPosition;
	}
}

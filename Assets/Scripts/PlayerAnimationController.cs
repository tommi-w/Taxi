using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour {


	private Animator anim;
    	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Horizontal") != 0)
            anim.SetBool("MoveHorizontal", true);
        else
            anim.SetBool("MoveHorizontal", false);
		if (Input.GetAxis ("Vertical") != 0)
			anim.SetBool ("MoveVertical", true);
		else
			anim.SetBool ("MoveVertical", false);
	}
}

using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

	private Animator animator;
	private CharController charController;

	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		charController = gameObject.GetComponentInParent<CharController>();
	}

	void Update () {
		animator.SetBool ("Jump", charController.justJump);
		animator.SetBool ("Dead1", charController.justDie1);
		animator.SetBool ("Dead2", charController.justDie2);
	}
}

using UnityEngine;
using System.Collections;

public class CollisionController : MonoBehaviour {

	public GameObject character;
	public AudioClip coinSound;
	private CharController charController;
	private ScoreController scoreController;
	private EnvironmentMaker environmentMaker;

	void Start () {
		charController = gameObject.GetComponentInParent<CharController> ();
		environmentMaker = GameObject.Find ("Environment").GetComponent<EnvironmentMaker>();
		scoreController = GameObject.Find ("CoinText").GetComponent<ScoreController> ();
		//scoreController = GameObject.Find ("CoinText").GetComponent<ScoreController> ();
	}

	void OnTriggerExit(Collider col) { //trees exit trigger
		if (!charController.canMove) {
			if (col.gameObject.name == "tree1(Clone)" || col.gameObject.name == "tree2(Clone)"||
				col.gameObject.name == "tree3(Clone)" || col.gameObject.name == "tree4(Clone)") {
				charController.canMove = true;
			}
		} 
	}

	void OnTriggerEnter (Collider col) { //vehicles
		if (col.gameObject.name == "veh_wagon1(Clone)" || col.gameObject.name == "veh_wagon2(Clone)" || col.gameObject.name == "veh_wagon3(Clone)" || col.gameObject.name == "veh_wagon4(Clone)" || 
			col.gameObject.name == "veh_suv1(Clone)" || col.gameObject.name == "veh_suv2(Clone)" || col.gameObject.name == "veh_suv3(Clone)" || col.gameObject.name == "veh_mini1(Clone)" ||
			col.gameObject.name == "veh_mini2(Clone)" || col.gameObject.name == "veh_mini3(Clone)" ||col.gameObject.name == "veh_mini4(Clone)" || col.gameObject.name == "veh_mini5(Clone)" ||
			col.gameObject.name == "veh_truck1(Clone)" || col.gameObject.name == "veh_truck2(Clone)" || col.gameObject.name == "veh_truck3(Clone)" || col.gameObject.name == "veh_truck4(Clone)" ||
			col.gameObject.name == "veh_truck5(Clone)" || col.gameObject.name == "veh_truck6(Clone)" || col.gameObject.name == "veh_police(Clone)" || col.gameObject.name == "veh_ambulance(Clone)" || 
			col.gameObject.name == "veh_cab(Clone)" || col.gameObject.name == "veh_fire(Clone)" || col.gameObject.name == "veh_bus(Clone)") {
			if (!charController.justDie1 && !charController.justDie2) {
				if (gameObject.name == "FrontCollider" || gameObject.name == "BackCollider") {
					charController.justDie1 = true;
					gameObject.transform.parent.parent = col.gameObject.transform;
					
				} else if (gameObject.name == "LeftCollider" || gameObject.name == "RightCollider") {
					charController.justDie2 = true;
				}
			}
		} else if (col.gameObject.name == "Coin") { //collision with collectible (coin)

			if (gameObject.name == "LeftCollider" && charController.lastJump == JumpDirection.left || gameObject.name == "RightCollider" && charController.lastJump == JumpDirection.right ||
				gameObject.name == "FrontCollider" && charController.lastJump == JumpDirection.up || gameObject.name == "BackCollider" && charController.lastJump == JumpDirection.down) {
				scoreController.coin++;
				scoreController.score += 5; //add 5 score for each coin
				col.gameObject.transform.parent.gameObject.SetActive (false);
				gameObject.GetComponentInParent<AudioSource>().PlayOneShot(coinSound,0.5f);
			}
		
		//trees
		} else if (col.gameObject.name == "tree1(Clone)" || col.gameObject.name == "tree2(Clone)" || col.gameObject.name == "tree3(Clone)" || col.gameObject.name == "tree4(Clone)") {
			charController.canMove = false;
			if (gameObject.name == "LeftCollider" && charController.lastJump == JumpDirection.left) { // left collider
				charController.endPosition.z -= CharController.jumpDistance;
			} else if (gameObject.name == "RightCollider" && charController.lastJump == JumpDirection.right) { // right collider
				charController.endPosition.z += CharController.jumpDistance;
			} else if (gameObject.name == "FrontCollider" && charController.lastJump == JumpDirection.up) { // front collider
				charController.endPosition.x -= CharController.jumpDistance;
				environmentMaker.collideFront = true;
				environmentMaker.charRoadPos--;
			} else if (gameObject.name == "BackCollider" && charController.lastJump == JumpDirection.down) { // back collider
				charController.endPosition.x += CharController.jumpDistance;
			}

		} else if (col.gameObject.name == "Boulder") { //collision with boulder
			charController.justDie2 = true;
		}
	}
}

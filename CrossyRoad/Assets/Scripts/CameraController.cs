using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public GameObject character;
	
	private float cameraFollowingSpeed; // speed for following character instatenously
	private float cameraMovingSpeed; // speed for constant moving to the front
	private float cameraSpeedUp; // speeding up for Camera Speed based on now character position

	// Use this for initialization
	void Start () {
		cameraFollowingSpeed = 10f;
		cameraMovingSpeed = .5f;
		cameraSpeedUp = .1f;
	}
	
	// Update is called once per frame
	void Update () {
		if (character.GetComponent<CharController>().isPlay) { // Play Button is already pressed
			/* If character not yet dead */
			if (!character.GetComponent<CharController> ().justDie3 && !character.GetComponent<CharController> ().justDie2 && !character.GetComponent<CharController> ().justDie1) {
				this.transform.position = new Vector3 (this.transform.position.x + Time.deltaTime * cameraMovingSpeed, this.transform.position.y, this.transform.position.z);

				/* If character moving left / right, following character */
				if (character.transform.position.z != this.transform.position.z) {
					this.transform.position = Vector3.Lerp (this.transform.position , new Vector3(this.transform.position.x,this.transform.position.y,character.transform.position.z),Time.deltaTime*cameraFollowingSpeed);
				}

				/* If character moving up, following character */
				if (character.transform.position.x > this.transform.position.x) {
					this.transform.position = Vector3.Lerp (this.transform.position, new Vector3 (character.transform.position.x, this.transform.position.y, this.transform.position.z), Time.deltaTime * cameraFollowingSpeed);
				}

				/* Character dead if out of camera */
				if ((character.transform.position.x + 5f) < this.transform.position.x) {
					character.GetComponent<CharController> ().justDie3 = true;
				}
				
				/* Accelerate speed */
				cameraMovingSpeed = (character.transform.position.x / 17.5f) * cameraSpeedUp + .5f;
			} else { // character's dead
				// camera follow if dead case is case 3
				if (character.GetComponent<CharController> ().justDie3) {
					gameObject.transform.position = Vector3.Lerp (gameObject.transform.position, character.transform.position, Time.deltaTime * 5f);
				}
			}
		}
	}
}

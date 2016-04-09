using UnityEngine;
using System.Collections;

public enum JumpDirection {not_jump,left,right,down,up};

public class CharController : MonoBehaviour {

	private bool firstInput;
	private float lerpTime;
	private float currentLerpTime;
	private float percentage;
	private Vector3 startPosition;

	public Vector3 endPosition;
	public bool isPlay; // play button detector
	public GameObject retryButton; // UI retry button
	public GameObject playButton; // UI play button

	private EnvironmentMaker environmentMaker;
	private CameraController cameraController;
	private AudioSource audioSource;

	public AudioClip jumpSound;
	public AudioClip dieSound;
	public AudioClip dieSound2;
	public AudioClip dieSound3;
	
	public bool canMove; // if character can Move (change position)
	public JumpDirection lastJump; // last Jump state
	private JumpDirection nowJump; // now Jump state

	public const float jumpDistance = 1.75f; // distance for each jump
	public const float jumpSpeed = 5f; // character jump speed
	public const int roadSideBoundary = 4; // left and side boundary of player

	public bool justJump;
	public bool justDie1; // die from the front or back side
	public bool justDie2; // die from left or right side 
	public bool justDie3; // die from 'out of camera'
	private bool justDead; // just dead (for Dead SFX), play once


	void Start() {
		justDie1 = false;
		justDie2 = false;
		justDie3 = false;
		justDead = true;
		canMove = true;
		lastJump = JumpDirection.not_jump;
		nowJump = JumpDirection.not_jump;
		percentage = 1f;
		endPosition = gameObject.transform.position;
		environmentMaker = GameObject.Find ("Environment").GetComponent<EnvironmentMaker>();
		cameraController = GameObject.Find ("Level").GetComponent<CameraController> ();
		audioSource = gameObject.GetComponent<AudioSource> ();
	}

	void Update() {
		if (isPlay) { // play button Pressed
			if (!justDie1 && !justDie2 && !justDie3) { // still alive
				/* Getting Input */
				if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
					audioSource.PlayOneShot (jumpSound, 0.2f); // jump SFX

					if (percentage == 1f || !canMove) { // char can move
						lerpTime = 1f;
						currentLerpTime = 0f;
						firstInput = true;
						justJump = true;

						if (Input.GetKeyDown(KeyCode.UpArrow)) { // frontward
							if (canMove || (nowJump != lastJump)) { // canMove or (stuck, but different direction input)
								endPosition.x += jumpDistance;
								canMove = true;	
							}
							environmentMaker.charRoadPos++;

							lastJump = nowJump;

							gameObject.transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (270f, 270f, 0f));
						} else if (Input.GetKeyDown(KeyCode.DownArrow)) { // backward
							nowJump = JumpDirection.down;
						
							if (canMove || (nowJump != lastJump)) {
								endPosition.x -= jumpDistance;
								canMove = true;
								environmentMaker.collideFront = false;
							}
							environmentMaker.charRoadPos--;
							lastJump = nowJump;

							gameObject.transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (270f, 90, 0f));
						} else if (Input.GetKeyDown(KeyCode.LeftArrow)) { // left
							nowJump = JumpDirection.left;
							if (canMove || (nowJump != lastJump)) {
								if (gameObject.transform.position.z < (roadSideBoundary * jumpDistance)) {
									endPosition.z += jumpDistance;
								}
								canMove = true;	
								environmentMaker.collideFront = false;
							}
							lastJump = nowJump;

							gameObject.transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (270f, 180f, 0f)); //right
						} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
							nowJump = JumpDirection.right;
							if (canMove || (nowJump != lastJump)) {
								if (gameObject.transform.position.z > (-roadSideBoundary * jumpDistance)) {
									endPosition.z -= jumpDistance;
								}
								canMove = true;	
								environmentMaker.collideFront = false;
							}
							lastJump = nowJump;

							gameObject.transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (270f, 0f, 0f));
						}
					}
				}

				startPosition = gameObject.transform.position;


				if (firstInput) {
					currentLerpTime += Time.deltaTime * jumpSpeed;
					percentage = currentLerpTime / lerpTime;

					gameObject.transform.position = Vector3.Lerp (startPosition, endPosition, percentage);
					if (percentage > 0.8f) {
						percentage = 1f;
					} 

					if (Mathf.Round (percentage) == 1f) {
						justJump = false;
					}
				}
			} else { // character dead
				if (justDead) { // just dead (execute one time event)
					if (justDie1) {
						audioSource.PlayOneShot (dieSound, 1f);
						retryButton.SetActive (true);
					} else if (justDie2) {
						audioSource.PlayOneShot (dieSound2, 0.3f);
						retryButton.SetActive (true);
					} else if (justDie3) {
						gameObject.transform.GetChild (1).gameObject.AddComponent<Rigidbody> ();
						gameObject.transform.GetChild (1).gameObject.GetComponent<Rigidbody> ().mass = 5f;
						audioSource.PlayOneShot (dieSound3, 0.3f);
					}
					justDead = false;
				}

				Destroy (this.GetComponent<BoxCollider> ());

				if (justDie3) { // wait for animation if case 'die3', to pop-up Retry
					if (gameObject.transform.GetChild (1).transform.position.y <= 1f) {
						retryButton.SetActive (true);
					}
				}
			}
		}
	}

}
using UnityEngine;
using System.Collections;

public enum JumpDirection {not_jump,left,right,down,up};

public class CharController : MonoBehaviour {

	public const int roadSideBoundary = 4;
	public const float jumpDistance = 1.75f;
	public const float jumpSpeed = 5f;

	private bool firstInput;
	public bool isPlay;
	public bool canMove;
	public JumpDirection lastJump;
	private JumpDirection nowJump;

	private float lerpTime, currentLerpTime;
	private float perc;
	private Vector3 startPosition;
	public Vector3 endPosition;

	public GameObject retryButton;
	public GameObject playButton;
	private EnvironmentMaker environmentMaker;
	private CameraController cameraController;
	private AudioSource audioSource;

	public AudioClip jumpSound;
	public AudioClip dieSound;
	public AudioClip dieSound2;
	public AudioClip dieSound3;

	public bool justJump;
	private bool justDead; // play sfx once
	public bool justDie1; // die from car crash
	public bool justDie2; // die from car crash from left or right side
	public bool justDie3; // die from timeout


	void Start() {
		justDie1 = false;
		justDie2 = false;
		justDie3 = false;
		justDead = true;
		canMove = true;
		lastJump = JumpDirection.not_jump;
		nowJump = JumpDirection.not_jump;
		perc = 1f;
		endPosition = gameObject.transform.position;
		environmentMaker = GameObject.Find ("Environment").GetComponent<EnvironmentMaker>();
		cameraController = GameObject.Find ("Level").GetComponent<CameraController> ();
		audioSource = gameObject.GetComponent<AudioSource> ();
	}

	void Update() {
		if (isPlay) {
			if (!justDie1 && !justDie2 && !justDie3) {
				//read keyboard input
				if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || 
					Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {

					audioSource.PlayOneShot(jumpSound, 0.2f);
				
					if (perc == 1f || !canMove) {
						lerpTime = 1f;
						currentLerpTime = 0f;
						firstInput = true;
						justJump = true;

						if (Input.GetKeyDown(KeyCode.UpArrow)) { //jump forward
							if (canMove || (nowJump != lastJump)) {
								endPosition.x += jumpDistance;
								canMove = true;	
							}
							lastJump = nowJump;
							environmentMaker.charRoadPos++;
							gameObject.transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (270f, 270f, 0f));
						} else if (Input.GetKeyDown(KeyCode.DownArrow)) { //jump backward
							nowJump = JumpDirection.down;
							if (canMove || (nowJump != lastJump)) {
								canMove = true;
								environmentMaker.collideFront = false;
								endPosition.x -= jumpDistance;
							}
							lastJump = nowJump;
							environmentMaker.charRoadPos--;
							gameObject.transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (270f, 90, 0f));
						} else if (Input.GetKeyDown(KeyCode.RightArrow)) { //jump to the right side
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
						} else if (Input.GetKeyDown(KeyCode.LeftArrow)) { //jump to the leftside
							nowJump = JumpDirection.left;
							if (canMove || (nowJump != lastJump)) {
								if (gameObject.transform.position.z < (roadSideBoundary * jumpDistance)) {
									endPosition.z += jumpDistance;
								}
								canMove = true;	
								environmentMaker.collideFront = false;
							}
							lastJump = nowJump;
							gameObject.transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (270f, 180f, 0f));
						}
					}
				}

				startPosition = gameObject.transform.position;

				if (firstInput) {
					perc = currentLerpTime / lerpTime;
					currentLerpTime += Time.deltaTime * jumpSpeed;

					gameObject.transform.position = Vector3.Lerp (startPosition, endPosition, perc);
					if (perc > 0.8f) {
						perc = 1f;
					} 

					if (Mathf.Round (perc) == 1f) {
						justJump = false;
					}
				}
			} else {//player dies
				if (justDead) {
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
						//retryButton.SetActive (true);
						/*if (gameObject.transform.GetChild (1).transform.position.y <= 1f) {
							retryButton.SetActive (true);
						}*/
					}
					justDead = false;
				}

				Destroy (this.GetComponent<BoxCollider> ());
				if (justDie3) {
					if (gameObject.transform.GetChild (1).transform.position.y <= 1f) {
						retryButton.SetActive (true);
					}
				}
			}
		}
	}

}
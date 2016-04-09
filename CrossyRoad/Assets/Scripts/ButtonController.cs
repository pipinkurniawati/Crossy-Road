using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonController : MonoBehaviour {
	CharController charController;

	void Start () {
		charController = GameObject.Find ("Character").GetComponent<CharController> ();
	}

	void Update () {
	}

	public void retry() // trigger onclick function for retry button
	{
		Application.LoadLevel (Application.loadedLevel);
	}

	
	public void play() { // trigger onclick function for play button
		charController.isPlay = true;
		charController.playButton.SetActive (false);
	}
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreController : MonoBehaviour {
	private EnvironmentMaker environmentMaker;
	private int score;
	public int coin;

	void Start () {
		score = 0;
		coin = 0;
		environmentMaker = GameObject.Find ("Environment").GetComponent<EnvironmentMaker>();
	}
		
	void Update () {
		score = environmentMaker.upperRoadPos;
		/* Score Update */
		if (score >= 0) {
			if (gameObject.name == "ScoreText") {
				gameObject.GetComponent<Text> ().text = score.ToString ();
			}
		}

		/* Coin Update */
		if (coin >= 0) {
			if (gameObject.name == "CoinText") {
				gameObject.GetComponent<Text> ().text = coin.ToString ();
			} 
		}
	}
}

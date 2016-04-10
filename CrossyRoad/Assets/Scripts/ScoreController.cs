using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreController : MonoBehaviour {
	private EnvironmentMaker environmentMaker;
	public int score;
	public int coin;

	void Start () {
		score = 0;
		coin = 0;
		environmentMaker = GameObject.Find ("Environment").GetComponent<EnvironmentMaker>();
	}
		
	void Update () {
		score = environmentMaker.upperRoadPos;

		//collected coins
		if (coin >= 0) {
			if (gameObject.name == "CoinText") {
				gameObject.GetComponent<Text> ().text = coin.ToString ();
			} 
		}

		//collected score
		/*if (score >= 0) {
			if (gameObject.name == "ScoreText") {
				gameObject.GetComponent<Text> ().text = score.ToString ();
			}
		}*/
	}
}

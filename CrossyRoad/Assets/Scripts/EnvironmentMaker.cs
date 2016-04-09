using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum EnvironmentType {jalanan,trotoar}; // type of environment
enum Direction {right,left}; // for vehicle direction (random occasion)

public class EnvironmentMaker : MonoBehaviour {
	/* Environment */
	public GameObject jalanan;
	public GameObject trotoar; 
	public GameObject vehicles; // vehicle Container
	public GameObject trees; // tree container
	public GameObject coins; // coin container
	
	public const int nKendaraan = 10;
	public GameObject[] kendaraan = new GameObject[nKendaraan]; // consist all type of 'kendaraan'

	public const int nPohon = 4;
	public GameObject[] pohon = new GameObject[nPohon]; // consist all type of 'pohon'

	public const int nCollectibles = 1;
	public GameObject[] collectibles = new GameObject[nCollectibles]; // consist all type of 'collectibles'

	public int charRoadPos; // now character position based on the Road
	public int upperRoadPos; // upper boundary for the road

	private const float bottomSpeed = 2f; // bottom boundary for vehicle speed
	private const float topSpeed = 3f; // top boundary for vehicle speed
	private const float vehicleGap = 6f; // gap for each vehicle in one track
	private const float environmentGap = 1.75f; // gap between each part of environment
	private const float leftRoadBoundary = 15f; // left boundary for vehicles
	private const float rightRoadBoundary = -23f; // right boundary for vehicles
	private const float offsetVehiclePos = -2.98f; // offset for vehicle position placement
	
	private const int nInitRoad = 15; // initial number of whole road
	private const int nInitTrotoar = 8; // initial number of trotoar in the very beginning (to cover the back surface)
	private const int nInitTree = 7; // initial number of tree in the very beginning (left and right each)
	private const int offsetTreeBoundary = 9; // offset index of tree boundary (left and right)
	private const int nMiddleTree = 6;	// number of tree in the middle
	private const int nCar = 4; // number of car each lane

	private int posRoad; // road offset position for recycling environment
	private int treePos; // random Tree Position for middle in grass lane
	private List<int> middleTrees = new List<int>(); // list of position trees in the middle, so not double

	private EnvironmentType environmentType;
	private Direction vehicleDirection;

	private List<float> vehicleSpeed = new List<float>(); // contains random number of speed {bottomSpeed...TopSpeed}

	private GameObject clone; // cloning object
	private GameObject vehicleContainer; // vehicle sub container for each 'jalan raya' lane
	private GameObject treeContainer; // tree sub container for each 'trotoar' lane

	private float initVehiclePos; // determine initial position of vehicle
	private string roadName; // name of the road, for checking to place vehicle
	private Vector3 vehiclePos; // position for each vehicle

	public bool collideFront; // front collide, for special case of hitting tree

	void Start () {
		posRoad = nInitRoad;
		upperRoadPos = 0;
		charRoadPos = 0;
		collideFront = false;

		initializeEnvironment ();
	}

	void Update () {
		/* Moving vehicles */
		for (int i = 0	; i < vehicles.transform.childCount;i++) {
			vehiclePos = vehicles.transform.GetChild (i).transform.position;
			vehicles.transform.GetChild (i).transform.position = new Vector3(vehiclePos.x,vehiclePos.y,vehiclePos.z+vehicleSpeed[i]*Time.deltaTime);

			for (int j = 0 ;j<vehicles.transform.GetChild (i).childCount;j++) {
				if (vehicles.transform.GetChild (i).transform.GetChild (j).transform.position.z>leftRoadBoundary) {
					vehiclePos = vehicles.transform.GetChild (i).transform.GetChild (j).transform.position;
					vehicles.transform.GetChild (i).transform.GetChild (j).transform.position = new Vector3(vehiclePos.x,vehiclePos.y,rightRoadBoundary + (vehiclePos.z - leftRoadBoundary));
				}
				else if (vehicles.transform.GetChild (i).transform.GetChild (j).transform.position.z< rightRoadBoundary) {
					vehiclePos = vehicles.transform.GetChild (i).transform.GetChild (j).transform.position;
					vehicles.transform.GetChild (i).transform.GetChild (j).transform.position = new Vector3(vehiclePos.x,vehiclePos.y,leftRoadBoundary - (rightRoadBoundary - vehiclePos.z));
				}
			}
		}

		/* Recycling environment*/
		if (charRoadPos > upperRoadPos) { // jumping up to 'new' road
			if (!collideFront) { // if not colliding front trees
				makeEnvironment (posRoad, posRoad + 1);
				posRoad++;
				upperRoadPos++;

				if (this.transform.GetChild (0).name == (jalanan.name + "(Clone)")) { // destroy vehicles
					Destroy (vehicles.transform.GetChild (0).gameObject);
					vehicleSpeed.RemoveAt (0);
					Destroy (coins.transform.GetChild(0).gameObject);
				}
				else if (this.transform.GetChild (0).name == (trotoar.name + "(Clone)")) { // destroy trees
					Destroy (trees.transform.GetChild (0).gameObject);
				}

				/* Destroy road */
				Destroy (this.transform.GetChild (0).gameObject);
			} 
		}
	}
	
	void makeEnvironment(int initRoad, int nRoad) { // making 'nRoad' based on 'initRoad' position
		for (int i = initRoad; i<nRoad; i++) {
			/* Get the environment and place them */
			environmentType = Helper.GetRandomEnum<EnvironmentType>();
			switch (environmentType) {
			case EnvironmentType.jalanan	:	
				clone = (GameObject) GameObject.Instantiate(jalanan);
				break;
			case EnvironmentType.trotoar	:	
				clone = (GameObject) GameObject.Instantiate(trotoar);
				break;
			}
			
			clone.transform.position = new Vector3 (i*environmentGap, 0, 0);
			clone.transform.parent = GameObject.Find ("Environment").transform;
			clone.SetActive (true);
			
			/* Checking for the name of cloning environment*/
			roadName = this.transform.GetChild(this.transform.childCount-1).name;
			if (roadName == (jalanan.name + "(Clone)")) { // if it's a road (vehicle Areas), clone Vehicles and Collectibles
				vehicleContainer = new GameObject();
				vehicleContainer.name = "Vehicle";
				
				/* Randoming direction of vehicle {Left / Right} */
				vehicleDirection = Helper.GetRandomEnum<Direction>();
				
				if (vehicleDirection == Direction.right) {
					vehicleSpeed.Add(Random.Range (bottomSpeed,topSpeed));
				} else if (vehicleDirection == Direction.left){
					vehicleSpeed.Add(Random.Range (-topSpeed,-bottomSpeed));
				}
				
				/* Randoming type of vehicle */
				initVehiclePos = Random.Range (rightRoadBoundary,leftRoadBoundary);
				for (int j = 0 ;j<nCar;j++) {
					clone = (GameObject)GameObject.Instantiate (kendaraan[Random.Range (0,nKendaraan)]);
					if (vehicleDirection == Direction.left) {
						clone.transform.rotation = Quaternion.Euler (new Vector3(-90f,180f,0f));
					} else {
						clone.transform.rotation = Quaternion.Euler (new Vector3(-90f,0f,0f));
					}
					clone.transform.position = new Vector3(offsetVehiclePos + i*environmentGap,clone.transform.position.y,initVehiclePos+j*vehicleGap);
					clone.SetActive(true);

					vehicleContainer.transform.parent = vehicles.transform;
					
					clone.transform.parent = vehicleContainer.transform;
				} 

				/* Randoming collectibles */
				clone = (GameObject)GameObject.Instantiate (collectibles[Random.Range (0,nCollectibles)]);
				clone.transform.position = new Vector3(i*environmentGap,clone.transform.position.y,Random.Range (-4,5) * environmentGap);
				clone.SetActive(true);
					
				clone.transform.parent = coins.transform;

			} else if (roadName == (trotoar.name + "(Clone)")) { // if it's a sidewalk (tree Areas), clone Trees
				/* Making tree boundaries (left & right)*/
				treeContainer = new GameObject();
				treeContainer.name = "Tree";
				treeContainer.transform.parent = GameObject.Find ("Trees").transform;

				/*Making random tree on the middle*/
				for (int j = 0 ;j<nMiddleTree;j++) {
					treePos = Random.Range (-offsetTreeBoundary+j*3,-offsetTreeBoundary+(j+1)*3);
					middleTrees.Add (treePos);
				}
				for (int j = 0 ;j <nMiddleTree ;j++) {
					clone = (GameObject) GameObject.Instantiate (pohon[Random.Range (0,nPohon)]);
					clone.transform.parent = treeContainer.transform;
					clone.SetActive(true);

					clone.transform.position = new Vector3(clone.transform.position.x + environmentGap * (i+1),clone.transform.position.y,clone.transform.position.z+middleTrees[j]*environmentGap);
				}

				middleTrees.Clear();
			}
		}
		

	}

	void initializeEnvironment() {
		/* Initialize Trotoar Awal && Trees */
		
		for (int i = -nInitTrotoar; i<= 0; i++) {
			/* Making trotoar */
			clone = (GameObject)GameObject.Instantiate (trotoar);
			clone.transform.parent = GameObject.Find ("Environment").transform;
			clone.SetActive (true);
			clone.transform.position = new Vector3(clone.transform.position.x + environmentGap*i,clone.transform.position.y,clone.transform.position.z);
			
			/* Making trees */
			treeContainer = new GameObject();
			treeContainer.name = "Tree";
			treeContainer.transform.parent = GameObject.Find ("Trees").transform;
			
			
			for (int j = 0 ;j<nInitTree;j++) {
				clone = (GameObject) GameObject.Instantiate (pohon[Random.Range (0,nPohon)]);
				clone.transform.parent = treeContainer.transform;
				clone.SetActive(true);
				clone.transform.position = new Vector3(clone.transform.position.x + environmentGap * i,clone.transform.position.y,clone.transform.position.z-((5+j)*environmentGap));
				
				clone = (GameObject) GameObject.Instantiate (pohon[Random.Range (0,nPohon)]);
				clone.transform.parent = treeContainer.transform;
				clone.SetActive(true);
				clone.transform.position = new Vector3(clone.transform.position.x + environmentGap * i,clone.transform.position.y,clone.transform.position.z+((5+j)*environmentGap));

			} 
		}
		
		/* Making initial road */
		clone = (GameObject)GameObject.Instantiate (jalanan);
		clone.transform.parent = GameObject.Find ("Environment").transform;
		clone.SetActive (true);
		clone.transform.position = new Vector3(clone.transform.position.x + 0f,0f,0f);
		
		vehicleContainer = new GameObject();
		vehicleContainer.name = "Vehicle";
		
		/* Randoming direction of vehicle {Left / Right} */
		vehicleDirection = Helper.GetRandomEnum<Direction>();
		
		if (vehicleDirection == Direction.right) {
			vehicleSpeed.Add(Random.Range (bottomSpeed,topSpeed));
		} else if (vehicleDirection == Direction.left){
			vehicleSpeed.Add(Random.Range (-topSpeed,-bottomSpeed));
		}
		
		/* Randoming type of vehicle */
		initVehiclePos = Random.Range (rightRoadBoundary,leftRoadBoundary);
		for (int j = 0 ;j<nCar;j++) {
			clone = (GameObject)GameObject.Instantiate (kendaraan[Random.Range (0,nKendaraan)]);
			if (vehicleDirection == Direction.left) {
				clone.transform.rotation = Quaternion.Euler (new Vector3(-90f,180f,0f));
			} else {
				clone.transform.rotation = Quaternion.Euler (new Vector3(-90f,0f,0f));
			}
			clone.transform.position = new Vector3(offsetVehiclePos,clone.transform.position.y,initVehiclePos+j*vehicleGap);
			clone.SetActive(true);
			
			vehicleContainer.transform.parent = vehicles.transform;
			
			clone.transform.parent = vehicleContainer.transform;

		} 
		makeEnvironment(1, nInitRoad); // initializing environment
	}
}

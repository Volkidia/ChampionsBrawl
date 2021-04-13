using UnityEngine;
using System.Collections;

public class CamLevel : MonoBehaviour {
	public float FOVmax;
	public float FOVmin;
	public float CamSpeed;

	Bounds Map;
	Bounds Player;

	Camera MainCam;

	// Use this for initialization
	void Start () {
		MainCam = this.GetComponent<Camera> ();
		GameObject[] Elements = GameObject.FindGameObjectsWithTag("map");
		foreach (GameObject map in Elements) {
			Map.Encapsulate(map.GetComponent<Renderer>().bounds);
		}
		Init ();

	}
	
	// Update is called once per frame
	void Update () {
		Init ();


		float FOV = FieldOfview (FrustrumHeight ());
		MainCam.fieldOfView = Mathf.Lerp (FOVmin,FOVmax,FOV) ;
		MainCam.transform.position = Vector3.Lerp ( MainCam.transform.position, PositionCamera(),CamSpeed * Time.deltaTime);
	}

	public float FrustrumHeight (){
		float frustumHeight = Mathf.Max( Player.size.y, Player.size.z/MainCam.aspect );
		return frustumHeight;
	}

	public float FieldOfview ( float FrustrumHeight){
		float FOV = 2 * Mathf.Atan (FrustrumHeight * 0.5f / MainCam.transform.position.x) * Mathf.Rad2Deg;

		if (FOV > FOVmax) {
			return 1;
		} else if (FOV < FOVmin) {
			return 0;
		} else {
			FOV = FOV/FOVmax;
			return FOV;
		}
	}

	public Vector3 PositionCamera (){

		Vector3 Newpos;
		Newpos = MainCam.transform.position;


		if ((Player.max.y - Player.center.y) > (Map.max.y - Player.center.y)) {
		
		} else if ((Player.center.y - Player.min.y) > (Player.center.y - Map.min.y)) {

		} else {
			Newpos.y = Player.center.y;
		}

		if ((Player.max.z - Player.center.z) > (Map.max.z - Player.center.z)) {

		}else if ((Player.center.z - Player.min.z) > (Player.center.z - Map.min.z)){

		}else{
			Newpos.z = Player.center.z;
		}
		return Newpos;

	}

	void Init (){

		GameObject[] Avatar = GameObject.FindGameObjectsWithTag ("player");
		if (Avatar != null) {
			Player = Avatar[0].GetComponent<Collider>().bounds;
		}
		foreach (GameObject perso in Avatar) {
			Player.Encapsulate(perso.GetComponent<Collider>().bounds);
		}

		float targetaspect = 16.0f / 9.0f;
		// determine the game window's current aspect ratio
		float windowaspect = (float)Screen.width / (float)Screen.height;
		// current viewport height should be scaled by this amount
		float scaleheight = windowaspect / targetaspect;
		// if scaled height is less than current height, add letterbox
		if (scaleheight < 1.0f){  
			Rect rect = MainCam.rect;

			rect.width = 1.0f;
			rect.height = scaleheight;
			rect.x = 0;
			rect.y = (1.0f - scaleheight) / 2.0f;

			MainCam.rect = rect;
		}else{ // add pillarbox
			float scalewidth = 1.0f / scaleheight;
			
			Rect rect = MainCam.rect;
			
			rect.width = scalewidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scalewidth) / 2.0f;
			rect.y = 0;
			
			MainCam.rect = rect;
		}
	}
}

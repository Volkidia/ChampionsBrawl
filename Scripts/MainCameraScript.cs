using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MainCameraScript : MonoBehaviour {


	public GameObject [] players ;
	//public GameObject AbilitiesRoot;
	//List<float>  DistanceBetweenPlayers = new List<float> () ;
	//List<Vector3> NewCameraPosition = new List<Vector3>();

	float BasicHorizontalDistance = 9;

	// Use this for initialization
	void Start () {
		ComputeAspectRatio();
	}
	void TakePlayer (GameObject[]PlayerScene){
		players = PlayerScene;
	}
	void ComputeAspectRatio() {
		float targetaspect = 16.0f / 9.0f;
		
		// determine the game window's current aspect ratio
		float windowaspect = (float)Screen.width / (float)Screen.height;
		
		// current viewport height should be scaled by this amount
		float scaleheight = windowaspect / targetaspect;
		
		// obtain camera component so we can modify its viewport
		Camera camera = GetComponent<Camera>();
		
		// if scaled height is less than current height, add letterbox
		if (scaleheight < 1.0f)
		{  
			Rect rect = camera.rect;

			rect.width = 1.0f;
			rect.height = scaleheight;
			rect.x = 0;
			rect.y = (1.0f - scaleheight) / 2.0f;
			
			camera.rect = rect;
		}
		else // add pillarbox
		{
			float scalewidth = 1.0f / scaleheight;
			
			Rect rect = camera.rect;
			
			rect.width = scalewidth;
			rect.height = 1.0f;
			rect.x = (1.0f - scalewidth) / 2.0f;
			rect.y = 0;
			
			camera.rect = rect;
		}
	}

	// Update is called once per frame
	void Update () {

		ComputeAspectRatio();
		Bounds bbox;
		bbox = players[0].GetComponent<Collider>().bounds;
		for (int n =1; n < players.Length; n++) {
			bbox.Encapsulate( players[n].GetComponent<Collider>().bounds );
		}


		bbox.Expand(new Vector3 (0, 4, 4)); 

		//AbilitiesRoot.BroadcastMessage ( "Scale", GetComponent<Camera>() , SendMessageOptions.DontRequireReceiver);

		float frustumHeight = Mathf.Max( bbox.size.y, bbox.size.z/GetComponent<Camera>().aspect );
		if (frustumHeight < 27) {
			MoveCamera (bbox.center,frustumHeight);
			if ((Mathf.Abs (bbox.size.y) > 9) || (Mathf.Abs (bbox.size.z) > 16)) {
				ScaleViewport (frustumHeight);
			} else {
				ScaleViewport (BasicHorizontalDistance);
			}
		}
		//AbilitiesRoot.SendMessage ( "scale", frustumHeight, SendMessageOptions.DontRequireReceiver);
		#if UNITY_EDITOR
		Debug.DrawLine (new Vector3 (0, bbox.min.y, bbox.min.z), new Vector3 (0, bbox.max.y, bbox.min.z));
		Debug.DrawLine (new Vector3 (0, bbox.min.y, bbox.max.z), new Vector3 (0, bbox.max.y, bbox.max.z));
		Debug.DrawLine (new Vector3 (0, bbox.min.y, bbox.min.z), new Vector3 (0, bbox.min.y, bbox.max.z));
		Debug.DrawLine (new Vector3 (0, bbox.max.y, bbox.min.z), new Vector3 (0, bbox.max.y, bbox.max.z));
		#endif

	}

	float FOVForHeightAndDistance(float height, float distance)
	{
		return 2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg;
	}

	void ScaleViewport(float Distance){
		float DistanceCamtoViewport = GetComponent<Camera>().transform.position.x;
		GetComponent<Camera>().fieldOfView = FOVForHeightAndDistance (Distance, DistanceCamtoViewport);
	}

	void MoveCamera (Vector3 cameraPosition, float FrustrumHeight){
		cameraPosition.x = GetComponent<Camera>().transform.position.x;
		float FrustrumWidth = FrustrumHeight * GetComponent<Camera>().aspect;
		if (cameraPosition.y < FrustrumHeight/2) {
			cameraPosition.y = FrustrumHeight/2;
		}
		if (cameraPosition.y > FrustrumHeight/2+ (27.5f - FrustrumHeight)){
			cameraPosition.y = FrustrumHeight/2 + (27.5f - FrustrumHeight);
		}
		if (cameraPosition.z < FrustrumWidth/2) {
			cameraPosition.z = FrustrumWidth/2;
		}
		if (cameraPosition.z > FrustrumWidth/2+ (48.5f - FrustrumWidth)){
			cameraPosition.z = FrustrumWidth/2 + (48.5f - FrustrumWidth);
		}
		GetComponent<Camera>().transform.position = cameraPosition;
	}
}

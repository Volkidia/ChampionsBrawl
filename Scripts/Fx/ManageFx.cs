using UnityEngine;
using System.Collections;

public class ManageFx : MonoBehaviour {
	public Sprite[] FxSprites;
	public Sprite thisSprite;
	public float FxWait;

	public enum FxType{
		Additive,
		Substractive
	};


	public FxType TypeName;
	Coroutine ActualCoroutine;
	// Use this for initialization
	void Start () {
		if (TypeName == FxType.Additive) {
			StartCoroutine (Add());
		} else if (TypeName == FxType.Substractive) {
			StartCoroutine (Sub());
		}
	}

	IEnumerator Add (){
		Debug.Log ("coucou");
		yield return null;
	}
	
	IEnumerator Sub (){
		foreach (Sprite fx in FxSprites) {
			thisSprite = fx;
			Debug.Log("hehehehe");
			yield return new WaitForSeconds(FxWait);
		}
	}




}

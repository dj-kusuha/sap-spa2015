using UnityEngine;
using System.Collections;

public class ClickSEManager : MonoBehaviour {

	public static ClickSEManager Instance{get;set;}

	[SerializeField]
	private GameObject sePrefab;


	private AudioSource seInstance;

	// Use this for initialization
	void Awake () {
		if( Instance == null ){
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		} else {
			Destroy(this.gameObject);
		}
	}
	
	public void PlaySE(){
		if(this.seInstance == null){
			var obj = (GameObject)Instantiate(this.sePrefab);
			obj.transform.SetParent( this.transform );
			this.seInstance = obj.GetComponent<AudioSource>();
		}
		this.seInstance.Play();
	}
}

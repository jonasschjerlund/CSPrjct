using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

	// Tiny wee script which plays the song

	public AudioSource m;


	// Use this for initialization
	void Start () {
		//AudioSource m = GetComponent<AudioSource> ();
		m.Play();
	}


}

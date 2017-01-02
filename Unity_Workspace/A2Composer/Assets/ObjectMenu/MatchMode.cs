using UnityEngine;
using System.Collections;

public class MatchMode : MonoBehaviour {
	public Color colorStart = Color.red;
	public Color colorEnd = Color.green;
	public float duration = 1.0F;
	public Renderer rend;
	public bool matchMode;

	void Start () {
		rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (matchMode) {
			transform.parent.gameObject.SetActive (true);
			float lerp = Mathf.PingPong (Time.time, duration) / duration;
			rend.material.color = Color.Lerp (colorStart, colorEnd, lerp);
		} else {
			transform.parent.gameObject.SetActive (false);
		}
	}
}

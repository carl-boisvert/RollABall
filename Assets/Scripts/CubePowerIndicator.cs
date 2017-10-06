using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePowerIndicator : MonoBehaviour {

    public Transform player;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if(this.player){
            this.transform.position = this.player.transform.position;
            this.transform.Rotate(new Vector3(1, 1, 1));
        } else{
            this.transform.Rotate(new Vector3(1, 1, 1));
        }
	}
}

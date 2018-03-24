using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Observable.Interval( System.TimeSpan.FromSeconds( 1f ) ).Subscribe( _ => {
            gameObject.SetActive( !gameObject.activeInHierarchy );
        } );
	}

}

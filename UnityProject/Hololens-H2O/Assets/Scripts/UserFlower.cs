using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserFlower : MonoBehaviour {

    public Transform User;

    void Start () {
        User = Camera.main.transform;
	}

	void Update () {
        gameObject.transform.localPosition = User.localPosition;
	}
}

using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	public float y_rotate_speed = 10.0f;
	public float move_speed = 10.0f;

	private float movement;
	private float y_movement;
	private float y_rotation;


	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		// forward/backward control
		if (Input.GetKey ("up") && !Input.GetKey ("down")) {
			movement = 1;
		} else if (Input.GetKey ("down") && !Input.GetKey ("up")) {
			movement = -1;
		} else {
			movement = 0;
		}
		// y rotation control
		if (Input.GetKey ("right") && !Input.GetKey ("left")) {
			y_rotation = 1;
		} else if (Input.GetKey ("left") && !Input.GetKey ("right")) {
			y_rotation = -1;
		} else {
			y_rotation = 0;
		}
		// vertical control
		if (Input.GetKey (KeyCode.Space) && !Input.GetKey (KeyCode.LeftShift)) {
			y_movement = 1;
		} else if (Input.GetKey (KeyCode.LeftShift) && !Input.GetKey (KeyCode.Space)) {
			y_movement = -1;
		} else {
			y_movement = 0;
		}
		//forward movement implemented in object space, rotation and vertical in world space 
		transform.Rotate (new Vector3 (0, y_rotation, 0 ) * Time.deltaTime * y_rotate_speed, Space.World);
		transform.Translate (new Vector3 (0, 0, movement) * Time.deltaTime * move_speed);
		transform.Translate (new Vector3 (0, y_movement, 0) * Time.deltaTime * move_speed, Space.World);

	}
		
}
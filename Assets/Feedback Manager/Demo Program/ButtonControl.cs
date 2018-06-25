using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonControl : MonoBehaviour {


	public MeshRenderer myRenderer;

    //////////////////////////////////////////////////////////////////
    //////////// HOW TO REFERENCE TABS IN THE SCRIPT /////////////////
    //////////////////////////////////////////////////////////////////

	public TabContinuous1Var lineTab;
	public TabCategorical pieTab;
	public TabContinuous2Var twoVarTab;

    ///////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////
	 
	public Color green = Color.green;
	public Color red = Color.red;
	public Color blue = Color.blue;
	public Text buttonText;

	public float standbyRotateSpeed;
	public float offRotateSpeed;
	public float onRotateSpeed;
	public float lowCountTime;
	public float highCountTime;

	private enum State {standby, off, on};
	private State state;
	private IDictionary<State, Color> stateColors;

	private float turnOnCount;
	private float timeStart;
	private float timeStop;
	private float rotateSpeed;


	// Use this for initialization
	void Start () {
		// set renderer, state/color dict, text and set state
		myRenderer = GetComponent<MeshRenderer> ();
		stateColors = new Dictionary<State, Color> () {
			{ State.standby, blue },
			{ State.off, red },
			{ State.on, green }
		};
		buttonText.text = "Click cube\nto begin";
		StartStandby();
	}

	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0, rotateSpeed * Time.deltaTime, 0), Space.World);
	}

    /// <summary>
    /// THIS is where the values are RECORDED to TABS.
    /// </summary>
	void OnMouseDown () {
		switch (state) {

		// if on standby, click sets button off and begins countdown to turn on
		case State.standby:
			StartButtonOff ();
			break;
          
		// if button's on, RECORD VALUE and set to standby
		case State.on:
			StartStandby ();
            
            /////////////////////////////////////////////////////////////
            ////////////////// RECORDING TO TABS ////////////////////////
            /////////////////////////////////////////////////////////////

			float reactionTime = timeStop - timeStart;
            // progressive tab
            lineTab.AddSingleValue(reactionTime);
        
            
            // categorical (pie chart) tab
            string speedRating;
                if (reactionTime < 0.25) {
                    speedRating = "Super Fast";
                } else if (reactionTime < 0.3) {
                    speedRating = "Fast";
                } else if (reactionTime < 0.35) {
                    speedRating = "Quite Fast";
                } else if (reactionTime < 0.4) {
                    speedRating = "Decent";
                } else if (reactionTime < 1.0f) {
                    speedRating = "Slow";
                } else {
                    speedRating = "Very Slow";
                }

                /*
                // an example pie chart with Red, Green and Blue categories

                if (reactionTime < 0.35) speedRating = "Green";
                else if (reactionTime < 0.6) speedRating = "Blue";
                else speedRating = "Red";
                */

 			pieTab.AddToCategory (speedRating);
			// two variable tab 
			twoVarTab.AddPoint(turnOnCount, reactionTime);
            
            //////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////

			// button text and debug log
			buttonText.text = "Reaction time:\n" + string.Format ("{0:N4}", timeStop - timeStart) + " seconds\nclick cube to begin";
			Debug.Log ("Reaction time: " + string.Format ("{0:N4}", timeStop - timeStart) + " seconds");
			break;

		// if button's off, display error message and set to standby.
		case State.off:
			buttonText.text = "Oops, clicked\ntoo soon!\nclick cube to begin";
			StartStandby ();
			break;

		}
	}

	void StartStandby() {
		// cancel invokes, record stop time and set on standby
		// effects: no rotation, blue material and blue text
		CancelInvoke ();
		timeStop = Time.time;
		state = State.standby;
		rotateSpeed = standbyRotateSpeed;
		SetRightMaterial ();

	}

	void TurnOn() {
		// set button on and record time turned on
		// effects: green material, faster rotate speed & message
		state = State.on;
		rotateSpeed = onRotateSpeed;
		SetRightMaterial ();
		buttonText.text = "NOW!";
		timeStart = Time.time;
	}

	void StartButtonOff() {
		// set button off and begin countdown
		// effects: red material, slower rotate speed & message
		state = State.off;
		rotateSpeed = offRotateSpeed;
		SetRightMaterial ();
		buttonText.text = "Ready to click\n cube when green...";
		CountDown ();
	}		

	void CountDown() {
		// invoke new count to green
		// random no. of secs, as defined (float)
		turnOnCount = Random.Range(lowCountTime, highCountTime);
		Invoke ("TurnOn", turnOnCount);
	}

	void SetRightMaterial() {
		// sets material (color) of button cube and text
		Color color;
		stateColors.TryGetValue (state, out color);
		myRenderer.material.color = color;
		buttonText.color = (color / 1.5f);
	}

}


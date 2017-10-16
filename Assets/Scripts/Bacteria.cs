//using System;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Bacteria : MonoBehaviour
    {
		private Boolean doNotMove;
        private MovementStates mMovementState = MovementStates.SessileState;
        private ModelParameter mParameter;

        // Individual bacteria are between 0.5 and 1.25 micrometers in diameter. From: https://microbewiki.kenyon.edu/index.php/Streptococcus_pneumoniae
        // So we take 1 roughly as guideline

        private float mCurrentAngle = 0;

        private float SinkBelowStepsize = 0.3f; //cs unter dieser geschwindigkeit sinkt die bakterie
        private float SinkSpeed = 0.4f;  //cs mit dieser geschwindigkeit sinkt die bakterie

        public float X { get { return transform.position.x; } }
        public float Y { get { return transform.position.y; } }
        public float Z { get { return transform.position.z; } }//cs

		public void disableMovement() {
			doNotMove = true;
			//Destroy(gameObject.GetComponent<Rigidbody> ());
		}

        public float StepSize
        {
            get
            {
                float step = 0;
                if (mMovementState == MovementStates.FlowingState)
                {
                    step = mParameter.MovementInFlowingPhase;
                }
                else
                {
                    step = mParameter.MovementInSessilePhase;
                }
                return step;
            }
        }
        public MovementStates State { get { return this.mMovementState; } }

        public enum MovementStates
        {
            SessileState,
            FlowingState
        }

        public void Start()
        {
            GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
            mParameter = gc.Parameter;

            SetNewHeight();
            StartCoroutine(NewHeadingCoroutine());
        }

        private void SetNewHeading()
        {
            mCurrentAngle = UnityEngine.Random.Range(0F, 1F) * 2 * Mathf.PI; // Random direction

        }
        private IEnumerator NewHeadingCoroutine()
        {
            while (true)
            {
                SetNewHeading();

                yield return new WaitForSeconds(0.1F);
            }
        }

        private void SetNewHeight()//cs
        {
            var RandomZ = UnityEngine.Random.Range(_globals.PlaygroundZMin, _globals.PlaygroundZMax);
            transform.position = new Vector3(transform.position.x, transform.position.y, RandomZ);
        }


        private void InterchangePhase()
        {
            var randNum = UnityEngine.Random.Range(0F, 1F);
            // Change state if probability is met. This will change the step size as well
            if (randNum > mParameter.ProbabilityInterchanged)
            {
                if (mMovementState == MovementStates.FlowingState)
                {
                    mMovementState = MovementStates.SessileState;
                }
                else
                {
                    mMovementState = MovementStates.FlowingState;
                }
            }
        }
        /// <summary>
        /// Moves the bacteria. It will jump to mobile phase with a certain propability
        /// </summary>
        public void Update()
        {
			if(doNotMove) {
				return;
			}
            InterchangePhase();
            InterchangePhase();

            if (transform.position.z < _globals.PlaygroundZMin)
            {
                var x = (float)(Mathf.Cos(mCurrentAngle) * StepSize);
                var y = (float)(Mathf.Sin(mCurrentAngle) * StepSize); // Step into the direction defined
                float z = 0;

                if (StepSize < SinkBelowStepsize)
                { z = (float)(SinkSpeed); }
                else
                { z = 0; }

                //D ebug.Log("winkelfloat:" + mCurrentAngle + "   Stepsize:" + StepSize);
                PlayerMovementClamping();

                // Apply and smooth out movement
                Vector3 movement = new Vector3(x, y, z);
                movement *= Time.deltaTime;
                transform.Translate(movement);
            }
        }

        void PlayerMovementClamping()
        {
			return;
            var viewpointCoord = Camera.main.WorldToViewportPoint(transform.position);
            viewpointCoord.x = Mathf.Clamp01(viewpointCoord.x);
            viewpointCoord.y = Mathf.Clamp01(viewpointCoord.y);
            // viewpointCoord.z = Mathf.Clamp(viewpointCoord.z);
            transform.position = Camera.main.ViewportToWorldPoint(viewpointCoord);
        }

    }
}

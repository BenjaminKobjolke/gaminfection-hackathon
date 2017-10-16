using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class Macrophage : MonoBehaviour
	{
		public AudioClip eat;
		AudioSource audioSource;
        // Model parameter
        ModelParameter mParameter;
        private Rigidbody mRigidBody;
        // Eaten bacteria
        private int mBacteriaEaten = 0;

        public float X { get { return transform.position.x; } }
        public float Y { get { return transform.position.y; } }
		// Biological parameters for macrophages		
        private Vector3 mDirection = Vector3.one;
        private GameObject target;
        public Vector2 velocity;
		private float cellTargetZ = -4f;

		private float searchRange = 30.0f;
		private float searchRangeBacteria = 5.0f;
		private Cell lastTargetCell;

        /// <summary>
        /// Movement states the macrophage can be in
        /// </summary>
        enum MovementStates
        {
            /// <summary>If not sensing chemokine or bacterias are close, we can idle around doing macrophagy things</summary>
            Idle,
            ///<summary>Somethings wrong in the neighbourhood, we should check it out</summary> 
            ChemokineFound,
            ///<summary>EXTERMINATE!!! EXTERMINAAAAATTTE!!!</summary> 
            BaceriaInRange
        }

        /// <summary>
        /// Current movement state
        /// </summary>
        private MovementStates movementState = MovementStates.Idle;
        /// <summary>
        /// Last movement state
        /// </summary>
        private MovementStates lastMovementState = MovementStates.Idle;

        /// <summary>
        /// The last state will always be remembered in lastMovementState
        /// </summary>
        /// <seealso cref="lastMovementState"/>
        private MovementStates MovementState
        {
            get { return movementState; }
            set { if (value != movementState) { lastMovementState = movementState; movementState = value; } }
        }

        // Internal state. Don't touch
        private int mBacteriaNear = 0;

        /// <summary>
        /// Number of bacteria in range
        /// </summary>
        private int BacteriaNear
        {
            set
            {
                mBacteriaNear = value;
                if (value > 0)
                {
                    MovementState = MovementStates.BaceriaInRange;
                }
                else
                {
                    MovementState = lastMovementState;
                }
            }
            get
            {
                return mBacteriaNear;
            }
        } // Counter for close bacterias. Also sets the state to exterminate!!!

        public void Start()
        {
			audioSource = GetComponent<AudioSource>();
            GameController gc = GameObject.Find("GameController").GetComponent<GameController>();
            mParameter = gc.Parameter;
            mRigidBody = GetComponent<Rigidbody>();
            if (!mRigidBody)
            {
                Debug.LogError("No rigidBody attached!");
            }
            StartCoroutine(NewHeadingCoroutine());
        }

        /// <summary>
        /// Set new heading depending on state
        /// </summary>
        private void SetNewHeading()
        {
            switch (movementState)
            {
			case MovementStates.Idle:
					transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
				mDirection = new Vector3(Random.Range(-1F, 1F), Random.Range(-1F, 1F)); // Random direction
                    target = this.gameObject;
                    break;
			case MovementStates.ChemokineFound:
				var cellList = GetObjectsAround<Cell> ("Cell", searchRange);
				Cell cellWithMaxChemokine = cellList.OrderByDescending (c => c.Chemokine).First ();
				target = cellWithMaxChemokine.gameObject;
				lastTargetCell = cellWithMaxChemokine;
				Debug.Log ("target cell set");
					Vector3 cTargetPos = new Vector3(target.transform.position.x, target.transform.position.y, cellTargetZ); 
					mDirection = (cTargetPos - transform.position).normalized;
                    break;
                case MovementStates.BaceriaInRange:
                    var bactList = GameObject.FindGameObjectsWithTag("Bacteria").ToList();
                    Bacteria nearestBact = bactList
                        .OrderByDescending(b => Vector3.Distance(transform.position, b.transform.position))
                        .First()
                        .GetComponent<Bacteria>();
                    target = nearestBact.gameObject;
                    mDirection = (target.transform.position - transform.position).normalized;
                    break;
                default:
                    Debug.LogError("Macrophage state not implemented!");
                    break;
            }
        }

        /// <summary>
        /// New heading coroutine. Will generate a new heading every 3 seconds if in idle state and every 100ms in agitated state
        /// </summary>
        /// <returns></returns>
        private IEnumerator NewHeadingCoroutine()
        {
            while (true)
            {                
                SetNewHeading();                
                yield return new WaitForSeconds(movementState == MovementStates.Idle ? 3F : 0.1F);
            }
        }

        public void FixedUpdate()
        {
            PlayerMovementClamping();
            var speed = mParameter.MacrophageMovement * 1;
            //Debug.DrawLine(transform.position, target.transform.position, Color.black);
            Vector3 myPosition = transform.position; // trick to convert a Vector3 to Vector2
            mRigidBody.MovePosition(myPosition + mDirection * speed * Time.deltaTime);

			if (myPosition.z <= cellTargetZ && movementState == MovementStates.ChemokineFound) {
				if (lastTargetCell != null) {
					//D ebug.Log ("EQUALS! " + lastTargetCell.Chemokine);
					if (lastTargetCell.Chemokine < 1) {
						//D ebug.Log ("FINISH CELL CLEANING!");
						movementState = MovementStates.Idle;
					}
				}

			}

            // If our spider senses are tingeling and we smell chemokine we switch to search mode.
            // But only if we don't have a bacteria inside. Need to exterminate them first
            if (MovementState != MovementStates.BaceriaInRange)
            {
				var cellList = GetObjectsAround<Cell>("Cell", searchRange);
				var bacList = GetObjectsAround<Bacteria>("Bacteria", searchRangeBacteria);
                if (cellList.Count > 0 && cellList.Max(c => c.Chemokine) > 1)
                {
					Debug.Log ("CHEMOKINE LEVELS FOUND! ");
					foreach (Cell cCell in cellList)
					{
						if (cCell.Chemokine > 1) {
							//D ebug.Log ("cellList Chemokine: " + cCell.Chemokine);
						}
					}
                    MovementState = MovementStates.ChemokineFound;
				} else if (bacList.Count > 0)
				{
					MovementState = lastMovementState;

					Debug.Log ("Found Bacteria near me!");
					var bactListSorted = GameObject.FindGameObjectsWithTag("Bacteria").ToList();
					Bacteria nearestBact = bactListSorted
						.OrderByDescending(b => Vector3.Distance(transform.position, b.transform.position))
						.First()
						.GetComponent<Bacteria>();
					target = nearestBact.gameObject;
					//D ebug.Log ("Target z is: " + target.transform.position.z);
					mDirection = (target.transform.position - transform.position).normalized;

					MovementState = MovementStates.BaceriaInRange;

				}
                else
                {
                    MovementState = lastMovementState;
                }
            }
        }

        /// <summary>
        /// Get all objects with a tag of type T within the radius
        /// </summary>
        /// <typeparam name="T">Object type to be returned</typeparam>
        /// <param name="tag">Tag of the object</param>
        /// <param name="radius">Radius to search for</param>
        /// <returns>List of T</returns>
        List<T> GetObjectsAround<T>(string tag, float radius)
        {
            return GameObject.FindGameObjectsWithTag(tag)
                .Where(go => Vector3.Distance(go.transform.position, transform.position) <= radius)
                .Select(go => go.GetComponent<T>())
                .ToList();
        }

        void PlayerMovementClamping()
        {
			return;
            var viewpointCoord = Camera.main.WorldToViewportPoint(transform.position);
            viewpointCoord.x = Mathf.Clamp01(viewpointCoord.x);
            viewpointCoord.y = Mathf.Clamp01(viewpointCoord.y);
            transform.position = Camera.main.ViewportToWorldPoint(viewpointCoord);
        }


#region Collider
        /// <summary>
        /// Handles the entry collision with this object
        /// </summary>
        /// <param name="e">other object</param>
		/// 
		/*
		private void OnCollisionEnter(Collision e)
        {
            if(e.gameObject.tag.Contains("Bacteria"))
            {
				Debug.Log ("Got Bacteria!");
                BacteriaNear++;
            }
        }
		*/

        /// <summary>
        /// Checks permanently for any objects inside the hit area
        /// </summary>
        /// <param name="e">other object</param>
		private void OnCollisionEnter(Collision e)
        {
			mRigidBody.velocity = Vector3.zero;

			if (e.gameObject.tag.Contains ("Bacteria")) {
				Debug.Log ("Got Bacteria!");
				BacteriaNear++;
				var distToBact = Vector2.Distance (transform.position, e.transform.position);
				var macBounds = 0.7F;
				//if (distToBact < macBounds)
				//{
				BacteriaNear--;
				Destroy (e.gameObject);
				mBacteriaEaten++;
				Debug.Log ("Bacteria exterminated");
				audioSource.PlayOneShot(eat, 1.0F);
				return;
				//}
			} else if (e.gameObject.tag.Contains ("Cell")) {
				Debug.Log ("TEEEEEEEEEEEST");
			}
        }

        private void OnTriggerExit2D(Collider2D e)
        {
            if (e.gameObject.name.Contains("Bacteria"))
            {
                BacteriaNear--;
            }
        }
#endregion
    }

}

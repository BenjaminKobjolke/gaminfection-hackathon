using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Assets.Scripts
{

    public class Torpedo : MonoBehaviour
    {

		public float movementSpeed;
		public GameObject explosion;

        // Use this for initialization
        void Start()
        {
			GetComponent<Rigidbody>().AddForce(transform.forward * movementSpeed, ForceMode.Force); 
			GetComponent<Rigidbody>().AddForce(-1 * transform.up * movementSpeed * 0.5f, ForceMode.Force);
        }

        // Update is called once per frame
        void Update()
        {

            //var speed = _globals  ;
            //Debug.DrawLine(transform.position, target.transform.position, Color.black);
            //Vector2 myPosition = transform.position; // trick to convert a Vector3 to Vector2
            //mRigidBody.MovePosition(myPosition + mDirection * speed * Time.deltaTime);
            //// If our spider senses are tingeling and we smell chemokine we switch to search mode.
            //// But only if we don't have a bacteria inside. Need to exterminate them first
            //if (MovementState != MovementStates.BaceriaInRange)
            //{
            //    var cellList = GetObjectsAround<Cell>("Cell", 1.5F);
            //    if (cellList.Count > 0 && cellList.Max(c => c.Chemokine) > 0)
            //    {
            //        MovementState = MovementStates.ChemokineFound;
            //    }
            //    else
            //    {
            //        MovementState = lastMovementState;
            //    }
            //}

        }
		private void OnCollisionEnter(Collision e)
		{
			//D ebug.Log (e.gameObject.tag);

			if(e.gameObject.tag.Contains("Floor") || e.gameObject.tag.Contains("Cell"))
			{
				//D ebug.Log ("HITTING THE FLOOR!");
				Instantiate(explosion, transform.position , e.gameObject.transform.rotation);   
				Destroy (gameObject);

                StartCoroutine(TakeSnapshot());
            }
		}

        static int i = 0;
        IEnumerator TakeSnapshot()
        {

            GameController gc = GameObject.Find("GameController").GetComponent<GameController>();

            ModelParameter lp = gc.Parameter;

            try
            {


                string parsPfad = "C:/Snapshots/Snapshot_U_penumia_Pars" + i + ".xml";
                if (File.Exists(parsPfad)) File.Delete(parsPfad);

                if (!Directory.Exists(Path.GetDirectoryName(parsPfad)))
                { Directory.CreateDirectory(Path.GetDirectoryName(parsPfad)); }


                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(ModelParameter));
                TextWriter writer = new StreamWriter(parsPfad);
                ser.Serialize(writer, lp);
            }
            catch (Exception ex)
            {
                Debug.Log("snapshoterr: " + ex.Message);
            }

            //string bacPfad = "C:/Snapshots/Snapshot_U_penumia_Bac" + i + ".xml";
            //if (File.Exists(bacPfad)) File.Delete(bacPfad);

            //Bacteria[] bac = gc.Bacterias;
            //System.Xml.Serialization.XmlSerializer serb = new System.Xml.Serialization.XmlSerializer(typeof(Bacteria));
            //TextWriter writerb = new StreamWriter(bacPfad);
            //serb.Serialize(writerb, bac);


            i++;

            yield return new WaitForSeconds(0F);
        }
    }
}
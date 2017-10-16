using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DigitalRuby.Tween;

namespace Assets.Scripts
{
    public class Cell : MonoBehaviour
    {
        private ModelParameter mParameter;
        private GameController mGameController;
        private ParticleSystem mChemokineEmitter;

        public float ChemokineLevels = 0F; // This is only a display in the unity UI
        public bool DebugInformation = false;
        private float mChemokine = 0F;

        public int BacteriaOnCell;
        public List<Macrophage> MacrophageOnCell = new List<Macrophage>();
        // Cell width is mParameter.EpithelialCellWidth (30)

		//private MeshRenderer cRenderer;

        public void Start()
        {
			//cRenderer = GetComponent<MeshRenderer> ();
            mGameController = GameObject.Find("GameController").GetComponent<GameController>();
            mChemokineEmitter = GetComponent<ParticleSystem>();

            mParameter = mGameController.Parameter;

			/*
			Color endColor = UnityEngine.Random.ColorHSV(0.0f, 1.0f, 0.0f, 1.0f, 0.5f, 1.0f, 1.0f, 1.0f);
			gameObject.Tween("ColorChange", cRenderer.material.color, endColor, 1.0f, TweenScaleFunctions.QuadraticEaseOut, (t) =>
				{
					// progress
					cRenderer.material.color = t.CurrentValue;
				}, (t) =>
				{
					// completion
				});
			*/
        }

		private void OnCollisionEnter(Collision e)
		{
			Debug.Log ("COLLISSIONSSSIOSUOIUS " + e.gameObject.tag);

			if (e.gameObject.tag.Contains("Bacteria"))
            {
				//D ebug.Log("Bacteria on cell!");
                BacteriaOnCell++;
				Bacteria cBac = e.gameObject.GetComponent<Bacteria>();
				cBac.disableMovement ();
            }
            else if (e.gameObject.name.Contains("Macrophage"))
            {
                var mac = e.gameObject.GetComponent<Macrophage>();
                if (!MacrophageOnCell.Contains(mac))
                {
                    MacrophageOnCell.Add(mac);
                }
            }
			else if (e.gameObject.tag.Contains("Torpedo"))
			{
				//D ebug.Log ("toOOORPEDO");
				mChemokineEmitter.Play ();
				Chemokine = 100f;
				gameObject.Tween("ChemokineDown_" + GetInstanceID(), Chemokine, 0.0f, 10.0f, TweenScaleFunctions.QuadraticEaseOut, (t) =>
					{
						// progress
						Chemokine = t.CurrentValue;
					}, (t) =>
					{
						// completion
						Debug.Log("TWEEN DONE");
						mChemokineEmitter.Stop();
						Chemokine = 0.0f;
					});

			}
        }
        private void OnTriggerExit2D(Collider2D e)
        {
            if (e.gameObject.name.Contains("Bacteria"))
            {
                BacteriaOnCell--;
            }
            else if (e.gameObject.name.Contains("Macrophage"))
            {
                var mac = e.gameObject.GetComponent<Macrophage>();
                if (MacrophageOnCell.Contains(mac))
                {
                    MacrophageOnCell.Remove(mac);
                }
            }
        }

        public void Update()
        {
        }

        public float Chemokine
        {
            set
            {
                mChemokine = value;
                //var input = Mathf.Clamp01(value * 1e4F);
                //var input = Mathf.InverseLerp(1e-6F, 1e-3F, value);
                //ChemokineLevels = input*100;
				//D ebug.Log ("ChemokineLevels: " + ChemokineLevels);
                if (mChemokineEmitter)
                {
					//D ebug.Log ("emitter found!");
                    //var m = mChemokineEmitter.main;
                    //var e = mChemokineEmitter.emission;
                    //m.maxParticles = (int)(input*100);
                    //e.rateOverTime = (int)(input*50);
                }
            }
            get
            {
				if (mParameter == null) {
					return 0.0f;
				}
				return mChemokine;
                //return Mathf.Round(mChemokine / mParameter.SensitivityToFeelCytokineGradient) * mParameter.SensitivityToFeelCytokineGradient;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public class TriggerSplashEffect : MonoBehaviour
    {
        #pragma warning disable CS0649
        Collider2D splashZone;
        List<GameObject> targets = new List<GameObject>();
        #pragma warning restore CS0649

        public void ApplyComabtAction(CharControlSingle user, G.CombatAction action)
        {
            CharControlSingle targetScript;

            foreach (GameObject target in targets)
            {
                targetScript = target.GetComponent<CharControlSingle>();

                if (targetScript != null)
                {
                    G.UseCombatAction(user, targetScript, action);
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            splashZone = gameObject.GetComponent<Collider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            targets.Add(col.gameObject);
        }

        void OnTriggerExit2D(Collider2D col)
        {
            targets.Remove(col.gameObject);
        }
    }
}
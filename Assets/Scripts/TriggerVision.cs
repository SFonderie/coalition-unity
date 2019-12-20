using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public class TriggerVision : MonoBehaviour
    {
        #pragma warning disable CS0649
        [SerializeField]
        TriggerDialogue dialogueRef;
        [SerializeField]
        TriggerCombat combatRef;
        #pragma warning restore CS0649

        Patrol patrolScript;
        ContactFilter2D raycastFilter;
        RaycastHit2D[] hits = new RaycastHit2D[1];
        List<Collider2D> targets;
        Vector2 start, end;
        float distance;
        CharControlSingle iSeeYou;
        bool active = true;

        // Start is called before the first frame update
        void Start()
        {
            patrolScript = transform.parent.GetComponent<Patrol>();
            raycastFilter = patrolScript.GetRaycastFilter();
            targets = new List<Collider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void FixedUpdate()
        {
            if(!active)
            {
                return;
            }

            start = new Vector2(transform.position.x, transform.position.y);

            foreach (Collider2D col in targets)
            {
                end = new Vector2(col.transform.position.x, col.transform.position.y);
                distance = Vector2.Distance(start, end);

                if (Physics2D.Raycast(start, end - start, raycastFilter, hits, distance) == 0)
                {
                    if((int) col.transform.gameObject.GetComponent<CharControlSingle>().GetFaction() * (int) transform.parent.GetComponent<CharControlSingle>().GetFaction() == -1)
                    {
                        if(dialogueRef && active)
                        {
                            dialogueRef.Activate();
                            active = false;
                        }

                        if(combatRef && active)
                        {
                            combatRef.Activate();
                            active = false;
                        }
                    }
                    //Debug.Log(col.transform.name + " is visible");
                }
                else
                {
                    //Debug.Log(hits[0].collider.transform.name + " is blocking the view of " + col.transform.name);
                }
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            iSeeYou = col.transform.gameObject.GetComponent<CharControlSingle>();

            if (iSeeYou != null)
            {
                targets.Add(col);
            }
        }

        void OnTriggerExit2D(Collider2D col)
        {
            targets.Remove(col);
        }
    }
}
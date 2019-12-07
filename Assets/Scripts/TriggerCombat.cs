using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public class TriggerCombat : MonoBehaviour
    {
        #pragma warning disable CS0649
        [SerializeField]
        bool isActive = true;
        [SerializeField]
        int doTimes = 1;
        [SerializeField]
        GameObject[] enemies;
        #pragma warning restore CS0649
        CharControlOverlord playerScript;
        int timesDone = 0;

        public void Activate()
        {
            timesDone++;
            playerScript.StartCombat(ref enemies);
        }

        // Start is called before the first frame update
        void Start()
        {
            playerScript = GameObject.Find("PlayerController").GetComponent<CharControlOverlord>();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (isActive && timesDone < doTimes && IsValidCollider(col))
            {
                Activate();
            }
        }

        bool IsValidCollider(Collider2D col)
        {
            return col.gameObject == playerScript.GetPlayer();
        }
    }
}
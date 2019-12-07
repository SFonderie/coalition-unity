using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Coalition
{
    public class TriggerMouse : MonoBehaviour
    {
        #pragma warning disable CS0649
        List<GameObject> activeObjects = new List<GameObject>();
        #pragma warning restore CS0649

        public List<GameObject> GetActiveObjects()
        {
            return activeObjects;
        }

        public void PrintActiveObjects()
        {
            string result = "";

            foreach (GameObject obj in activeObjects)
            {
                result += obj.name + " ";
            }

            Debug.Log(result);
        }

        public G.Faction SearchForAnyFaction()
        {
            CharControlSingle character;

            foreach (GameObject obj in activeObjects)
            {
                character = obj.GetComponent<CharControlSingle>();

                if (character != null)
                {
                    return character.GetFaction();
                }
            }

            return G.Faction.none;
        }

        public bool SearchForFaction(G.Faction faction)
        {
            CharControlSingle character;

            foreach(GameObject obj in activeObjects)
            {
                character = obj.GetComponent<CharControlSingle>();

                if (character != null && character.GetFaction() == faction)
                {
                    return true;
                }
            }

            return false;
        }

        public GameObject SearchForCharOfFaction(G.Faction faction)
        {
            CharControlSingle character;

            foreach (GameObject obj in activeObjects)
            {
                character = obj.GetComponent<CharControlSingle>();

                if (character != null && character.GetFaction() == faction)
                {
                    return obj;
                }
            }

            return null;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            activeObjects.Add(col.gameObject);
        }

        void OnTriggerExit2D(Collider2D col)
        {
            activeObjects.Remove(col.gameObject);
        }
    }
}
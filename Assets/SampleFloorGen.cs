using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleFloorGen : MonoBehaviour
{
    [SerializeField]
    int width = 10;

    [SerializeField]
    int height = 10;

    [SerializeField]
    Sprite primary;

    [SerializeField]
    Sprite secondary;

    GameObject go;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        for (float i = -1 * (height / 2); i <= height / 2; i += 0.5f)
        {
            for (float j = -1 * (width / 2); j <= width / 2; ++j)
            {
                go = new GameObject("FloorTile1");
                sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = primary;
                go.transform.Translate(j, i, 0.5f);
                go.transform.localScale = new Vector3(1.65f, 1.65f, 1f);
            }
        }

        for (float i = -1 * (height / 2) - 0.25f; i < (height / 2) + 0.5f; i += 0.25f)
        {
            for (float j = -1 * (width / 2) + 0.5f; j <= (width / 2) - 0.5f; ++j)
            {
                go = new GameObject("FloorTile2");
                sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = secondary;
                go.transform.Translate(j, i, 0.5f);
                go.transform.localScale = new Vector3(1.65f, 1.65f, 1f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Tubes : MonoBehaviour
{
    [Header("Tubes")]
    public GameObject tubePrefab;
    public List<GameObject> tubes;

    private float tubesDistance = 1.5f;
    private GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (gm.gameBegun)
            foreach (var tube in tubes.ToArray())
            {
                if (tube != null)
                {
                    tube.transform.position += -transform.right * gm.slideSpeed * Time.deltaTime;
                    if (tube.transform.position.x < -1.7)
                    {
                        tubes.Remove(tube);
                        Destroy(tube);
                        InstantianeTube(tube);
                    }
                }
            }
    }

    void InstantianeTube(GameObject tube)
    {
        GameObject inst = Instantiate(tubePrefab, new Vector3(tube.transform.position.x + 3 * tubesDistance, Random.Range(-1.0f, 0.6f), 0), Quaternion.identity);
        tubes.Add(inst);
    }
}

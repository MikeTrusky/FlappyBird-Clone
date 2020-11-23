using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int points = 0;
    public float slideSpeed = 1.0f;
    public bool gameBegun = false;
    public bool dead = false;
    public GameObject tubesManager;

    private AudioSource audiosource;

    void Start()
    {
        audiosource = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
            if (tubesManager.GetComponent<Tubes>().tubes[0].transform.position.x <= -0.8 && !tubesManager.GetComponent<Tubes>().tubes[0].GetComponent<Tube>().counted)
            {
                tubesManager.GetComponent<Tubes>().tubes[0].GetComponent<Tube>().counted = true;
                points += 1;
                audiosource.Play();
            }
    }
}

using UnityEngine;

public class CameraMovementSound : MonoBehaviour
{
    public AudioSource audioSource;   // Assign in inspector
    public AudioClip moveSound;       // Assign in inspector

    public float movementThreshold = 0.01f; // Minimum movement needed
    public float soundCooldown = 0.2f;      // Time between sound plays

    private Vector3 lastPosition;
    private float lastSoundTime;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        float movement = Vector3.Distance(transform.position, lastPosition);

        if (movement > movementThreshold)
        {
            if (Time.time > lastSoundTime + soundCooldown)
            {
                audioSource.PlayOneShot(moveSound);
                lastSoundTime = Time.time;
            }
        }

        lastPosition = transform.position;
    }
}

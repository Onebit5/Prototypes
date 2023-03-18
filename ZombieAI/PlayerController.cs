using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 6.0f;
    public float runSpeed = 12.0f;
    public Rigidbody rb;
    public AudioClip[] walkSounds;
    public AudioClip[] runSounds;
    public AudioSource audioSource;
    public float mouseSensitivity = 100.0f;
    public Transform playerBody;
    public Camera playerCamera;
    public float maxRotationX = 60.0f;

    private bool isRunning = false;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxRotationX, maxRotationX);

        yRotation += mouseX;

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        playerBody.rotation = Quaternion.Euler(0.0f, yRotation, 0.0f);
        rb.transform.rotation = Quaternion.Euler(0.0f, yRotation, 0.0f);

        Vector3 moveDirection = new Vector3(horizontal, 0.0f, vertical);
        moveDirection = transform.TransformDirection(moveDirection);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
            moveDirection *= runSpeed;
        }
        else
        {
            isRunning = false;
            moveDirection *= speed;
        }

  
        if (rb.velocity.magnitude > 0.1f)
        {
            if (!audioSource.isPlaying || audioSource.clip != GetCurrentAudioClip())
            {
                audioSource.clip = GetCurrentAudioClip();
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }

        rb.MovePosition(rb.position + moveDirection * Time.deltaTime);

        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, isRunning ? runSpeed : speed);

    }

    private AudioClip GetCurrentAudioClip()
    {
        if (isRunning)
        {
            return runSounds[Random.Range(0, runSounds.Length)];
        }
        else
        {
            return walkSounds[Random.Range(0, walkSounds.Length)];
        }
    }
}

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera playerCam;

    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float jumpPower = 0f;
    public float gravity = 10f;

    public float lookSpeed = 2f;
    public float lookXLimit = 75f;
    public float cameraRotationSmooth = 5f;

    public AudioClip[] woodFootstepSounds;
    public AudioClip[] tileFootstepSounds;
    public AudioClip[] carpetFootstepSounds;
    public Transform footstepAudioPosition;
    public AudioSource audioSource;

    private bool isWalking = false;
    private bool isFootstepCoroutineRunning = false;
    private AudioClip[] currentFootstepSounds;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float rotationY = 0;

    public int ZoomFOV = 35;
    public int initialFOV;
    public float cameraZoomSmooth = 1;

    private bool isZoomed = false;
    private bool canMove = true;

    CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentFootstepSounds = woodFootstepSounds;

        if (playerCam != null && initialFOV <= 0)
            initialFOV = Mathf.RoundToInt(playerCam.fieldOfView);
    }

    void Update()
    {
        if (characterController == null)
            return;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float speed = isRunning ? runSpeed : walkSpeed;

        float curSpeedX = canMove
            ? speed * Input.GetAxis("Vertical")
            : 0f;

        float curSpeedY = canMove
            ? speed * Input.GetAxis("Horizontal")
            : 0f;

        float movementDirectionY = moveDirection.y;

        Vector3 horizontalMovement =
            (forward * curSpeedX) + (right * curSpeedY);

        horizontalMovement = Vector3.ClampMagnitude(
            horizontalMovement,
            Mathf.Max(0f, speed)
        );

        moveDirection = horizontalMovement;

        if (Input.GetButton("Jump") && canMove &&
            characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove && playerCam != null)
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            rotationY += Input.GetAxis("Mouse X") * lookSpeed;

            Quaternion targetRotationX = Quaternion.Euler(rotationX, 0, 0);
            Quaternion targetRotationY = Quaternion.Euler(0, rotationY, 0);

            playerCam.transform.localRotation = Quaternion.Slerp(
                playerCam.transform.localRotation,
                targetRotationX,
                Time.deltaTime * cameraRotationSmooth
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotationY,
                Time.deltaTime * cameraRotationSmooth
            );
        }

        if (Input.GetButtonDown("Fire2"))
            isZoomed = true;

        if (Input.GetButtonUp("Fire2"))
            isZoomed = false;

        if (playerCam != null)
        {
            playerCam.fieldOfView = Mathf.Lerp(
                playerCam.fieldOfView,
                isZoomed ? ZoomFOV : initialFOV,
                Time.deltaTime * cameraZoomSmooth
            );
        }

        bool moving = curSpeedX != 0f || curSpeedY != 0f;
        isWalking = moving;

        if (moving && !isFootstepCoroutineRunning)
        {
            float footstepDelay = 1.3f / Mathf.Max(0.01f, speed);
            StartCoroutine(PlayFootstepSounds(footstepDelay));
        }
    }

    IEnumerator PlayFootstepSounds(float footstepDelay)
    {
        isFootstepCoroutineRunning = true;

        while (isWalking)
        {
            if (currentFootstepSounds == null ||
                currentFootstepSounds.Length == 0 ||
                audioSource == null ||
                footstepAudioPosition == null)
            {
                break;
            }

            int randomIndex = Random.Range(
                0,
                currentFootstepSounds.Length
            );

            AudioClip footstepClip = currentFootstepSounds[randomIndex];

            if (footstepClip != null)
            {
                audioSource.transform.position = footstepAudioPosition.position;
                audioSource.clip = footstepClip;
                audioSource.Play();
            }

            yield return new WaitForSeconds(footstepDelay);
        }

        isFootstepCoroutineRunning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wood"))
            currentFootstepSounds = woodFootstepSounds;
        else if (other.CompareTag("Tile"))
            currentFootstepSounds = tileFootstepSounds;
        else if (other.CompareTag("Carpet"))
            currentFootstepSounds = carpetFootstepSounds;
    }
}
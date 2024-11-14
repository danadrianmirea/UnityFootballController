using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject crosshair;
    public GameObject ball; // Reference to the ball GameObject
    public Transform player;               // Reference to the player's transform
    public Vector3 offset = new Vector3(0, 30, -30); // Offset from the player
    public float positionSmoothSpeed = 0.1f; // Smooth speed for position transitions
    public float rotationSmoothSpeed = 0.1f; // Smooth speed for rotation transitions
    public float lookSpeedX = 3f;  // Mouse look speed for X axis (horizontal)
    public float lookSpeedY = 3f;  // Mouse look speed for Y axis (vertical)
    public float resetDistance = 5.0f;

    public float controlBallForceMultiplier = 10f; // Multiplier to control the kick force
    public float kickForceMultiplier = 500f; // Multiplier to control the kick force
    public float kickDistanceThreshold = 5f; // Distance threshold to allow the player to kick the ball

    private float currentRotationX = 0f; // Current rotation around the X axis
    private float currentRotationY = 0f; // Current rotation around the Y axis
    private bool isRotationEnabled = false; // Flag to track whether camera rotation is enabled
    private bool isFirstPerson = true; // Flag to track whether the camera is in first-person mode
    private Rigidbody ballRb;

    private void Start()
    {
        ballRb = ball.GetComponent<Rigidbody>();
        // Initialize currentRotationX and currentRotationY based on the current camera rotation
        currentRotationX = transform.eulerAngles.x;
        currentRotationY = transform.eulerAngles.y;

        //isRotationEnabled = true;

        if (Application.isEditor == false)
        {
            EnableCameraRotation();
        }

        //Invoke("EnableCameraRotation", 0.5f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isFirstPerson = !isFirstPerson;
        }
        player.GetComponent<Renderer>().enabled = !isFirstPerson;


        if (Input.GetKeyDown(KeyCode.Q)) // kick ball slightly forward to move with it
        {
            float distanceToBall = Vector3.Distance(player.transform.position, ball.transform.position);

            if (distanceToBall <= kickDistanceThreshold)
            {
                ballRb.velocity = transform.forward * controlBallForceMultiplier;
            }
        }
        else if (Input.GetKeyDown(KeyCode.R)) // debug: reset ball
        {
            ball.transform.position = player.transform.position + resetDistance * player.transform.forward;
            ballRb.velocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }
        else if (Input.GetKeyDown(KeyCode.E)) // stop ball
        {
            float distanceToBall = Vector3.Distance(player.transform.position, ball.transform.position);

            if (distanceToBall <= kickDistanceThreshold)
            {
                ballRb.velocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }
        }

        //if (Input.GetMouseButtonDown(1)) // 1 is the right mouse button
        //{
          //  KickBall();
            //isFirstPerson = true;
            //ToggleCrosshair(true);
        //}
        //else if (Input.GetMouseButtonUp(1)) // Right mouse button released
        //{
            //isFirstPerson = false;
            //ToggleCrosshair(false);
            //KickBall();
        //}

        // If camera rotation is enabled, handle the mouse look
        if (isRotationEnabled)
        {
            HandleMouseLook();
        }

        // Always follow the player (smooth position interpolation)
        FollowPlayer();

        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (isRotationEnabled == true)
                {
                    DisableCameraRotation();
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            if (isRotationEnabled == false)
            {
                EnableCameraRotation();
            }
            else
            {
                KickBall();
            }
        }
    }

    // Camera follows the player with smooth position and rotation interpolation
    private void FollowPlayer()
    {
        if (isFirstPerson)
        {
            // In first-person mode, position the camera at the player's position
            transform.position = player.position;

            // Keep the camera rotating based on currentRotationX and currentRotationY
            Quaternion targetRotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);
            transform.rotation = targetRotation;

            // Set the player's rotation to match the camera's Y rotation
            player.rotation = Quaternion.Euler(0, currentRotationY, 0f); // Y-axis only rotation
        }
        else
        {
            // In third-person mode, calculate the desired position based on player's position and offset
            Vector3 desiredPosition = player.position + player.rotation * offset;

            // Smoothly interpolate the camera's position
            transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed);

            // Smoothly rotate to look at the player
            Quaternion desiredRotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed);

            // Sync player's rotation to camera's Y rotation
            player.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); // Match Y-axis rotation only
        }
    }

    // Handle mouse look based on mouse input
    private void HandleMouseLook()
    {
        // Get the mouse input for the X and Y axes
        float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeedY;

        // Update the current rotation based on the mouse input
        currentRotationX -= mouseY;
        currentRotationX = Mathf.Clamp(currentRotationX, -60f, 60f); // Clamp vertical rotation to avoid flipping

        currentRotationY += mouseX;

        if (isFirstPerson)
        {
            // In first-person mode, the camera should freely rotate with the mouse input
            // No need to interpolate, as it's first-person (direct camera rotation)
            transform.rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);

            // Set the player's rotation to match the camera's Y rotation
            player.rotation = Quaternion.Euler(0, currentRotationY, 0f); // Y-axis only rotation
        }
        else
        {
            // In third-person mode, update the camera's rotation based on mouse input
            // The camera will rotate horizontally (Y axis) and vertically (X axis) based on mouse input
            currentRotationY += mouseX; // Y-axis rotation
            currentRotationX -= mouseY; // X-axis rotation
            currentRotationX = Mathf.Clamp(currentRotationX, -60f, 60f); // Clamp vertical rotation to avoid flipping

            // Update camera's rotation in third-person mode
            transform.rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);

            // Sync player's rotation to camera's Y rotation only in third-person
            player.rotation = Quaternion.Euler(0, currentRotationY, 0f); // Y-axis only rotation
        }
    }

    // Enable camera rotation and lock the cursor
    private void EnableCameraRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isRotationEnabled = true;
    }

    // Disable camera rotation and unlock the cursor
    private void DisableCameraRotation()
    {
        isRotationEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void ToggleCrosshair(bool show)
    {
        crosshair.SetActive(show);
    }

    private void KickBall()
    {
        float distanceToBall = Vector3.Distance(player.transform.position, ball.transform.position);

        if (distanceToBall <= kickDistanceThreshold)
        {
            ball.transform.forward = transform.forward;
            ballRb.velocity = transform.forward * kickForceMultiplier;
        }
    }
}

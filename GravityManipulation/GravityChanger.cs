using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    public float gravityStrength = 9.81f;
    public GameObject player;
    public float rotationSpeed = 2.0f;

    private GravityDirection currentDirection;
    private Quaternion targetRotation;

    private enum GravityDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    private void Update()
    {
        float adjustedGravityStrength = gravityStrength / Time.timeScale;
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentDirection != GravityDirection.Up)
        {
            Physics.gravity = new Vector3(0, adjustedGravityStrength, 0);
            currentDirection = GravityDirection.Up;
            SetTargetRotation(0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && currentDirection != GravityDirection.Down)
        {
            Physics.gravity = new Vector3(0, -adjustedGravityStrength, 0);
            currentDirection = GravityDirection.Down;
            SetTargetRotation(180);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentDirection != GravityDirection.Left)
        {
            Physics.gravity = new Vector3(-adjustedGravityStrength, 0, 0);
            currentDirection = GravityDirection.Left;
            SetTargetRotation(-90);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && currentDirection != GravityDirection.Right)
        {
            Physics.gravity = new Vector3(adjustedGravityStrength, 0, 0);
            currentDirection = GravityDirection.Right;
            SetTargetRotation(90);
        }
    }

    private void FixedUpdate()
    {
        RotatePlayer();
    }

    private void SetTargetRotation(float targetZRotation)
    {
        targetRotation = Quaternion.Euler(0, 0, targetZRotation);
    }

    private void RotatePlayer()
    {
        if (player.transform.rotation != targetRotation)
        {
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}

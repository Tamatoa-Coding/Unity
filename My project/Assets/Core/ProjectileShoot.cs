using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float lookSpeed = 3f;
    public float fireCooldown = 0.5f;
    private float nextFireTime = 0f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private float rotationY = 0;
    private CharacterController characterController;

    private bool canMove = true;

    public float walkSpeed = 2f;
    public float sprintSpeed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (canMove)
        {
            // Rotation verticale (caméra haut/bas)
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            // Enlever la limite de la rotation verticale
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Rotation horizontale (joueur gauche/droite)
            rotationY += Input.GetAxis("Mouse X") * lookSpeed;
            // Enlever la limite de la rotation horizontale
            transform.rotation = Quaternion.Euler(0, rotationY, 0);

            // Vérification si le joueur est au sol
            isGrounded = characterController.isGrounded;

            // Gravité et mouvement
            float moveDirectionY = velocity.y;
            velocity = new Vector3(0, moveDirectionY, 0);
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            // Déplacement horizontal avec les touches WASD
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 move = transform.right * moveX + transform.forward * moveZ;

            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

            characterController.Move(move * currentSpeed * Time.deltaTime);

            // Appliquer la gravité
            velocity.y += gravity * Time.deltaTime;

            characterController.Move(velocity * Time.deltaTime);

            // Gestion du saut
            if (isGrounded && Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Tirer un projectile
            if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
            {
                FireProjectile();
                nextFireTime = Time.time + fireCooldown;
            }
        }
    }

    void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = playerCamera.transform.forward;

            rb.AddForce(direction * 10f, ForceMode.Impulse);
        }
    }
}

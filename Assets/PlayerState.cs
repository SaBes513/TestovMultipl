using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerState : NetworkBehaviour
{
    enum Status
    {
        Dash, Damage, Move
    }
    public float MaxSpeed = 7, MouseSensitivity = 2f, DashDistance = 2, DashColldown = 3, ImmunityTime = 3;
    public MeshRenderer Mesh;
    public Material[] MatMovDasDam;
    float DashTemp;
    Vector3 velocity;
    Status status;
    Rigidbody ThisRB;
    bool menu; 
    internal bool PlayMath = true;

    [Header("Camera")]
    [SerializeField]
    private float moveSmooth = 20f;
    Transform cameraTransformV;
    Transform cameraTransformH;
    float cameraEulerV = 25f;
    float cameraEulerH;


    void CameraRotation()
    {
        cameraEulerH = Mathf.Repeat(cameraEulerH + Input.GetAxis("Mouse X") * MouseSensitivity, 360f);
        cameraEulerV = Mathf.Clamp(cameraEulerV - Input.GetAxis("Mouse Y") * MouseSensitivity, -80f, 89f);
        cameraTransformV.localEulerAngles = new Vector3(cameraEulerV, 0, 0);
        cameraTransformH.eulerAngles = new Vector3(0, cameraEulerH, 0);
    }

    void CameraMove()
    {
        cameraTransformH.position = Vector3.Lerp(cameraTransformH.position, transform.position, moveSmooth * Time.deltaTime);
    }

    private void Awake()
    {
        cameraTransformV = Camera.main.transform.parent;
        cameraTransformH = cameraTransformV.parent;
    }

    void Start()
    {
        if (isOwned)
        {
            UIManager.main.Menu.gameObject.SetActive(menu);
            Cursor.lockState = !menu ? CursorLockMode.Locked : CursorLockMode.Confined;
        }
        ThisRB = GetComponent<Rigidbody>();
        status = Status.Move;
    }

    private void FixedUpdate()
    {
        if (isOwned && !menu && status == Status.Move && NetManager.singleton._GameIsActive)
            Movement();
    }

    private void Update()
    {
        if (isOwned && Input.GetButtonDown("Cancel")) { ToggleMenu(); }
        if (isOwned && !menu) MovementLogic();
    }

    private void LateUpdate()
    {
        if (isOwned && !menu && NetManager.singleton._GameIsActive)
        {
            ViewLogic();
        }
    }

    private void ViewLogic()
    {
        CameraRotation();
        CameraMove();
    }

    private void MovementLogic()
    {
        velocity.x = Input.GetAxis("Horizontal");
        velocity.z = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Fire1") && velocity != Vector3.zero && status == Status.Move && DashTemp < Time.time)
        {
            Dash();
        }
    }

    void Dash()
    {
        status = Status.Dash;
        ThisRB.velocity = ThisRB.velocity.normalized * DashDistance * 5;
        Mesh.material = MatMovDasDam[1];
        CmdDash(transform.position, transform.forward);
        DashTemp = Time.time + DashColldown;
    }

    [Command]
    private void CmdDash(Vector3 position, Vector3 forward)
    {
        RpcDash(position, forward);
    }
    
    [ClientRpc]
    private void RpcDash(Vector3 position, Vector3 forward)
    {
        transform.position = position;
        transform.forward = forward;
        status = Status.Dash;
        Mesh.material = MatMovDasDam[1];
        Invoke("DefaultStatus", .2f);
    }

    void Movement()
    {
        transform.rotation = cameraTransformH.rotation;
        if (velocity != Vector3.zero)
        {
            ThisRB.velocity = Vector3.ClampMagnitude((transform.forward * velocity.z + transform.right * velocity.x),1) * MaxSpeed;
            if (ThisRB.velocity.magnitude > MaxSpeed)
            {
                ThisRB.velocity = ThisRB.velocity.normalized * MaxSpeed;
            }
        }
    }

    void DefaultStatus()
    {
        Mesh.material = MatMovDasDam[0];
        status = Status.Move;
        if (ThisRB.velocity.magnitude > MaxSpeed)
            ThisRB.velocity = ThisRB.velocity.normalized * MaxSpeed;
    }

    [ClientRpc]
    public void RpcDamage()
    {
        
        Mesh.material = MatMovDasDam[2];
        Invoke("DefaultStatus", ImmunityTime);

    }

    [Server]
    private void OnCollisionEnter(Collision collision)
    {
        if (status == Status.Dash && collision.gameObject.layer == 9)
        {
            collision.collider.GetComponent<PlayerState>().RpcDamage();
            Player temp = netIdentity.connectionToClient.identity.GetComponent<Player>();
            GameManager.main.AddScore(temp);
        }
    }

    [ClientRpc]
    public void RpcTransp(Vector3 pos)
    {
        transform.position = pos;
    }

    void ToggleMenu()
    {
        menu = !menu;
        Cursor.lockState = !menu ? CursorLockMode.Locked : CursorLockMode.Confined;
        UIManager.main.Menu.gameObject.SetActive(menu);
    }
}

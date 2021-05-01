using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public Camera myCam;
    public AudioListener myAudioListener;
    public GameObject myCanvas;
    public LayerMask groundMask;
    public LayerMask rayMask;
    public Transform MeleeRangeCheck;
    public float gunRange = 500f;
    public GameObject towerPrefab;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public ParticleSystem gunParticle;
    public CharacterController controller;
    public Transform groundCheck;
    public Transform playerBody;
    public Transform muzzle;
    public int gunDamage  = 10;
    public LineRenderer shot;
    public GameObject pauseMenu;
    public GameObject scoreBoard;
    private NetworkManager networkManager;
    public GameObject crosshair;


    float currentSpeed = 5f;
    bool isGrounded;
    bool constructionMode = false;
    Vector3 velocity;
    float gravity = -19.62f;
    float jumpHeight = 2f;
    float shotWidth = 0.1f;
    float shotDuration = 1f;

    private void Start()
    {
        pauseMenu.SetActive(false);
        scoreBoard.SetActive(false);
        crosshair.SetActive(true);
        networkManager = NetworkManager.singleton;
    }

    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);  //Check if the player is on the ground (prevent infinite jumping)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        //Change the state of the cursor
        if (Input.GetButtonDown("Cursor"))
        {
            ChangeCursorLockState();
        }
        if (Input.GetButtonDown("TCM"))
        {
            constructionMode = !constructionMode;
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {

            /*____________________________MOUSE CAMERA________________________________*/

            myCam.GetComponent<CameraBis>().UpdateCamera();  //Update camera and capsule rotation

            /*_____________________________MOVEMENTS____________________________________*/

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * currentSpeed * Time.deltaTime);

            //Run command
            if (Input.GetButtonDown("Run") && isGrounded)
            {
                ChangeSpeed();
            }

            //Reset gravity 
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            //Jump command
            if(Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }


            /*_______________________________PEW PEW_____________________________*/

            //Fire command
            if (Input.GetButtonDown("Fire1"))
            {
                if (constructionMode)
                {
                    CmdBuild();
                }
                else
                {
                    shot.SetPosition(0, muzzle.position);
                    CmdTryShoot(muzzle.position, Camera.main.transform.forward, gunRange);
                    shot.enabled = true;
                    shotDuration = 1;
                }
            }

            if (Input.GetButtonDown("Fire2"))
            {
                if (constructionMode)
                {
                    CmdDestroy();
                }
                else
                {
                    shot.SetPosition(0, muzzle.position);
                    CmdTryShoot(muzzle.position, Camera.main.transform.forward, gunRange);
                    shot.enabled = true;
                    shotDuration = 1;
                }
            }
           /* if (Input.GetButtonDown("Fire1"))
            {
                shot.SetPosition(0, muzzle.position);
                CmdTryShoot(muzzle.position,Camera.main.transform.forward, gunRange); //Appel du serveur pour faire apparaitre Raycat + LineRenderer
                shot.enabled = true;
                shotDuration = 1;
                //CmdFire();
            }*/
            shotDuration = shotDuration > 0f ? shotDuration - Time.deltaTime : 0f; //Efface le LineRenderer au bout d'une seconde 
            shot.enabled = !(shotDuration == 0f);

        }
    }

    //Change lock state of cursor
   public void ChangeCursorLockState()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;   //Masque la souris quand le curseur est vérouillé
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        crosshair.SetActive(!crosshair.activeSelf);
    }

    //Change movement speed
    void ChangeSpeed()
    {
        if(currentSpeed == 5f)
        {
            currentSpeed = 8f;
        }
        else
        {
            currentSpeed = 5f;
        }
    }

    // Client --> Server

    [Command]
    void CmdTryShoot(Vector3 origin, Vector3 direction, float range)
    {
        // Création d'un raycast et d'un LineRenderer simultanément
        if (!gunParticle.isPlaying)
        {
            RpcStartParticles();
        }
        Ray ray = new Ray(origin, direction);
        Vector3 endPosition = origin + (range * direction);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,range,rayMask))
        {
            endPosition = hit.point;
            if(hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Health>().TakeDamage(20);
            }
        }
        
        shot.SetPosition(1, endPosition);
    }

    [Command]
    void CmdBuild()
    {
        GameObject aimed = getAimingObject();
        if (aimed != null && aimed.tag == "TurretSpawnPoints")
        {
            aimed.GetComponent<TurretSpawning>().upgrade = true;
        }
    }

    [Command]
    void CmdDestroy()
    {
        GameObject aimed = getAimingObject();
        if (aimed != null && aimed.tag == "TurretSpawnPoints")
        {
            aimed.GetComponent<TurretSpawning>().destroy = true;
        }
    }

    //Server --> Client
    [ClientRpc]
    //Both next functions : Start playing gun particles
    public void RpcStartParticles(){
        StartParticles();
    }

    public void StartParticles(){
        gunParticle.Play();
    }

    //Enable camera and audioListener on connection of the player
    public override void OnStartLocalPlayer(){

        GetComponent<MeshRenderer>().material.color = Color.blue;
        if(!myCam.enabled || !myAudioListener.enabled || !myCanvas){
            myCanvas.gameObject.SetActive(true);
            myCam.enabled = true;
            myAudioListener.enabled = true;
        }
        Vector3[] initShotPosition = new Vector3[2] { Vector3.zero, Vector3.zero };
        shot.SetPositions(initShotPosition);
        shot.startWidth = shotWidth;
        shot.endWidth = shotWidth;
    }

    public GameObject getAimingObject()
    {
        Ray ray = new Ray(myCam.transform.position, myCam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 7.5f))
        {
            return hit.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    public void OnClickButton()
    {   
        
        SceneManager.LoadScene("MainMenu");
        
    }

    public void OnMainMenu()
    {
        if (isClientOnly)
        {
            networkManager.StopClient();
        }
        else
        {
            networkManager.StopHost();
        }
        SceneManager.LoadScene("MainMenu");
    }

}

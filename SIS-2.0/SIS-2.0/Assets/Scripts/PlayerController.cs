using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    //Player related variables
    public CharacterController controller;
    public Transform groundCheck;
    public Transform playerBody;
    public LayerMask groundMask;
    public GameObject towerPrefab;
    public Camera myCam;
    public AudioListener myAudioListener;

    bool isGrounded;
    Vector3 velocity;
    float gravity = -19.62f;
    float jumpHeight = 2f;


    //Interface & Sound related variables
    public GameObject myCanvas;
    public AudioSource gunSource;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject commandsMenu;
    public GameObject scoreBoard;
    public GameObject sureMenu;
    private NetworkManager networkManager;
    public GameObject crosshair;
    public AudioClip[] soundArray;   //All sounds we can invoke in the game


    //Gun related variables
    public Animator animator; //Reload animation currently used (depend on weapon)
    public ParticleSystem gunParticle;
    public LayerMask rayMask; //Layer the raycat registers when shooting a gun
    public int maxMunitions; //Weapon's ammuntions clip's size
    
    [SyncVar(hook = "OnStockChanged")]
    public int munitions; //Ammunition the player currently has

    public float gunRange;
    public Transform muzzle;
    public GameObject holster;
    public GameObject weapon;
    public GameObject[] holsterArray;
    public Transform holsterTransform;
    public Text UImunitions;
    public Text UIstock;

    public NetworkAnimator networkAnimator;

    [SyncVar(hook = "OnWeaponChanged")]
    public int activeWeapon = 0;

    bool constructionMode;
    bool isReloading;
    bool canShoot;
    float currentSpeed;
    float reloadTime;
    float fireRate;
    int gunDamage;
    [SyncVar(hook = "OnAmmoChanged")]
    int nbMunitions; //Ammunitions currently in the gun chamber
    int indexWeapon;


    private void Start()
    {
        //Initialize all variables
        soundArray = new AudioClip[] {
            Resources.Load("EmptyGun") as AudioClip,
            Resources.Load("GunFire") as AudioClip,
            Resources.Load("Pick up") as AudioClip,
            Resources.Load("Reload") as AudioClip
        };
        constructionMode = false;
        currentSpeed = 5f;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        commandsMenu.SetActive(false);
        sureMenu.SetActive(false);
        scoreBoard.SetActive(false);

        networkManager = NetworkManager.singleton;
        /*weapon = holsterArray[0];
        ChangeWeaponStats(0);*/
        indexWeapon = 0;
        canShoot = true;
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
        if (Input.GetButtonDown("Cursor") && !pauseMenu.activeSelf && !settingsMenu.activeSelf && !commandsMenu.activeSelf && !sureMenu.activeSelf)
        {
            ChangeCursorLockState();
        }
        //Change ShootMode into ConstructionMode and vice-versa
        if (Input.GetButtonDown("TCM"))
        {
            constructionMode = !constructionMode;
        }
        //Following instructions executed only if the player isn't in a menu
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

            //Reset gravity to keep constant velocity
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

            if (Input.GetButtonDown("Fire1")){
                if (constructionMode){
                    CmdBuild(); //Build or upgrade a turret/trap
                }
                else{
                    if(!isReloading){
                        if (nbMunitions > 0 && canShoot){
                            StartCoroutine(Shoot());
                            nbMunitions--;                            
                        }
                        else{
                            if (nbMunitions <= 0 && canShoot){
                                gunSource.clip = soundArray[0];
                                gunSource.Play();
                            }
                        }
                    }
                }
            }

            if (Input.GetButtonDown("Fire2") && constructionMode)
            {
                CmdDestroy();
            }

            if(Input.GetButtonDown("Reload"))
            {
                if(munitions > 0 && nbMunitions < maxMunitions) //if gun isn't full and player have ammunations left, reload gun
                {
                    StartCoroutine(Reload());
                    return;
                }
            }


            if(Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                //If user scroll up, change equipped weapon
                indexWeapon = indexWeapon == 1 ? 0 : indexWeapon + 1;
                CmdChangeActiveWeapon(indexWeapon);
                ChangeWeaponStats(indexWeapon);
            }

            
            

            /*____________________________SCOREBOARD_____________________________*/

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                scoreBoard.SetActive(!scoreBoard.activeSelf);
            }

        }
    }

    public void OnAmmoChanged(int _old, int _new)
    {
        UImunitions.text = $"{nbMunitions} / {maxMunitions} ";
    }

    public void OnStockChanged(int _old, int _new)
    {
        UIstock.text = $"Stock: {munitions}";
    }

    //Activate new equipped weapon and deactivate the previous one
    public void OnWeaponChanged(int _old, int _new)
    {
        if(_old >= 0 && _old < holsterArray.Length && holsterArray[_old] != null)
        {
            holsterArray[_old].SetActive(false);
        }
        if (_new >= 0 && _new < holsterArray.Length && holsterArray[_new] != null)
        {
            holsterArray[_new].SetActive(true);
        }
    }

    //Synchronize new weapon on server
    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        activeWeapon = newIndex;
    }

    //Update weapon characteristics to those of the new weapon
    public void ChangeWeaponStats(int index)
    {
        var weapon = holsterArray[index];
        gunDamage = weapon.GetComponent<WeaponCharacteristics>().damage;
        reloadTime = weapon.GetComponent<WeaponCharacteristics>().reloadSpeed;
        maxMunitions = weapon.GetComponent<WeaponCharacteristics>().munitions;
        fireRate = weapon.GetComponent<WeaponCharacteristics>().fireRate;
        animator = weapon.GetComponent<WeaponCharacteristics>().animator;
        networkAnimator.animator = weapon.GetComponent<WeaponCharacteristics>().animator;
        if (nbMunitions >= maxMunitions)
        {
            nbMunitions -= maxMunitions;
            munitions += nbMunitions - maxMunitions;
        }
        else
        {
            if(munitions >= maxMunitions - nbMunitions)
            {
                nbMunitions = maxMunitions;
                munitions -= maxMunitions - nbMunitions;
            }
            else
            {
                nbMunitions += munitions;
                munitions = 0;
            }
        }
    }

    //Change lock state of cursor
   public void ChangeCursorLockState()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;   //Masque la souris quand le curseur est vérouillé (Not working because of a Unity bug)
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        pauseMenu.SetActive(!pauseMenu.activeSelf);
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

    void UpdateMunitions(bool isClipEmpty, bool canFullLoad)
    {
        if(isClipEmpty)
        {
            if(canFullLoad)
            {
                nbMunitions = maxMunitions;
                munitions -= maxMunitions;
            }
            else
            {
                nbMunitions = munitions;
                munitions = 0;
            }
        }
        else
        {
            if(canFullLoad)
            {
                munitions -= maxMunitions - nbMunitions;
                nbMunitions = maxMunitions;
            }
            else
            {
                if(maxMunitions-nbMunitions >= munitions)
                {
                    nbMunitions += munitions;
                    munitions = 0;
                }
                else
                {
                    munitions -= maxMunitions - nbMunitions;
                    nbMunitions = maxMunitions;
                }
            }
        }
    }

    //Reloading function
    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("isReloading", true);
        gunSource.clip = soundArray[3];
        gunSource.Play();

        yield return new WaitForSeconds(reloadTime);

        UpdateMunitions(nbMunitions == 0, munitions >= maxMunitions);
        animator.SetBool("isReloading", false);
        isReloading = false;
    }

    IEnumerator Shoot()
    {
        canShoot = false;
        CmdTryShoot(myCam.transform.position, myCam.transform.forward, gunRange);

        yield return new WaitForSecondsRealtime(fireRate);

        canShoot = true;
    }

    // Client --> Server
    //Try shooting a ray between gun muzzle and a point in front of the camera
    [Command]
    void CmdTryShoot(Vector3 origin, Vector3 direction, float range)
    {
        // Création d'un raycast
        if (!gunParticle.isPlaying)
        {
            RpcStartParticles();
        }
        if(gunSource.isPlaying)
        {
            gunSource.Stop();
        }
        gunSource.volume = PlayerPrefs.GetFloat("Effects");
        gunSource.clip = soundArray[1];
        gunSource.Play();
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,range,rayMask))
        {
            if(hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Health>().TakeDamage(gunDamage);
            }
        }
    }

    //Build a turret/trap
    [Command]
    void CmdBuild()
    {
        GameObject aimed = getAimingObject();
        if (aimed != null && aimed.tag == "TurretSpawnPoints")
        {
            GetComponent<Money>().money = aimed.GetComponent<TurretSpawning>().TryUpgrade(GetComponent<Money>().money);
        }
    }

    //Destroy a turret/trap
    [Command]
    void CmdDestroy()
    {
        GameObject aimed = getAimingObject();
        if (aimed != null && aimed.tag == "TurretSpawnPoints")
        {
            GetComponent<Money>().money = aimed.GetComponent<TurretSpawning>().TryDestroy(GetComponent<Money>().money);
        }
    }

    //Server --> Client
    //Both next functions : Start playing gun particles
    [ClientRpc]
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
        gunSource.volume = PlayerPrefs.GetFloat("Effects");
        munitions = 20;
        isReloading = false;
        var weapon = holsterArray[0];
        ChangeWeaponStats(0);
        nbMunitions = maxMunitions;
        networkAnimator.animator = weapon.GetComponent<WeaponCharacteristics>().animator;
        /*UImunitions.text = $"{nbMunitions} / {maxMunitions} ";
        UIstock.text = $"Stock: {munitions}";*/
    }

    //Get the point where player is looking at
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

    //Bring back player to main menu
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

    public NetworkManager GetNetworkManager() => networkManager;

}

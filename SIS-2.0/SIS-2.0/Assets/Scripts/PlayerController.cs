﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    public Camera myCam;
    public AudioListener myAudioListener;
    public GameObject myCanvas;
    public AudioSource gunSource;
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
    public GameObject pauseMenu;
    public GameObject scoreBoard;
    private NetworkManager networkManager;
    public GameObject crosshair;
    public AudioClip[] soundArray;
    public Animator animator;
    public GameObject holster;
    public int maxMunitions; //Taille du chargeur de l'arme
    public int munitions; //Munitions en réserve


    float currentSpeed = 5f;
    int nbMunitions; //Nombre de munitions dans le chargeur
    int indexWeapon = 0;
    bool isGrounded;
    bool constructionMode = false;
    bool isReloading;
    Vector3 velocity;
    float gravity = -19.62f;
    float jumpHeight = 2f;

    private void Start()
    {
        soundArray = new AudioClip[] {
            Resources.Load("EmptyGun") as AudioClip,
            Resources.Load("GunFire") as AudioClip,
            Resources.Load("Pick up") as AudioClip,
            Resources.Load("Reload") as AudioClip
        };
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
                    if(!isReloading)
                    {
                        if (nbMunitions > 0)
                        {
                            CmdTryShoot(myCam.transform.position, myCam.transform.forward, gunRange);
                            nbMunitions--;
                        }
                        else
                        {
                            gunSource.clip = soundArray[0];
                            gunSource.Play();
                        }
                    }
                    else
                    {
                        return;
                    }

                }
            }

            if (Input.GetButtonDown("Fire2") && constructionMode)
            {
                CmdDestroy();
            }

            if(Input.GetButtonDown("Reload"))
            {
                if(munitions > 0 && nbMunitions < maxMunitions)
                {
                    StartCoroutine(Reload());
                    return;
                }
            }

            if(Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                indexWeapon = indexWeapon == 1 ? 0 : indexWeapon + 1;
                holster.GetComponent<WeaponSwitching>().SelectWeapon(indexWeapon);
            }
           

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

    IEnumerator Reload()
    {
        isReloading = true;
        animator.SetBool("isReloading", true);
        gunSource.clip = soundArray[3];
        gunSource.Play();

        yield return new WaitForSeconds(1f);

        nbMunitions = munitions > maxMunitions ? maxMunitions : munitions;
        munitions -= nbMunitions;
        animator.SetBool("isReloading", false);
        isReloading = false;
    }

    // Client --> Server

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
        gunSource.volume = PlayerPrefs.GetFloat("Effects");
        nbMunitions = maxMunitions;
        munitions = 20;
        isReloading = false;
        Debug.Log(munitions);
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

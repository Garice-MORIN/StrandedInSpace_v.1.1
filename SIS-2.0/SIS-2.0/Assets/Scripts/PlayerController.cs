using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using System.Net;
using System;

public class PlayerController : NetworkBehaviour
{
    int startingMoney;
    [SyncVar(hook = "OnMoneyChanged")] public int money;
    public string _name;
    public int death;

    //Player related variables
    private float startRoundThree = -1f;
    public CharacterController controller;
    public EnemyKill kills;
    public EnemiesSpawner spawner;
    public StopWatch watch;
    public Transform groundCheck;
    public Transform playerBody;
    public LayerMask groundMask;
    public GameObject towerPrefab;
    private GameObject door;
    private Door doorScript;
    public Camera myCam;
    public Camera miniMapCamera;
    public AudioListener myAudioListener;
    public List<Type> killedEnemies;
    public static int score;
    public static int deltaMoney;
    public static bool win;
    public bool canWinPoints;
    public Variables state;

    public bool isGrounded;
    Vector3 velocity;
    float gravity = -19.62f;
    float jumpHeight = 2f;
    int points;
    //Interface & Sound related variables
    public GameObject myCanvas;
    public AudioSource gunSource;

    public GameObject hudTrap;
    public GameObject hudTurret;
    public Image TurretSelect1;
    public Image TurretSelect2;
    public Image TurretSelect3;
    public Image TurretSelect4;
    public Image TrapSelect1;
    public Image TrapSelect2;
    public Image TrapSelect3;
    public Image TrapSelect4;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject commandsMenu;
    public GameObject scoreBoard;
    public GameObject sureMenu;
    private NetworkManager networkManager;
    public NetworkConnection networkConnection;
    public GameObject crosshair;
    public Animator transition;
    public Text UIMoney;
    //Gun related variables
    public Animator animator; //Reload animation currently used (depend on weapon)
    public ParticleSystem gunParticle;
    public LayerMask rayMask; //Layer the raycat registers when shooting a gun
    public int maxMunitions; //Weapon's ammuntions clip's size
    [SyncVar(hook = "OnStockChanged")]
    public int munitions; //Ammunition the player currently ha
    public float gunRange;
    public Transform muzzle;
    public GameObject holster;
    public GameObject weapon;
    public GameObject[] holsterArray;
    public Transform holsterTransform;
    public Text UImunitions;
    public Text UIstock;
    public GameObject panel;
    public Text panelText;
    public Text life;
    [SyncVar(hook = "IpPanel")]
    public bool isGameLaunched;
    public NetworkAnimator networkAnimator;
    [SyncVar(hook = "OnWeaponChanged")]
    public int activeWeapon = 0;
    public NetworkManager GetNetworkManager() => networkManager;
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
    int indexPlacement;
    private void Start()
    {
        //Initialize all variables
        constructionMode = false;
        currentSpeed = 5f;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        commandsMenu.SetActive(false);
        sureMenu.SetActive(false);
        scoreBoard.SetActive(false);
        networkManager = NetworkManager.singleton;
        indexWeapon = 0;
        canShoot = true;
        isGameLaunched = false;
        networkManager.offlineScene = "MainMenu";
        killedEnemies = FindObjectOfType<EnemyKill>().killedEnemies;
        money = (int)(PlayerPrefs.GetFloat("StartingMoney") * money);
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);  //Check if the player is on the ground (prevent infinite jumping)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //Change the state of the cursor
        if (Input.GetButtonDown("Cursor") && !settingsMenu.activeSelf && !commandsMenu.activeSelf && !sureMenu.activeSelf)
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

            if (Input.GetButtonDown("StartGame"))
            {
                if (isServer && !FindObjectOfType<EnemiesSpawner>().isStarted)
                {
                    doorScript.OpenDoor();
                    panel.SetActive(false);
                }
            }

            /*____________________________MOUSE CAMERA________________________________*/

            myCam.GetComponent<CameraBis>().UpdateCamera();  //Update camera and capsule rotation

            /*_____________________________MOVEMENTS____________________________________*/

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * currentSpeed * Time.deltaTime);

            //Run command
            if (Input.GetButtonDown("Run") && isGrounded)
            {
                currentSpeed = currentSpeed == 5f ? 8f : 5f;
            }

            //Reset gravity to keep constant velocity
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            //Jump command
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }


            /*_______________________________PEW PEW_____________________________*/

            //Fire command

            if (Input.GetButtonDown("Fire1"))
            {
                if (constructionMode)
                {
                    CmdBuild(); //Build or upgrade a turret/trap
                }
                else
                {
                    if (!isReloading)
                    {
                        if (nbMunitions > 0 && canShoot)
                        {
                            StartCoroutine(Shoot());
                            nbMunitions--;
                        }
                        else
                        {
                            if (nbMunitions <= 0 && canShoot)
                            {
                                gunSource.clip = GetNetworkManager().GetComponentInParent<SpawnTable>().GetGunSound(0);
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

            if (Input.GetButtonDown("Reload"))
            {
                if (munitions > 0 && nbMunitions < maxMunitions)
                { //if gun isn't full and player have ammunations left, reload gun
                    StartCoroutine(Reload());
                    return;
                }
            }


            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                //If user scroll up
                if (constructionMode)
                {
                    //Change construction to build
                    indexPlacement = indexPlacement == 3 ? 0 : indexPlacement + 1;
                }
                else
                {
                    //Change equipped weapon
                    indexWeapon = indexWeapon == 1 ? 0 : indexWeapon + 1;
                    CmdChangeActiveWeapon(indexWeapon);
                    ChangeWeaponStats(indexWeapon);
                }

            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                //If user scroll down
                if (constructionMode)
                {
                    //Change construction to build
                    indexPlacement = indexPlacement == 0 ? 3 : indexPlacement - 1;
                }
                else
                {
                    //Change equipped weapon
                    indexWeapon = indexWeapon == 0 ? 1 : indexWeapon - 1;
                    CmdChangeActiveWeapon(indexWeapon);
                    ChangeWeaponStats(indexWeapon);
                }
            }


            /*____________________________SCOREBOARD_____________________________*/

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                scoreBoard.SetActive(!scoreBoard.activeSelf);
                Debug.Log(_name);
            }

            /*___________________________HUDforConstruction______________________*/
            if (constructionMode)
            {
                GameObject aimed = getAimingObject();
                if (aimed != null)
                {
                    if (aimed.tag == "TurretSpawnPoints")
                    {
                        hudTrap.SetActive(false);
                        hudTurret.SetActive(true);
                        switch (indexPlacement)
                        {
                            case 0:
                                TurretSelect1.gameObject.SetActive(true);
                                TurretSelect2.gameObject.SetActive(false);
                                TurretSelect3.gameObject.SetActive(false);
                                TurretSelect4.gameObject.SetActive(false);
                                break;
                            case 1:
                                TurretSelect1.gameObject.SetActive(false);
                                TurretSelect2.gameObject.SetActive(true);
                                TurretSelect3.gameObject.SetActive(false);
                                TurretSelect4.gameObject.SetActive(false);
                                break;
                            case 2:
                                TurretSelect1.gameObject.SetActive(false);
                                TurretSelect2.gameObject.SetActive(false);
                                TurretSelect3.gameObject.SetActive(true);
                                TurretSelect4.gameObject.SetActive(false);
                                break;
                            default:
                                TurretSelect1.gameObject.SetActive(false);
                                TurretSelect2.gameObject.SetActive(false);
                                TurretSelect3.gameObject.SetActive(false);
                                TurretSelect4.gameObject.SetActive(true);
                                break;
                        }
                    }
                    else if (aimed.tag == "TrapSpawnPoint")
                    {
                        hudTrap.SetActive(true);
                        hudTurret.SetActive(false);
                        switch (indexPlacement)
                        {
                            case 0:
                                TrapSelect1.gameObject.SetActive(true);
                                TrapSelect2.gameObject.SetActive(false);
                                TrapSelect3.gameObject.SetActive(false);
                                TrapSelect4.gameObject.SetActive(false);
                                break;
                            case 1:
                                TrapSelect1.gameObject.SetActive(false);
                                TrapSelect2.gameObject.SetActive(true);
                                TrapSelect3.gameObject.SetActive(false);
                                TrapSelect4.gameObject.SetActive(false);
                                break;
                            case 2:
                                TrapSelect1.gameObject.SetActive(false);
                                TrapSelect2.gameObject.SetActive(false);
                                TrapSelect3.gameObject.SetActive(true);
                                TrapSelect4.gameObject.SetActive(false);
                                break;
                            default:
                                TrapSelect1.gameObject.SetActive(false);
                                TrapSelect2.gameObject.SetActive(false);
                                TrapSelect3.gameObject.SetActive(false);
                                TrapSelect4.gameObject.SetActive(true);
                                break;
                        }
                    }
                    else
                    {
                        hudTrap.SetActive(false);
                        hudTurret.SetActive(false);
                    }
                }
                else
                {
                    hudTrap.SetActive(false);
                    hudTurret.SetActive(false);
                }
            }
            else
            {
                hudTrap.SetActive(false);
                hudTurret.SetActive(false);
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
    public void OnMoneyChanged(int _old, int _new)
    {
        UIMoney.text = $"{money}";
    }

    public void IpPanel(bool _old, bool _new)
    {
        panel.SetActive(!_new);
    }


    //Activate new equipped weapon and deactivate the previous one
    public void OnWeaponChanged(int _old, int _new)
    {
        if (_old >= 0 && _old < holsterArray.Length && holsterArray[_old] != null)
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
        nbMunitions = weapon.GetComponent<WeaponCharacteristics>().currentAmmo;
    }
    //Change lock state of cursor
    public void ChangeCursorLockState()
    {
        if (Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        hudTrap.SetActive(false);
        hudTurret.SetActive(false);
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
    void UpdateMunitions(bool isClipEmpty, bool canFullLoad)
    {
        if (isClipEmpty)
        {
            if (canFullLoad)
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
            if (canFullLoad)
            {
                munitions -= maxMunitions - nbMunitions;
                nbMunitions = maxMunitions;
            }
            else
            {
                if (maxMunitions - nbMunitions >= munitions)
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
        gunSource.clip = GetNetworkManager().GetComponentInParent<SpawnTable>().GetGunSound(3);
        gunSource.Play();
        yield return new WaitForSeconds(reloadTime);

        UpdateMunitions(nbMunitions == 0, munitions >= maxMunitions);
        var weapon = holsterArray[indexWeapon];
        weapon.GetComponent<WeaponCharacteristics>().UpdateAmmo(true, nbMunitions);
        animator.SetBool("isReloading", false);
        isReloading = false;
    }
    IEnumerator Shoot()
    {
        canShoot = false;
        CmdTryShoot(myCam.transform.position, myCam.transform.forward, gunRange);
        var weapon = holsterArray[indexWeapon];
        weapon.GetComponent<WeaponCharacteristics>().UpdateAmmo();
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
        if (gunSource.isPlaying)
        {
            gunSource.Stop();
        }
        gunSource.volume = PlayerPrefs.GetFloat("Effects");
        gunSource.clip = GetNetworkManager().GetComponentInParent<SpawnTable>().GetGunSound(1);
        gunSource.Play();
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range, rayMask))
        {
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Health>().TakeDamage(gunDamage);
            }
            else if (hit.collider.tag == "TurretSpawnPoints")
                Debug.Log("Raycast hit");
        }
    }
    //Build a turret/trap
    [Command]
    void CmdBuild()
    {
        GameObject aimed = getAimingObject();
        if (aimed != null)
        {
            if (aimed.tag == "TurretSpawnPoints" || aimed.tag == "Tower")
            {
                GameObject spawner = aimed.tag == "Tower" ? aimed.GetComponent<TurretInfo>().linkedSpawner : aimed;
                spawner.GetComponent<TurretSpawning>().upgradeDamageSent = PlayerPrefs.GetFloat("TowerDamage");
                spawner.GetComponent<TurretSpawning>().upgradeStatusSent = PlayerPrefs.GetFloat("TowerStatus");
                money -= spawner.GetComponent<TurretSpawning>().TryBuild(money, indexPlacement);
            }
            if (aimed.tag == "TrapSpawnPoint")
            {
                aimed.GetComponent<TrapSpawning>().upgradeDamageKept = PlayerPrefs.GetFloat("TrapDamage");
                aimed.GetComponent<TrapSpawning>().upgradeUsesKept = PlayerPrefs.GetFloat("TrapUses");
                money -= aimed.GetComponent<TrapSpawning>().TryBuild(money, indexPlacement);
            }
            if (aimed.tag == "BarricadeSpawnPoint")
            {
                money -= aimed.GetComponent<BarricadeSpawning>().TryBuild(money);
            }
        }
    }

    //Destroy a turret/trap
    [Command]
    void CmdDestroy()
    {
        GameObject aimed = getAimingObject();
        if (aimed != null)
        {
            if (aimed.tag == "TurretSpawnPoints" || aimed.tag == "Tower")
            {
                money += (aimed.tag == "Tower" ? aimed.GetComponent<TurretInfo>().linkedSpawner : aimed).GetComponent<TurretSpawning>().TryDestroy();
            }
            if (aimed.tag == "TrapSpawnPoint" || aimed.tag == "Trap")
            {
                money += (aimed.tag == "Trap" ? aimed.GetComponent<TrapInfo>().linkedSpawner : aimed).GetComponent<TrapSpawning>().TryDestroy();
            }
        }
    }
    //Server --> Client
    //Both next functions : Start playing gun particles
    [ClientRpc]
    public void RpcStartParticles()
    {
        StartParticles();
    }
    public void StartParticles()
    {
        gunParticle.Play();
    }
    //Enable camera and audioListener on connection of the player
    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
        if (!myCam.enabled || !myAudioListener.enabled || !myCanvas || !miniMapCamera.enabled)
        {
            myCanvas.gameObject.SetActive(true);
            myCam.enabled = true;
            myAudioListener.enabled = true;
            miniMapCamera.enabled = true;
        }
        _name = PlayerPrefs.GetString("name");
        score = 0;
        gunSource.volume = PlayerPrefs.GetFloat("Effects");
        munitions = 20;
        isReloading = false;
        var weapon = holsterArray[0];
        ChangeWeaponStats(0);
        nbMunitions = maxMunitions;
        networkAnimator.animator = weapon.GetComponent<WeaponCharacteristics>().animator;
        foreach (var gun in holsterArray)
        {
            gun.GetComponent<WeaponCharacteristics>().currentAmmo = gun.GetComponent<WeaponCharacteristics>().munitions;
        }
        if (isServer)
        {
            panel.SetActive(true);
            panelText.text = LocalIPAddress();
            door = GameObject.FindGameObjectWithTag("Door");
            doorScript = door.GetComponent<Door>();
        }
        startingMoney = GetComponent<Money>().money;
        money = startingMoney;
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
    }
    public static string LocalIPAddress()
    {
        var discovery = FindObjectOfType<Mirror.Discovery.NetworkDiscovery>();
        int tmp = discovery.adress.GetHashCode();

        return Math.Abs(tmp).ToString();
    }
    public void OnEndGame(bool victory)
    {
        win = victory;
        canWinPoints = startRoundThree != -1f;
        Cursor.lockState = CursorLockMode.None;
        deltaMoney = canWinPoints ? CountPoints(FindObjectOfType<EnemyKill>().killedEnemies) : 0;
        PlayerPrefs.SetInt("krux", PlayerPrefs.GetInt("krux") + deltaMoney);
        networkManager.offlineScene = "WinScene";
        if (!isClientOnly)
        {
            networkManager.StopHost();
            NetworkServer.Shutdown();
        }
        else
        {
            networkManager.StopClient();
        }

    }

    public void SavePlayer()
    {
        StatsManager.instance.money += CountPoints(FindObjectOfType<EnemyKill>().killedEnemies);
    }

    public void SetRoundThree(float start) => startRoundThree = start;

    string RandomString()
    {
        string s = "";
        for (int i = 0; i < 6; i++)
        {
            s += (char)UnityEngine.Random.Range(65, 91);
        }
        return s;
    }

    public int CountPoints(List<Type> list)
    {
        float total = 0;
        foreach (var enemyType in list)
        {
            switch (enemyType)
            {
                case Type.FLYING:
                    total += 15;
                    break;
                case Type.NORMAL:
                    total += 30;
                    break;
                case Type.EXPLOSIVE:
                    total += 50;
                    break;
                default:
                    total += 100;
                    break;
            }
        }
        total /= death < 2 ? 1 : death / 2;
        total += money / 100;
        total /= 26;

        return Mathf.CeilToInt(total);
    }
}

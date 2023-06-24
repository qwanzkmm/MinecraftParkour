using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using YandexSDK;

public class PlayerInput : MonoBehaviour
{
    [Header("Settings")]
    Camera cam;
    public bool isPaused = false;
    
    [HideInInspector] public bool isCanPressEsc = true;
    [HideInInspector] public bool IsOnPs;
    
    [HideInInspector] public bool isAiming = false;

    [Header("Knife")]
    [SerializeField] private float knifeDamage;
    [SerializeField] private float knifeReloadTime;
    [HideInInspector] public bool isKnifeInHands;
    private bool canUseKnife;

    [Header("Monster Drink Vars")]
    [SerializeField] private float MonsterDrinkDamagePower = 35f;
    public float timeOfUsingMonsterDrink;

    [Header("Arms")]
    public GameObject knifeArms;
    public GameObject PistolArms;
    public GameObject AK47Arms;
    public GameObject MinigunArms;

    [Header("Camera shake")]
    public float shakePower;

    [Header("Pause")]
    [SerializeField] public GameObject PauseWindow;
    [SerializeField] private GameObject SettingsWindow;

    [Header("Player Music")]
    public GameObject SoundInstPos;
    private bool isCanPlayAndInstantiateSound = true;
    [Space(10)]
    [SerializeField] private AudioSource ShootSound;
    [SerializeField] private AudioSource Ak47ShootSound;
    [SerializeField] private AudioSource PistolShootSound;
    [SerializeField] private AudioSource DeathSound;
    [SerializeField] public AudioSource TakeSomeItemSound;

    [Space(10)]
    [SerializeField] private AudioSource ReloadingAk47Sound;

    [Space(10)]
    [SerializeField] private AudioSource PistolReloadingSound;

    [Space(10)]
    [SerializeField] private AudioSource KnifeAttackSound;
    [SerializeField] private AudioSource TakeInHandsKnifeSound;

    [Header("Zombie Music")]
    public GameObject ZombieSoundInstPos;
    public AudioSource[] ZombieSounds;
    public float ZombieSoundsCooldown;
    public float RandomZombieSoundsCooldown;
    [HideInInspector] public bool isCanZombieSoundsPlay = true;
    [HideInInspector] public bool isCanRandomZombieSoundsPlay = true;
    public bool isPlayerOnAttackZone = false;

    [Header("Leaderboard")]
    public int AllKilledZombies = 0;
    public int KilledZombiesOnThisLevel = 0;
    [DllImport("__Internal")] private static extern void SetToLeaderboard(int value);



    private PlayerController playerController;
    private Animator knifeArmsAnim;
    private Animator PistolArmsAnim;
    private Animator AK47ArmsAnim;

    private Ad _ad;
    private SpawnZombies _spawnZombies;

    private bool isPistolCanFire
    {
        get
        {
            if (currentPistolAmmo > 0 && !isPistolReloading)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private bool isCanAk47Fire
    {
        get
        {
            if (currentAk47Ammo > 0 && !isAk47Reloading)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    private void Awake()
    {
        cam = GetComponent<Camera>();
        playerController = GetComponentInParent<PlayerController>();

        knifeArmsAnim = knifeArms.GetComponent<Animator>();
        PistolArmsAnim = PistolArms.GetComponent<Animator>();
        AK47ArmsAnim = AK47Arms.GetComponent<Animator>();

        PistolleftAmmo = MaxPistolBullets;
        currentPistolAmmo = maxPistolAmmo;

        Ak47leftAmmo = MaxAk47Bullets;
        currentAk47Ammo = maxAk47Ammo;

        isPistolInHands = false;
        isKnifeInHands = false;
        isAk47InHands = true;
        isCanTakeWeapon = true;
        canUseKnife = false;

        Ak47reloadTime = 0.8f;

        knifeArms.SetActive(false);
        PistolArms.SetActive(false);
        AK47Arms.SetActive(true);

        PistolBulletsAmountText.gameObject.SetActive(false);
        PistolLeftBulletsAmountText.gameObject.SetActive(false);

        Ak47BulletsAmountText.gameObject.SetActive(true);
        Ak47LeftBulletsAmountText.gameObject.SetActive(true);

        _ad = FindObjectOfType<Ad>();
        _spawnZombies = FindObjectOfType<SpawnZombies>();

        Time.timeScale = 1f;

        if (YaSDK.instance.currentPlatform == Platform.desktop)
        {
            IsOnPs = true;
        }

        if (YandexPlayerPrefs.HasKey("AllKilledZombies"))
        {
            AllKilledZombies = YandexPlayerPrefs.GetInt("AllKilledZombies");
        }
    }

    private void Start()
    {
        /*if(YandexSDK.YaSDK.instance.isInterstitialReady && _ad.IsWhatchingAd)
            Cursor.lockState = CursorLockMode.Confined;
        else
            Cursor.lockState = CursorLockMode.Locked;*/
        
        cam.fieldOfView = 70f;
        Debug.Log(Cursor.lockState);
        isPaused = false;
        Time.timeScale = 1f;
        PauseWindow.SetActive(false);
    }

    void Update()
    {
        PistolBulletsAmountText.text = currentPistolAmmo.ToString();
        PistolLeftBulletsAmountText.text = PistolleftAmmo.ToString();

        Ak47BulletsAmountText.text = currentAk47Ammo.ToString();
        Ak47LeftBulletsAmountText.text = Ak47leftAmmo.ToString();


        //if(!YaSDK.instance.isInterstitialReady)
        //{
        //    AudioListener.pause = false;
        //}

        //-----------------------------------------------------------------
        if (Input.GetKeyDown(KeyCode.Mouse0) && YaSDK.instance.currentPlatform != Platform.phone)
        {
            ShootFromPistolFunc();
            KnifeAttackFunc();
        }

        if (Input.GetKey(KeyCode.Mouse0) && YaSDK.instance.currentPlatform != Platform.phone)
        {
            ShootFromAk47Func();
        }

        if (Input.GetKey(KeyCode.Mouse1) && YaSDK.instance.currentPlatform != Platform.phone)
        {
            KnifeAttackFunc2();
        }

        //------------------------------ RELOADING ------------------------------

        if (Input.GetKey(KeyCode.Mouse1) && (isPistolInHands || isAk47InHands) && _ad.IsWhatchingAd == false)
        {
            Aim();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1) && (isPistolInHands || isAk47InHands) && _ad.IsWhatchingAd == false)
        {
            EndAim();
        }

        if (Input.GetKeyDown(KeyCode.R) && YaSDK.instance.currentPlatform != Platform.phone)
        {
            Reloading();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && isPaused == false) { _ad.ShowMinigunRewarded(); }
        if (Input.GetKeyDown(KeyCode.Alpha5) && isPaused == false) { _ad.ShowHealthRewarded(); }
        if (Input.GetKeyDown(KeyCode.Alpha6) && isPaused == false) { _ad.ShowBulletsRewarded(); }

        if (currentPistolAmmo == 0 && PistolleftAmmo >= 1 && isPistolInHands)
        {
            isPistolReloading = true;
            float addedReloadTime = 1.25f;

            isCanTakeWeapon = false;

            if (isCanPlayAndInstantiateSound)
            {
                isCanPlayAndInstantiateSound = false;

                Instantiate(PistolReloadingSound, SoundInstPos.transform, false);
                PistolReloadingSound.Play();
            }
            
            Debug.Log("Reloading pistol!");
            PistolArmsAnim.SetBool("WalkingWithPistol", false);
            PistolArmsAnim.SetBool("GettingPistolInHands", false);
            PistolArmsAnim.SetBool("ShootingFromPistol", false);
            PistolArmsAnim.SetBool("ReloadingPistol", true);

            Invoke(nameof(PistolReload), PistolreloadTime);
            Invoke(nameof(PistolReloadEnded), PistolreloadTime + addedReloadTime);
        }
        if (currentAk47Ammo == 0 && Ak47leftAmmo >= 1 && isAk47InHands)
        {
            isAk47Reloading = true;
            float addedReloadTime = 1f;

            isCanTakeWeapon = false;

            if (isCanPlayAndInstantiateSound)
            {
                isCanPlayAndInstantiateSound = false;

                Instantiate(ReloadingAk47Sound, SoundInstPos.transform, false);
                ReloadingAk47Sound.Play();
            }

            Debug.Log("Reloading Ak-47!");
            AK47ArmsAnim.SetBool("ShootingFromAk-47", false);
            AK47ArmsAnim.SetBool("GettingAk-47InHands", false);
            AK47ArmsAnim.SetBool("Ak-47Walking", false);
            AK47ArmsAnim.SetBool("Ak-47Reloading", true);

            Invoke(nameof(Ak47Reload), Ak47reloadTime);
            Invoke(nameof(Ak47ReloadEnded), Ak47reloadTime + addedReloadTime);
        }
        if(currentPistolAmmo == 0 && PistolleftAmmo == 0)
        {
            isPistolReloading = false;
            isCanTakeWeapon = true;
        }
        if (Ak47leftAmmo == 0)
        {
            isAk47Reloading = false;
            isCanTakeWeapon = true;
        }
        //------------------------------ CHOOSING WEAPON ------------------------------

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TakeAk47InHadsFunc();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TakePistolInHandsFunc();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TakeKnifeInHandsFunc();
        }

        //------------------------------ PAUSE ------------------------------

        if (Input.GetKeyDown(KeyCode.Escape) && isCanPressEsc && YaSDK.instance.currentPlatform != Platform.phone)
        {
            Pause();
        }

        // ------------- SOUNDS -------------

        if (isCanRandomZombieSoundsPlay && isPlayerOnAttackZone && !_spawnZombies.isBreakBetweenAttacks)
        {
            int randInt = Random.Range(0, ZombieSounds.Length);
            ZombieSounds[randInt].Play();
            isCanRandomZombieSoundsPlay = false;
            Invoke(nameof(CanRandomZombieSoundsPlayAgain), RandomZombieSoundsCooldown);
        }

        // ------------------ MINIGUN --------------------------
        if (_ad.IsHaveMinigun)
        {
            MinigunArms.SetActive(true);
            MinigunButton.SetActive(false);

            MinigunFireButton.SetActive(true);
            KnifeAndPistolFireButton.SetActive(false);
            Ak47FireButton.SetActive(false);

            _ad.IsHaveMinigun = false;
            isAlreadyTookMinigun = false;

            isPistolInHands = false;
            isKnifeInHands = false;
            isAk47InHands = false;
            isCanTakeWeapon = false;
            canUseKnife = false;

            knifeArms.SetActive(false);
            PistolArms.SetActive(false);
            AK47Arms.SetActive(false);

            isCanTakeWeapon = false;
            Invoke(nameof(EndMinigun), 20f);
        }
    }

    public void EndMinigun()
    {
        if (!isAlreadyTookMinigun)
        {
            MinigunArms.SetActive(false);
            MinigunButton.SetActive(true);

            MinigunFireButton.SetActive(false);
            Ak47FireButton.SetActive(false);
            KnifeAndPistolFireButton.SetActive(true);

            _ad.IsHaveMinigun = false;

            isPistolInHands = false;
            isAk47InHands = false;
            isKnifeInHands = true;
            canUseKnife = true;

            isCanTakeWeapon = false;

            knifeArms.SetActive(true);
            PistolArms.SetActive(false);
            AK47Arms.SetActive(false);

            Instantiate(TakeInHandsKnifeSound, SoundInstPos.transform, false);
            TakeInHandsKnifeSound.Play();

            knifeArmsAnim.SetBool("KnifeWalking", false);
            knifeArmsAnim.SetBool("KnifeAttak", false);
            knifeArmsAnim.SetBool("KnifeAttak2", false);
            knifeArmsAnim.SetBool("KnifeInHands", true);

            PistolBulletsAmountText.gameObject.SetActive(false);
            PistolLeftBulletsAmountText.gameObject.SetActive(false);

            Ak47BulletsAmountText.gameObject.SetActive(false);
            Ak47LeftBulletsAmountText.gameObject.SetActive(false);
            Invoke(nameof(canTakeWeapon), TimeBetweenYouCanTakeWeapon);
            isAlreadyTookMinigun = true;
        }
    }


    //------------------------------ OTHER   ------------------------------

    public void ContinueGame()
    {
        isPaused = false;
        if(YandexSDK.YaSDK.instance.currentPlatform == YandexSDK.Platform.desktop)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 1f;
        PauseWindow.SetActive(false);
        AudioListener.pause = false;
    }

    private void PistolReloadEnded()
    {
        isPistolReloading = false;

        PistolArmsAnim.SetBool("GettingPistolInHands", false);
        PistolArmsAnim.SetBool("ShootingFromPistol", false);
        PistolArmsAnim.SetBool("ReloadingPistol", false);
        PistolArmsAnim.SetBool("WalkingWithPistol", true);

        isCanTakeWeapon = true;
        isCanPlayAndInstantiateSound = true;
    }
    private void Ak47ReloadEnded()
    {
        isAk47Reloading = false;

        AK47ArmsAnim.SetBool("Ak-47Reloading", false);
        AK47ArmsAnim.SetBool("ShootingFromAk-47", false);
        AK47ArmsAnim.SetBool("GettingAk-47InHands", false);
        AK47ArmsAnim.SetBool("Ak-47Walking", true);

        isCanTakeWeapon = true;
        isCanPlayAndInstantiateSound = true;
    }
    //public void ShakeCamera()
    //{
    //    cam.transform.localRotation = Quaternion.Euler(-shakePower * Time.deltaTime, 0f, 0f);
    //}
    private void CanUseKnifeAgain()
    {
        canUseKnife = true;
    }
    private void CanUseAk47Again()
    {
        isCanUseAk47Again = true;
        isCanReloadAk47 = true;
        isCanTakeWeapon = true;
    }

    private void StopKnifeAttackAnim()
    {
        knifeArmsAnim.SetBool("KnifeAttak", false);
        knifeArmsAnim.SetBool("KnifeInHands", false);
        knifeArmsAnim.SetBool("KnifeAttak2", false);
        knifeArmsAnim.SetBool("KnifeWalking", true);

        isCanTakeWeapon = true;
    }
    private void StopPistolShootingAnim()
    {
        Debug.Log("ShootingFromPistol: false");
        PistolArmsAnim.SetBool("GettingPistolInHands", false);
        PistolArmsAnim.SetBool("ShootingFromPistol", false);
        PistolArmsAnim.SetBool("ReloadingPistol", false);
        PistolArmsAnim.SetBool("WalkingWithPistol", true);

        isCanTakeWeapon = true;
    }

    private void StopAk47ShootingAnim()
    {
        AK47ArmsAnim.SetBool("Ak-47Reloading", false);
        AK47ArmsAnim.SetBool("ShootingFromAk-47", false);
        AK47ArmsAnim.SetBool("GettingAk-47InHands", false);
        AK47ArmsAnim.SetBool("Ak-47Walking", true);
    }
    private void Ak47WalkingAnim()
    {
        AK47ArmsAnim.SetBool("Ak-47Reloading", false);
        AK47ArmsAnim.SetBool("ShootingFromAk-47", false);
        AK47ArmsAnim.SetBool("GettingAk-47InHands", false);
        AK47ArmsAnim.SetBool("Ak-47Walking", true);
    }
    private void canTakeWeapon()
    {
        isCanTakeWeapon = true;
    }

    private void DisableCrest()
    {
        RedCrest.SetActive(false);
        Crest.SetActive(false);
    }

    private void KnifeAttackSoundPlay()
    {
        Instantiate(KnifeAttackSound, SoundInstPos.transform, false);
        KnifeAttackSound.Play();
    }

    public void CanZombieSoundsPlayAgain()
    {
        isCanZombieSoundsPlay = true;
    }
    private void CanRandomZombieSoundsPlayAgain()
    {
        isCanRandomZombieSoundsPlay = true;
    }

    //------------------------------------------------- MAIN FUNCTIONS -------------------------------------------------
    public void ShootFromAk47Func()
    {
        if (isAk47InHands && isCanAk47Fire && !isAk47Reloading && !isPaused)
        {
            Ray ray = cam.ScreenPointToRay(new Vector2(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f));
            RaycastHit hit;
            
            if (isCanUseAk47Again)
            {
                //Debug.Log(hit.transform.gameObject.name);

                isCanReloadAk47 = false;
                currentAk47Ammo--;

                //if(IsOnPs)
                //    ShakeCamera();

                AK47ArmsAnim.SetBool("Ak-47Reloading", false);
                AK47ArmsAnim.SetBool("GettingAk-47InHands", false);
                AK47ArmsAnim.SetBool("Ak-47Walking", false);
                AK47ArmsAnim.SetBool("ShootingFromAk-47", true);

                isCanTakeWeapon = false;

                Instantiate(Ak47ShootSound, SoundInstPos.transform, false);
                Ak47ShootSound.pitch = Random.Range(0.9f, 1.1f);
                Ak47ShootSound.Play();
                
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.transform.gameObject.tag == "HeadColider")
                    {
                        Debug.Log("HeadColider has damaged by Player");
                        Health health = hit.transform.gameObject.GetComponentInParent<Health>();

                        ZombieController zombieController = hit.transform.gameObject.GetComponentInParent<ZombieController>();
                        zombieController.IsNeedToStop = false;
                        zombieController.IsNeedToGoToPlayer = true;
                        if (playerController.isMonsterDrinkDamagePowerUsed)
                        {
                            if (health.currentHealth >= 1f)
                            {
                                health.GetDamade((weaponDamage + (weaponDamage / 2) + MonsterDrinkDamagePower) * 2f);

                                if (health.currentHealth <= 0f)
                                {
                                    health.currentHealth = 0f;
                                    AllKilledZombies++;
                                    KilledZombiesOnThisLevel++;
                                    SetToLeaderboard(AllKilledZombies);
                                    YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                                }
                            }
                            else
                            {
                                health.currentHealth = 0f;
                                AllKilledZombies++;
                                KilledZombiesOnThisLevel++;
                                SetToLeaderboard(AllKilledZombies);
                                YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                            }
                        }
                        else
                        {
                            if (health.currentHealth >= 1f)
                            {
                                health.GetDamade(weaponDamage + (weaponDamage / 2) * 2f);

                                if (health.currentHealth <= 0f)
                                {
                                    health.currentHealth = 0f;
                                    AllKilledZombies++;
                                    KilledZombiesOnThisLevel++;
                                    SetToLeaderboard(AllKilledZombies);
                                    YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                                }
                            }
                            else
                            {
                                health.currentHealth = 0f;
                                AllKilledZombies++;
                                KilledZombiesOnThisLevel++;
                                SetToLeaderboard(AllKilledZombies);
                                YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                            }
                        }
                        if (isCanZombieSoundsPlay)
                        {
                            int randInt = Random.Range(0, ZombieSounds.Length);
                            Instantiate(ZombieSounds[randInt], ZombieSoundInstPos.transform, false);
                            ZombieSounds[randInt].Play();
                            isCanZombieSoundsPlay = false;
                            Invoke(nameof(CanZombieSoundsPlayAgain), ZombieSoundsCooldown);
                        }

                        if (health.currentHealth < 9)
                        {
                            RedCrest.SetActive(true);
                            Invoke(nameof(DisableCrest), CrestTime);
                        }
                        else
                        {
                            Crest.SetActive(true); 
                            Invoke(nameof(DisableCrest), CrestTime);
                        }
                    }
                    if (hit.transform.gameObject.tag == "BodyColider")
                    {
                        Debug.Log("BodyColider has damaged by Player");
                        Health health = hit.transform.gameObject.GetComponentInParent<Health>();

                        ZombieController zombieController = hit.transform.gameObject.GetComponentInParent<ZombieController>();
                        zombieController.IsNeedToStop = false;
                        zombieController.IsNeedToGoToPlayer = true;

                        if (playerController.isMonsterDrinkDamagePowerUsed)
                        {
                            if (health.currentHealth >= 1f) 
                            { 
                                health.GetDamade(weaponDamage + (weaponDamage / 2) + MonsterDrinkDamagePower);
                                if (health.currentHealth <= 0f) 
                                { 
                                    health.currentHealth = 0f; 
                                    AllKilledZombies++;
                                    KilledZombiesOnThisLevel++; 
                                    SetToLeaderboard(AllKilledZombies); 
                                    YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                                }
                            }
                            else
                            { 
                                health.currentHealth = 0f;
                                AllKilledZombies++;
                                KilledZombiesOnThisLevel++;
                                SetToLeaderboard(AllKilledZombies);
                                YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                            }
                        }
                        else
                        {
                            if (health.currentHealth >= 1f)
                            { 
                                health.GetDamade(weaponDamage + (weaponDamage / 2));

                                if (health.currentHealth <= 0f)
                                { 
                                    health.currentHealth = 0f;
                                    AllKilledZombies++;
                                    KilledZombiesOnThisLevel++;
                                    SetToLeaderboard(AllKilledZombies);
                                    YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                                }
                            }
                            else
                            {
                                health.currentHealth = 0f;
                                AllKilledZombies++;
                                KilledZombiesOnThisLevel++;
                                SetToLeaderboard(AllKilledZombies);
                                YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                            }
                        }

                        if (isCanZombieSoundsPlay)
                        {
                            int randInt = Random.Range(0, ZombieSounds.Length);
                            Instantiate(ZombieSounds[randInt], ZombieSoundInstPos.transform, false);
                            ZombieSounds[randInt].Play();
                            isCanZombieSoundsPlay = false; 
                            Invoke(nameof(CanZombieSoundsPlayAgain), ZombieSoundsCooldown);
                        }

                        if (health.currentHealth < 9)
                        { 
                            RedCrest.SetActive(true);
                            Invoke(nameof(DisableCrest), CrestTime);
                        }
                        else
                        {
                            Crest.SetActive(true);
                            Invoke(nameof(DisableCrest), CrestTime);
                                
                        }
                    }
                    if (hit.transform.gameObject.tag == "JerryCan")
                    { 
                        CanOfOilExplode canOfOilExplode = hit.transform.gameObject.GetComponent<CanOfOilExplode>();
                        canOfOilExplode.Explode();
                        RedCrest.SetActive(true);
                        Invoke(nameof(DisableCrest), CrestTime);
                        AllKilledZombies++;
                        KilledZombiesOnThisLevel++;
                        SetToLeaderboard(AllKilledZombies);
                        Debug.Log("Explode JerryCan");
                    }
                }
                
                Invoke(nameof(StopAk47ShootingAnim), 0.15f);
                isCanUseAk47Again = false;
                Invoke(nameof(CanUseAk47Again), Ak47TimeToNextShoot);
            }
        }
    }
    public void ShootFromPistolFunc()
    {
        if (isPistolInHands && !isPaused)
        {
            Ray ray = cam.ScreenPointToRay(new Vector2(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f));
            RaycastHit hit;
            
            if (isPistolCanFire)
            {
                //Debug.Log(hit.transform.gameObject.name);

                currentPistolAmmo--;
                //if(IsOnPs)
                //    ShakeCamera();

                PistolArmsAnim.SetBool("WalkingWithPistol", false);
                PistolArmsAnim.SetBool("ReloadingPistol", false);
                PistolArmsAnim.SetBool("GettingPistolInHands", false);
                PistolArmsAnim.SetBool("ShootingFromPistol", true);
                    
                Instantiate(PistolShootSound, SoundInstPos.transform, false);
                PistolShootSound.pitch = Random.Range(0.9f, 1.1f);
                PistolShootSound.Play();

                isCanTakeWeapon = false;
                
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.transform.gameObject.tag == "HeadColider")
                    {
                        Debug.Log("HeadColider has damaged by Player");
                        Health health = hit.transform.gameObject.GetComponentInParent<Health>();

                        ZombieController zombieController = hit.transform.gameObject.GetComponentInParent<ZombieController>();
                        zombieController.IsNeedToStop = false;
                        zombieController.IsNeedToGoToPlayer = true;

                        if (playerController.isMonsterDrinkDamagePowerUsed)
                        {
                            if (health.currentHealth >= 1f)
                            {
                                health.GetDamade((weaponDamage + MonsterDrinkDamagePower) * 2f);

                                if (health.currentHealth <= 0f)
                                {
                                    health.currentHealth = 0f;
                                    AllKilledZombies++;
                                    KilledZombiesOnThisLevel++;
                                    SetToLeaderboard(AllKilledZombies);
                                    YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                                }
                            }
                            else
                            {
                                health.currentHealth = 0f;
                                AllKilledZombies++;
                                KilledZombiesOnThisLevel++;
                                SetToLeaderboard(AllKilledZombies);
                                YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                            }
                        }
                        else
                        {
                            if (health.currentHealth >= 1f)
                            {
                                health.GetDamade(weaponDamage * 2f);

                                if (health.currentHealth <= 0f)
                                {
                                    health.currentHealth = 0f;
                                    AllKilledZombies++;
                                    KilledZombiesOnThisLevel++;
                                    SetToLeaderboard(AllKilledZombies);
                                    YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                                }
                            }
                            else
                            {
                                health.currentHealth = 0f;
                                AllKilledZombies++;
                                KilledZombiesOnThisLevel++;
                                SetToLeaderboard(AllKilledZombies);
                                YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                            }
                        }
                        if (isCanZombieSoundsPlay)
                        {
                            int randInt = Random.Range(0, ZombieSounds.Length);
                            Instantiate(ZombieSounds[randInt], ZombieSoundInstPos.transform, false);
                            ZombieSounds[randInt].Play();
                            isCanZombieSoundsPlay = false;
                            Invoke(nameof(CanZombieSoundsPlayAgain), ZombieSoundsCooldown);
                        }
                        if (health.currentHealth < 9)
                        {
                            RedCrest.SetActive(true);
                            Invoke(nameof(DisableCrest), CrestTime);
                        }
                        else
                        {
                            Crest.SetActive(true);
                            Invoke(nameof(DisableCrest), CrestTime);
                        }
                    }
                    if (hit.transform.gameObject.tag == "BodyColider")
                    {
                        Debug.Log("BodyColider has damaged by Player");
                        Health health = hit.transform.gameObject.GetComponentInParent<Health>();

                        ZombieController zombieController = hit.transform.gameObject.GetComponentInParent<ZombieController>();
                        zombieController.IsNeedToStop = false;
                        zombieController.IsNeedToGoToPlayer = true;

                        if (playerController.isMonsterDrinkDamagePowerUsed)
                        {
                            if (health.currentHealth >= 1f)
                            {
                                health.GetDamade(weaponDamage + MonsterDrinkDamagePower);

                                if (health.currentHealth <= 0f)
                                {
                                    health.currentHealth = 0f;
                                    AllKilledZombies++;
                                    KilledZombiesOnThisLevel++;
                                    SetToLeaderboard(AllKilledZombies);
                                    YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                                }
                            }
                            else
                            {
                                health.currentHealth = 0f;
                                AllKilledZombies++;
                                KilledZombiesOnThisLevel++;
                                SetToLeaderboard(AllKilledZombies);
                                YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                            }
                        }
                        else
                        {
                            if (health.currentHealth >= 1f)
                            {
                                health.GetDamade(weaponDamage);

                                if (health.currentHealth <= 0f)
                                {
                                    health.currentHealth = 0f;
                                    AllKilledZombies++;
                                    KilledZombiesOnThisLevel++;
                                    SetToLeaderboard(AllKilledZombies);
                                    YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                                }
                            }
                            else
                            {
                                health.currentHealth = 0f;
                                AllKilledZombies++;
                                KilledZombiesOnThisLevel++;
                                SetToLeaderboard(AllKilledZombies);
                                YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                            }
                        }
                        if (isCanZombieSoundsPlay)
                        {
                            
                            int randInt = Random.Range(0, ZombieSounds.Length);
                            Instantiate(ZombieSounds[randInt], ZombieSoundInstPos.transform, false);
                            ZombieSounds[randInt].Play();
                            isCanZombieSoundsPlay = false;
                            Invoke(nameof(CanZombieSoundsPlayAgain), ZombieSoundsCooldown);
                        }
                        if (health.currentHealth < 9)
                        {
                            RedCrest.SetActive(true);
                            Invoke(nameof(DisableCrest), CrestTime);
                        }
                        else
                        {
                            Crest.SetActive(true);
                            Invoke(nameof(DisableCrest), CrestTime);
                        }
                    }
                    if (hit.transform.gameObject.tag == "JerryCan")
                    {
                        CanOfOilExplode canOfOilExplode = hit.transform.gameObject.GetComponent<CanOfOilExplode>();
                        canOfOilExplode.Explode();

                        RedCrest.SetActive(true);
                        Invoke(nameof(DisableCrest), CrestTime);

                        AllKilledZombies++;
                        KilledZombiesOnThisLevel++;
                        SetToLeaderboard(AllKilledZombies);

                        Debug.Log("Explode JerryCan");
                    }
                }
                Invoke(nameof(StopPistolShootingAnim), 0.15f);
            }
        }
    }
    public void KnifeAttackFunc()
    {
        if (isKnifeInHands && canUseKnife && isPaused == false)
        {
            Ray ray = cam.ScreenPointToRay(new Vector2(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f));
            RaycastHit hit;

            Invoke(nameof(KnifeAttackSoundPlay), 0.1f);

            isCanTakeWeapon = false;

            knifeArmsAnim.SetBool("KnifeInHands", false);
            knifeArmsAnim.SetBool("KnifeWalking", false);
            knifeArmsAnim.SetBool("KnifeAttak2", false);
            knifeArmsAnim.SetBool("KnifeAttak", true);


            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if (hit.transform.gameObject.tag == "HeadColider" || hit.transform.gameObject.tag == "BodyColider")
                {
                    Health health = hit.transform.gameObject.GetComponentInParent<Health>();
                    if (health != null) { health.GetDamade(knifeDamage); }
                    if(health != null && health.currentHealth <= 0)
                    {
                        AllKilledZombies++;
                        KilledZombiesOnThisLevel++;
                        SetToLeaderboard(AllKilledZombies);
                        YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                    }

                    if (isCanZombieSoundsPlay)
                    {
                        
                        int randInt = Random.Range(0, ZombieSounds.Length);
                        Instantiate(ZombieSounds[randInt], ZombieSoundInstPos.transform, false);
                        ZombieSounds[randInt].Play();
                        isCanZombieSoundsPlay = false;
                        Invoke(nameof(CanZombieSoundsPlayAgain), ZombieSoundsCooldown);
                    }

                    if (health.currentHealth < 9)
                    {
                        RedCrest.SetActive(true);
                        Invoke(nameof(DisableCrest), CrestTime);
                    }
                    else
                    {
                        Crest.SetActive(true);
                        Invoke(nameof(DisableCrest), CrestTime);
                    }
                }
            }

            canUseKnife = false;
            Invoke(nameof(CanUseKnifeAgain), knifeReloadTime + 1f);
            Invoke(nameof(StopKnifeAttackAnim), 1.25f);
        }
    }
    public void KnifeAttackFunc2()
    {
        if (isKnifeInHands && canUseKnife && isPaused == false)
        {
            Ray ray = cam.ScreenPointToRay(new Vector2(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f));
            RaycastHit hit;

            Invoke(nameof(KnifeAttackSoundPlay), 0.1f);

            isCanTakeWeapon = false;

            knifeArmsAnim.SetBool("KnifeInHands", false);
            knifeArmsAnim.SetBool("KnifeWalking", false);
            knifeArmsAnim.SetBool("KnifeAttak", false);
            knifeArmsAnim.SetBool("KnifeAttak2", true);

            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if (hit.transform.gameObject.tag == "HeadColider" || hit.transform.gameObject.tag == "BodyColider")
                {
                    Health health = hit.transform.gameObject.GetComponentInParent<Health>();
                    if (health != null) { health.GetDamade(knifeDamage); }
                    if (health != null && health.currentHealth <= 0)
                    {
                        AllKilledZombies++;
                        KilledZombiesOnThisLevel++;
                        SetToLeaderboard(AllKilledZombies);
                        YandexPlayerPrefs.SetInt("AllKilledZombies", AllKilledZombies);
                    }

                    if (isCanZombieSoundsPlay)
                    {
                        
                        int randInt = Random.Range(0, ZombieSounds.Length);
                        Instantiate(ZombieSounds[randInt], ZombieSoundInstPos.transform, false);
                        ZombieSounds[randInt].Play();
                        isCanZombieSoundsPlay = false;
                        Invoke(nameof(CanZombieSoundsPlayAgain), ZombieSoundsCooldown);
                    }
                    if (health.currentHealth < 9)
                    {
                        RedCrest.SetActive(true);
                        Invoke(nameof(DisableCrest), CrestTime);
                    }
                    else
                    {
                        Crest.SetActive(true);
                        Invoke(nameof(DisableCrest), CrestTime);
                    }
                }
            }

            canUseKnife = false;
            Invoke(nameof(CanUseKnifeAgain), knifeReloadTime + 1f);
            Invoke(nameof(StopKnifeAttackAnim), 1.25f);
        }
    }


    public void TakeKnifeInHandsFunc()
    {
        if (isCanTakeWeapon && !isKnifeInHands && (!isAk47Reloading || !isPistolReloading))
        {
            isPistolInHands = false;
            isAk47InHands = false;
            isKnifeInHands = true;
            canUseKnife = true;

            isCanTakeWeapon = false;

            knifeArms.SetActive(true);
            PistolArms.SetActive(false);
            AK47Arms.SetActive(false);

            Instantiate(TakeInHandsKnifeSound, SoundInstPos.transform, false);
            TakeInHandsKnifeSound.Play();

            knifeArmsAnim.SetBool("KnifeWalking", false);
            knifeArmsAnim.SetBool("KnifeAttak", false);
            knifeArmsAnim.SetBool("KnifeAttak2", false);
            knifeArmsAnim.SetBool("KnifeInHands", true);

            PistolBulletsAmountText.gameObject.SetActive(false);
            PistolLeftBulletsAmountText.gameObject.SetActive(false);

            Ak47BulletsAmountText.gameObject.SetActive(false);
            Ak47LeftBulletsAmountText.gameObject.SetActive(false);
            Invoke(nameof(canTakeWeapon), TimeBetweenYouCanTakeWeapon);
        }
        
    }
    public void TakePistolInHandsFunc()
    {
        if (isCanTakeWeapon && !isPistolInHands && (!isAk47Reloading || !isPistolReloading))
        {
            isKnifeInHands = false;
            canUseKnife = false;
            isAk47InHands = false;
            isPistolInHands = true;

            isCanTakeWeapon = false;

            knifeArms.SetActive(false);
            PistolArms.SetActive(true);
            AK47Arms.SetActive(false);


            PistolArmsAnim.SetBool("WalkingWithPistol", false);
            PistolArmsAnim.SetBool("ReloadingPistol", false);
            PistolArmsAnim.SetBool("ShootingFromPistol", false);
            PistolArmsAnim.SetBool("GettingPistolInHands", true);

            PistolBulletsAmountText.gameObject.SetActive(true);
            PistolLeftBulletsAmountText.gameObject.SetActive(true);

            Ak47BulletsAmountText.gameObject.SetActive(false);
            Ak47LeftBulletsAmountText.gameObject.SetActive(false);

            Invoke(nameof(canTakeWeapon), TimeBetweenYouCanTakeWeapon);
        }
    }
    public void TakeAk47InHadsFunc()
    {
        if (isCanTakeWeapon && !isAk47InHands && (!isAk47Reloading || !isPistolReloading))
        {
            isKnifeInHands = false;
            canUseKnife = false;
            isPistolInHands = false;
            isAk47InHands = true;

            isCanTakeWeapon = false;

            knifeArms.SetActive(false);
            PistolArms.SetActive(false);
            AK47Arms.SetActive(true);


            AK47ArmsAnim.SetBool("Ak-47Walking", false);
            AK47ArmsAnim.SetBool("Ak-47Reloading", false);
            AK47ArmsAnim.SetBool("ShootingFromAk-47", false);
            AK47ArmsAnim.SetBool("GettingAk-47InHands", true);

            Invoke(nameof(Ak47WalkingAnim), 0.3f);

            PistolBulletsAmountText.gameObject.SetActive(false);
            PistolLeftBulletsAmountText.gameObject.SetActive(false);

            Ak47BulletsAmountText.gameObject.SetActive(true);
            Ak47LeftBulletsAmountText.gameObject.SetActive(true);

            Invoke(nameof(canTakeWeapon), TimeBetweenYouCanTakeWeapon);
        }
    }


    private void PistolReload()
    {
        isPistolReloading = true;
        if (PistolleftAmmo >= maxPistolAmmo)
        {
            PistolleftAmmo -= (maxPistolAmmo - currentPistolAmmo);

            int bulletsNeeded = maxPistolAmmo - currentPistolAmmo;
            currentPistolAmmo += bulletsNeeded;
        }
        else
        {
            if (PistolleftAmmo <= 14)
            {
                currentPistolAmmo += PistolleftAmmo;
                PistolleftAmmo = 0;
                if (currentPistolAmmo > 15)
                {
                    int ReturnBullets = currentPistolAmmo - 15;
                    PistolleftAmmo += ReturnBullets;
                    currentPistolAmmo = 15;
                }
            }
        }
    }
    private void Ak47Reload()
    {
        isAk47Reloading = true;
        if (Ak47leftAmmo >= maxAk47Ammo)
        {
            Ak47leftAmmo -= (maxAk47Ammo - currentAk47Ammo);

            int bulletsNeeded = maxAk47Ammo - currentAk47Ammo;
            currentAk47Ammo += bulletsNeeded;
        }
        else
        {
            if (Ak47leftAmmo <= 29)
            {
                currentAk47Ammo += Ak47leftAmmo;
                Ak47leftAmmo = 0;
                if (currentAk47Ammo > 30)
                {
                    int ReturnBullets = currentAk47Ammo - 30;
                    Ak47leftAmmo += ReturnBullets;
                    currentAk47Ammo = 30;
                }
            }
        }
    }
    public void Reloading()
    {
        if (isPistolInHands && !isPistolReloading && currentPistolAmmo <= 14 && currentPistolAmmo >= 1 && PistolleftAmmo != 0)
        {
            isPistolReloading = true;
            float addedReloadTime = 0.45f;

            isCanTakeWeapon = false;

            Instantiate(PistolReloadingSound, SoundInstPos.transform, false);
            PistolReloadingSound.Play();

            Debug.Log("Reloading pistol!");
            PistolArmsAnim.SetBool("WalkingWithPistol", false);
            PistolArmsAnim.SetBool("GettingPistolInHands", false);
            PistolArmsAnim.SetBool("ShootingFromPistol", false);
            PistolArmsAnim.SetBool("ReloadingPistol", true);

            Invoke(nameof(PistolReload), PistolreloadTime);
            Invoke(nameof(PistolReloadEnded), PistolreloadTime + addedReloadTime);
        }
        if (isAk47InHands && !isAk47Reloading && currentAk47Ammo <= 29 && currentAk47Ammo >= 1 && isCanReloadAk47 && Ak47leftAmmo != 0)
        {
            isAk47Reloading = true;
            float addedReloadTime = 1f;

            isCanTakeWeapon = false;

            Instantiate(ReloadingAk47Sound, SoundInstPos.transform, false);
            ReloadingAk47Sound.Play();

            Debug.Log("Reloading Ak-47!");
            AK47ArmsAnim.SetBool("ShootingFromAk-47", false);
            AK47ArmsAnim.SetBool("GettingAk-47InHands", false);
            AK47ArmsAnim.SetBool("Ak-47Walking", false);
            AK47ArmsAnim.SetBool("Ak-47Reloading", true);

            Invoke(nameof(Ak47Reload), Ak47reloadTime);
            Invoke(nameof(Ak47ReloadEnded), Ak47reloadTime + addedReloadTime);
        }
    }


    public void Pause()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;

            PauseWindow.SetActive(false);
            SettingsWindow.SetActive(false);
            AudioListener.pause = false;
        }
        else
        {
            isPaused = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;

            PauseWindow.SetActive(true);
            AudioListener.pause = true;
        }
    }


    public void Aim()
    {
        float timer = 0;
        float timeToLoad = 1f;
        bool isCanChangeFieldOfView = true;

        if (isCanChangeFieldOfView)
        {
            if (cam.fieldOfView >= 50f)
            {
                timer += Time.deltaTime;
                float fillAmount = Mathf.Clamp01(timer / timeToLoad);
                cam.fieldOfView -= fillAmount * 300f;

                if (cam.fieldOfView < 50)
                {
                    cam.fieldOfView = 50f;
                    isCanChangeFieldOfView = false;
                }
            }
        }
        shakePower = 500f;
        playerController.SlowPlayer();
        Vinette.SetActive(true);
    }
    public void EndAim()
    {
        //float timer = 0;
        //float timeToLoad = 1f;
        //bool isCanChangeFieldOfView = true;


        //if (!playerController.isRunning)
        //{
        //    if (isCanChangeFieldOfView)
        //    {
        //        if (cam.fieldOfView >= 70f)
        //        {
        //            timer += Time.deltaTime;
        //            float fillAmount = Mathf.Clamp01(timer / timeToLoad);
        //            cam.fieldOfView -= fillAmount * 300f;

        //            if (cam.fieldOfView > 70)
        //            {
        //                cam.fieldOfView = 70f;
        //                isCanChangeFieldOfView = false;
        //            }
        //        }
        //    }
        //}

        //shakePower = 1000f;
        //playerController.ReturnSpeed();
        //Vinette.SetActive(false);

        cam.fieldOfView = 70f;
        shakePower = 1000f;
        playerController.ReturnSpeed();
        Vinette.SetActive(false);
    }
    public void MobileEndAim()
    {
        cam.fieldOfView = 70f;
        shakePower = 1000f;
        playerController.ReturnSpeed();
        Vinette.SetActive(false);
    }


    public void ShootFromKnifeOrPistolMobile()
    {
        if (isPistolInHands)
        {
            ShootFromPistolFunc();
        }
        if(isKnifeInHands)
        {
            int randInt = Random.Range(0, 1);
            if(randInt == 0)
            {
                KnifeAttackFunc();
            }
            if(randInt == 1)
            {
                KnifeAttackFunc2();
            }
        }
    }
}
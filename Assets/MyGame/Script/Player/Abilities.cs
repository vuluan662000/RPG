using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    PlayerStats stats;
    PlayerController playerController;

    public LayerMask groundLayer;
    [Header("skill 1")]
    public Image abilityImage1;
    public Text abilityText1;
    public KeyCode ability1Key;
    public float ability1Cooldown = 5;
    public SkillSwordWave swordWavePrefab;
    private ObjectPool swordPool;

    [Header("skill 2")]
    public Image abilityImage2;
    public Text abilityText2;
    public KeyCode ability2Key;
    public float ability2Cooldown = 5;
    public SkillSnowStorm snowStormPrefab;
    private ObjectPool snowStormPool;

    [Header("skill 3")]
    public Image abilityImage3;
    public Text abilityText3;
    public KeyCode ability3Key;
    public float ability3Cooldown = 5;

    [Header("skill 4")]
    public Image abilityImage4;
    public Text abilityText4;
    public KeyCode ability4Key;
    public float ability4Cooldown = 5;
    public float maxTeleportDistance = 10f;

    private bool isAbility1Cooldown = false;
    private bool isAbility2Cooldown = false;
    private bool isAbility3Cooldown = false;
    private bool isAbility4Cooldown = false;

    private float currentAbility1Cooldown;
    private float currentAbility2Cooldown;
    private float currentAbility3Cooldown;
    private float currentAbility4Cooldown;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        playerController = GetComponent<PlayerController>();
    }
    private void Start()
    {
        abilityImage1.fillAmount = 0;
        abilityImage2.fillAmount = 0;
        abilityImage3.fillAmount = 0;
        abilityImage4.fillAmount = 0;

        abilityText1.text = "";
        abilityText2.text = "";
        abilityText3.text = "";
        abilityText4.text = "";
    }

    private void Update()
    {
        Ability1Input();
        Ability2Input();
        Ability3Input();
        Ability4Input();

        AbilityCooldown(ref currentAbility1Cooldown, ability1Cooldown, ref isAbility1Cooldown, abilityImage1, abilityText1);
        AbilityCooldown(ref currentAbility2Cooldown, ability2Cooldown, ref isAbility2Cooldown, abilityImage2, abilityText2);
        AbilityCooldown(ref currentAbility3Cooldown, ability3Cooldown, ref isAbility3Cooldown, abilityImage3, abilityText3);
        AbilityCooldown(ref currentAbility4Cooldown, ability4Cooldown, ref isAbility4Cooldown, abilityImage4, abilityText4);

        CreateSwordWavePool();

    }
    // skill 1------------------------------
    public void CreateSwordWavePool()
    {
        if (swordPool == null)
        {
            swordPool = ObjectPool.CreateInstance(swordWavePrefab, 2);
        }
    }

    private void SpawnSwordWave()
    {
        PoolableObject poolableObject = swordPool.GetObject();
        if (poolableObject != null)
        {
            SkillSwordWave swordWave = poolableObject.GetComponent<SkillSwordWave>();

            swordWave.transform.position = transform.position + transform.forward ;
            swordWave.transform.rotation = transform.rotation;

            swordWave.Spawn(transform.forward, stats._attackDamage * 2);
        }
    }

    private void Ability1Input()
    {
        if (Input.GetKeyDown(ability1Key) && !isAbility1Cooldown)
        {
            isAbility1Cooldown = true;
            currentAbility1Cooldown = ability1Cooldown;
            playerController._animation.AttackAnimation();
            SpawnSwordWave();
        }
    }

    // skill 2-------------------------------------------

    private void Ability2Input()
    {
        if (Input.GetKeyDown(ability2Key) && !isAbility2Cooldown)
        {
            Vector3 mouseWorldPos;
            if (GetMouseWorldPosition(out mouseWorldPos))
            {
                isAbility2Cooldown = true;
                currentAbility2Cooldown = ability2Cooldown;
                SpawnSnowStorm(mouseWorldPos);
            }
        }
    }
    private bool GetMouseWorldPosition(out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            worldPosition = hit.point;
            return true;
        }
        return false;
    }
    private void SpawnSnowStorm(Vector3 position)
    {
        if (snowStormPool == null)
        {
            snowStormPool = ObjectPool.CreateInstance(snowStormPrefab, 2);
        }

        PoolableObject poolableObject = snowStormPool.GetObject();
        if (poolableObject != null)
        {
            SkillSnowStorm snowStorm = poolableObject.GetComponent<SkillSnowStorm>();

            snowStorm.transform.position = position;
            snowStorm.Spawn(position, stats._attackDamage); // Truyền sát thương
        }
    }
    // skil 3 --------------------------
    private void Ability3Input()
    {
        if (Input.GetKeyDown(ability3Key) && !isAbility3Cooldown)
        {
            isAbility3Cooldown = true;
            currentAbility3Cooldown = ability3Cooldown;

            playerController._vfx.PlayHealthVFX();
            int healAmount = Mathf.FloorToInt(stats.maxHealth * 0.2f);
            stats.health = Mathf.Min(stats.health + healAmount, stats.maxHealth);

            playerController.healthSlider.value = stats.health;
        }
    }
    // skill 4 --------------------------
    private void Ability4Input()
    {
        if (Input.GetKeyDown(ability4Key) && !isAbility4Cooldown)
        {
            isAbility4Cooldown = true;
            currentAbility4Cooldown = ability4Cooldown;
            TeleportToMousePosition();
        }
    }
    private void TeleportToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f); 

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            if (distance <= maxTeleportDistance) 
            {
                transform.position = hit.point;
            }
            else
            {
                Vector3 direction = (hit.point - transform.position).normalized;
                transform.position += direction * maxTeleportDistance;
            }
            playerController._vfx.PlayBlinkVFX();
        }
    }
    private void AbilityCooldown(ref float currentCooldown, float maxCooldown, ref bool isCooldown, Image skillImage, Text skillText)
    {
        if (isCooldown) 
        {
            currentCooldown -= Time.deltaTime;

            if(currentCooldown <= 0f)
            {
                isCooldown = false;
                currentCooldown = 0f;

                if(skillImage != null)
                {
                    skillImage.fillAmount = 0f;
                }

                if(skillText != null)
                {
                    skillText.text = "";
                }
            }

            else
            {
                if(skillText != null)
                {
                    skillImage.fillAmount = currentCooldown / maxCooldown;
                }
                if(skillText != null)
                {
                    skillText.text = Mathf.Ceil(currentCooldown).ToString();
                }
            }
        }

    }
}

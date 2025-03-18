using UnityEngine;

[CreateAssetMenu(fileName = "Attack Configuration", menuName = "ScriptableObject/Attack Configuration")]
public class AttackScriptableObject : ScriptableObject
{
    public bool isRanged = false;
    public int damage = 5;
    public float attackRadius = 1.5f;
    public float attackDelay = 1.5f;

    // Ranged Configs
    public Bullet bulletPrefab;
    public Vector3 bulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask lineOfSightLayers;

    public AttackScriptableObject ScaleUpForLevel(ScalingScriptableObject scaling, int level)
    {
        AttackScriptableObject scaledUpConfiguration = CreateInstance<AttackScriptableObject>();
        scaledUpConfiguration.isRanged = isRanged;
        scaledUpConfiguration.damage = Mathf.FloorToInt(damage * scaling.DamageCurve.Evaluate(level));
        scaledUpConfiguration.attackRadius = attackRadius;
        scaledUpConfiguration.attackDelay = attackDelay;

        scaledUpConfiguration.bulletPrefab = bulletPrefab;
        scaledUpConfiguration.bulletSpawnOffset = bulletSpawnOffset;
        scaledUpConfiguration.lineOfSightLayers = lineOfSightLayers;

        return scaledUpConfiguration;
    }
    public void SetupEnemy(Enemy enemy)
    {
        (enemy.attackRadius.sphereCollider == null ? enemy.attackRadius.GetComponent<SphereCollider>() : enemy.attackRadius.sphereCollider).radius = attackRadius;
        enemy.attackRadius.attackDelay = attackDelay;
        enemy.attackRadius.damage = damage;

        if (isRanged)
        {
            RangeAttackRadius rangedAttackRadius = enemy.attackRadius.GetComponent<RangeAttackRadius>();

            rangedAttackRadius.bulletPrefab = bulletPrefab;
            rangedAttackRadius.bulletSpawnOffset = bulletSpawnOffset;
            rangedAttackRadius.layerMask = lineOfSightLayers;

            rangedAttackRadius.CreateBulletPool();
        }
    }
}

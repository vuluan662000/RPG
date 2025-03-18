using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class EnemyLineOfSightChecker : MonoBehaviour
{
    public SphereCollider sphereCollider;
    public float fieldOfView = 90f;
    public LayerMask lineOfSightLayer;

    public delegate void GainSightEvent(PlayerController player);
    public GainSightEvent onGainEvent;
    public delegate void LossSightEvent(PlayerController player);
    public LossSightEvent onLossEvent;

    private Coroutine CheckForLineOfSightCoroutine;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player;
        if(other.TryGetComponent<PlayerController>(out player))
        {
            if (!CheckLineOfSight(player))
            {            
                CheckForLineOfSightCoroutine =  StartCoroutine(CheckForLineOfSight(player));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {

        PlayerController player;
        if (other.TryGetComponent<PlayerController>(out player))
        {
            onLossEvent?.Invoke(player);
            if(CheckForLineOfSightCoroutine != null)
            {
                StopCoroutine(CheckForLineOfSightCoroutine);
            }
        }
    }

    private bool CheckLineOfSight(PlayerController player)
    {

        Vector3 direction = (player.transform.position - transform.position).normalized;
        float angle = Vector3.Dot(transform.forward, direction);
        float threshold = Mathf.Cos(fieldOfView * Mathf.Deg2Rad);


        if (angle >= threshold)
        {
            RaycastHit hit;
            Vector3 raycastOrigin = transform.position + Vector3.up * 1.5f; // Nâng điểm bắn lên

            Debug.DrawRay(raycastOrigin, direction * sphereCollider.radius, Color.red, 1f);

            if (Physics.Raycast(raycastOrigin, direction, out hit, sphereCollider.radius, lineOfSightLayer))
            {

                if (hit.transform.GetComponent<PlayerController>() != null)
                {
                    onGainEvent?.Invoke(player);
                    return true;
                }
            }
        }
        return false;
    }

    private IEnumerator CheckForLineOfSight(PlayerController player)
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (!CheckLineOfSight(player))
        {
            yield return wait;
        }
    }
}

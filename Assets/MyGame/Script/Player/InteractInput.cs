using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractInput : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI textOnScreen;
    InteractableObject hoveringOverObject;
    void Update()
    {
        CheckInteractObject();

        if(Input.GetMouseButtonDown(0))
        {
            if(hoveringOverObject != null)
            {
                hoveringOverObject.Interact();
            }    
        }    
    }
    public void CheckInteractObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;

        int layerMask = ~LayerMask.GetMask("EnemyAttackRadius");

        hits = Physics.RaycastAll(ray, Mathf.Infinity, layerMask);

        foreach (RaycastHit hit in hits)
        {
            InteractableObject interactableObject = hit.transform.GetComponent<InteractableObject>();
            if (interactableObject != null)
            {
                hoveringOverObject = interactableObject;
                textOnScreen.text = hoveringOverObject.name;
                return; 
            }
        }

        hoveringOverObject = null;
        textOnScreen.text = "";
    }    
}

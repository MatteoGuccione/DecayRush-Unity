using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] List<GameObject> weaponPrefabs;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float spawnForce = 10f;
    [SerializeField] int maxUses = 3;
    private int currentUses;

    [Header("Held Weapon Settings")]
    [SerializeField] Transform weaponHoldPoint; 
    [SerializeField] float holdDuration = 10f;

    private float timer = 0f;
    private bool timerIsOn;
    private GameObject heldWeapon;       
    private Coroutine holdTimerCoroutine; 

    
    private GameObject nearestPickupableWeapon; 

    void Start()
    {
        currentUses = maxUses; 
        timerIsOn = false;
    }

    void Update()
    {
        if (timerIsOn)
        {
            timer += Time.deltaTime;
            if (timer > 20f)
            {
                timer = 0;
                timerIsOn= false;
            }
        }
        else if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKey(KeyCode.JoystickButton2)) && timerIsOn == false) //for joystick XBOX use B for PS use Circle
        {
            Debug.Log("Spawned");
            timerIsOn = true;
            if (heldWeapon == null && currentUses > 0)
            {
                GenerateWeapon(); 
                currentUses--;    
            }
            else if (currentUses == 0)
            {
                Debug.Log("Non ci sono pi� oggetti da generare!");
            }
        }

       
        
    }

    void GenerateWeapon()
    {
        if (weaponPrefabs == null || weaponPrefabs.Count == 0)
        {
            
            return;
        }

        int randomIndex = Random.Range(0, weaponPrefabs.Count);
        GameObject chosenWeaponPrefab = weaponPrefabs[randomIndex];

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        GameObject newWeapon = Instantiate(chosenWeaponPrefab, spawnPosition, spawnRotation);

        Rigidbody weaponRb = newWeapon.GetComponent<Rigidbody>();
        if (weaponRb != null)
        {
            Vector3 forwardForce = spawnPoint != null ? spawnPoint.forward : transform.forward;
            weaponRb.AddForce(forwardForce * spawnForce, ForceMode.Impulse);
            weaponRb.AddForce(Vector3.up * (spawnForce * 0.5f), ForceMode.Impulse);
        }

        //Debug.Log("Generata arma: " + newWeapon.name + ". Usi rimanenti: " + currentUses);
       
    }

    IEnumerator HoldWeapon(GameObject weaponToHold) 
    {
        heldWeapon = weaponToHold; 
        nearestPickupableWeapon = null; 

        Rigidbody weaponRb = heldWeapon.GetComponent<Rigidbody>();
        if (weaponRb != null)
        {
            weaponRb.isKinematic = true; 
        }

        
        Collider weaponCollider = heldWeapon.GetComponent<Collider>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        weaponToHold.transform.SetParent(weaponHoldPoint);
        weaponToHold.transform.localPosition = Vector3.zero; 
        weaponToHold.transform.localRotation = Quaternion.identity; 

        Debug.Log("Arma raccolta: " + weaponToHold.name);

        yield return new WaitForSeconds(holdDuration);

        Debug.Log("Arma distrutta: " + weaponToHold.name);
        Destroy(heldWeapon);
        heldWeapon = null; 
        holdTimerCoroutine = null; 
    }

   
    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Weapon") && other.GetComponent<Rigidbody>() != null)
        {
           
            if (other.gameObject != heldWeapon)
            {
                nearestPickupableWeapon = other.gameObject;
                Debug.Log("Rilevata arma raccoglibile: " + nearestPickupableWeapon.name + ". Premi 'F' per raccoglierla.");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
       
        if (other.gameObject == nearestPickupableWeapon)
        {
            nearestPickupableWeapon = null;
            Debug.Log("Arma raccoglibile uscita dal raggio.");
        }
    }

    public void AddUses(int amount)
    {
        currentUses = Mathf.Min(currentUses + amount, maxUses);
    }

    public void ResetUses()
    {
        currentUses = maxUses;
    }
}

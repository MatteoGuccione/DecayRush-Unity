using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class InteractManager : MonoBehaviour
{
    [SerializeField] PlayerManager player;
    [SerializeField] GameObject solved;
    bool isResolved = false;

    //Check if the player has the item needed to progress the stage

    private void Start()
    {
        if (this.player == null)
        {
            GameObject gameObject = GameObject.FindWithTag("Player");
            this.player = gameObject.GetComponent<PlayerManager>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (player.Inventory.Contains("Fuel") && isResolved == false)
            {
                isResolved = true;
                GetComponent<AudioSource>().Play();
                Destroy(solved);
                player.Inventory.Remove("Fuel");
            }
            if (player.Inventory.Contains("Key"))
            {
                SceneLoader.LoadScene("MainMenu");
            }
        }
        
    }
}

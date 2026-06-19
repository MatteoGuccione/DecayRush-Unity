using UnityEngine;

public class EnemyResearchButton : MonoBehaviour
{
    [SerializeField] private EnemySpawner enemySpawner;
    
    public bool EnemyIsActive = false;

    private void Start()
    {
        //enemySpawner.enabled = false;
    }

    public void OnOffEnemyScript() //Set Enemy can take player or not
    {
        if (EnemyIsActive == false) //Go take Player
        {
            enemySpawner.enabled = true;

        }
        else if (EnemyIsActive == true) //Dont go take Player
        {
            //baseEnemy.enabled = false;
            //baseEnemy.Agent.isStopped = true;
            //baseEnemy.Agent.destination = baseEnemy.transform.position;
            //EnemyIsActive = false;
            enemySpawner.enabled = false;
        }
    }
}

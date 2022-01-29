using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem deadParticles;
    [SerializeField]
    private ParticleSystem respawnParticles;

    [SerializeField]
    private GameSettings gSettings;


    private Rigidbody pRigidbody;

    bool isGGWP = false;
    // Start is called before the first frame update
    void Start()
    {

        pRigidbody = this.GetComponent<Rigidbody>();
        this.GGWP(false);

    }
    
    public void GGWP(bool isItReal) 
    {
        if (!isItReal) 
        {
            this.pRigidbody.AddForce(Vector3.right * gSettings.PlayerSpeed, ForceMode.Impulse);
        }
        else 
        {
            this.pRigidbody.velocity = Vector3.zero;
        }
        isGGWP = isItReal;
    }

    public void Respawn() 
    {
        this.transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position + new Vector3(-3, 2, 0);
        this.GetComponent<TrailRenderer>().Clear();
        respawnParticles.Play(false);
        GGWP(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if ( !other.gameObject.transform.parent.gameObject.GetComponent<BlockMechanics>().isBlocksKiller()) 
            {
                return;
            }
            else 
            {
                if (!isGGWP)
                {
                    GGWP(true);
                    Debug.LogError("GameOver");
                    StartCoroutine(dieAnimation());
                }

            }
            
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            // GG WP
            Debug.LogError("GG WP");
            pRigidbody.velocity = Vector3.zero;
            GGWP(true);
            // TODO
            Camera.main.GetComponent<GameController>().restartGame();
        }
    }

    private IEnumerator dieAnimation() 
    {
        deadParticles.Play(false);

        //yield return new WaitUntil(() => { return !this.isGGWP; });
        yield return new WaitForSeconds(2f);

        float startTime = deadParticles.time;
        float simulationTime = startTime / deadParticles.main.simulationSpeed;
        float deltaTime = deadParticles.main.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        while (simulationTime > 0.0f)
        {
            deadParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            deadParticles.Play(false);

            simulationTime -= (deltaTime * deadParticles.main.simulationSpeed) * gSettings.PlayerRewindSpeedMultiplier;

            deadParticles.Simulate(simulationTime, false, false, true);
  
            yield return new WaitForEndOfFrame();
        }
        deadParticles.Play(false);
        deadParticles.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);

        Camera.main.GetComponent<GameController>().resetGame();
    }
}

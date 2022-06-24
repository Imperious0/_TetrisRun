using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem deadParticles;
    [SerializeField]
    private ParticleSystem respawnParticles;

    [SerializeField]
    private GameSettings gSettings;

    //Events
    public EventHandler<endGameEventArgs> endGameEvent;

    private Rigidbody pRigidbody;

    bool isGameEnd = false;
    bool isGGWP = false;
    // Start is called before the first frame update
    void Start()
    {
        pRigidbody = this.GetComponent<Rigidbody>();

        if (PlayerPrefs.GetInt("IsFirstTimePlay", 1) != 1)
        {
            this.Respawn();
        }
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
    public void Restart()
    {
        if (!isGGWP && isGameEnd)
            isGameEnd = false;
        else
            Respawn();
    }
    public void Respawn() 
    {
        this.transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position + new Vector3(-3, 2, 0);
        this.GetComponent<TrailRenderer>().Clear();
        this.pRigidbody.AddForce(Vector3.right * gSettings.PlayerSpeed, ForceMode.Impulse);
        isGameEnd = false;
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
                if (!isGGWP && !isGameEnd)
                {

                    Debug.LogError("GameOver");
                    isGGWP = false;
                    isGameEnd = true;
                    pRigidbody.velocity = Vector3.zero;
                    StartCoroutine(dieAnimation());
                    this.endGameEvent?.Invoke(this, new endGameEventArgs(false));
                }

            }
            
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            // GG WP
            Debug.LogError("GG WP");
            pRigidbody.velocity = Vector3.zero;
            isGGWP = true;
            isGameEnd = true;
            this.endGameEvent?.Invoke(this, new endGameEventArgs(true));
        }
    }

    
    private IEnumerator dieAnimation() 
    {
        deadParticles.Play(false);
        //yield return new WaitUntil(() => { return !this.isGGWP; });
        yield return new WaitUntil(() => { return !isGameEnd; });

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

        this.transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position + new Vector3(-3, 2, 0);
        this.GetComponent<TrailRenderer>().Clear();
        this.pRigidbody.AddForce(Vector3.right * gSettings.PlayerSpeed, ForceMode.Impulse);
    }
    public class endGameEventArgs : EventArgs
    {
        bool isGameSuccess;
        public endGameEventArgs(bool isGameSuccess)
        {
            this.isGameSuccess = isGameSuccess;
        }
        public bool IsGameSuccess { get => isGameSuccess; }
    }
}

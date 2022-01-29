using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Focus Settings")]
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform pivot;
    [SerializeField]
    private Vector3 offset;


    [Header("Effect Settings")]
    [SerializeField]
    private float _effectDuration = 1;
    [SerializeField]
    private AnimationCurve aCurve;

    private bool _isStartShake = false;


    Vector3 startPosition = Vector3.zero;

    private void Start()
    {
        if(pivot == null) 
        {
            pivot = this.transform;
        }
        pivot.position = target.position + offset;
        startPosition = this.gameObject.transform.localPosition;
    }

    private void FixedUpdate()
    {
        if(pivot.position != (target.position + offset)) 
        {
            pivot.position = target.position + offset;
        }

    }

    public void ShakeIt() 
    {
        StartCoroutine(ShakeItWorker());
    }
    private IEnumerator ShakeItWorker() 
    {

        float elapsedTime = 0f;

        while(elapsedTime < _effectDuration) 
        {
            elapsedTime += Time.deltaTime;
            float effectStrength = aCurve.Evaluate(elapsedTime / _effectDuration);
            Vector3 power = Random.insideUnitSphere * effectStrength;
            power = Vector3.Scale(power, Vector3.up);
            transform.localPosition = startPosition + power;
            yield return new WaitForEndOfFrame();
        }
        this.gameObject.transform.localPosition = startPosition;
    }
}

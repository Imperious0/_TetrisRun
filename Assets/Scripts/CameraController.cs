using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform camTransfrom;

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


    Vector3 startPosition = Vector3.zero;

    private void Start()
    {
        if(pivot == null) 
        {
            pivot = transform;
        }
        camTransfrom = transform;
        pivot.position = target.position + offset;
        startPosition = camTransfrom.localPosition;
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
        startPosition = camTransfrom.localPosition;
        while(elapsedTime < _effectDuration) 
        {
            elapsedTime += Time.deltaTime;
            float effectStrength = aCurve.Evaluate(elapsedTime / _effectDuration);
            Vector3 power = Random.insideUnitSphere * effectStrength;
            power = Vector3.Scale(power, Vector3.up);
            camTransfrom.localPosition = startPosition + power;
            yield return new WaitForEndOfFrame();
        }
        camTransfrom.localPosition = startPosition;
    }
}

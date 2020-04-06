using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField]
    Vector3 movementVector;

    [SerializeField]
    float period = 2f;

    private float movementFactor;
    private Vector3 startingPos;

    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }

        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2f;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementFactor * movementVector;
        transform.position = startingPos + offset;
    }
}

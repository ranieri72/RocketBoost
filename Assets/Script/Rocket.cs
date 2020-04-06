using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] AudioClip mainEngineAudio;
    [SerializeField] AudioClip successAudio;
    [SerializeField] AudioClip deathAudio;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    private const string friendlyTag = "Friendly";
    private const string finishTag = "Finish";
    private const float successTime = 2f;
    private const float deathTime = 2.5f;

    private Rigidbody rigidbody;
    private AudioSource audioSource;
    bool colisionDisabled = false;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }
    }

    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            colisionDisabled = !colisionDisabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || colisionDisabled) { return; }

        switch (collision.gameObject.tag)
        {
            case friendlyTag:
                // do nothing
                break;
            case finishTag:
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(successAudio);
        successParticles.Play();
        Invoke("LoadNextLevel", successTime);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathAudio);
        deathParticles.Play();
        Invoke("ReloadLevel", deathTime);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentSceneIndex + 1;
        if (nextScene == SceneManager.sceneCountInBuildSettings) {
            nextScene = 0;
        }
        SceneManager.LoadScene(nextScene);
    }

    private void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager. LoadScene(currentSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        float rotationThisFrame = mainThrust * Time.deltaTime;

        rigidbody.AddRelativeForce(Vector3.up * rotationThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineAudio);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidbody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidbody.freezeRotation = false;
    }
}

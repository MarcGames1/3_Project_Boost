using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2.3f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;


    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    bool collisionsareDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    void Update()
    {
        Movement();
        
        if (Debug.isDebugBuild) //works only in debug mode
        {
        RespondToDebugKeys();
        }

       
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsareDisabled = !collisionsareDisabled; //toggle collision
        }
    }

    private void Movement()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive|| collisionsareDisabled) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
            break;
            case "Finish":
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
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
       
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();

        Invoke("StartFirstScene", levelLoadDelay);
    }

   
    private void StartFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int  currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex+1;
        if (nextSceneIndex != SceneManager.sceneCountInBuildSettings)
        {
        SceneManager.LoadScene(nextSceneIndex); //todo allow for more than 2levels
        }
        else { StartFirstScene(); }
          
        
        
    }

    private void RespondToThrustInput() //key binding controls + particles
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
        mainEngineParticles.Play();
        float thrustForce = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustForce);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
    } //controls ony when state is alive

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation

        float rotationSpeed = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }
        rigidBody.freezeRotation = false; // resume phisics control of rotation
    } //controls ony when state is alive
    
  
}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// Manages a single DummyBot who acts randomly on the environment.
/// </summary>
public class BouncyBot : MonoBehaviour
{
    [Tooltip("The material when the bot is healthy")]
    public Material healthyMaterial;

    [Tooltip("The material when the bot is infected")]
    public Material infectiousMaterial;

    [Tooltip("The material when the bot is infected")]
    public Material recoveredMaterial;

    [Tooltip("The maximum possible distance for exposure to occur aka radius")]
    [HideInInspector]
    public float exposureRadius = 8f;

    [Tooltip("Infection Coefficient")]
    [HideInInspector]
    public float infectionCoeff;

    [Tooltip("The probability of exposure at that maximum distance")]
    [Range(0.0f, 0.001f)]
    public float probability;

    //The PandemicArea
    private PandemicArea pandemicArea;

    //The gameObject of the Pandemic Area
    private GameObject pandemicAreaObj;

    // Speed of agent rotation.
    public float turnSpeed = 300;

    // Speed of agent movement.
    public float moveSpeed = 2;

    //Check if agent is frozen or not;
    public bool isFrozen = false;

    private Vector3 initialVelocity;

    //Last Velocity 
    private Vector3 lastFrameVelocity;

    //Constant Speed
    public float constantSpeed = 30f;
    //Rigidbody
    private Rigidbody rb;

    /// <summary>
    /// States for being healthy or infectious
    /// </summary>
    public enum agentStatus
    {
        HEALTHY,
        INFECTED,
        RECOVERED
    }
    [Tooltip("Recovery time after the infection starts")]
    public float recoverTime;

    public agentStatus m_InfectionStatus = agentStatus.HEALTHY;

    public void changeAgentStatus()
    {
        switch (m_InfectionStatus)
        {
            case agentStatus.HEALTHY:
                GetComponentInChildren<Renderer>().material = healthyMaterial;
                break;
            case agentStatus.INFECTED:
                GetComponentInChildren<Renderer>().material = infectiousMaterial;
                pandemicAreaObj.GetComponent<PandemicArea>().healthyCounter--;
                pandemicAreaObj.GetComponent<PandemicArea>().infectedCounter++;
                //Add - reward here.
                break;
            case agentStatus.RECOVERED:
                GetComponentInChildren<Renderer>().material = recoveredMaterial;
                pandemicAreaObj.GetComponent<PandemicArea>().infectedCounter--;
                pandemicAreaObj.GetComponent<PandemicArea>().recoveredCounter++;
                break;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Bounce(collision.contacts[0].normal);
    }
    private void Bounce(Vector3 collisionNormal)
    {
        var tweak = new Vector3(Random.Range(0, 1f), 0, Random.Range(0, 1f));
        var direction = Vector3.Reflect((lastFrameVelocity + tweak).normalized, collisionNormal);
        rb.velocity = direction * constantSpeed;
    }

    /// <summary>
    /// Called when the agent's collider enters a trigger collider
    /// </summary>
    /// <param name="other">The trigger collider</param>
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnterOrStay(other);
    }

    /// <summary>
    /// Called when the agent's collider stays in a trigger collider
    /// </summary>
    /// <param name="other">The trigger collider</param>
    private void OnTriggerStay(Collider other)
    {
        TriggerEnterOrStay(other);
    }

    /// <summary>
    /// Handles when the agen'ts collider enters or stays in a trigger collider
    /// </summary>
    /// <param name="collider">The trigger collider</param>
    private void TriggerEnterOrStay(Collider collider)
    {

        //Check if our agent is healthy, otherwise there is nothing like reinfection
        if (m_InfectionStatus == agentStatus.HEALTHY)
        {

            //Check if its a dummyBot   
            if (collider.CompareTag("dummyBot"))
            {
                //If it is infected 
                if (collider.gameObject.GetComponent<BouncyBot>().m_InfectionStatus == BouncyBot.agentStatus.INFECTED)
                {
                    exposeInfection(collider.gameObject);
                }
            }
        }

    }
    /// <summary>
    /// Gets the distance between agents and expose with infenction probability.
    /// There is an inverse proportion between distance and infection probability.
    /// </summary>
    /// <param name="infector">The agent who is inside of the collider</param>
    private void exposeInfection(GameObject infector)
    {
        //Distance between two agents
        float distance = Vector3.Distance(infector.transform.position, transform.position);
        probability = Mathf.InverseLerp(exposureRadius, 0, distance) / infectionCoeff;

        //Debug.Log("Probability of getting infected is: " + probability);

        if (Random.Range(0f, 1f) < probability)
        {
            // Debug.Log("You got infected");
            m_InfectionStatus = agentStatus.INFECTED;
            changeAgentStatus();
        }
    }

    private void Awake()
    {
        //Get the PandemicArea
        pandemicArea = GetComponentInParent<PandemicArea>();
        pandemicAreaObj = pandemicArea.gameObject;

        GetComponent<SphereCollider>().radius = exposureRadius;
        recoverTime = pandemicArea.recoverTime;

        rb = GetComponent<Rigidbody>();
        initialVelocity = new Vector3(UnityEngine.Random.Range(5, 20), 0, UnityEngine.Random.Range(5, 20));
        //Debug.Log("initial Vel:" + initialVelocity);
        rb.velocity = initialVelocity.normalized * constantSpeed;



    }

    private void FixedUpdate()
    {
        if (!isFrozen)
        {
            lastFrameVelocity = rb.velocity;
        }
        if (m_InfectionStatus == agentStatus.INFECTED)
        {
            if (recoverTime <= 0)
            {
                m_InfectionStatus = agentStatus.RECOVERED;
                changeAgentStatus();
            }
            else
            {
                recoverTime -= Time.deltaTime;
            }
        }

    }
}

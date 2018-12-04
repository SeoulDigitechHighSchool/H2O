using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FishObject : MonoBehaviour

{
    public List<GameObject> fishModel = new List<GameObject>();
    public Transform tankCenterGoal;

    public float obstacleSensingDistance = 0.8f;

    public float swimSpeedMin = 0.2f;
    public float swimSpeedMax = 0.6f;
    public float maxTurnRateY = 5f;
    public float maxWanderAngle = 45f;
    public float wanderPeriodDuration = 0.8f;
    public float wanderProbability = 0.15f;

    public bool isWiggle = true;
    public bool isRandomBody = true;

    public bool IsOut = false;

    [HideInInspector]
    public float swimSpeed;

    private Vector3 swimDirection
    {
        get { return transform.TransformDirection(Vector3.forward); }
    }

    private bool obstacleDetected = false;
    private float wanderPeriodStartTime;
    private Quaternion goalLookRotation;
    private Transform bodyTransform;
    private float randomOffset;
    private Vector3 hitPoint;
    private Vector3 goalPoint;

    void Start()
    {
        float randomSize = Random.Range(0.2f, 0.5f);
        tankCenterGoal = Camera.main.transform;
        bodyTransform = transform.Find("Body");
        if (isRandomBody)
        {
            Instantiate(fishModel[Random.Range(0, fishModel.Count)], bodyTransform).transform.localScale = new Vector3(randomSize, randomSize, randomSize);
        }
        randomOffset = Random.value;
    }

    private void FixedUpdate()
    {
        Wiggle();
        Wander();
        AvoidObstacles();
        CheckOuter();
        DrawDebugAids();
        UpdatePosition();
    }

    private void OnDrawGizmos()
    {
        DrawDebugAids();
    }

    private void OnTriggerStay(Collider Collider)
    {
        if (Collider.gameObject.tag.Equals("range"))
        {
            IsOut = false;
        }
    }
   
    void OnTriggerExit(Collider Collider)
    {
        if (Collider.gameObject.tag.Equals("range"))
        {
            IsOut = true;
        }
    }


    void Wiggle()
    {
        if (!isWiggle) return;
        float speedPercent = swimSpeed / swimSpeedMax;
        float minWiggleSpeed = 12f;
        float maxWiggleSpeed = minWiggleSpeed + 1f;
        float wiggleSpeed = Mathf.Lerp(minWiggleSpeed, maxWiggleSpeed, speedPercent);

        float angle = Mathf.Sin(Time.time * wiggleSpeed) * 5f;
        var wiggleRotation = Quaternion.AngleAxis(angle, Vector3.up);

        bodyTransform.localRotation = wiggleRotation;
    }

    void Wander()
    {
        float noiseScale = .5f;
        float speedPercent = Mathf.PerlinNoise(Time.time * noiseScale + randomOffset, randomOffset);
        speedPercent = Mathf.Pow(speedPercent, 2);
        swimSpeed = Mathf.Lerp(swimSpeedMin, swimSpeedMax, speedPercent);

        if (obstacleDetected) return;

        if (Time.time > wanderPeriodStartTime + wanderPeriodDuration)
        {
            wanderPeriodStartTime = Time.time;

            if (Random.value < wanderProbability)
            {
                var randomAngle = Random.Range(-maxWanderAngle, maxWanderAngle);
                var relativeWanderRotation = Quaternion.AngleAxis(randomAngle, Vector3.up);
                goalLookRotation = transform.rotation * relativeWanderRotation;
            }
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, goalLookRotation, Time.deltaTime / 2f);
    }

    void AvoidObstacles()
    {
        RaycastHit hit;
        obstacleDetected = Physics.Raycast(transform.position, swimDirection, out hit, obstacleSensingDistance);

        if (obstacleDetected)
        {
            hitPoint = hit.point;

            Vector3 reflectionVector = Vector3.Reflect(swimDirection, hit.normal);
            float goalPointMinDistanceFromHit = 1f;
            Vector3 reflectedPoint = hit.point + reflectionVector * Mathf.Max(hit.distance, goalPointMinDistanceFromHit);

            goalPoint = (reflectedPoint + tankCenterGoal.position) / 2f;

            Vector3 goalDirection = goalPoint - transform.position;
            goalLookRotation = Quaternion.LookRotation(goalDirection);

            float dangerLevel = Mathf.Pow(1 - (hit.distance / obstacleSensingDistance), 4f);

            dangerLevel = Mathf.Max(0.01f, dangerLevel);

            float turnRate = maxTurnRateY * dangerLevel;

            Quaternion rotation = Quaternion.Slerp(transform.rotation, goalLookRotation, Time.deltaTime * turnRate);
            transform.rotation = rotation;
        }
    }

    void CheckOuter()
    {
        if (IsOut)
        {
            var resive = tankCenterGoal.position - transform.position;
            goalLookRotation = Quaternion.LookRotation(resive);
            float turnRate = maxTurnRateY;
            Quaternion rotation = Quaternion.Slerp(transform.rotation, goalLookRotation, Time.deltaTime * turnRate);
            transform.rotation = rotation;
        }
    }
    
    void DrawDebugAids()
    {
        Color rayColor = obstacleDetected ? Color.red : Color.cyan;
        Debug.DrawRay(transform.position, swimDirection * obstacleSensingDistance, rayColor);
        
        if (obstacleDetected)
        {
            Debug.DrawLine(hitPoint, goalPoint, Color.green);
        }
    }

    private void UpdatePosition()
    {
        Vector3 position = transform.position + swimDirection * swimSpeed * Time.fixedDeltaTime;
        transform.position = position;
    }
}
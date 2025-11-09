using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int maxHP = 100;
    public int currentHP;
    public Move[] moves;
    public Transform model;   // reference to mesh/model
    public float attackForce = 7f; // new

    [HideInInspector] 
    public bool hasCollided = false;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        // Prevent spinning from physics forces
        rb.freezeRotation = true;
    }

    // Detect collision (called during smash attack)
    void OnCollisionEnter(Collision collision)
    {
        hasCollided = true;
    }
    
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
    }

    IEnumerator ShakeDamageEffect()
    {
        Vector3 originalPos = transform.localPosition;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localPosition = originalPos + (Random.insideUnitSphere * 0.05f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;

        // ✅ turn physics back on after animation
        GetComponent<Rigidbody>().isKinematic = false;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnAfterSeconds : MonoBehaviour
{
    [SerializeField] private float despawnTimeInSeconds = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("Despawn", despawnTimeInSeconds);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}

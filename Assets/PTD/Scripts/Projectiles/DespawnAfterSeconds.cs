//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

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

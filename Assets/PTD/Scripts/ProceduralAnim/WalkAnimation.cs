using UnityEngine;
using Unity.Mathematics;


sealed class WalkAnimation : MonoBehaviour
{
    #region Editable attributes

    [Space]
    [SerializeField] Transform _hip = null;
    [SerializeField] Transform _handL = null;
    [SerializeField] Transform _handR = null;
    [SerializeField] Transform _footL = null;
    [SerializeField] Transform _footR = null;
    [Space]
    [SerializeField] uint _seed = 100;
    [SerializeField, Range(0, 20)] float _walkSpeed = 10;
    [SerializeField, Range(0, 2)] float _armSwing = 0.5f;
    [SerializeField, Range(0, 0.5f)] float _stride = 0.12f;
    [SerializeField, Range(0, 0.2f)] float _footUp = 0.06f;
    [SerializeField, Range(0, 0.2f)] float _hipUp = 0.02f;
    [SerializeField, Range(0, 1)] float _hipShake = 0.2f;
    [SerializeField, Range(0, 0.2f)] float _noise = 0.02f;
    [SerializeField] public float Amplitude = 0.1f;
    #endregion

    #region Public properties


    #endregion

    #region Private variables

    float _time;

    #endregion

    #region MonoBehavour implementation

    void Update()
    {
        // PRNG
        

        // Time parameter
        

        // Hip target
        {
            var walk = math.sin(_time * 2 + math.PI * 0.25f) * _hipUp * Amplitude;
            var move = _noise;
            _hip.localPosition = move + math.float3(0, walk - 0.2f * _noise, 0);
            _hip.localRotation = quaternion.RotateY(math.sin(_time) * -_hipShake * Amplitude);
        }

        // Hand targets
        {
            var phi = math.sin(_time) * _armSwing * Amplitude;
            _handL.parent.localRotation = quaternion.RotateY(-phi);
            _handR.parent.localRotation = quaternion.RotateY(+phi);
        }

        // Foot targets
        {
            var y = math.cos(_time) * -_footUp * Amplitude;
            var z = math.sin(_time) * _stride * Amplitude;
            _footL.localPosition = math.float3(0, math.min(0, -y), +z);
            _footR.localPosition = math.float3(0, math.min(0, +y), -z);
        }

        // Time step
        _time += Time.deltaTime * _walkSpeed * math.pow(Amplitude, 0.5f);
    }

    #endregion
}

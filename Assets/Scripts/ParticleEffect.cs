using UnityEngine;

public class ParticleEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;

    public void EmitAtPosition(Vector3 position, int count = 30)
    {
        ParticleSystem.EmitParams emitParams = new();

        emitParams.position = position;
        emitParams.applyShapeToPosition = true;

        ps.Emit(emitParams, count);
    }
}
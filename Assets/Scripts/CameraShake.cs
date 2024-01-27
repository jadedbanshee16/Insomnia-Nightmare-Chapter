using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class CameraShake : CinemachineExtension
{
    //How big is the shake.
    [Tooltip("Amplitude of the shake")]
    public float m_Range = 1f;

    //Get a pipeline call when camera moves (or when the post pipeline system runs)
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        //If the stage is currently at body, then add offset to correction.
        if (stage == CinemachineCore.Stage.Body)
        {
            Vector3 shakeAmount = GetOffset();
            state.PositionCorrection += shakeAmount * Time.fixedDeltaTime;
        }
    }

    //Using the range, get a random shake.
    Vector3 GetOffset()
    {
        return new Vector3(
            0,
            Random.Range(-m_Range, m_Range),
            0);
    }
}

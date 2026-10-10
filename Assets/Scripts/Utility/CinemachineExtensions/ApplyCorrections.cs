using UnityEngine;


[Unity.Cinemachine.SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
public class ApplyCorrections : Unity.Cinemachine.CinemachineExtension
{
    protected override void PostPipelineStageCallback(
        Unity.Cinemachine.CinemachineVirtualCameraBase vcam,
        Unity.Cinemachine.CinemachineCore.Stage stage, ref Unity.Cinemachine.CameraState state, float deltaTime)
    {
        if (stage == Unity.Cinemachine.CinemachineCore.Stage.Finalize)
        {
            vcam.transform.position = state.PositionCorrection;
            state.RawPosition = state.PositionCorrection;
            state.PositionCorrection = Vector3.zero;
            state.RawOrientation = state.OrientationCorrection;
            state.OrientationCorrection = Quaternion.identity;
        }
    }
}
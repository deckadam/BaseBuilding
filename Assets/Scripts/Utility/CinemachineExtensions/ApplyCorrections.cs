using UnityEngine;
using Cinemachine;

[SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
public class ApplyCorrections : CinemachineExtension
{
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Finalize)
        {
            vcam.transform.position = state.CorrectedPosition;
            state.RawPosition = state.CorrectedPosition;
            state.PositionCorrection = Vector3.zero;
            state.RawOrientation = state.CorrectedOrientation;
            state.OrientationCorrection = Quaternion.identity;
        }
    }
}
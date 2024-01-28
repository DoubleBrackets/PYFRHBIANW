using UnityEngine;

public class CoopCamera : MonoBehaviour
{
    [field: SerializeField] public Camera Cam { get; private set; }

    [field: SerializeField] public GameObject CameraContainer { get; private set; }

    [field: SerializeField] public Canvas CoopUICanvas { get; private set; }

    [field: SerializeField] public RectTransform CoopUICanvasRoot { get; private set; }

    [field: SerializeField] public Transform PlayerModel { get; private set; }


    public int PlayerNumber { get; set; }

    public void Initialize(int playerNumber)
    {
        PlayerNumber = playerNumber;
        CameraSplitScreenManager.Instance.AddCamera(this);
        Cam.cullingMask |= 1 << (32 - playerNumber);
        // player model ignore
        Cam.cullingMask -= 1 << (16 - playerNumber);
        CameraContainer.gameObject.layer = 32 - playerNumber;


        PlayerModel.gameObject.layer = 16 - playerNumber;
    }

    public void SetupMultipleDisplay(int displayIndex)
    {
        Cam.targetDisplay = displayIndex;
        Cam.rect = new Rect(0, 0, 1f, 1f);
        CoopUICanvas.targetDisplay = displayIndex;
        CoopUICanvasRoot.offsetMin = new Vector2(0, 0);
        CoopUICanvasRoot.offsetMax = new Vector2(0, 0);
    }

    public void SetupSplitScreen(Rect cameraRect)
    {
        Cam.rect = cameraRect;
        CoopUICanvas.targetDisplay = 0;
        CoopUICanvasRoot.sizeDelta = Vector2.zero;
        CoopUICanvasRoot.offsetMax = new Vector2(0, 0);
    }

    private void OnDestroy()
    {
        CameraSplitScreenManager.Instance.RemoveCamera(this);
    }
}
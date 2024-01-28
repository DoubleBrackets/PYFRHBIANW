using UnityEngine;

public class SFXOneshotPlayer : MonoBehaviour
{
    public static SFXOneshotPlayer Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PlaySFXOneshot(AudioClip audioClip)
    {
        AudioSource.PlayClipAtPoint(audioClip, Vector3.zero);
    }
}
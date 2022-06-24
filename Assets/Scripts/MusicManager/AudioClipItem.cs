using UnityEngine;

[System.Serializable]
public class AudioClipItem : IAudioClip
{
    [SerializeField]
    string _clipTag;
    [SerializeField]
    AudioClip _currentClip;
    [SerializeField]
    bool _isOneShot;

    public string GetClipName { get => _clipTag; }

    public AudioClip GetClip { get => _currentClip; }

    public bool IsOneShot { get => _isOneShot; }

    public AudioClipItem()
    {

    }
}

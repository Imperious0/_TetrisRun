using UnityEngine;

public interface IAudioClip
{
    string GetClipName { get; }
    AudioClip GetClip { get; }
    bool IsOneShot { get; }
}

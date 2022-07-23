using UnityEngine;

public interface IAudioSource
{
    void muteSource(bool isMuted);
    void toggleSourceStatus();
    void playClip(IAudioClip audioClip);
    void playClipSelf(string clipName);
    AudioSource Source { get; }
}
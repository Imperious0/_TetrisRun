using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAudioClip
{
    string GetClipName { get; }
    AudioClip GetClip { get; }
    bool IsOneShot { get; }
}

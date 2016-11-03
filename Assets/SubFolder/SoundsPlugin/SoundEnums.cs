using System;


public enum SoundLoopMode
{
    DoNotLoop = 0,
    LoopSubitem = 1,
    LoopSequence = 2,
}


public enum SoundPickSubItemMode
{
    Disabled = 0,
    Random = 1,
    RandomNotSameTwice = 2,
    Sequence = 3,
}


public enum SoundClipType
{
    Unknown = 0,
    Clip = 1,
    StreamingClip = 2,
    Native = 3,
}
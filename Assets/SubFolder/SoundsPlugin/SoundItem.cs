using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class SoundItem
{
    public const float MIN_TIME_BETWEEN_PLAY_CALLS = 0.05f;

    public string Name;
    public SoundLoopMode Loop = SoundLoopMode.DoNotLoop;
    public int loopSequenceCount = 0;
    public bool DestroyOnLoad = true;
    public float Volume = 1;
    public SoundPickSubItemMode SubItemPickMode = SoundPickSubItemMode.RandomNotSameTwice;
    public int MaxInstanceCount = 0; // Assures that the same audio item will not be played more than <c>MaxInstanceCount</c> times simultaneously. Set to 0 to disable.
    public bool SkipWhenExeeded;
    public float Delay = 0;
    public float OverlapTime = 0;

    public bool overrideAudioSourceSettings = false;
    public float audioSource_MinDistance = 1;
    public float audioSource_MaxDistance = 500;

    [SerializeField] public SoundSettings SoundSettingsOverride;
    public SoundSubItem[] subItems;

    internal int lastChosen = -1;
    internal double lastPlayedTime = -1; // high precision system time

    [System.NonSerializedAttribute]
    private SoundsCategory category;


    public SoundsCategory Category
    {
        private set { category = value; }
        get { return category; }
    }


    void Awake()
    {
        lastChosen = -1;
    }


    internal void Initialize( SoundsCategory categ )
    {
        Category = categ;

        for (int i = 0; i < subItems.Length; i++)
        {
            subItems[i].Initialize( this );
        }

		ComputeNormalizedProbability();
    }


	private void ComputeNormalizedProbability()
    {
        float sum = 0.0f;
        int subItemID = 0;
      
		for (int j = 0; j < subItems.Length; j++) 
		{
			SoundSubItem i = subItems [j];
			sum += i.Probability;
			subItemID++;
		}

        float summedProb = 0;
		for (int j = 0; j < subItems.Length; j++) 
		{
			SoundSubItem i = subItems [j];
			summedProb += i.Probability / sum;
			i.SummedProbability = summedProb;
		}
    }


    public void UnloadAudioClip()
    {
//        foreach( var si in subItems )
//        {
//            if(si.Clip)
//            {
//                Resources.UnloadAsset( si.Clip );
//            }
//            else if (si.StreamingAudioClipData != null)
//            {
//                AudioController.Instance.StreamingAudioBuffer.UnloadClip(si.StreamingAudioClipData.Name);
//            }
//        }
    }


    public SoundSubItem[ ] ChooseSubItems(SoundPickSubItemMode pickMode)
    {
        if ( subItems == null ) return null;
        int arraySize = subItems.Length;
        if ( arraySize == 0 ) return null;

        int chosen = 0;
        SoundSubItem[] chosenItems;

        if ( arraySize > 1 )
        {
            switch ( pickMode )
            {
                case SoundPickSubItemMode.Disabled:
                    return null;
                case SoundPickSubItemMode.Random:
                    chosen = ChooseRandomSubitem(true);
                    break;
                case SoundPickSubItemMode.RandomNotSameTwice:
                    chosen = ChooseRandomSubitem(false);
                    break;
            }
        }

        lastChosen = chosen;

        chosenItems = new SoundSubItem[ 1 ];
        chosenItems[0] = subItems[ chosen ];

        return chosenItems;
    }


    public int ChooseRandomSubitem(bool allowSameElementTwiceInRow)
    {
        int arraySize = subItems.Length; // is >= 2 at this point 
        int chosen = 0;

        float probRange;
        float lastProb = 0;

        if ( !allowSameElementTwiceInRow )
        {
            // find out probability of last chosen sub item
            if ( lastChosen >= 0 )
            {
                lastProb = subItems[ lastChosen ].SummedProbability;
                if ( lastChosen >= 1 )
                {
                    lastProb -= subItems[ lastChosen - 1 ].SummedProbability;
                }
            }
            else
                lastProb = 0;

            probRange = 1.0f - lastProb;
        }
        else
            probRange = 1.0f;

        float rnd = UnityEngine.Random.Range( 0, probRange );

        int i;
        for ( i = 0; i < arraySize - 1; i++ )
        {
            float prob;

            prob = subItems[ i ].SummedProbability;

            if ( !allowSameElementTwiceInRow )
            {
                if ( i == lastChosen ) 
                {
                    if ( prob != 1.0f)
                    {
                        continue;
                    }  
                }

                if ( i > lastChosen )
                {
                    prob -= lastProb;
                }
            }

            if ( prob > rnd )
            {
                chosen = i;
                break;
            }
        }
        if ( i == arraySize - 1 )
        {
            chosen = arraySize - 1;
        }

        return chosen;
    }
}
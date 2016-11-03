#if false
#define AUDIO_PLUGIN
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


[CustomEditor( typeof(SoundsManager) )]
public class SoundsManagerEditor : SoundsEditor 
{
    public static bool globalFoldout = true;
    public static bool playlistFoldout = true;
    public static bool musicFoldout = true;
    public static bool categoryFoldout = true;
    public static bool itemFoldout = true;
    public static bool subitemFoldout = true;

    GUIStyle foldoutStyle;
    GUIStyle centeredTextStyle;
    GUIStyle popupStyleColored;
    GUIStyle textAttentionStyle;
    GUIStyle textAttentionStyleLabel;
    GUIStyle textInfoStyleLabel;

    int lastCategoryIndex = -1;
    int lastItemIndex = -1;
    int lastSubItemIndex = -1;


    SoundsManager AC 
    {
        get 
        {
            return target as SoundsManager;
        }
    }


    int currentCategoryIndex
    {
        get
        {
            return AC._currentInspectorSelection.currentCategoryIndex;
        }
        set
        {
            AC._currentInspectorSelection.currentCategoryIndex = value;
        }
    }


    int currentItemIndex
    {
        get
        {
            return AC._currentInspectorSelection.currentItemIndex;
        }
        set
        {
            AC._currentInspectorSelection.currentItemIndex = value;
        }
    }


    int currentSubitemIndex
    {
        get
        {
            return AC._currentInspectorSelection.currentSubitemIndex;
        }
        set
        {
            AC._currentInspectorSelection.currentSubitemIndex = value;
        }
    }


    int currentPlaylistIndex
    {
        get
        {
            return AC._currentInspectorSelection.currentPlaylistIndex;
        }
        set
        {
            AC._currentInspectorSelection.currentPlaylistIndex = value;
        }
    }


    SoundsCategory currentCategory
    {
        get
        {
            if ( currentCategoryIndex < 0 || AC.AudioCategories == null || currentCategoryIndex >= AC.AudioCategories.Length )
            {
                return null;
            }
            return AC.AudioCategories[ currentCategoryIndex ];
        }
    }


    SoundItem currentItem
    {
        get
        {
            SoundsCategory curCategory = currentCategory;

            if ( currentCategory == null )
            {
                return null;
            }

            if ( currentItemIndex < 0 || curCategory.AudioItems == null || currentItemIndex >= curCategory.AudioItems.Length )
            {
                return null;
            }
            return currentCategory.AudioItems[ currentItemIndex ];
        }
    }


    SoundSubItem currentSubItem
    {
        get
        {
            SoundItem curItem = currentItem;

            if ( curItem == null )
            {
                return null;
            }

            if ( currentSubitemIndex < 0 || curItem.subItems == null || currentSubitemIndex >= curItem.subItems.Length )
            {
                return null;
            }
            return curItem.subItems[ currentSubitemIndex ];
        }
    }

    public int currentCategoryCount
    {
        get {
            if( AC.AudioCategories != null )
            {
                return AC.AudioCategories.Length;
            }
            else 
                return 0;
        }
    }

    public int currentItemCount
    {
        get
        {
            if ( currentCategory != null )
            {
                if ( currentCategory.AudioItems != null )
                {
                    return currentCategory.AudioItems.Length;
                }
                return 0;
            }
            else
                return 0;
        }
    }

    public int currentSubItemCount
    {
        get
        {
            if ( currentItem != null )
            {
                if ( currentItem.subItems != null )
                {
                    return currentItem.subItems.Length;
                }
                return 0;
            }
            else
                return 0;
        }
    }

    const string _playWithInspectorNotice = "Volume and pitch of audios are only correct when played during playmode. You can ignore the following Unity warning (if any).";
    const string _playNotSupportedOnMac = "On MacOS playing audios is only supported during play mode.";
    const string _nameForNewCategoryEntry = "!!! Enter Unique Category Name Here !!!";
    const string _nameForNewAudioItemEntry = "!!! Enter Unique Audio ID Here !!!";



    protected override void LogUndo( string label )
    {
        Undo.RecordObject( target, "SoundsPlugin: " + label );
    }

    public new void SetStyles()
    {
        base.SetStyles();

        foldoutStyle = new GUIStyle( EditorStyles.foldout );

        var foldoutColor = new UnityEngine.Color( 0.0f, 0.0f, 0.2f );

        foldoutStyle.onNormal.background = EditorStyles.boldLabel.onNormal.background;
        foldoutStyle.onFocused.background = EditorStyles.boldLabel.onNormal.background;
        foldoutStyle.onActive.background = EditorStyles.boldLabel.onNormal.background;
        foldoutStyle.onHover.background = EditorStyles.boldLabel.onNormal.background;


        foldoutStyle.normal.textColor = foldoutColor;
        foldoutStyle.focused.textColor = foldoutColor;
        foldoutStyle.active.textColor = foldoutColor;
        foldoutStyle.hover.textColor = foldoutColor;
        foldoutStyle.fixedWidth = 500;

        centeredTextStyle = new GUIStyle( EditorStyles.label );
        centeredTextStyle.alignment = TextAnchor.UpperCenter;
        centeredTextStyle.stretchWidth = true;

        popupStyleColored = new GUIStyle( stylePopup );

        bool isDarkSkin = popupStyleColored.normal.textColor.grayscale > 0.5f;

        if ( isDarkSkin )
        {
            popupStyleColored.normal.textColor = new Color( 0.9f, 0.9f, 0.5f );
        } else
            popupStyleColored.normal.textColor = new Color( 0.6f, 0.1f, 0.0f );


        textAttentionStyle = new GUIStyle( EditorStyles.textField );

        if ( isDarkSkin )
        {
            textAttentionStyle.normal.textColor = new Color( 1, 0.3f, 0.3f );
        } else
            textAttentionStyle.normal.textColor = new Color( 1, 0f, 0f );

        textAttentionStyleLabel = new GUIStyle( EditorStyles.label );

        if ( isDarkSkin )
        {
            textAttentionStyleLabel.normal.textColor = new Color( 1, 0.3f, 0.3f );
        }
        else
            textAttentionStyleLabel.normal.textColor = new Color( 1, 0f, 0f );

        textInfoStyleLabel = new GUIStyle( EditorStyles.label );

        if ( isDarkSkin )
        {
            textInfoStyleLabel.normal.textColor = new Color( 0.4f, 0.4f, 0.4f );
        }
        else
            textInfoStyleLabel.normal.textColor = new Color( 0.6f, 0.6f, 0.6f );
    }


    public override void OnInspectorGUI()
    {
        SetStyles();

        BeginInspectorGUI();

        _ValidateCurrentCategoryIndex();
        _ValidateCurrentItemIndex();
        _ValidateCurrentSubItemIndex();

        if( lastCategoryIndex != currentCategoryIndex ||
            lastItemIndex != currentItemIndex ||
            lastSubItemIndex != currentSubitemIndex )
        {
            GUIUtility.keyboardControl = 0; // workaround for Unity weirdness not changing the value of a focused GUI element when changing a category/item
            lastCategoryIndex = currentCategoryIndex;
            lastItemIndex = currentItemIndex;
            lastSubItemIndex = currentSubitemIndex;
        }


        EditorGUILayout.Space();

        if ( globalFoldout = EditorGUILayout.Foldout( globalFoldout, "Global Audio Settings", foldoutStyle ) )
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Streaming Audio Buffer", labelFieldOption );
            AC.StreamingAudioBuffer = EditorGUILayout.ObjectField(AC.StreamingAudioBuffer, typeof(StreamingAudioBuffer), true) as StreamingAudioBuffer;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio listener", labelFieldOption );
            AC.AudioListenerReference = EditorGUILayout.ObjectField(AC.AudioListenerReference, typeof(AudioListener), true) as AudioListener;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Simple Sound Prefab", labelFieldOption );
            AC.SimpleSoundPrefab = EditorGUILayout.ObjectField(AC.SimpleSoundPrefab, typeof(SoundObject), true) as SoundObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stream Sound Prefab", labelFieldOption );
            AC.StreamingSoundPrefab = EditorGUILayout.ObjectField(AC.StreamingSoundPrefab, typeof(SoundObject), true) as SoundObject;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Native Sound Prefab", labelFieldOption );
            AC.NativeSoundPrefab = EditorGUILayout.ObjectField(AC.NativeSoundPrefab, typeof(SoundObject), true) as SoundObject;
            EditorGUILayout.EndHorizontal();
           
            EditBool( ref AC.Persistent, "Persist Scene Loading", "A non-persisting AudioController will get destroyed when loading the next scene." );
            EditBool( ref AC.UnloadAudioClipsOnDestroy, "Unload Audio On Destroy", "This option will unload all AudioClips from memory which referenced by this AudioController if the controller gets destroyed (e.g. when loading a new scene and the AudioController is not persistent). \n" +
                "Use this option in combination with additional none-persistent AudioControllers to keep only those audios in memory that are used by the current scene. Use the primary persistent AudioController for all global audio that is used throughout all scenes."
            );

            bool currentlyDisabled = AC.DisableAudio;

            bool changed = EditBool( ref currentlyDisabled, "Disable Audio", "Disables all audio" );
            if( changed )
            {
                AC.DisableAudio = currentlyDisabled;
            }

            float vol = AC.Volume;

            EditFloat01( ref vol, "Volume", "%" );

            AC.Volume = vol;

            EditPrefab( ref AC.SoundSettings, "Sound Settings", "You may specify a prefab here that will contains settings for all sounds" );
            EditBool( ref AC.UsePooledAudioObjects, "Use Pooled AudioObjects", "Pooling increases performance when playing many audio files. Strongly recommended particularly on mobile platforms." );
            EditBool( ref AC.PlayWithZeroVolume, "Play With Zero Volume", "If disabled Play() calls with a volume of zero will not create an AudioObject." );
        }

        VerticalSpace();


        int categoryCount = AC.AudioCategories != null ? AC.AudioCategories.Length : 0;
        currentCategoryIndex = Mathf.Clamp( currentCategoryIndex, 0, categoryCount - 1 );

        if ( categoryFoldout = EditorGUILayout.Foldout( categoryFoldout, "Category Settings", foldoutStyle ) )
        {

            // Audio Items 
            EditorGUILayout.BeginHorizontal();

            bool justCreatedNewCategory = false;

            var categoryNames = GetCategoryNames();

            int newCategoryIndex = PopupWithStyle( "Category", currentCategoryIndex, categoryNames, popupStyleColored );
            if ( GUILayout.Button( "+", GUILayout.Width( 30 ) ) )
            {
                bool lastEntryIsNew = false;

                if ( categoryCount > 0 )
                {
                    lastEntryIsNew = AC.AudioCategories[ currentCategoryIndex ].Name == _nameForNewCategoryEntry;
                }

                if ( !lastEntryIsNew )
                {
                    newCategoryIndex = AC.AudioCategories != null ? AC.AudioCategories.Length : 0;
                    ArrayHelper.AddArrayElement( ref AC.AudioCategories, new SoundsCategory( AC ) );
                    AC.AudioCategories[ newCategoryIndex ].Name = _nameForNewCategoryEntry;
                    justCreatedNewCategory = true;
                    KeepChanges();
                }
            }

            if ( GUILayout.Button( "-", GUILayout.Width( 30 ) ) && categoryCount > 0 )
            {

                if ( currentCategoryIndex < AC.AudioCategories.Length - 1 )
                {
                    newCategoryIndex = currentCategoryIndex;
                }
                else
                {
                    newCategoryIndex = Mathf.Max( currentCategoryIndex - 1, 0 );
                }
                ArrayHelper.DeleteArrayElement( ref AC.AudioCategories, currentCategoryIndex );
                KeepChanges();
            }

            EditorGUILayout.EndHorizontal();

            if ( newCategoryIndex != currentCategoryIndex )
            {
                currentCategoryIndex = newCategoryIndex;
                currentItemIndex = 0;
                currentSubitemIndex = 0;
                _ValidateCurrentItemIndex();
                _ValidateCurrentSubItemIndex();
            }


            SoundsCategory curCat = currentCategory;

            if ( curCat != null )
            {
                if ( curCat.SoundsManager == null )
                {
                    curCat.SoundsManager = AC;//TODO ???
                }
                if ( justCreatedNewCategory )
                {
                    SetFocusForNextEditableField();
                }
                EditString( ref curCat.Name, "Name", curCat.Name == _nameForNewCategoryEntry ? textAttentionStyle : null );

                float volTmp = curCat.Volume;
                EditFloat01( ref volTmp, "Volume", " %" );
                curCat.Volume = volTmp;

                EditPrefab( ref curCat.SoundSettingsOverride, "Sound Settings Override", "Use different settinfs if you want to specify different parameters such as the volume rolloff etc. per category" );

                int selectedParentCategoryIndex;

                var catList = _GenerateCategoryListIncludingNone( out selectedParentCategoryIndex, curCat.ParentCategory );

                int newIndex = Popup( "Parent Category", selectedParentCategoryIndex, catList, "The effective volume of a category is multiplied with the volume of the parent category." );
                if ( newIndex != selectedParentCategoryIndex )
                {
                    KeepChanges();

                    if ( newIndex <= 0 )
                    {
                        curCat.ParentCategory = null;
                    }
                    else
                        curCat.ParentCategory = _GetCategory( catList[ newIndex ] );
                }

                int itemCount = currentItemCount;
                _ValidateCurrentItemIndex();

                VerticalSpace();

                SoundItem curItem = currentItem;

                if (itemFoldout = EditorGUILayout.Foldout(itemFoldout, "Audio Item Settings", foldoutStyle))
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add selected audio clips", EditorStyles.miniButton))//TODO ???
                    {
                        UnityEngine.Object[ ] audioClips = GetSelectedAudioObjects();
                        if (audioClips.Length > 0)
                        {
                            int firstIndex = itemCount;
                            currentItemIndex = firstIndex;
                            foreach (AudioClip audioClip in audioClips)
                            {
                                ArrayHelper.AddArrayElement(ref curCat.AudioItems);
                                SoundItem audioItem = curCat.AudioItems[currentItemIndex];
                                audioItem.Name = audioClip.name;
                                ArrayHelper.AddArrayElement(ref audioItem.subItems).Clip = audioClip;
                                currentItemIndex++;
                            }
                            currentItemIndex = firstIndex;
                            KeepChanges();
                        }
                    }

                    GUILayout.Label("use inspector lock!");
                    EditorGUILayout.EndHorizontal();

                    // AudioItems

                    EditorGUILayout.BeginHorizontal();

                    int newItemIndex = PopupWithStyle("Item", currentItemIndex, GetItemNames(), popupStyleColored);
                    bool justCreatedNewItem = false;


                    if (GUILayout.Button("+", GUILayout.Width(30)))
                    {
                        bool lastEntryIsNew = false;

                        if (itemCount > 0)
                        {
                            lastEntryIsNew = curCat.AudioItems[currentItemIndex].Name == _nameForNewAudioItemEntry;
                        }

                        if (!lastEntryIsNew)
                        {
                            newItemIndex = curCat.AudioItems != null ? curCat.AudioItems.Length : 0;
                            ArrayHelper.AddArrayElement(ref curCat.AudioItems);
                            curCat.AudioItems[newItemIndex].Name = _nameForNewAudioItemEntry;
                            justCreatedNewItem = true;
                            KeepChanges();
                        }
                    }

                    if (GUILayout.Button("-", GUILayout.Width(30)) && itemCount > 0)
                    {
                        if (currentItemIndex < curCat.AudioItems.Length - 1)
                        {
                            newItemIndex = currentItemIndex;
                        }
                        else
                        {
                            newItemIndex = Mathf.Max(currentItemIndex - 1, 0);
                        }
                        ArrayHelper.DeleteArrayElement(ref curCat.AudioItems, currentItemIndex);
                        KeepChanges();
                    }



                    if (newItemIndex != currentItemIndex)
                    {
                        currentItemIndex = newItemIndex;
                        currentSubitemIndex = 0;
                        _ValidateCurrentSubItemIndex();
                    }

                    curItem = currentItem;

                    EditorGUILayout.EndHorizontal();

                    if (curItem != null)
                    {
                        GUILayout.BeginHorizontal();
                        if (justCreatedNewItem)
                        {
                            SetFocusForNextEditableField();
                        }

                        bool isNewDummyName = curItem.Name == _nameForNewAudioItemEntry;
                        EditString(ref curItem.Name, "Name", isNewDummyName ? textAttentionStyle : null, "You must specify a unique name here (=audioID). This is the ID used in the script code to play this audio item.");

                        GUILayout.EndHorizontal();

                        int newItemCategoryIndex = Popup("Move to Category", currentCategoryIndex, GetCategoryNames());

                        if (newItemCategoryIndex != currentCategoryIndex)
                        {
                            var newCat = AC.AudioCategories[newItemCategoryIndex];
                            var oldCat = currentCategory;
                            ArrayHelper.AddArrayElement(ref newCat.AudioItems, curItem);
                            ArrayHelper.DeleteArrayElement(ref oldCat.AudioItems, currentItemIndex);
                            currentCategoryIndex = newItemCategoryIndex;
                            KeepChanges();
                            //AC.InitializeAudioItems(); //TODO
                            currentItemIndex = newCat.AudioItems.Length - 1;
                        }

                        if (EditFloat01(ref curItem.Volume, "Volume", " %"))
                        {
                            _AdjustVolumeOfAllAudioItems(curItem, null);
                        }

                        EditFloat(ref curItem.Delay, "Delay", "sec", "Delays the playback"); //TODO
                        EditInt(ref curItem.MaxInstanceCount, "Max Instance Count", "", "Sets the maximum number of simultaneously playing audio files of this particular audio item. If the maximum number would be exceeded, the oldest playing audio gets stopped.");
                        EditBool(ref curItem.SkipWhenExeeded, "Skip sound if Exeeded Limit", "");
                        EditBool(ref curItem.DestroyOnLoad, "Stop When Scene Loads", "If disabled, this audio item will continue playing even if a different scene is loaded.");
                        //TODO override prefab

                        curItem.Loop = (SoundLoopMode)EnumPopup("Loop Mode", curItem.Loop, "The Loop mode determines how the audio subitems are looped. \n'LoopSubitem' means that the chosen sub-item will loop. \n'LoopSequence' means that one subitem is played after the other. In which order the subitems are chosen depends on the subitem pick mode.");
                        if (curItem.Loop == SoundLoopMode.LoopSubitem)
                        {
                            EditFloat(ref curItem.OverlapTime, "When Overlap", "sec", "define time when start next clip"); //TODO
                        }
                        curItem.SubItemPickMode = (SoundPickSubItemMode)EnumPopup("Pick Subitem Mode", curItem.SubItemPickMode, "Determines which subitem is chosen when the audio item is played.");
                       
                        EditPrefab( ref curItem.SoundSettingsOverride, "Sound Settings Override", "Use different settinfs if you want to specify different parameters such as the volume rolloff etc. per item" );

                        EditBool( ref curItem.overrideAudioSourceSettings, "Override AudioSource Settings" );
                        if ( curItem.overrideAudioSourceSettings )
                        {
                            //EditorGUI.indentLevel++;

                            EditFloat( ref curItem.audioSource_MinDistance, "   Min Distance", "", "Overrides the 'Min Distance' parameter in the AudioSource settings of the AudioObject prefab (for 3d sounds)" );
                            EditFloat( ref curItem.audioSource_MaxDistance, "   Max Distance", "", "Overrides the 'Max Distance' parameter in the AudioSource settings of the AudioObject prefab (for 3d sounds)" );

                            //EditorGUI.indentLevel--;
                        }

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("");

                        if (GUILayout.Button("Play", GUILayout.Width(60)) && curItem != null)
                        {
                            if (_IsAudioControllerInPlayMode())
                            {
                                SoundsManager.Play(curItem.Name);
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                        VerticalSpace();

                        int subItemCount = curItem.subItems != null ? curItem.subItems.Length : 0;
                        currentSubitemIndex = Mathf.Clamp(currentSubitemIndex, 0, subItemCount - 1);
                        SoundSubItem subItem = currentSubItem;

                        if (subitemFoldout = EditorGUILayout.Foldout(subitemFoldout, "Audio Sub-Item Settings", foldoutStyle))
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Add selected audio clips", EditorStyles.miniButton))
                            {
                                UnityEngine.Object[] audioObjects = GetSelectedAudioObjects();
                                if (audioObjects.Length > 0)
                                {
                                    int firstIndex = subItemCount;
                                    currentSubitemIndex = firstIndex;
                                    foreach (UnityEngine.Object audioObject in audioObjects)
                                    {
                                        SoundSubItem cSubItem = ArrayHelper.AddArrayElement(ref curItem.subItems);
                                        cSubItem.Clip = audioObject;
                                        currentSubitemIndex++; 
                                    }
                                    currentSubitemIndex = firstIndex;
                                    KeepChanges();
                                }
                            }
                            GUILayout.Label("use inspector lock!");
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();

                            currentSubitemIndex = PopupWithStyle("SubItem", currentSubitemIndex, GetSubitemNames(), popupStyleColored);

                            if (GUILayout.Button("+", GUILayout.Width(30)))
                            {
                                bool lastEntryIsNew = false;

                                SoundClipType curSubItemType = SoundClipType.Clip;

                                if (subItemCount > 0)
                                {
                                    curSubItemType = curItem.subItems[currentSubitemIndex].SubItemType;
                                    if (curSubItemType == SoundClipType.Clip)
                                    {
                                        lastEntryIsNew = curItem.subItems[currentSubitemIndex].Clip == null;
                                    }
                                }

                                if (!lastEntryIsNew)
                                {
                                    currentSubitemIndex = subItemCount;
                                    ArrayHelper.AddArrayElement(ref curItem.subItems);
                                    KeepChanges();
                                }
                            }

                            if (GUILayout.Button("-", GUILayout.Width(30)) && subItemCount > 0)
                            {
                                ArrayHelper.DeleteArrayElement(ref curItem.subItems, currentSubitemIndex);
                                if (currentSubitemIndex >= curItem.subItems.Length)
                                {
                                    currentSubitemIndex = Mathf.Max(curItem.subItems.Length - 1, 0);
                                }
                                KeepChanges();
                            }
                            EditorGUILayout.EndHorizontal();

                            subItem = currentSubItem;

                            if (subItem != null)
                            {
                                _SubitemTypePopup(subItem);
                                _DisplaySubItem_Clip(subItem, subItemCount, curItem);
                            } 
                        }
                    }
                }
            }
        }

        VerticalSpace();

        EditorGUILayout.Space();

        if ( EditorApplication.isPlaying )
        {
            EditorGUILayout.BeginHorizontal();
            if ( GUILayout.Button( "Stop All Sounds" ) )
            {
                if ( EditorApplication.isPlaying && SoundsManager.InstanceIfExist )
                {
                    SoundsManager.StopAllSounds();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        EndInspectorGUI();

        #if AUDIO_PLUGIN
        if ( GUILayout.Button( "convert from audio controller" ) )
        {
            AudioController audio = GameObjectExtension.GetAllObjectsInScene<AudioController>()[0];
            AC.AudioCategories = new SoundsCategory[audio.AudioCategories.Length];

            for (int i = 0; i < audio.AudioCategories.Length; i++)
            {
                AudioCategory cat = audio.AudioCategories[i];

                SoundsCategory scat = new SoundsCategory(AC);
                scat.AudioItems = new SoundItem[cat.AudioItems.Length];
                scat.Name = cat.Name;
                scat.Volume = cat.Volume;
                if (cat.parentCategory != null)
                {
                    scat.ParentCategory = SoundsManager.Instance.GetCategory(cat.parentCategory.Name);
                }
                AC.AudioCategories[i] = scat;

                for (int j = 0; j < cat.AudioItems.Length; j++)
                {
                    AudioItem item = cat.AudioItems[j];

                    SoundItem sitem = new SoundItem();
                    sitem.subItems = new SoundSubItem[item.subItems.Length];
                    sitem.Name = item.Name;
                    sitem.Volume = item.Volume;
                    sitem.MaxInstanceCount = item.MaxInstanceCount;
                    sitem.SkipWhenExeeded = item.SkipWhenExeeded;
                    sitem.Delay = item.Delay;
                    sitem.overrideAudioSourceSettings = item.overrideAudioSourceSettings;
                    sitem.audioSource_MinDistance = item.audioSource_MinDistance;
                    sitem.audioSource_MaxDistance = item.audioSource_MaxDistance;
                    sitem.Loop = (SoundLoopMode)item.Loop;
                    sitem.SubItemPickMode = (SoundPickSubItemMode)item.SubItemPickMode;

                    scat.AudioItems[j] = sitem;

                    for (int k = 0; k < item.subItems.Length; k++)
                    {
                        AudioSubItem subItem = item.subItems[k];

                        SoundSubItem ssubItem = new SoundSubItem();
                        UnityEngine.Object asset = subItem.Clip;
                        if (asset == null)
                        {
                            string path = subItem.StreamingAudioClipData.Path;
                            if (path.Length > 0)
                            {
//                                asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/StreamingAssets" + path);
                                Debug.Log(asset);
                            }
                            else
                            {
                                Debug.LogError(cat.Name + "  " + item.Name + "  missing path");
                            }
                        }
                        else
                        {
                            Debug.LogError("use existing clip : " + asset);
                        }
                        ssubItem.Clip = asset;
                        ssubItem.Volume = subItem.Volume;
                        ssubItem.RandomVolume = subItem.RandomVolume;
                        ssubItem.Delay = subItem.Delay;
                        ssubItem.Pan2D = subItem.Pan2D;
                        ssubItem.Probability = subItem.Probability;
                        ssubItem.RandomPitch = subItem.RandomPitch;
                        ssubItem.RandomDelay = subItem.RandomDelay;
						ssubItem.FadeIn = subItem.FadeIn;
						ssubItem.FadeOut = subItem.FadeOut;

                        sitem.subItems[k] = ssubItem;
                    }
                }
            }

            AC. _currentInspectorSelection = new SoundsManager.InspectorSelection();
            AC.ValidateSounds();
        }
        #endif

		if (GUILayout.Button("print .wav items"))
		{
			foreach(SoundsCategory cat in AC.AudioCategories)
			{
				foreach(SoundItem item in cat.AudioItems)
				{
					foreach(SoundSubItem subItem in item.subItems)
					{
						if(subItem.SubItemType != SoundClipType.Native)
						{
							Debug.Log(cat.Name + "/" + item.Name + "/" + subItem.Name);
						}
					}
				}
			}
		}

        if (GUILayout.Button("convert all to caf"))
        {
            foreach(SoundsCategory cat in AC.AudioCategories)
            {
                foreach(SoundItem item in cat.AudioItems)
                {
                    foreach(SoundSubItem subItem in item.subItems)
                    {
                        if(subItem.clipref.FullPath.Length > 0)
                        {
                            ConvertToCaff(cat, item, subItem);
                        }
                    }
                }
            }
        }


        if (GUILayout.Button("convert wav to caf"))
        {
            foreach(SoundsCategory cat in AC.AudioCategories)
            {
                foreach(SoundItem item in cat.AudioItems)
                {
                    foreach(SoundSubItem subItem in item.subItems)
                    {
                        if(subItem.clipref.FullPath.Length > 0 && subItem.SubItemType == SoundClipType.Clip)
                        {
                            string path = ConvertToCaff(cat, item, subItem);
                            AssetDatabase.Refresh();
                            subItem.Clip = AssetDatabase.LoadAssetAtPath<DefaultAsset>("Assets" + path);
                        }
                    }
                }
            }
        }


		if (GUILayout.Button ("convert all to ogg")) 
		{
			string rootDirectory = Application.dataPath + "/StreamingAssets/";
			var allFiles = Directory.GetFiles(rootDirectory, "*.caf", SearchOption.AllDirectories);

			foreach (string file in allFiles)
			{
				string filePath = file;

				int fileExtPos = file.LastIndexOf(".");
				if (fileExtPos >= 0 )
				{
					filePath = file.Substring(0, fileExtPos);
				}

				string sourceFile = filePath + ".caf";
				string destanationFile = filePath + ".ogg";

				string processPath = Application.dataPath + "/SoundsPlugin/ffmpeg";
				string attributes = string.Format("-i {0} -acodec libvorbis {1}", sourceFile.ShellString(), destanationFile.ShellString());

				System.Diagnostics.Process p = System.Diagnostics.Process.Start(processPath, attributes);
				p.WaitForExit();
			}
		}
    }


    string ConvertToCaff(SoundsCategory cat, SoundItem item, SoundSubItem subItem)
    {
        int bitrate = 96;
        string sourcePath = Application.dataPath + subItem.clipref.FullPath.Substring(6);
        string fileName = System.IO.Path.GetFileNameWithoutExtension(sourcePath);

        string projectDir = "/StreamingAssets/Audio/iOS/" + cat.Name + "/" + item.Name + "/";
        string projectPath = projectDir + fileName + ".caf";

        string targetDir = Application.dataPath + projectDir;
        string destanationPath = targetDir + fileName + ".caf";
        System.IO.Directory.CreateDirectory(targetDir);

        string channels = "";
        if (cat.SoundSettingsOverride != null && cat.SoundSettingsOverride.gameObject.name.Contains("3D"))
        {
            channels = " -c 1";
        }

        string attributes = string.Format("-f 'caff' -d aac -s 2 -b {0}000 {3} {1} {2}", bitrate, sourcePath, destanationPath, channels);
        //                            Debug.Log(attributes);

        System.Diagnostics.Process p = System.Diagnostics.Process.Start("afconvert", attributes);
        p.WaitForExit();

        return projectPath;
    }


    private string[] GetCategoryNames()
    {
        if ( AC.AudioCategories == null )
        {
            return new string[ 0 ];
        }
        var names = new string[ AC.AudioCategories.Length ];
        for( int i=0; i< AC.AudioCategories.Length; i++ )
        {
            names[ i ] = AC.AudioCategories[ i ].Name;

            if ( names[ i ] == _nameForNewCategoryEntry )
            {
                names[ i ] = "---";
            }
        }
        return names;
    }


    private void _ValidateCurrentCategoryIndex()
    {
        int categoryCount = currentCategoryCount;
        if ( categoryCount > 0 ) currentCategoryIndex = Mathf.Clamp( currentCategoryIndex, 0, categoryCount - 1 );
        else currentCategoryIndex = -1;
    }

    private void _ValidateCurrentSubItemIndex()
    {
        int subitemCount = currentSubItemCount;
        if ( subitemCount > 0 ) currentSubitemIndex = Mathf.Clamp( currentSubitemIndex, 0, subitemCount - 1 );
        else currentSubitemIndex = -1;
    }

    private void _ValidateCurrentItemIndex()
    {
        int itemCount = currentItemCount;
        if ( itemCount > 0 ) currentItemIndex = Mathf.Clamp( currentItemIndex, 0, itemCount - 1 );
        else currentItemIndex = -1;
    }


    private string[] _GenerateCategoryListIncludingNone( out int selectedParentCategoryIndex, SoundsCategory selectedAudioCategory )
    {
        string[] names;
        selectedParentCategoryIndex = 0;

        if ( AC.AudioCategories != null )
        {
            names = new string[ AC.AudioCategories.Length ];

            int index = 1;

            var curCat = currentCategory;

            for (int i=0; i< AC.AudioCategories.Length; i++)
            {
                if ( _IsCategoryChildOf( AC.AudioCategories[ i ], curCat ) ) // prevent loops in tree
                {
                    continue;
                }
                names[index] = AC.AudioCategories[i].Name;
                if ( selectedAudioCategory == AC.AudioCategories[ i ] )
                {
                    selectedParentCategoryIndex = index;
                }

                index++;
                if ( index == names.Length )
                {
                    break; // in case currentCategory is not found
                }
            }

            if ( index < names.Length )
            {
                var newNames = new string[ index ];
                Array.Copy( names, newNames, index );
                names = newNames;
            }
        }
        else
        {
            names = new string[ 1 ];
        }

        names[ 0 ] = "*none*";
        return names;
    }


    bool _IsCategoryChildOf( SoundsCategory toTest, SoundsCategory parent )
    {
        var cat = toTest;
        while ( cat != null )
        {
            if ( cat.SoundsManager == null )
            {
                cat.SoundsManager = AC;
            }

            if ( cat == parent ) return true;

            cat = cat.ParentCategory;
        }
        return false;
    }


    SoundsCategory _GetCategory( string name )
    {
        foreach ( SoundsCategory cat in AC.AudioCategories )
        {
            if ( cat.Name == name )
            {
                return cat;
            }
        }
        return null;
    }


    static UnityEngine.Object[] GetSelectedAudioObjects()
    { 
        return Selection.GetFiltered( typeof( UnityEngine.Object ), SelectionMode.DeepAssets );
    }


    private string[] GetItemNames()
    {
        SoundsCategory curCat = currentCategory;
        if ( curCat == null || curCat.AudioItems == null )
        {
            return new string[0];
        }

        var names = new string[ curCat.AudioItems.Length ];
        for ( int i = 0; i < curCat.AudioItems.Length; i++ )
        {
            names[ i ] = curCat.AudioItems[ i ] != null ? curCat.AudioItems[ i ].Name : "";

            if ( names[ i ] == _nameForNewAudioItemEntry )
            {
                names[ i ] = "---";
            }
        }
        return names;
    }


    private void _AdjustVolumeOfAllAudioItems( SoundItem curItem, SoundSubItem subItem )
    {
//        if ( _IsAudioControllerInPlayMode() )
//        {
//            var audioObjs = AudioController.GetPlayingAudioObjects();
//
//            foreach( var a in audioObjs )
//            {
//                if ( curItem != a.audioItem ) continue;
//                if ( subItem != null )
//                {
//                    if ( subItem != a.subItem ) continue;
//                }
//                a.volumeItem = a.audioItem.Volume * a.subItem.Volume;
//            }
//        }
    }


    private bool _IsAudioControllerInPlayMode()
    {
        return EditorApplication.isPlaying && SoundsManager.InstanceIfExist;
    }


    static string _GetAssetPath( UnityEngine.Object asset )
    {
        return AssetDatabase.GetAssetPath(asset);
    }


    private string[] GetSubitemNames()
    {
        SoundItem curItem = currentItem;
        if ( curItem == null || curItem.subItems == null )
        {
            return new string[ 0 ];
        }

        var names = new string[ curItem.subItems.Length ];
        for ( int i = 0; i < curItem.subItems.Length; i++ )
        {
            string title = null;
            var subItem = curItem.subItems[ i ];

            if (subItem.Clip != null)
            {
                title = subItem.Clip.name;
            }
            else
            {
                title = "*unset*";
            }

            names[ i ] = string.Format( "CLIP {0}: {1}", i, title );
        }
        return names;
    }


    private void _SubitemTypePopup( SoundSubItem subItem )
    {
        var typeNames = new string[4];
        typeNames[ 0 ] = "Unknown";
        typeNames[ 1 ] = "Single Audio Clip";
        typeNames[ 2 ] = "Streaming Audio Item";
        typeNames[ 3 ] = "Native Audio Clip";

        int curIndex = 0;
        switch( subItem.SubItemType )
        {
            case SoundClipType.Unknown: curIndex = 0; break;
            case SoundClipType.Clip: curIndex = 1; break;
            case SoundClipType.StreamingClip: curIndex = 2; break;
            case SoundClipType.Native: curIndex = 3; break;
        }
        GUI.enabled = false;
        switch( Popup( "SubItem Type", curIndex, typeNames ) )
        {
            case 0: subItem.SubItemType = SoundClipType.Unknown; break;
            case 1: subItem.SubItemType = SoundClipType.Clip; break;
            case 2: subItem.SubItemType = SoundClipType.StreamingClip; break;
            case 3: subItem.SubItemType = SoundClipType.Native; break;
        }
        GUI.enabled = true;
    }


    private void _DisplaySubItem_Clip( SoundSubItem subItem, int subItemCount, SoundItem curItem )
    {
        if ( subItem != null )
        {
            UnityEngine.Object clip = subItem.Clip;
            EditAudioClip( ref clip, "Audio Clip" );
            subItem.Clip = clip;

            if( EditFloat01( ref subItem.Volume, "Volume", " %" ) )
            {
                _AdjustVolumeOfAllAudioItems( curItem, subItem );
            }

            EditFloat01( ref subItem.RandomVolume, "Random Volume", "±%" );

            EditFloat( ref subItem.Delay, "Delay", "sec" );
            EditFloatPlusMinus1( ref subItem.Pan2D, "Pan2D", "%left/right" );
            if( _IsRandomItemMode( curItem.SubItemPickMode ) )
            {
                EditFloat01( ref subItem.Probability, "Probability", " %", "Choose a higher value (in comparison to the probability values of the other audio clips) to increase the probability for this clip when using a random subitem pick mode." );
            }
            EditFloat( ref subItem.PitchShift, "Pitch Shift", "semitone" );
            EditFloat( ref subItem.RandomPitch, "Random Pitch", "±semitone" );
            EditFloat( ref subItem.RandomDelay, "Random Delay", "sec" ); 
			EditFloat( ref subItem.FadeIn, "FadeIn", "sec");
			EditFloat( ref subItem.FadeOut, "FadeOut", "sec");
        }

        EditorGUILayout.BeginHorizontal(); 
        GUILayout.Label( " " );
        GUI.enabled = _IsAudioControllerInPlayMode();

        if ( GUILayout.Button( "Play", GUILayout.Width( 60 ) ) && subItem != null )
        {
            if ( _IsAudioControllerInPlayMode() )
            {
                var audioListener = SoundsManager.GetCurrentAudioListener();
                Vector3 pos;
                if ( audioListener != null )
                {
                    pos = audioListener.transform.position + audioListener.transform.forward;
                }
                else
                    pos = Vector3.zero;

                AC.PlayAudioSubItem(subItem.Item.Category, curItem, subItem, pos, null, false, null, null);
            }
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
    }


    protected void EditAudioClip( ref UnityEngine.Object clip, string label ) 
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label( label, styleLabel );
        System.Type clipType = /*clip != null ? clip.GetType() :*/ typeof(UnityEngine.Object);
        clip = EditorGUILayout.ObjectField(clip, clipType, false);
        AudioClip audio = clip as AudioClip;
        if ( audio != null)
        {
            EditorGUILayout.Space();
            GUILayout.Label( string.Format( "{0:0.0} sec", audio.length ), GUILayout.Width( 60 ) );
        }
        EditorGUILayout.EndHorizontal();
    }


    private bool _IsRandomItemMode( SoundPickSubItemMode audioPickSubItemMode )
    {
        switch ( audioPickSubItemMode )
        {
            case SoundPickSubItemMode.Random: return true;
            case SoundPickSubItemMode.RandomNotSameTwice: return true;
        }
        return false;
    }
}

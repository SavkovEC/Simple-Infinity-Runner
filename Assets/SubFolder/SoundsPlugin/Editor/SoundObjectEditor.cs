using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor( typeof(SoundObject), true )]
public class SoundObjectEditor : Editor
{
    private class Styles
    {
        public GUIContent volumeLabel = new GUIContent ("Volume", "Sets the overall volume of the sound.");
        public GUIContent pitchLabel = new GUIContent ("Pitch", "Sets the frequency of the sound. Use this to slow down or speed up the sound.");
        public GUIContent priorityLabel = new GUIContent ("Priority", "Sets the priority of the source. Note that a sound with a larger priority value will more likely be stolen by sounds with smaller priority values.");
        public GUIContent priorityLeftLabel = new GUIContent ("High");
        public GUIContent priorityRightLabel = new GUIContent ("Low");
        public GUIContent spatialLeftLabel = new GUIContent ("2D");
        public GUIContent spatialRightLabel = new GUIContent ("3D");
        public GUIContent panLeftLabel = new GUIContent ("Left");
        public GUIContent outputMixerGroupLabel = new GUIContent ("Output", "Set whether the sound should play through an Audio Mixer first or directly to the Audio Listener");
        public GUIContent rolloffLabel = new GUIContent ("Volume Rolloff", "Which type of rolloff curve to use");
        public string controlledByCurveLabel = "Controlled by curve";
        public GUIContent audioClipLabel = new GUIContent ("AudioClip", "The AudioClip asset played by the AudioSource. Can be undefined if the AudioSource is generating a live stream of audio via OnAudioFilterRead.");
        public GUIContent panStereoLabel = new GUIContent ("Stereo Pan", "Only valid for Mono and Stereo AudioClips. Mono sounds will be panned at constant power left and right. Stereo sounds will Stereo sounds have each left/right value faded up and down according to the specified pan value.");
        public GUIContent spatialBlendLabel = new GUIContent ("Spatial Blend", "Sets how much this AudioSource is treated as a 3D source. 3D sources are affected by spatial position and spread. If 3D Pan Level is 0, all spatial attenuation is ignored.");
        public GUIContent reverbZoneMixLabel = new GUIContent ("Reverb Zone Mix", "Sets how much of the signal this AudioSource is mixing into the global reverb associated with the zones. [0, 1] is a linear range (like volume) while [1, 1.1] lets you boost the reverb mix by 10 dB.");
        public GUIContent dopplerLevelLabel = new GUIContent ("Doppler Level", "Specifies how much the pitch is changed based on the relative velocity between AudioListener and AudioSource.");
        public GUIContent spreadLabel = new GUIContent ("Spread", "Sets the spread of a 3d sound in speaker space");
        public GUIContent panRightLabel = new GUIContent ("Right");
    }

    SoundObjectEditor.Styles ms_Styles = new SoundObjectEditor.Styles ();

 
    public override void OnInspectorGUI()
    {
        SoundObject soundObject = target as SoundObject;

        if (soundObject.SubItem != null)
        {
            EditorGUILayout.LabelField("Category", soundObject.SubItem.Item.Category.Name);
            EditorGUILayout.LabelField("Item", soundObject.SubItem.Item.Name);
            EditorGUILayout.LabelField("SubItem", soundObject.SubItem.Name);
        }

//        EditorGUILayout.PropertyField (this.m_AudioClip, ms_Styles.audioClipLabel, new GUILayoutOption[0]);
//        EditorGUILayout.Space ();
        soundObject.Mute = EditorGUILayout.Toggle ("Mute", soundObject.Mute, new GUILayoutOption[0]);
//        soundObject.BypassEffects = EditorGUILayout.Toggle ("BypassEffects", soundObject.BypassEffects, new GUILayoutOption[0]);
//        soundObject.BypassListenerEffects = EditorGUILayout.Toggle ("BypassListenerEffects", soundObject.BypassListenerEffects, new GUILayoutOption[0]);
//        soundObject.BypassReverbZones =  EditorGUILayout.Toggle ("BypassReverbZones", soundObject.BypassReverbZones, new GUILayoutOption[0]);
//        soundObject.PlayOnAwake = EditorGUILayout.Toggle ("PlayOnAwake", soundObject.PlayOnAwake, new GUILayoutOption[0]);
        soundObject.Loop = EditorGUILayout.Toggle ("Loop", soundObject.Loop, new GUILayoutOption[0]);
        EditorGUILayout.Space ();
        soundObject.Priority = (int)Slider (this.ms_Styles.priorityLabel, soundObject.Priority, 0, 256, ms_Styles.priorityLeftLabel, ms_Styles.priorityRightLabel, new GUILayoutOption[0]);
        EditorGUILayout.Space ();
        soundObject.Volume = EditorGUILayout.Slider (ms_Styles.volumeLabel, soundObject.Volume, 0, 1, new GUILayoutOption[0]);
        EditorGUILayout.Space();
        GUI.enabled = false;
        EditorGUILayout.Slider ("Total volume", soundObject.TotalVolume, 0, 1, new GUILayoutOption[0]);
        GUI.enabled = true;
        EditorGUILayout.Space ();
        soundObject.Pitch = EditorGUILayout.Slider (ms_Styles.pitchLabel, soundObject.Pitch, -3, 3, new GUILayoutOption[0]);
        EditorGUILayout.Space ();
        soundObject.Pan = Slider (ms_Styles.panStereoLabel, soundObject.Pan, -1, 1, ms_Styles.panLeftLabel, ms_Styles.priorityRightLabel, new GUILayoutOption[0]);
        EditorGUILayout.Space ();
        soundObject.SpatialBlend = Slider (ms_Styles.spatialBlendLabel, soundObject.SpatialBlend, 0, 1, ms_Styles.spatialLeftLabel, ms_Styles.spatialRightLabel, new GUILayoutOption[0]);
        EditorGUILayout.Space ();

        soundObject.Spread = EditorGUILayout.Slider (ms_Styles.spreadLabel, soundObject.Spread, 0, 360, new GUILayoutOption[0]);
        soundObject.MinDistance = EditorGUILayout.FloatField("Min Distance", soundObject.MinDistance);
        soundObject.MaxDistance = EditorGUILayout.FloatField("Max Distance", soundObject.MaxDistance);

        EditorGUILayout.Space ();
        EditorGUILayout.Space ();

        EditorGUILayout.BeginHorizontal();
        if ( GUILayout.Button( "Pause", EditorStyles.miniButton) )
        {
            soundObject.Pause(true, "");
        }
        if ( GUILayout.Button( "Unpause", EditorStyles.miniButton ) )
        {
            soundObject.Pause(false, "");
        }
        if ( GUILayout.Button( "Stop", EditorStyles.miniButton ) )
        {
            soundObject.Stop( soundObject.SubItem.FadeOut );
        }
        EditorGUILayout.EndHorizontal();

        EditorUtility.SetDirty(soundObject);
    }


    public float Slider (GUIContent label, float value, float leftValue, float rightValue, GUIContent leftLabel, GUIContent rightLabel, params GUILayoutOption[] options)
    {
        Rect position = EditorGUILayout.GetControlRect(true, options);
        value = EditorGUI.Slider(position, label, value, leftValue, rightValue);

        bool enable = GUI.enabled;
        GUI.enabled = false;
        {
            position = EditorGUI.PrefixLabel(position, 0, new GUIContent(" "));
            float num2 = position.width - 5 - EditorGUIUtility.fieldWidth;
            position = new Rect(position.x, position.y, num2, position.height);
            Rect rect = new Rect(position.x, position.y + 10, position.width, position.height);
            DoTwoLabels(rect, leftLabel, rightLabel, EditorStyles.miniLabel);
        }
        GUI.enabled = enable;
       
        return value;
    }


    void DoTwoLabels (Rect rect, GUIContent leftLabel, GUIContent rightLabel, GUIStyle labelStyle)
    {
        TextAnchor alignment = labelStyle.alignment;
        labelStyle.alignment = TextAnchor.UpperLeft;
        GUI.Label (rect, leftLabel, labelStyle);
        labelStyle.alignment = TextAnchor.UpperRight;
        GUI.Label (rect, rightLabel, labelStyle);
        labelStyle.alignment = alignment;
    }
}

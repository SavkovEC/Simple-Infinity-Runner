using UnityEngine;
using System.Collections;

[System.Serializable]
public class SoundsCategory
{
    #region Fields
    [SerializeField] public string Name;
    [SerializeField] private float volume = 1.0f;
    [SerializeField] private string parentCategoryName;
    [SerializeField] public SoundSettings SoundSettingsOverride;
    [SerializeField] public SoundItem[ ] AudioItems;
    [SerializeField] private SoundsManager soundsManager;

    private SoundsCategory parentCategory;
    #endregion



    #region Properties

    public float Volume
    {
        get { return volume; }
        set { volume = value; _ApplyVolumeChange(); }
    }


    public float VolumeTotal
    {
        get 
        {
            if (parentCategory != null)
            {
                return parentCategory.VolumeTotal * volume;
            }
            else
            {
                return volume; 
            }
        }
    }


    public SoundsManager SoundsManager
    {
        get
        {
            return soundsManager;
        }
        set
        {
            soundsManager = value;
        }
    }


    public SoundsCategory ParentCategory
    {
        set
        {
            parentCategory = value;

            if (value != null)
            {
                parentCategoryName = parentCategory.Name;
            }
            else
            {
                parentCategoryName = null;
            }
        }
        get
        {
            if ( string.IsNullOrEmpty( parentCategoryName ) )
            {
                return null;
            }
            if ( parentCategory == null )
            {
                if ( soundsManager != null )
                {
                    parentCategory = soundsManager.GetCategory( parentCategoryName );
                }
                else
                {
                    Debug.LogWarning( "_audioController == null" );
                }
            }
            return parentCategory;
        }
    }

    #endregion
   


    #region Public methods

    public SoundsCategory( SoundsManager soundsManager )
    {
        this.soundsManager = soundsManager;
    }


    private void _ApplyVolumeChange()
    {
        
    }

    internal void Initialize( SoundsManager soundsManager )
    {
        this.soundsManager = soundsManager;

        for (int i = 0; i < AudioItems.Length; i++)
        {
            AudioItems[i].Initialize( this );
        }
    }

    #endregion
}

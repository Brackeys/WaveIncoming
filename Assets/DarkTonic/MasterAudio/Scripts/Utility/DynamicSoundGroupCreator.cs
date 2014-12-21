using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is used to configure and create temporary per-Scene Sound Groups and Buses
/// </summary>
public class DynamicSoundGroupCreator : MonoBehaviour
{
    public const int EXTRA_HARD_CODED_BUS_OPTIONS = 1;

    public MasterAudio.DragGroupMode curDragGroupMode = MasterAudio.DragGroupMode.OneGroupPerClip;
    public Texture logoTexture;
    public Texture deleteTexture;
    public Texture settingsTexture;
    public Texture playTexture;
    public Texture stopTrackTexture;
    public GameObject groupTemplate;
    public GameObject variationTemplate;
    public bool createOnAwake = true;
    public List<DynamicSoundGroupInfo> soundGroupsToCreate;
    public bool soundGroupsAreExpanded = true;
    public bool removeGroupsOnSceneChange = true;
    public bool showCustomEvents = true;
    public MasterAudio.AudioLocation bulkVariationMode = MasterAudio.AudioLocation.Clip;
    public List<CustomEvent> customEventsToCreate = new List<CustomEvent>();
    public string newEventName = "my event";
    public bool showMusicDucking = true;
    public List<DuckGroupInfo> musicDuckingSounds = new List<DuckGroupInfo>();
    public List<GroupBus> groupBuses = new List<GroupBus>();

    private bool hasCreated = false;
    private List<Transform> groupsToRemove = new List<Transform>();
    private Transform trans;

    private AudioSource _previewerInstance;
    private List<DynamicSoundGroup> groupsToCreate = new List<DynamicSoundGroup>();

    public DynamicSoundGroupCreator()
    {
        soundGroupsToCreate = new List<DynamicSoundGroupInfo>() {
			new DynamicSoundGroupInfo() {
				variations = new List<DynamicSoundGroupVariation>() {
					new DynamicSoundGroupVariation()
				}
			}
		};
    }

    void Awake()
    {
        this.trans = this.transform;
        hasCreated = false;
    }

    void Start()
    {
        if (soundGroupsToCreate.Count > 0)
        {
            Debug.LogError("You have data in an old format in your Game Object '" + this.name + "'. It will not be used as is. Please click stop and upgrade it to the new format by clicking the Upgrade Data button in the Inspector.");
            return;
        }

        if (createOnAwake)
        {
            CreateItems();
        }
    }

    void OnDisable()
    {
        if (MasterAudio.AppIsShuttingDown)
        {
            return;
        }

        // scene changing
        if (!removeGroupsOnSceneChange)
        {
            // nothing to do.
            return;
        }

        // delete any buses we created too
        for (var i = 0; i < groupBuses.Count; i++)
        {
            var aBus = groupBuses[i];

            if (aBus.isExisting)
            {
                continue; // don't delete!
            }

            MasterAudio.DeleteBusByName(aBus.busName);
        }

        for (var i = 0; i < groupsToRemove.Count; i++)
        {
            MasterAudio.RemoveSoundGroup(groupsToRemove[i]);
        }

        for (var i = 0; i < customEventsToCreate.Count; i++)
        {
            var anEvent = customEventsToCreate[i];
            MasterAudio.DeleteCustomEvent(anEvent.EventName);
        }
    }

    /// <summary>
    /// This method will create the Sound Groups, Variations, buses and ducking triggers specified in the Dynamic Sound Group Creator's Inspector. It is called automatically if you check the "auto-create" checkbox, otherwise you will need to call this method manually.
    /// </summary>
    public void CreateItems()
    {
        if (hasCreated)
        {
            Debug.LogWarning("DynamicSoundGroupCreator '" + this.transform.name + "' has already created its groups. Cannot create again.");
            return;
        }

        var ma = MasterAudio.Instance;
        if (ma == null)
        {
            return;
        }

        PopulateGroupData();

        for (var i = 0; i < groupBuses.Count; i++)
        {
            var aBus = groupBuses[i];

            if (aBus.isExisting)
            {
                var confirmBus = MasterAudio.GrabBusByName(aBus.busName);
                if (confirmBus == null)
                {
                    MasterAudio.LogWarning("Existing bus '" + aBus.busName + "' was not found, specified in prefab '" + this.name + "'.");
                }
                continue; // already exists.
            }

            if (!MasterAudio.CreateBus(aBus.busName))
            {
                continue;
            }

            var createdBus = MasterAudio.GrabBusByName(aBus.busName);
            if (createdBus == null)
            {
                continue;
            }

            createdBus.volume = aBus.volume;
            createdBus.voiceLimit = aBus.voiceLimit;
        }

        for (var i = 0; i < groupsToCreate.Count; i++)
        {
            var aGroup = groupsToCreate[i];

            var busName = string.Empty;
            var selectedBusIndex = aGroup.busIndex == -1 ? 0 : aGroup.busIndex;
            if (selectedBusIndex >= HardCodedBusOptions)
            {
                var selectedBus = groupBuses[selectedBusIndex - HardCodedBusOptions];
                busName = selectedBus.busName;
            }
            aGroup.busName = busName;

            var groupTrans = MasterAudio.CreateNewSoundGroup(aGroup, this.trans.name);

            // remove fx components
            for (var v = 0; v < aGroup.groupVariations.Count; v++)
            {
                var aVar = aGroup.groupVariations[v];
                if (aVar.LowPassFilter != null)
                {
                    GameObject.Destroy(aVar.LowPassFilter);
                }
                if (aVar.HighPassFilter != null)
                {
                    GameObject.Destroy(aVar.HighPassFilter);
                }
                if (aVar.DistortionFilter != null)
                {
                    GameObject.Destroy(aVar.DistortionFilter);
                }
                if (aVar.ChorusFilter != null)
                {
                    GameObject.Destroy(aVar.ChorusFilter);
                }
                if (aVar.EchoFilter != null)
                {
                    GameObject.Destroy(aVar.EchoFilter);
                }
                if (aVar.ReverbFilter != null)
                {
                    GameObject.Destroy(aVar.ReverbFilter);
                }
            }

            if (groupTrans == null)
            {
                continue;
            }

            groupsToRemove.Add(groupTrans);
        }

        for (var i = 0; i < musicDuckingSounds.Count; i++)
        {
            var aDuck = musicDuckingSounds[i];
            if (aDuck.soundType == MasterAudio.NO_GROUP_NAME)
            {
                continue;
            }

            MasterAudio.AddSoundGroupToDuckList(aDuck.soundType, aDuck.riseVolStart);
        }

        for (var i = 0; i < customEventsToCreate.Count; i++)
        {
            var anEvent = customEventsToCreate[i];
            MasterAudio.CreateCustomEvent(anEvent.EventName);
        }

        hasCreated = true;
    }

    private void PopulateGroupData()
    {
        groupsToCreate.Clear();

        for (var i = 0; i < trans.childCount; i++)
        {
            var aGroup = trans.GetChild(i).GetComponent<DynamicSoundGroup>();
            if (aGroup == null)
            {
                continue;
            }

            aGroup.groupVariations.Clear();

            for (var c = 0; c < aGroup.transform.childCount; c++)
            {
                var aVar = aGroup.transform.GetChild(c).GetComponent<DynamicGroupVariation>();
                if (aVar == null)
                {
                    continue;
                }

                aGroup.groupVariations.Add(aVar);
            }

            groupsToCreate.Add(aGroup);
        }
    }

    public AudioSource PreviewerInstance
    {
        get
        {
            if (_previewerInstance == null)
            {
                _previewerInstance = this.GetComponent<AudioSource>();
                if (_previewerInstance == null)
                {
                    this.gameObject.AddComponent<AudioSource>();
                    _previewerInstance = this.GetComponent<AudioSource>();
                    _previewerInstance.priority = AudioPrioritizer.MAX_PRIORITY;
                }

                _previewerInstance.playOnAwake = false;
            }

            return _previewerInstance;
        }
    }

    public static int HardCodedBusOptions
    {
        get
        {
            return MasterAudio.HARD_CODED_BUS_OPTIONS + EXTRA_HARD_CODED_BUS_OPTIONS;
        }
    }

    /// <summary>
    /// This property can be used to read and write the Dynamic Sound Groups.
    /// </summary>	
    public List<DynamicSoundGroup> GroupsToCreate
    {
        get
        {
            return groupsToCreate;
        }
    }
}
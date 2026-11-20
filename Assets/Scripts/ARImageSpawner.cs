using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARImageSpawner : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] ARTrackedImageManager imageManager;

    [Header("Available Elements")]
    [SerializeField] List<ElementData> elements = new List<ElementData>();

    Dictionary<TrackableId, TrackedElement> spawnedElements = new Dictionary<TrackableId, TrackedElement>();

    public IEnumerable<TrackedElement> SpawnedElements => spawnedElements.Values;

    private void OnEnable()
    {
        imageManager.trackablesChanged.AddListener(OnImagesChanged);
    }

    private void OnDisable()
    {
        imageManager.trackablesChanged.RemoveListener(OnImagesChanged);
    }

    void OnImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach(ARTrackedImage addedImage in args.added)
        {
            RegisterImage(addedImage);
        }

        foreach (ARTrackedImage updatedImage in args.updated)
        {
            RegisterImage(updatedImage);
        }

        foreach (var removedImage in args.removed)
        {
            RemoveImage(removedImage.Key);
        }
    }

    void RegisterImage(ARTrackedImage image)
    {
        TrackableId id = image.trackableId;

        if(spawnedElements.TryGetValue(id, out TrackedElement existing))
        {
            existing.trackedImage = image;
            UpdateVisibility(existing);
            return;
        }

        string imageName = image.referenceImage.name;
        ElementData data = FindElement(imageName);

        if (data == null || data.prefab == null)
            return;

        GameObject model = Instantiate(data.prefab, image.transform);

        TrackedElement element = new TrackedElement(image, data, model);

        spawnedElements.Add(id, element);

        UpdateVisibility(element);

        Debug.Log("Element detected: " + data.elementName);
    }

    ElementData FindElement(string imageName)
    {
        foreach(ElementData data in elements)
        {
            if(data != null && data.elementName == imageName)
            {
                return data;
            }
        }

        return null;
    }

    void RemoveImage(TrackableId id)
    {
        if(spawnedElements.TryGetValue(id, out TrackedElement element))
        {
            element.isRemoved = true;

            if(element.instance != null)
            {
                Destroy(element.instance);
            }

            spawnedElements.Remove(id);
        }
    }

    //Muestra un elemento si esta siendo trackeado y no esta participando en una reaccion
    void UpdateVisibility(TrackedElement element)
    {
        bool shouldBeVisible = element.IsTracking() && !element.isReacting;

        element.SetVisible(shouldBeVisible);
    }

    //Llamada oir ARReactionManager
    public void SetElementReacting(TrackedElement element, bool reacting)
    {
        element.SetReacting(reacting);
        UpdateVisibility(element);
    }
}

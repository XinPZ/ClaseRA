using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ARReactionManager : MonoBehaviour
{
    [Header("Spawner")]
    [SerializeField] private ARImageSpawner imageSpawner;

    [Header("Available Reactions")]
    [SerializeField] private List<ElementReaction> reactions = new List<ElementReaction>();

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    GameObject resultInstance;

    //Elementos participando en la reaccion activa
    TrackedElement reactingA;
    TrackedElement reactingB;

    bool hasInteracted = false;

    private void Update()
    {
        if (hasInteracted)
        {
            if(reactingA == null || reactingB == null || !reactingA.IsTracking() || !reactingB.IsTracking())
            {
                CancelReaction();
                return;
            }

            return;
        }

        //Obtener los elementos trackeados actualmente
        List<TrackedElement> visibleElements = new List<TrackedElement>();

        foreach (TrackedElement element in imageSpawner.SpawnedElements)
        {
            if(element == null)
            {
                Debug.LogWarning("ARReactionManager: TrackedElement is NULL!");
                continue;
            }

            if(element.IsTracking() && !element.isReacting)
            {
                visibleElements.Add(element);
            }
        }

        for(int i = 0; i < visibleElements.Count; i++)
        {
            for(int j = i + 1; j < visibleElements.Count; j++)
            {
                if (CheckReaction(visibleElements[i], visibleElements[j]))
                    return;
            }
        }
    }

    bool CheckReaction(TrackedElement a, TrackedElement b)
    {
        foreach (ElementReaction reaction in reactions)
        {
            if (reaction == null)
                continue;

            if (!reaction.CanReact(a.data, b.data))
                continue;

            float distance = Vector3.Distance(a.GetPosition(), b.GetPosition());

            if (distance > reaction.interactionDistance)
                continue;

            if (reaction.result == null || reaction.result.prefab == null)
                continue;

            ExecuteReaction(a, b, reaction);

            return true;
        }

        return false;
    }

    void ExecuteReaction(TrackedElement a, TrackedElement b, ElementReaction reaction)
    {
        hasInteracted = true;

        reactingA = a;
        reactingB = b;

        imageSpawner.SetElementReacting(a, true);
        imageSpawner.SetElementReacting(b, true);

        Vector3 midPoint = (a.GetPosition() + b.GetPosition()) / 2f;

        resultInstance = Instantiate(reaction.result.prefab, midPoint, Quaternion.identity);

        ModelInfo info = resultInstance.GetComponent<ModelInfo>();

        if(info != null)
        {
            info.ElementData = reaction.result;
        }

        if(audioSource != null && reaction.reactionSound != null)
        {
            audioSource.PlayOneShot(reaction.reactionSound);
        }

        Debug.Log($"Reaction: {a.data.elementName} + {b.data.elementName} = {reaction.result.elementName}");
    }

    void CancelReaction()
    {
        //Destruir resultado
        if (resultInstance != null)
        {
            Destroy(resultInstance);
            resultInstance = null;
        }

        //Restaurar elementos originales
        if(reactingA != null)
        {
            imageSpawner.SetElementReacting(reactingA, false);
        }

        if (reactingB != null)
        {
            imageSpawner.SetElementReacting(reactingB, false);
        }

        reactingA = reactingB = null;

        hasInteracted = false;

        Debug.Log("Reaction cancelled");
    }

    public void ResetReaction()
    {
        if (hasInteracted)
        {
            CancelReaction();
        }
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "NrewReaction", menuName = "AR/Element Reaction")]

public class ElementReaction : ScriptableObject
{
    [Header("Required Elements")]
    public ElementData elementA;
    public ElementData elementB;

    [Header("Result")]
    public ElementData result;

    [Header("Interaction")]
    public float interactionDistance = 0.15f;
    public AudioClip reactionSound;

    public bool CanReact(ElementData a, ElementData b)
    {
        return (a == elementA && b == elementB) || (a == elementB && b == elementA);
    }
}

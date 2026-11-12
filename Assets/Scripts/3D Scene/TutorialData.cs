using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial", menuName = "Game/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    public string title;
    [TextArea(3, 6)] public string bodyText;
    public Sprite icon;
    public AudioClip voiceOver;
}
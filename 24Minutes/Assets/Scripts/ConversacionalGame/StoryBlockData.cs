using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Story Block", menuName = "Story/Story Block")]
public class StoryBlockData : ScriptableObject
{
    [TextArea(3, 10)] public string storyText;
    public string option1Text;
    public string option2Text;
    public string option3Text;
    public string option4Text;
    public StoryBlockData option1Block;
    public StoryBlockData option2Block;
    public StoryBlockData option3Block;
    public StoryBlockData option4Block;
}


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
    public bool option1IsTransition;
    public string option1TransitionScene;
    public StoryBlockData option2Block;
    public bool option2IsTransition;
    public string option2TransitionScene;
    public StoryBlockData option3Block;
    public bool option3IsTransition;
    public string option3TransitionScene;
    public StoryBlockData option4Block;
    public bool option4IsTransition;
    public string option4TransitionScene;
}


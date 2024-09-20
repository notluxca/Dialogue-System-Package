using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DialogueSystem.Editor.Windows
{
    using Elements;
    using Enumerations;

    public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DSGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(DSGraphView dsGraphView)
        {
            graphView = dsGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")),
                //new SearchTreeGroupEntry(new GUIContent("Dialogue Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Create Dialogue", indentationIcon))
                {
                    userData = DSDialogueType.SingleChoice,
                    level = 1
                },
                //new SearchTreeEntry(new GUIContent("Multiple Choice", indentationIcon))
                //{
                //    userData = DSDialogueType.MultipleChoice,
                //    level = 2
                //},
                //new SearchTreeGroupEntry(new GUIContent("Dialogue Groups"), 1),
                new SearchTreeEntry(new GUIContent("Add Group", indentationIcon))
                {
                    userData = new Group(),
                    level = 1
                }
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

            switch (SearchTreeEntry.userData)
            {
                case DSDialogueType.SingleChoice:
                    {
                        DSSingleChoiceNode singleChoiceNode = (DSSingleChoiceNode)graphView.CreateNode("", DSDialogueType.SingleChoice, null, localMousePosition);

                        graphView.AddElement(singleChoiceNode);

                        return true;
                    }

                case DSDialogueType.MultipleChoice:
                    {
                        DSMultipleChoiceNode multipleChoiceNode = (DSMultipleChoiceNode)graphView.CreateNode("", DSDialogueType.MultipleChoice, null, localMousePosition);

                        graphView.AddElement(multipleChoiceNode);

                        return true;
                    }

                case Group _:
                    {
                        graphView.CreateGroup("DialogueGroup", localMousePosition);

                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }
    }
}
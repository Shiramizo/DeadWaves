using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DeadWaves
{
    [CustomEditor(typeof(KidBehaviour))]
    public class KidBehaviourEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var kidScript = (KidBehaviour)target;
            CharacterInfo kidInfo = kidScript.TryGetCharacterInfo();

            GUILayout.Space(10f);

            string charDescriptionTitle = "CHARACTER INFO";
            string charDescription = "*** NO INFO PROVIDED ***";

            if (kidInfo != null && !string.IsNullOrWhiteSpace(kidInfo.characterDescription)) charDescription = kidInfo.characterDescription;

            GUILayout.Label(charDescriptionTitle, EditorStyles.boldLabel);
            GUILayout.Label(charDescription);
        }
    }
}
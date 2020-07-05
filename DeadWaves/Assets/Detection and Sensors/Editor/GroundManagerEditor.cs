using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DeadWaves
{
    [CustomEditor(typeof(GroundManager))]
    public class GroundManagerEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Priority is defined by the TOP MOST child object\nso move them up and down to change it", EditorStyles.boldLabel,
                GUILayout.MinHeight(50f));

            var groundManager = (GroundManager)target;

            if (groundManager.GetAllSensorsLength() > 0) {
                var sensorArray = groundManager.GetAllSensors();
                for (int i = 0; i < sensorArray.Length; i++) {
                    if (sensorArray[i] == null) continue;
                    EditorGUILayout.LabelField($"{i + 1}- {sensorArray[i].gameObject.name}");
                }
            }
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace SF
{
    [CustomEditor(typeof(AudioDebug))]
    public class AudioDebugIns:Editor
    {
#if UNITY_EDITOR && AUDIO_DEBUG
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical();

            var result = AudioService.GetInfo();
            EditorGUILayout.LabelField(result.Item1);
            EditorGUILayout.LabelField(result.Item2);
            EditorGUILayout.LabelField(result.Item3);
            EditorGUILayout.LabelField(result.Item4);

            EditorGUILayout.EndVertical();
        }
#endif
    }
}
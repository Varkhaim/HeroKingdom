using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Hero Kingdom/Database/Spell", order = 2)]
public class Spell : ScriptableObject
{
    public int spellID;
    public string Name;
    public SpellEffectID spellEffectID;

    [Header("Spell Params")]
    public GameObject MissilePrefab;
    public SpellEffectID ReachEffectID;
}

[CustomEditor(typeof(Spell))]
public class Spell_Editor : Editor
{
    SerializedProperty spellID;
    SerializedProperty Name;
    SerializedProperty spellEffectID;

    SerializedProperty MissilePrefab;
    SerializedProperty ReachEffectID;

    void OnEnable()
    {
        spellID = serializedObject.FindProperty("spellID");
        Name = serializedObject.FindProperty("Name");
        spellEffectID = serializedObject.FindProperty("spellEffectID");
        MissilePrefab = serializedObject.FindProperty("MissilePrefab");
        ReachEffectID = serializedObject.FindProperty("ReachEffectID");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(spellID);
        EditorGUILayout.PropertyField(Name);
        EditorGUILayout.PropertyField(spellEffectID);
        EditorGUI.indentLevel += 1;
        SetParams(spellEffectID.intValue);

        serializedObject.ApplyModifiedProperties();
    }

    private void SetParams(int val)
    {
        SpellEffectID spellEffectID = (SpellEffectID)val;

        switch (spellEffectID)
        {
            case SpellEffectID.INSTANT_DAMAGE:
                break;
            case SpellEffectID.SPAWN_PROJECTILE:
                {
                    EditorGUILayout.PropertyField(MissilePrefab);
                    EditorGUILayout.PropertyField(ReachEffectID);
                }
                break;
        }
    }
}
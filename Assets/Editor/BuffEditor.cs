using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;

public class BuffEditor : EditorWindow
{
    static SkillData skillData;
    BuffData buffData = new BuffData();
    static string[] skillNames;
    static string savePath;
    GameObject model;
    Vector2 scol;
    public static void Init(string title, string path)
    {
        GetWindow<BuffEditor>(title).Show();
        savePath = path;
        try
        {
            string json = Resources.Load<TextAsset>(path).text;
            skillData = JsonConvert.DeserializeObject<SkillData>(json);
        }
        catch (System.Exception)
        {
            skillData = new SkillData();
        }

        skillNames = new string[skillData.buffs.Count];
        for (int i = 0; i < skillData.buffs.Count; i++)
        {
            skillNames[i] = skillData.buffs[i].name;
        }
    }

    private void OnGUI()
    {

        GUILayout.Space(10);
        GUILayout.Label("������");

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("�������:");
        buffData.name = EditorGUILayout.TextField(buffData.name);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("�������:");
        buffData.description = EditorGUILayout.TextField(buffData.description);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("�˺�");
        buffData.damage = EditorGUILayout.FloatField(buffData.damage);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("����");
        buffData.recaver = EditorGUILayout.FloatField(buffData.recaver);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("����");
        buffData.Add = EditorGUILayout.FloatField(buffData.Add);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("����ʱ��");
        buffData.triggerTime = EditorGUILayout.FloatField(buffData.triggerTime);
        GUILayout.EndHorizontal();

        //�����Ϣ
        GUILayout.BeginHorizontal();

        GUILayout.Label("Ч��");
        model = Resources.Load<GameObject>(buffData.modelPath);
        model = (GameObject)EditorGUILayout.ObjectField(model, typeof(GameObject), true);
        if (model != null)
        {
            string str = AssetDatabase.GetAssetPath(model);
            str = str.Replace("Assets/Resources/", "");
            str = str.Replace(".prefab", "");
            buffData.modelPath = str;
        }

        if (GUILayout.Button("���"))
        {
            skillData.buffs.Add(buffData);
        }

        GUILayout.EndHorizontal();

        scol = GUILayout.BeginScrollView(scol);

        var buffs = skillData.buffs;
        GUILayout.Space(20);
        for (int i = 0; i < buffs.Count; i++)
        {
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.Label("�������:" + buffs[i].name);
            GUILayout.Label("�������:" + buffs[i].description);
            if (GUILayout.Button("ɾ��"))
            {
                buffs.RemoveAt(i);
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("����ʱ��:" + buffs[i].triggerTime);
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("����"))
        {
            string json = JsonConvert.SerializeObject(skillData);
            File.WriteAllText(Application.dataPath + "/Resources/" + savePath + ".json", json);
            AssetDatabase.Refresh();
        }
    }

}

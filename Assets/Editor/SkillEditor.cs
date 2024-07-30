using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
public class SkillEditor : EditorWindow
{
    static List<SkillData> skillDataList;
    static SkillData skillData;
    GameObject model;
    Sprite sprite;
    AudioClip audioClip;
    AnimationClip animationClip;
    GameObject bot;
    GameObject effect;
    static int currentSkillId;
    static string[] audios;
    static string[] animations;
    static List<GameObject> destoryList;
    static List<TrackAsset> trackAssets;
    TimelineAsset timelineAsset;
    static string path;
    static string saveFilePath;

    static string shareSkillData;

    [MenuItem("SkillEditor/Skill")]
    public static void Init()
    {
        saveFilePath = "Data/SkillData" + currentSkillId;
        GetWindow<SkillEditor>().Show();
        destoryList = new List<GameObject>();
        try
        {
            skillDataList = JsonConvert.DeserializeObject<List<SkillData>>(Resources.Load<TextAsset>("Data/AllSkill").text);
        }
        catch (System.Exception)
        {
            skillDataList = new List<SkillData>();
        }
        audios = new string[] { "Audio/Reload", "Audio/hit" };
        animations = new string[] { "Aniamtion/Attack2", "Aniamtion/Attack3" };
        skillData = new SkillData();
        trackAssets = new List<TrackAsset>();
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        if (GUILayout.Button("选择文件夹"))
        {
            path = EditorUtility.OpenFolderPanel("Open Floder", @"Assets/Resources", "");
        }
        GUILayout.Label(path);

        if (GUILayout.Button("导入技能配置表"))
        {
            string filePath = EditorUtility.OpenFilePanel("Load png Textures", path, "json");
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    filePath = filePath.Replace("E:/AA_SkillEditor/Project/WeekExam2Copy/Assets/Resources/", "");
                    filePath = filePath.Replace(".json", "");
                    string json = Resources.Load<TextAsset>(filePath).text;
                    skillData = JsonConvert.DeserializeObject<SkillData>(json);
                }
                catch (System.Exception)
                {
                    Debug.LogWarning("File read Error ! ! !");
                }
            }
        }

        SetBaseData();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("预览技能效果", GUILayout.Height(20), GUILayout.Width(100)))
        {
            if (model == null)
            {
                Debug.Log("请选择技能效果");
                return;
            }

            if (bot == null)
            {
                bot = Instantiate(Resources.Load<GameObject>("Model/TeddyBear"));
                destoryList.Add(bot);
            }

            if (effect == null)
            {
                //实例化技能
                effect = Instantiate(model);
                destoryList.Add(effect);
            }

            PlayableDirector playableDirector = effect.GetComponent<PlayableDirector>();
            timelineAsset = playableDirector.playableAsset as TimelineAsset;
            TrackAsset trackAsset = timelineAsset.GetOutputTrack(0);
            playableDirector.SetGenericBinding(trackAsset, bot);

            //添加buff
            for (int i = 0; i < skillData.buffs.Count; i++)
            {
                var buff = Instantiate(Resources.Load<GameObject>(skillData.buffs[i].modelPath));
                destoryList.Add(buff);
                TrackAsset track = timelineAsset.CreateTrack<ActivationTrack>();
                trackAssets.Add(track);
                TimelineClip tlc = track.CreateDefaultClip();
                tlc.start = skillData.buffs[i].triggerTime;
                tlc.duration = 1;
                playableDirector.SetGenericBinding(track, buff);
            }

            playableDirector.Play();
        }

        if (GUILayout.Button("保存", GUILayout.Height(20), GUILayout.Width(100)))
        {
            string json = JsonConvert.SerializeObject(skillData);
            File.WriteAllText(Application.dataPath + "/Resources/" + saveFilePath + ".json", json);
            AssetDatabase.Refresh();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("导出", GUILayout.Height(20), GUILayout.Width(100)))
        {
            string filePath = EditorUtility.SaveFilePanel("SaveJson", "Resources", "saveSkills", "json");
            if (!string.IsNullOrEmpty(filePath))
            {
                string json = JsonConvert.SerializeObject(skillData);
                File.WriteAllText(filePath, json);
                AssetDatabase.Refresh();
            }
        }

        if (GUILayout.Button("分享", GUILayout.Height(20), GUILayout.Width(100)))
        {
            shareSkillData = JsonConvert.SerializeObject(skillData);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("导入分享"))
        {
            try
            {
                if (!string.IsNullOrEmpty(shareSkillData))
                {
                    skillData = JsonConvert.DeserializeObject<SkillData>(shareSkillData);
                }
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Please frist select share button");
            }
        }
        GUILayout.EndHorizontal();
    }

    private void SetBaseData()
    {
        GUILayout.Space(10);
        GUILayout.Label("选择模型");
        model = Resources.Load<GameObject>(skillData.modelPath);
        model = (GameObject)EditorGUILayout.ObjectField(model, typeof(GameObject), true);
        if (model != null)
        {
            string str = AssetDatabase.GetAssetPath(model);
            str = str.Replace("Assets/Resources/", "");
            str = str.Replace(".prefab", "");
            skillData.modelPath = str;
        }
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("技能名称:");
        skillData.name = EditorGUILayout.TextField(skillData.name);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("技能描述:");
        skillData.description = EditorGUILayout.TextField(skillData.description);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("技能触发条件:");
        skillData.triggerType = (TriggerType)EditorGUILayout.EnumPopup(skillData.triggerType);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("技能范围:");
        skillData.skillRange = (SkillRange)EditorGUILayout.EnumPopup(skillData.skillRange);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("技能图标:");
        sprite = Resources.Load<Sprite>(skillData.iconPath);
        sprite = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), true);

        if (sprite != null)
        {
            string str = AssetDatabase.GetAssetPath(sprite);
            str = str.Replace("Assets/Resources/", "");
            str = str.Replace(".png", "");
            skillData.modelPath = str;
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("技能动画:");
        animationClip = Resources.Load<AnimationClip>(animations[skillData.animationId]);
        skillData.animationId = EditorGUILayout.Popup(skillData.animationId, animations);

        if (GUILayout.Button("预览动画"))
        {
            if (bot == null)
            {
                bot = Instantiate(Resources.Load<GameObject>("Model/TeddyBear"));
                destoryList.Add(bot);
            }
            var botAnimator = bot.GetComponent<Animator>();
            RuntimeAnimatorController controller = botAnimator.runtimeAnimatorController;
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(controller);
            animatorOverrideController["Idle1"] = animationClip;
            bot.GetComponent<Animator>().runtimeAnimatorController = animatorOverrideController;

            destoryList.Add(bot);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("技能音频:");
        audioClip = Resources.Load<AudioClip>(audios[skillData.audioId]);
        skillData.audioId = EditorGUILayout.Popup(skillData.audioId, audios);

        if (GUILayout.Button("预览声音"))
        {
            GameObject audio = new GameObject();
            var source = audio.AddComponent<AudioSource>();
            source.clip = audioClip;
            source.Play();
            destoryList.Add(audio);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        if (GUILayout.Button("创建", GUILayout.Height(20)))
        {
            skillData.id = skillDataList.Count;
            currentSkillId++;
            if (!skillDataList.Contains(skillData))
            {
                skillDataList.Add(skillData);
            }

            string json = JsonConvert.SerializeObject(skillDataList);
            File.WriteAllText("Assets/Resources/Data/AllSkill.json", json);
            AssetDatabase.Refresh();
        }

        GUILayout.BeginVertical();
        GUILayout.Label("已添加的技能:");

        for (int i = 0; i < skillDataList.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("技能名称:" + skillDataList[i].name);
            GUILayout.Label("技能描述:" + skillDataList[i].description);
            if (GUILayout.Button("添加增益效果"))
            {
                BuffEditor.Init(skillDataList[i].name, saveFilePath);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
    }



    private void OnDestroy()
    {
        for (int i = 0; i < destoryList.Count; i++)
        {
            DestroyImmediate(destoryList[i]);
        }
        for (int i = 0; i < trackAssets.Count; i++)
        {
            timelineAsset.DeleteTrack(trackAssets[i]);
        }
    }
}

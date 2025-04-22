﻿using KModTool;
using MinionAge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MinionAge_DLC
{
    //  炽热金属分享者 
    public class SpecializationScorchingMetalSharer
    {

        public static void TriggerScan(GameObject minion)
        {
            if (minion == null)
            {
                Debug.LogError("Minion 对象不能为空！");
                return;
            }
            // 检查对象是否具有目标 Tag和buff
            var prefabID = minion.GetComponent<KPrefabID>();
            // 检查 CoolWanderer Buff 是否存在
            float bufftime = KModDeBuff.GetEffectRemainingTime(minion, "ScorchingMetalSharer");
            if (bufftime <= 0 || prefabID.HasTag("Corpse"))
            {
                KModDeBuff.RemoveEffect(minion, "ScorchingMetalSharer");
                return;
            }

            SpawnRandomProjectiles(minion.gameObject.transform.GetPosition() + new Vector3(0f, 1f, 0f));
            AudioUtil.PlaySound(ModAssets.Sounds.WW, CameraController.Instance.GetVerticallyScaledPosition(minion.gameObject.transform.GetPosition()), 1f);
          
          
        }










        public static void SpawnRandomProjectiles(Vector3 spawnPosition)
        {
            // 预定义的 Tag 列表
            List<Tag> elementTags = new List<Tag>
            {
                new Tag("Gold"),
                new Tag("Iron"),
                new Tag("Copper"),
                new Tag("Aluminum"),
                new Tag("Steel"),
                new Tag("EnrichedUranium"),
                new Tag("Glass"),
                new Tag("TestElement")
            };

            for (int i = 0; i < 15; i++)
            {
                // 随机力度和方向
                float minHorizontalSpeed = 4f;
                float maxHorizontalSpeed = 20f;
                float minVerticalSpeed = 4f;
                float maxVerticalSpeed = 10f;

                float horizontalSpeed = UnityEngine.Random.Range(minHorizontalSpeed, maxHorizontalSpeed);
                horizontalSpeed *= UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1; // 随机选择正负方向

                float verticalSpeed = UnityEngine.Random.Range(minVerticalSpeed, maxVerticalSpeed);

                Vector2 initialVelocity = new Vector2(horizontalSpeed, verticalSpeed);

                // 随机选择一个 Tag
                Tag randomTag = elementTags[UnityEngine.Random.Range(0, elementTags.Count)];

                // 使用 GameScheduler 添加延迟
                GameScheduler.Instance.Schedule(
                    "SpawnDelay", // 任务名称
                    i * 0.2f,     // 延迟时间（秒）
                    (object _) =>
                    {
                        // 获取预制体
                        GameObject prefab = Assets.GetPrefab(randomTag);
                        if (prefab == null)
                        {
                            Debug.LogWarning($"未找到预制体: {randomTag}");
                            return;
                        }

                        // 实例化物品
                        GameObject instance = GameUtil.KInstantiate(prefab, spawnPosition, Grid.SceneLayer.Ore);
                        if (instance == null){ Debug.LogWarning($"实例化失败: {randomTag}" ); return;}


                        PrimaryElement component2 = instance.GetComponent<PrimaryElement>();
                        component2.Temperature = 20f + 273.15f;
                        component2.Units = 50;

                        instance.SetActive(true);

                        // 添加重力组件
                        if (!GameComps.Gravities.Has(instance))
                        {
                            GameComps.Gravities.Add(instance, initialVelocity);
                        }

                        //添加 Faller 组件
                        if (!GameComps.Fallers.Has(instance))
                        {
                            GameComps.Fallers.Add(instance, initialVelocity);
                        }
                       
                    },
                    null
                );
            }
        }











    }
}

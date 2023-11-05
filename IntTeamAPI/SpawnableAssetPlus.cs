using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//This API is released under the zlib license, by using it for your mod and/or downloading it you confirm that you read and agreed to the terms of said license.
//Link to the original repository: https://github.com/Int-team/Int-Team-API
//API DEPENDENCIES: IntAPI.cs

namespace IntTeamAPI
{
    public class SpawnableAssetPlus : ScriptableObject
    {
        internal static bool loaded = false;
        public static Dictionary<string, SpawnableAssetPlus> spawnableByName = new Dictionary<string, SpawnableAssetPlus>();

        public SpawnableAsset originalItem;
        public string itemName;
        public Action<GameObject> afterSpawn;

        public static Action errorAction = () => IntAPI.ErrorNotify("SpawnableAssetPlus is not enabled. (Add CustomSpawnableAsset.Load() to Main())");

        internal class CustomSerializationInstructions : MonoBehaviour, Messages.IOnAfterDeserialise
        {
            internal string originalAsset;

            public void OnAfterDeserialise(List<GameObject> gameObjects)
            {
                FindSpawnablePlus(originalAsset).afterSpawn(gameObject);
            }
        }

        /// <summary>
        /// Must be called once in Main for this to work properly."/>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Load()
        {
            loaded = true;
            spawnableByName.Clear();
        }

        public static void Register(SpawnableAsset originalItem, string nameOverride, Action<GameObject> afterSpawn)
        {
            if (!loaded)
                errorAction();
            SpawnableAssetPlus asset = CreateInstance<SpawnableAssetPlus>();
            asset.originalItem = originalItem;
            asset.itemName = nameOverride;
            asset.afterSpawn = afterSpawn;
            spawnableByName.Add(nameOverride, asset);
        }

        public static SpawnableAssetPlus FindSpawnablePlus(string name)
        {
            if (!loaded)
                errorAction();
            return spawnableByName.TryGetValue(name, out SpawnableAssetPlus asset) ? asset : null;
        }

        public GameObject Spawn(Vector2 position, bool flipped)
        {
            if (!loaded)
                errorAction();
            GameObject instance = Instantiate(originalItem.Prefab, position, Quaternion.identity);
            instance.AddComponent<DeregisterBehaviour>();
            if (flipped)
            {
                Vector3 localScale = instance.transform.localScale;
                localScale.x *= -1f;
                instance.transform.localScale = localScale;
            }
            instance.AddComponent<TexturePackApplier>();
            instance.AddComponent<AudioSourceTimeScaleBehaviour>();
            instance.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = originalItem;
            Destroy(instance.GetComponent<RemoveWhenModded>());
            instance.name = itemName;
            CatalogBehaviour.SpawnedGameObjects.Add(instance);
            UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(instance));
            CatalogBehaviour.PerformMod(originalItem, instance);

            afterSpawn(instance);
            instance.AddComponent<CustomSerializationInstructions>().originalAsset = itemName;

            return instance;
        }

        /// <summary>
        /// Spawns a <see cref="SpawnableAssetPlus"/> through an item.
        /// </summary>
        /// <remarks>
        /// The item will be scaled and rotated in accordance to the item. The <paramref name="localPosition"/> will also be scaled with the <paramref name="transform"/>. The method does not make the <paramref name="item"/> a child of <paramref name="transform"/>.<br/>
        /// Using this method will also make it so that when the spawning of the <paramref name="transform"/>'s item is undone with the 'undo' key, so will the spawning of this item be undone. Deleting the <paramref name="transform"/>'s object manually will not delete this item.
        /// </remarks>
        /// <param name="transform">The transform through which the item will be spawned.</param>
        /// <param name="localPosition">The position where the item will be spawned, relative to the <paramref name="transform"/></param>
        /// <param name="spawnSpawnParticles">Should spawn particles be played when the item appears? False by default. Should be false for e.g. arrows.</param>
        /// <returns>
        /// The <see cref="GameObject"/> of the item.
        /// </returns>
        public GameObject SpawnThroughItem(Transform transform, Vector3 localPosition = default, bool spawnSpawnParticles = false)
        {
            if (!loaded)
                errorAction();
            localPosition = transform.position + transform.rotation * Vector2.Scale(localPosition, transform.lossyScale);
            GameObject instance = Instantiate(originalItem.Prefab, localPosition, Quaternion.identity);
            instance.AddComponent<DeregisterBehaviour>();

            instance.transform.rotation = transform.rotation;
            instance.transform.localScale = transform.lossyScale;

            foreach (PhysicalBehaviour phys in instance.GetAllComponents<PhysicalBehaviour>())
            {
                phys.SpawnSpawnParticles = spawnSpawnParticles;
            }

            instance.AddComponent<TexturePackApplier>();
            instance.AddComponent<AudioSourceTimeScaleBehaviour>();
            instance.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = originalItem;
            Destroy(instance.GetComponent<RemoveWhenModded>());
            instance.name = itemName;
            CatalogBehaviour.SpawnedGameObjects.Add(instance);

            if (UndoControllerBehaviour.FindRelevantAction(transform.root.gameObject, out IUndoableAction undoableAction))
            {
                if (undoableAction is ObjectCreationAction objectCreationAction)
                {
                    objectCreationAction.RelevantObjects.Add(instance);
                }
                else if (undoableAction is PasteLoadAction pasteLoadAction)
                {
                    List<UnityEngine.Object> relevantObjects = (List<UnityEngine.Object>)typeof(PasteLoadAction).GetProperty("RelevantObjects").GetValue(pasteLoadAction);
                    relevantObjects.Add(instance);
                }
            }

            CatalogBehaviour.PerformMod(originalItem, instance);
            afterSpawn(instance);
            instance.AddComponent<CustomSerializationInstructions>().originalAsset = itemName;

            return instance;
        }
    }
}

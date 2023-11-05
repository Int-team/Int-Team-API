using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//This API is released under the zlib license, by using it for your mod and/or downloading it you confirm that you read and agreed to the terms of said license.
//Link to the original repository: https://github.com/Int-team/Int-Team-API
//API DEPENDENCIES: None

namespace IntTeamAPI
{
    public static class CreationPlus
    {
        /// <summary>
        /// A copy of the game's method for spawning items, excluding incrementing the stats. Should be used sparingly.
        /// </summary>
        /// <returns>
        /// The <see cref="GameObject"/> of the item.
        /// </returns>
        public static GameObject Spawn(this SpawnableAsset e, Vector3 position, bool flipped)
        {
            GameObject instance = UnityEngine.Object.Instantiate(e.Prefab, position, Quaternion.identity);
            instance.AddComponent<DeregisterBehaviour>();
            if (flipped)
            {
                Vector3 localScale = instance.transform.localScale;
                localScale.x *= -1f;
                instance.transform.localScale = localScale;
            }
            instance.AddComponent<TexturePackApplier>();
            instance.AddComponent<AudioSourceTimeScaleBehaviour>();
            instance.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = e;
            instance.name = e.name;
            CatalogBehaviour.SpawnedGameObjects.Add(instance);
            UndoControllerBehaviour.RegisterAction(new ObjectCreationAction(instance));
            CatalogBehaviour.PerformMod(e, instance);

            return instance;
        }

        /// <summary>
        /// A copy of the game's method for spawning items, with a few useful modifications.
        /// </summary>
        /// <remarks>
        /// The item will be scaled and rotated in accordance to the item. The <paramref name="localPosition"/> will also be scaled with the <paramref name="transform"/>. The method does not make the <paramref name="item"/> a child of <paramref name="transform"/>.<br/>
        /// Using this method will also make it so that when the spawning of the <paramref name="transform"/>'s item is undone with the 'undo' key, so will the spawning of this item be undone. Deleting the <paramref name="transform"/>'s object manually will not delete this item.
        /// </remarks>
        /// <param name="item">The item you wish to spawn.</param>
        /// <param name="transform">The transform through which the item will be spawned.</param>
        /// <param name="localPosition">The position where the item will be spawned, relative to the <paramref name="transform"/></param>
        /// <param name="spawnSpawnParticles">Should spawn particles be played when the item appears? False by default. Should be false for e.g. arrows.</param>
        /// <returns>
        /// The <see cref="GameObject"/> of the item.
        /// </returns>
        public static GameObject SpawnThroughItem(this SpawnableAsset item, Transform transform, Vector3 localPosition = default, bool spawnSpawnParticles = false)
        {
            localPosition = transform.position + transform.rotation * Vector2.Scale(localPosition, transform.lossyScale);

            GameObject instance = UnityEngine.Object.Instantiate(item.Prefab, localPosition, Quaternion.identity);
            instance.AddComponent<DeregisterBehaviour>();

            instance.transform.rotation = transform.rotation;
            instance.transform.localScale = transform.lossyScale;

            foreach (PhysicalBehaviour phys in instance.GetAllComponents<PhysicalBehaviour>())
            {
                phys.SpawnSpawnParticles = spawnSpawnParticles;
            }

            instance.AddComponent<TexturePackApplier>();
            instance.AddComponent<AudioSourceTimeScaleBehaviour>();
            instance.AddComponent<SerialiseInstructions>().OriginalSpawnableAsset = item;
            instance.name = item.name;
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
            CatalogBehaviour.PerformMod(item, instance);

            return instance;
        }

        /// <summary>
        /// Creates a fixed joint between 2 objects.
        /// </summary>
        /// <param name="main">The object that will contain the <see cref="FixedJoint2D"/> component.</param>
        /// <param name="other">The object that will be attached to <paramref name="main"/>.</param>
        /// <returns>
        /// The <see cref="FixedJoint2D"/> component.
        /// </returns>
        public static FixedJoint2D CreateFixedJoint(this GameObject main, GameObject other)
        {
            FixedJoint2D joint = main.AddComponent<FixedJoint2D>();
            joint.dampingRatio = 1;
            joint.frequency = 0;
            joint.connectedBody = other.GetComponent<Rigidbody2D>();
            return joint;
        }

        /// <summary>
        /// Creates a <see cref="FixedJoint2D"/> between 2 objects.
        /// </summary>
        /// <remarks>
        /// Contains an extra <paramref name="position"/> parameter if you need to change it for some reason.
        /// </remarks>
        /// <param name="main">The object that will contain the <see cref="FixedJoint2D"/> component.</param>
        /// <param name="other">The object that will be attached to <paramref name="main"/>.</param>
        /// <returns>
        /// The <see cref="FixedJoint2D"/> component.
        /// </returns>
        public static FixedJoint2D CreateFixedJoint(this GameObject main, GameObject other, Vector2 position)
        {
            FixedJoint2D joint = main.AddComponent<FixedJoint2D>();
            joint.dampingRatio = 1;
            joint.frequency = 0;
            joint.connectedBody = other.GetComponent<Rigidbody2D>();
            joint.anchor = position;
            return joint;
        }

        /// <summary>
        /// Creates a <see cref="HingeJoint2D"/> between 2 objects.
        /// </summary>
        /// <param name="main">The object that will contain the <see cref="HingeJoint2D"/> component.</param>
        /// <param name="other">The object that will be attached to <paramref name="main"/>.</param>
        /// <param name="position">The position of the hinge joint.</param>
        /// <returns>
        /// The <see cref="HingeJoint2D"/> component.
        /// </returns>
        public static HingeJoint2D CreateHingeJoint(this GameObject main, GameObject other, Vector2 position)
        {
            HingeJoint2D joint = main.AddComponent<HingeJoint2D>();
            joint.connectedBody = other.GetComponent<Rigidbody2D>();
            joint.anchor = position;
            joint.useLimits = false;
            return joint;
        }

        /// <summary>
        /// Creates a <see cref="HingeJoint2D"/> between 2 objects.
        /// </summary>
        /// <param name="main">The object that will contain the <see cref="HingeJoint2D"/> component.</param>
        /// <param name="other">The object that will be attached to <paramref name="main"/>.</param>
        /// <param name="position">The position of the hinge joint.</param>
        /// <param name="minDeg">The minimum degree of rotation for the <see cref="HingeJoint2D"/>.</param>
        /// <param name="maxDeg">The maximum degree of rotation for the <see cref="HingeJoint2D"/>.</param>
        /// <returns>
        /// The <see cref="HingeJoint2D"/> component.
        /// </returns>
        public static HingeJoint2D CreateHingeJoint(this GameObject main, GameObject other, Vector2 position, float minDeg, float maxDeg)
        {
            HingeJoint2D joint = main.AddComponent<HingeJoint2D>();
            joint.connectedBody = other.GetComponent<Rigidbody2D>();
            joint.anchor = position;
            JointAngleLimits2D limits = joint.limits;
            limits.min = minDeg;
            limits.max = maxDeg;
            joint.limits = limits;
            joint.useLimits = true;
            return joint;
        }


        /// <summary>
        /// A method that will help with creating debris for an object.
        /// </summary>
        /// <remarks>
        /// This method is outdated and has issues like the debris not being able to be copied/saved. It is recommended to use <see cref="SpawnableAssetPlus"/> instead.
        /// </remarks>
        /// <param name="name">The name of the debris.</param>
        /// <param name="parent">The parent of the debris.</param>
        /// <param name="sprite">The sprite of the debris.</param>
        /// <param name="localPosition">The position of the debris relative to <paramref name="parent"/>.</param>
        /// <returns></returns>
        [Obsolete]
        public static GameObject CreateDebris(string name, Transform parent, Sprite sprite, Vector2 localPosition = default)
        {
            GameObject myGameObject = ModAPI.CreatePhysicalObject(name, sprite);
            myGameObject.transform.position = parent.position + parent.rotation * Vector2.Scale(localPosition, parent.localScale);
            myGameObject.transform.rotation = parent.rotation;
            myGameObject.transform.localScale = parent.lossyScale;
            parent.gameObject.GetOrAddComponent<InformalChildren>().childrenObjects.Add(myGameObject);
            myGameObject.AddComponent<DebrisComponent>();
            return myGameObject;
        }

        /// <summary>
        /// Creates a particle effect from a <see cref="GameObject"/>.
        /// </summary>
        /// <remarks>
        /// Some objects, like the glass pane, have a special <see cref="GameObject"/> with a cool particle effect, but they also spawn with some extra objects which makes it inpractical to use. This method removes the spare stuff and in general adapts such objects to only leave the particle behind.
        /// </remarks>
        /// <param name="item"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns>
        /// The <see cref="ParticleSystem"/>, aka the particles.
        /// </returns>
        public static ParticleSystem CreateParticles(GameObject item, Transform parent, Vector2 position = default, Quaternion rotation = default)
        {
            GameObject particleObject = UnityEngine.Object.Instantiate(item, parent.position, parent.rotation * rotation);
            particleObject.transform.SetParent(parent);
            particleObject.transform.localPosition = position;
            particleObject.transform.localScale = parent.localScale.GetAbs();
            UnityEngine.Object.Destroy(particleObject.GetComponent<DestroyWhenAllChildrenDestroyed>());
            foreach (Transform myTransform in particleObject.GetComponentsInChildren<Transform>())
            {
                if (myTransform != particleObject.transform)
                    UnityEngine.Object.Destroy(myTransform.gameObject);
            }
            ParticleSystem particles = particleObject.GetComponent<ParticleSystem>();
            UnityEngine.Object.Destroy(particleObject, particles.main.duration);
            return particles;
        }

        private class InformalChildren : MonoBehaviour
        {
            public List<GameObject> childrenObjects = new List<GameObject>();
            protected int count;

            protected void FixedUpdate()
            {
                foreach (GameObject myObject in childrenObjects)
                {
                    if (myObject == null)
                    {
                        childrenObjects.Remove(myObject);
                        if (childrenObjects.Count == 0)
                            Destroy(gameObject);
                        break;
                    }
                }
            }

            protected void OnDestroy()
            {
                foreach (GameObject myObject in childrenObjects)
                {
                    Destroy(myObject);
                }
            }
        }       
    }
}


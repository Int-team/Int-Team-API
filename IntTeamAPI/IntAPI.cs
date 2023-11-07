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
    public static class IntAPI
    {
        public const float ton = 25f;

        /// <remarks>
        /// For reference, a rod weighs around 3 kilograms
        /// </remarks>
        public const float kilogram = 0.025f;

        public const float liter = 2.8f;
        public const float syringe = 0.5f * liter;
        public const float flask = liter;
        public const float bloodTank = 5f * liter;

        /// <summary>
        /// Allows you to make something happen only once without having to make an extra method for it. Here is how to use it:
        /// </summary>
        /// <remarks>
        /// <code>
        /// (Instance) =>
        /// {
        ///     if(IntAPI.IsFirstSpawn(Instance))
        ///     {
        ///         //Your code here
        ///     }
        /// }
        /// </code>
        /// </remarks>
        public static bool IsFirstSpawn(GameObject gameObject)
        {
            if (gameObject.HasComponent<OnFirstSpawn>())
                return false;
            gameObject.AddComponent<OnFirstSpawn>();
            return true;
        }

        private class OnFirstSpawn : MonoBehaviour
        {
        }

        public static float LiquidUnitsToLiters(float num)
        {
            return num / liter;
        }

        public static float LitersToLiquidUnits(float num)
        {
            return num * liter;
        }

        /// <summary>
        /// Allows you to toggle the collision of multiple things, most useful for entities.
        /// </summary>
        /// <param name="affectItself">Should the collisions between different colliders in <paramref name="others"/> be toggled off as well? The default is false.</param>
        public static void IgnoreEntityCollision(this Collider2D main, Collider2D[] others, bool ignColl, bool affectItself = false)
        {
            foreach (Collider2D a in others)
            {
                IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(main, a, ignColl);
                foreach (Collider2D b in others)
                {
                    if (a && b && a != b && a.transform != b.transform && affectItself)
                        IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(a, b, ignColl);
                }
            }
        }

        /// <summary>
        /// It's the same thing <see cref="IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(Collider2D, Collider2D, bool)"/> but does not take up the entire line.
        /// </summary>
        public static void IgnoreCollision(this Collider2D main, Collider2D other, bool ignColl)
        {
            IgnoreCollisionStackController.IgnoreCollisionSubstituteMethod(main, other, ignColl);
        }


        /// <remarks>
        /// Doesn't actually change the alpha of <paramref name="color"/>, but instead returns a new color with the same RGB values but a new alpha.
        /// </remarks>
        /// <returns>
        /// The new <see cref="Color"/>.
        /// </returns>
        public static Color ChangeAlpha(this Color color, float alpha)
        {
            float trueAlpha = Mathf.Clamp01(alpha);
            return new Color(color.r, color.g, color.b, trueAlpha);
        }

        /// <remarks>
        /// <see cref="ChangeAlpha(Color, float)"/>, but for <see cref="Color32"/> instead.
        /// </remarks>
        /// <returns>
        /// The new <see cref="Color32"/>.
        /// </returns>
        public static Color32 ChangeAlpha(this Color32 color, byte alpha)
        {
            return new Color32(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// A special function that's meant to take an input that changes as time passes.
        /// </summary>
        /// <remarks>
        /// The method outputs 0 at even multiples of <paramref name="period"/> (0 * <paramref name="period"/>, 2 * <paramref name="period"/>, 4 * <paramref name="period"/>, etc.).<br/>
        /// The method outputs 1 at odd multiples of <paramref name="period"/> (1 * <paramref name="period"/>, 3 * <paramref name="period"/>, 5 * <paramref name="period"/>, etc.).<br/>
        /// The values in-between follow a smooth wave shape.
        /// </remarks>
        /// <param name="num">The input of the method, which should be a value that changes somehow as time passes</param>
        /// <param name="period">The period of the cosine wave. Should be a constant value.</param>
        /// <returns>
        /// A value ranging from 0 to 1, based on the conditions mentioned prior
        /// </returns>
        public static float WaveClamp01(float num, float period)
        {
            return -0.5f * Mathf.Cos(num * Mathf.PI / period) + 0.5f;
        }

        /// <summary>
        /// A special function that's meant to take an input that changes as time passes.
        /// </summary>
        /// <remarks>
        /// The method outputs 0 at even multiples of <paramref name="period"/> (0 * <paramref name="period"/>, 2 * <paramref name="period"/>, 4 * <paramref name="period"/>, etc.).<br/>
        /// The method outputs <paramref name="maxNum"/> at odd multiples of <paramref name="period"/> (1 * <paramref name="period"/>, 3 * <paramref name="period"/>, 5 * <paramref name="period"/>, etc.).<br/>
        /// The values in-between follow a smooth wave shape.
        /// </remarks>
        /// <param name="num">The input of the method, which should be a value that changes somehow as time passes</param>
        /// <param name="period">The period of the cosine wave. Should be a constant value.</param>
        /// <param name="maxNum">The maximum number that the wave function reaches. Cannot equal 0, as that would make no sense.</param>
        /// <returns>
        /// A value ranging from 0 to <paramref name="maxNum"/>, based on the conditions mentioned prior.
        /// </returns>
        public static float WaveClamp0x(float num, float period, float maxNum)
        {
            if (maxNum == 0)
                ErrorNotify("WaveClamp0x(): maxNum cannot equal 0");
            float trueMaxNum = Mathf.Abs(maxNum);
            return trueMaxNum * (-0.5f * Mathf.Cos(num * Mathf.PI / period) + 0.5f);
        }

        /// <summary>
        /// A special function that's meant to take an input that changes as time passes.
        /// </summary>
        /// <remarks>
        /// The method outputs <paramref name="minNum"/> at even multiples of <paramref name="period"/> (0 * <paramref name="period"/>, 2 * <paramref name="period"/>, 4 * <paramref name="period"/>, etc.).<br/>
        /// The method outputs <paramref name="maxNum"/> at odd multiples of <paramref name="period"/> (1 * <paramref name="period"/>, 3 * <paramref name="period"/>, 5 * <paramref name="period"/>, etc.).<br/>
        /// The values in-between follow a smooth wave shape.
        /// </remarks>
        /// <param name="num">The input of the method, which should be a value that changes somehow as time passes</param>
        /// <param name="period">The period of the cosine wave. Should be a constant value.</param>
        /// <param name="minNum">The minimum number that the wave function reaches. Cannot equal <paramref name="maxNum"/>, as that would make no sense.</param>
        /// <param name="maxNum">The maximum number that the wave function reaches. Cannot equal <paramref name="minNum"/>, as that would make no sense.</param>
        /// <returns>
        /// A value ranging from <paramref name="minNum"/> to <paramref name="maxNum"/>, based on the conditions mentioned prior.
        /// </returns>
        public static float WaveClamp(float num, float period, float minNum, float maxNum)
        {
            if (maxNum == minNum)
                ErrorNotify("WaveClamp(): maxNum and minNum cannot be equal");
            return (maxNum - minNum) * (-0.5f * Mathf.Cos(num * Mathf.PI / period) + 0.5f) + minNum;
        }

        /// <summary>
        /// Normalizes a <see cref="byte"/> to a <see cref="float"/> between 0f and 1f.
        /// </summary>
        public static float ToFloat(this byte value)
        {
            float newValue = value;
            float returnValue = newValue / 255f;
            return Mathf.Clamp01(returnValue);
        }

        /// <summary>
        /// Scales a <see cref="float"/> between 0f and 1f to a <see cref="byte"/>.
        /// </summary>
        /// <remarks>
        /// Values less than 0f or greater than 1f will be turned into 0 or 255 respectively.
        /// </remarks>
        public static byte ToByte(this float value)
        {
            float newValue = Mathf.Clamp01(value) * 255f;
            return (byte)newValue;
        }

        /// <summary>
        /// Returns the reciprocal of <paramref name="num"/>, which simply means 1 / <paramref name="num"/>
        /// </summary>
        /// <remarks>
        /// Inputting 0 will return 0, and inputting 1 will return 1 by default.
        /// </remarks>
        public static float Inv(this float num)
        {
            if (num == 0f) 
                return 0f;
            if (num == 1f)
                return 1f;
            return 1f / num;
        }

        public static Vector2 GetAbs(this Vector2 originalVector)
        {
            return new Vector2(Mathf.Abs(originalVector.x), Mathf.Abs(originalVector.y));
        }

        public static Vector3 GetAbs(this Vector3 originalVector)
        {
            return new Vector3(Mathf.Abs(originalVector.x), Mathf.Abs(originalVector.y), Mathf.Abs(originalVector.z));
        }

        public static float Sum(params float[] values)
        {
            float sum = 0f;
            foreach (float value in values)
            {
                sum += value;
            }
            return sum;
        }

        public static int Sum(params int[] values)
        {
            int sum = 0;
            foreach (int value in values)
            {
                sum += value;
            }
            return sum;
        }

        public static float Round(float value, int decimalPlaces)
        {
            return Mathf.Round(value * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);
        }

        /// <summary>
        /// Plays an <see cref="AudioClip"/> and sends a <see cref="ModAPI.Notify(object)"/> at the same time.
        /// </summary>
        public static void Talk(this PhysicalBehaviour phys, object message, AudioClip clip)
        {
            phys.PlayClipOnce(clip);
            ModAPI.Notify(message);
        }

        /// <summary>
        /// Add a new audio source to a selected object that will work correctly with time scaling
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns>
        /// The <see cref="AudioSource"/> component.
        /// </returns>
        public static AudioSource AddAudioSource(this GameObject gameObject)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            Global.main.AddAudioSource(audioSource, false);
            return audioSource;
        }

        /// <summary>
        /// Copy a selected audio source to a selected object that will work correctly with time scaling
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="originalAudioSource"></param>
        /// <returns>
        /// The <see cref="AudioSource"/> component.
        /// </returns>
        public static AudioSource AddAudioSource(this GameObject gameObject, AudioSource originalAudioSource)
        {
            AudioSource audioSource = gameObject.AddAudioSource();
            audioSource.clip = originalAudioSource.clip;
            audioSource.volume = originalAudioSource.volume;
            audioSource.pitch = originalAudioSource.pitch;
            audioSource.loop = originalAudioSource.loop;

            return audioSource;
        }

        public static T[] GetAllComponents<T>(this GameObject gameObject, bool includesInactive = false, bool getallParent = true, bool getAllChildren = true)
        {
            List<T> allComponents = new List<T>();
            if (getallParent)
                allComponents.AddRange(gameObject.GetComponentsInParent<T>(includesInactive));
            if (getAllChildren)
                allComponents.AddRange(gameObject.GetComponentsInChildren<T>(includesInactive));
            allComponents.AddRange(gameObject.GetComponents<T>());

            return allComponents.ToArray();
        }

        public static void Move<T>(this List<T> list, T item, int newIndex)
        {
            if (newIndex < 0)
                ErrorNotify("Move(): newIndex cannot be below 0!");
            if (!list.Contains(item))
                ErrorNotify("Move(): The list does not contain the item!");
            list.Remove(item);
            list.Insert(newIndex, item);
        }

        /// <remarks>
        /// <paramref name="afterSpawn"/> only plays once, not every time. Use <b>SpawnableAssetPlus</b> if you're looking for that kind of complexity
        /// </remarks>
        public static GameObject CreatePhysicalObject(string name, Sprite sprite, Action<GameObject> afterSpawn)
        {
            GameObject instance = ModAPI.CreatePhysicalObject(name, sprite);
            afterSpawn(instance);
            return instance;
        }

        public static void ErrorNotify(object message)
        {
            if(!UserPreferenceManager.Current.LogDebugMessages)
                ModAPI.Notify(message);
            throw new Exception($"{message}");
        }
    }
}


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
    //Special thanks to pjstatt12 for creating AddLiquidToItem and the original LiquidReaction and TripleLiquidReaction!
    public static class ChemistryPlus
    {
        public static Dictionary<string, Liquid> LiquidRegistry
        {
            get
            {
                return (Dictionary<string, Liquid>)typeof(Liquid).GetField("Registry", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
            }
        }
        public static Dictionary<Liquid, string> LiquidByDisplayNameRegistry
        {
            get
            {
                return LiquidRegistry.ToDictionary(entry => entry.Value, entry => entry.Value.GetDisplayName());
            }
        }


        /// <summary>
        /// This method adds a FlaskBehaviour to an item and a given amount of a liquid
        /// </summary>
        /// <param name="item">The SpawnableAsset (i.e. item) to add the liquid to</param>
        /// <param name="newLiquidID">The ID of the liquid</param>
        /// <param name="amount">The amount of liquid to add. The capacity of the container will be automatically set to the amount</param>
        public static void AddLiquidToItem(this SpawnableAsset item, string newLiquidID, float amount)
        {
            FlaskBehaviour flask = item.Prefab.AddComponent<FlaskBehaviour>();
            flask.Capacity = amount;
            flask.StartLiquid = new BloodContainer.SerialisableDistribution
            {
                LiquidID = newLiquidID,
                Amount = amount
            };           
        }

        /// <summary>
        /// This method adds a FlaskBehaviour to an item and a given amount of a liquid
        /// </summary>
        /// <remarks>
        /// Should be called in OnLoad() for most optimal gameplay.
        /// </remarks>
        /// <param name="item">The SpawnableAsset (i.e. item) to add the liquid to.</param>
        /// <param name="newLiquidID">The ID of the liquid.</param>
        /// <param name="amount">The amount of liquid to add.</param>
        /// <param name="capacity">The desired capacity of the container. Cannot be larger than the amount.</param>
        /// <exception cref="ArgumentOutOfRangeException">The set <paramref name="amount"/> is larger than the set <paramref name="capacity"/> allows.</exception>
        public static void AddLiquidToItem(this SpawnableAsset item, string newLiquidID, float amount, float capacity)
        {
            if (amount > capacity)
                IntAPI.ErrorNotify("AddLiquidToItem(): Amount cannot exceed capacity!");
            FlaskBehaviour flask = item.Prefab.AddComponent<FlaskBehaviour>();
            flask.Capacity = amount;
            flask.StartLiquid = new BloodContainer.SerialisableDistribution
            {
                LiquidID = newLiquidID,
                Amount = amount
            };
        }

        /// <summary>
        /// This method adds a new Liquid mix reaction to the game.
        /// </summary>
        /// <remarks>
        /// Must be called in Main() to work properly.
        /// </remarks>
        /// <param name="input1">The first liquid of the reaction.</param>
        /// <param name="input2">The second liquid of the reaction.</param>
        /// <param name="target">The target liquid of the reaction.</param>
        /// <param name="ratePerSecond">The rate at whih the reaction should occur. The larger it is, the faster it will occur. Should not be set below 0.02f.</param>
        public static void LiquidReaction(Liquid input1, Liquid input2, Liquid target, float ratePerSecond = 0.05f)
        {
            if (ratePerSecond < 0.02f)
            {
                Debug.LogWarning("LiquidReaction: ratePerSecond set to value below 0.02f. Resetting to default value.");
                ratePerSecond = default;
            }
            LiquidMixInstructions mixer = new LiquidMixInstructions(
                input1,
                input2,
                target,
                ratePerSecond);

            LiquidMixingController.MixInstructions.Add(mixer);
        }
        /// <summary>
        /// This method adds a new Liquid mix reaction to the game.
        /// </summary>
        /// <remarks>
        /// Must be called in Main() to work properly.
        /// </remarks>
        /// <param name="input1">The first liquid id of the reaction.</param>
        /// <param name="input2">The second liquid id of the reaction.</param>
        /// <param name="target">The target liquid id of the reaction.</param>
        /// <param name="ratePerSecond">The rate at whih the reaction should occur. The larger it is, the faster it will occur. Should not be set below 0.02f.</param>
        public static void LiquidReaction(string input1, string input2, string target, float ratePerSecond = 0.05f)
        {
            LiquidReaction(Liquid.GetLiquid(input1), Liquid.GetLiquid(input2), Liquid.GetLiquid(target), ratePerSecond);
        }

        /// <summary>
        /// This method adds a new Liquid mix reaction to the game.
        /// </summary>
        /// <remarks>
        /// Must be called in Main() to work properly.
        /// </remarks>
        /// <param name="input1">The first liquid of the reaction.</param>
        /// <param name="input2">The second liquid of the reaction.</param>
        /// <param name="input3">The third liquid of the reaction.</param>
        /// <param name="target">The target liquid of the reaction.</param>
        /// <param name="ratePerSecond">The rate at whih the reaction should occur. The larger it is, the faster it will occur. Should not be set below 0.02f.</param>
        public static void LiquidReaction(Liquid input1, Liquid input2, Liquid input3, Liquid target, float ratePerSecond = 0.05f)
        {
            if (ratePerSecond < 0.02f)
            {
                Debug.LogWarning("LiquidReaction: ratePerSecond set to value below 0.02f. Resetting to default value.");
                ratePerSecond = default;
            }
            LiquidMixInstructions mixer = new LiquidMixInstructions(
                input1,
                input2,
                input3,
                target,
                ratePerSecond);

            LiquidMixingController.MixInstructions.Add(mixer);
        }
        /// <summary>
        /// This method adds a new Liquid mix reaction to the game.
        /// </summary>
        /// <remarks>
        /// Must be called in Main() to work properly.
        /// </remarks>
        /// <param name="inputs">The array of input liquids for the reaction.</param>
        /// <param name="target">The target liquid of the reaction.</param>
        /// <param name="ratePerSecond">The rate at whih the reaction should occur. The larger it is, the faster it will occur. Should not be set below 0.02f.</param>
        public static void LiquidReaction(Liquid[] inputs, Liquid target, float ratePerSecond = 0.05f)
        {
            if (ratePerSecond < 0.02f)
            {
                Debug.LogWarning("LiquidReaction: ratePerSecond set to value below 0.02f. Resetting to default value.");
                ratePerSecond = default;
            }
            LiquidMixInstructions mixer = new LiquidMixInstructions(inputs, target, ratePerSecond);
            LiquidMixingController.MixInstructions.Add(mixer);
        }
        /// <summary>
        /// This method adds a new Liquid mix reaction to the game.
        /// </summary>
        /// <remarks>
        /// Must be called in Main() to work properly.
        /// </remarks>
        /// <param name="inputs">The array of input liquid ids for the reaction.</param>
        /// <param name="target">The target liquid of the reaction.</param>
        /// <param name="ratePerSecond">The rate at whih the reaction should occur. The larger it is, the faster it will occur. Should not be set below 0.02f.</param>
        public static void LiquidReaction(string[] inputs, string target, float ratePerSecond = 0.05f)
        {
            LiquidReaction(inputs.Select(liquid => Liquid.GetLiquid(liquid)).ToArray(), Liquid.GetLiquid(target), ratePerSecond);
        }

        /// <summary>
        /// A special method that creates a Liquid mix process that takes multiple inputs and produces multiple outputs.
        /// </summary>
        /// <remarks>
        /// It's possible that through the usage of this method, your inputed <paramref name="ratePerSecond"/> would be set below 0.02f due to division magic. The method will automatically set the <paramref name="ratePerSecond"/> to the lowest allowed value automatically and throw a <see cref="Debug.LogWarning(object))"/> containing the value the method set the <paramref name="ratePerSecond"/> to.<br/>
        /// Must be called in Main() to work properly.
        /// </remarks>
        /// <param name="inputs">The array of input liquids for the reaction.</param>
        /// <param name="targets">The array of output liquids for the reaction. They are all outputted in a 1 : 1 : ... ratio.</param>
        /// <param name="ratePerSecond">The rate at whih the reaction should occur. The larger it is, the faster it will occur. Should not be set below 0.02f.</param>
        public static void LiquidReaction(Liquid[] inputs, Liquid[] targets, float ratePerSecond = 0.05f)
        {
            float minRate = 0.02f * targets.Length;
            if (ratePerSecond < minRate)
            {
                Debug.LogWarning($"LiquidReaction: ratePerSecond is below the minimum value allowed. Setting the value to {minRate}f");
            }
            ratePerSecond = Mathf.Max(minRate, ratePerSecond);
            foreach (Liquid target in targets)
            {
                LiquidMixInstructions mixer = new LiquidMixInstructions(inputs, target, ratePerSecond / targets.Length);
                LiquidMixingController.MixInstructions.Add(mixer);
            }
        }

        /// <summary>
        /// A special method that creates a Liquid mix process that takes multiple inputs and produces multiple outputs with given ratios.
        /// </summary>
        /// <remarks>
        /// It's possible that through the usage of this method, your inputed <paramref name="ratePerSecond"/> would be set below 0.02f due to division magic. The method will automatically set the <paramref name="ratePerSecond"/> to the lowest allowed value automatically and throw a <see cref="Debug.LogWarning(object))"/> containing the value the method set the <paramref name="ratePerSecond"/> to.<br/>
        /// Must be called in Main() to work properly.
        /// </remarks>
        /// <param name="inputs">The array of input liquids for the reaction. The order of inputs matches the order of <paramref name="ratios"/>, and in case it doesn't the method throws an error.</param>
        /// <param name="targets">The array of output liquids for the reaction. They are all outputted in a 1 : 1 : ... ratio.</param>
        /// <param name="ratios">The array of ratio integers. The order of ratios matches the order of <paramref name="inputs"/>, and in case it doesn't the method throws an error. "Ratio" means that for every step of the reaction, x parts of liquid one, y parts of liquid two, z parts of liquid three, etc. Will be turned into output liquids.</param>
        /// <param name="ratePerSecond">The rate at whih the reaction should occur. The larger it is, the faster it will occur. Should not be set below 0.02f.</param>
        /// <exception cref="ArgumentOutOfRangeException">The set <paramref name="inputs"/> array is not of the same length as the <paramref name="ratios"/> array, or any element of the <paramref name="ratios"/> array is below 1.</exception>
        public static void LiquidReaction(Liquid[] inputs, Liquid[] targets, int[] ratios, float ratePerSecond = 0.05f)
        {
            if (targets.Length != ratios.Length)
            {
                string errorType = targets.Length < ratios.Length ? "targets" : "ratios";
                IntAPI.ErrorNotify($"LiquidReaction(): Not enough {errorType} elements!");
            }
            foreach (int num in ratios)
            {
                if (num < 1)
                    IntAPI.ErrorNotify("LiquidReaction(): No ratio can be 0 or less!");
            }
            int divisor = IntAPI.Sum(ratios);
            float minRate = 0.02f * divisor / Mathf.Min(ratios);
            if (ratePerSecond < minRate)
            {
                Debug.LogWarning($"LiquidReaction(): ratePerSecond is below the minimum value allowed. Setting the value to {minRate}f");
            }
            ratePerSecond = Mathf.Max(minRate, ratePerSecond);
            for (int i = 0; i < targets.Length; i++)
            {
                float multiplier = (float)ratios[i] / divisor;
                LiquidMixInstructions mixer = new LiquidMixInstructions(inputs, targets[i], ratePerSecond * multiplier);
                LiquidMixingController.MixInstructions.Add(mixer);
            }
        }

        /// <summary>
        /// This method adds a bottle opening to a BloodContainer (any liquid container). To input the liquid from such container to another liquid, hold the activation key with the set opening near another liquid container.
        /// </summary>
        /// <param name="container">The BloodContainer script in the selected item</param>
        /// <param name="position">The position at which the opening should be, from the object's visual center. It is recommended to set it 1-2 pixels more than the sprite would suggest.</param>
        /// <param name="outerSpace">Sets in which kind of <see cref="Space"/> the opening is in. Idk what it means, but don't change it from the default <see cref="Space.Self"/></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static PointLiquidTransferBehaviour AddBottleOpening(this BloodContainer container, Vector2 position, Space outerSpace = Space.Self)
        {
            if (container == null)
                IntAPI.ErrorNotify("AddBottleOpening(): The object does not contain a BloodContainer class.");
            PointLiquidTransferBehaviour cupOpening = container.gameObject.AddComponent<PointLiquidTransferBehaviour>();
            cupOpening.Point = position;
            cupOpening.Space = outerSpace;
            cupOpening.Layers = ModAPI.FindSpawnable("Bottle").Prefab.GetComponent<PointLiquidTransferBehaviour>().Layers;
            return cupOpening;
        }
    }
}


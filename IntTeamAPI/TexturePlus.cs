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
    public static class TexturePlus
    {
        /// <summary>
        /// Creates a light sprite.
        /// </summary>
        /// <param name="lightObject">The object that contains the light sprite.</param>
        /// <param name="parentObject">The parent of <paramref name="lightObject"/>.</param>
        /// <param name="sprite">The sprite for the light sprite. Instead of coloring this sprite, it should be only made with shades of white and then colored with the <paramref name="color"/> parameter for better flexibility.</param>
        /// <param name="position">The <b>local</b> position of the sprite, from the center of <paramref name="lightObject"/>.</param>
        /// <param name="color">The color of the light sprite.</param>
        /// <param name="activate">Optional parameter that can be useful in some cases. Enables or disables the <paramref name="lightObject"/> upon creation.</param>
        public static void CreateLightSprite(out GameObject lightObject, Transform parentObject, Sprite sprite, Vector2 position, Color color, bool activate = true)
        {
            lightObject = new GameObject("Light");
            lightObject.transform.SetParent(parentObject);
            lightObject.transform.rotation = parentObject.rotation;
            lightObject.transform.localPosition = position;
            lightObject.transform.localScale = Vector2.one;

            var lightSprite = lightObject.AddComponent<SpriteRenderer>();
            lightSprite.sprite = sprite; //This sprite should be a shade of white, or else the re-coloring will act weird
            lightSprite.material = ModAPI.FindMaterial("VeryBright");

            lightSprite.color = color;
            lightSprite.sortingLayerName = parentObject.GetComponent<SpriteRenderer>().sortingLayerName;
            lightSprite.sortingOrder = parentObject.GetComponent<SpriteRenderer>().sortingOrder + 1;

            lightObject.SetActive(activate);
        }

        /// <summary>
        /// Creates a glowing light sprite.
        /// </summary>
        /// <param name="lightObject">The object that contains the light sprite.</param>
        /// <param name="parentObject">The parent of <paramref name="lightObject"/>.</param>
        /// <param name="sprite">The sprite for the light sprite. Instead of coloring this sprite, it should be only made with shades of white and then colored with the <paramref name="color"/> parameter for better flexibility.</param>
        /// <param name="position">The <b>local</b> position of the sprite, from the center of <paramref name="lightObject"/>.</param>
        /// <param name="color">The color of the light sprite.</param>
        /// <param name="glow">The glowing sphere around the light sprite. Its color is set to be the same as the <paramref name="color"/> parameter, except that its alpha is set to maximum value.</param>
        /// <param name="radius">The radius of the <paramref name="glow"/>.</param>
        /// <param name="brightness">The brightnes of the <paramref name="glow"/>.</param>
        /// <param name="activate">Optional parameter that can be useful in some cases. Enables or disables the <paramref name="lightObject"/> upon creation.</param>
        public static void CreateLightSprite(out GameObject lightObject, Transform parentObject, Sprite sprite, Vector2 position, Color color, out LightSprite glow, float radius = 5f, float brightness = 1.5f, bool activate = true)
        {
            lightObject = new GameObject("Light");
            lightObject.transform.SetParent(parentObject);
            lightObject.transform.rotation = parentObject.rotation;
            lightObject.transform.localPosition = position;
            lightObject.transform.localScale = Vector2.one;

            var lightSprite = lightObject.AddComponent<SpriteRenderer>();
            lightSprite.sprite = sprite; //This sprite should be a shade of white, or else the re-coloring will act weird
            lightSprite.material = ModAPI.FindMaterial("VeryBright");

            lightSprite.color = color;
            lightSprite.sortingLayerName = parentObject.GetComponent<SpriteRenderer>().sortingLayerName;
            lightSprite.sortingOrder = parentObject.GetComponent<SpriteRenderer>().sortingOrder + 1;

            glow = ModAPI.CreateLight(lightObject.transform, color.ChangeAlpha(1f), radius, brightness);
            glow.transform.localPosition = Vector3.zero;

            lightObject.SetActive(activate);
        }

        /// <summary>
        /// Change the color of the light sprite
        /// </summary>
        /// <param name="lightObject">The lightObject, as defined by the <see cref="CreateLightSprite"/> method.</param>
        /// <param name="glow">The glow component, as defined by the <see cref="CreateLightSprite"/> method.</param>
        /// <param name="newColor">The new color for the light sprite.</param>
        public static void ChangeLightColor(GameObject lightObject, LightSprite glow, Color newColor)
        {
            lightObject.GetComponent<SpriteRenderer>().color = newColor;
            glow.Color = newColor.ChangeAlpha(1f);
        }

        /// <summary>
        /// Change the color of the light sprite
        /// </summary>
        /// <param name="lightObject">The lightObject, as defined by the <see cref="CreateLightSprite"/> method.</param>
        /// <param name="newColor">The new color for the light sprite.</param>
        public static void ChangeLightColor(GameObject lightObject, Color newColor)
        {
            lightObject.GetComponent<SpriteRenderer>().color = newColor;
        }

        /// <summary>
        /// Replaces an item sprite.
        /// </summary>
        public static void ReplaceItemSprite(this SpawnableAsset item, Sprite replaceTexture)
        {
            item.Prefab.GetComponent<SpriteRenderer>().sprite = replaceTexture;
        }

        /// <summary>
        /// Replaces a sprite for the child of an item.
        /// </summary>
        /// <remarks>
        /// Here is an example:
        /// <code>
        /// ModAPI.FindSpawnable("Axe").ReplaceItemSpriteOfChild("Axe handle/Axe head", ModAPI.LoadSprite("Futuristic Axe Head.png"));
        /// </code>
        /// All child paths for base game items can be found on the <a href="https://www.studiominus.nl/ppg-modding/gameAssets.html">official PPG modding wiki</a>.
        /// </remarks>
        /// <param name="item">The <see cref="SpawnableAsset"/> for which to change the texture.</param>
        /// <param name="childObject">The child path (see example in the method summary).</param>
        public static void ReplaceItemSprite(this SpawnableAsset item, string childObject, Sprite childReplaceTexture)
        {
            item.Prefab.transform.Find(childObject).GetComponent<SpriteRenderer>().sprite = childReplaceTexture;
        }

        /// <summary>
        /// Replaces the sprites for the root object and a child of an item
        /// </summary>
        /// <remarks>
        /// All child paths for base game items can be found on the <a href="https://www.studiominus.nl/ppg-modding/gameAssets.html">official PPG modding wiki</a>.
        /// </remarks>
        /// <param name="item">The <see cref="SpawnableAsset"/> for which to change the texture.</param>
        /// <param name="childObject">The child path (see method summary)</param>
        public static void ReplaceItemSprite(this SpawnableAsset item, Sprite replaceTexture, string childObject, Sprite childReplaceTexture)
        {
            item.ReplaceItemSprite(replaceTexture);
            item.ReplaceItemSprite(childObject, childReplaceTexture);
        }

        /// <summary>
        /// Replaces the sprites for an unlimited number of children of an item
        /// </summary>
        /// <remarks>
        /// All child paths for base game items can be found on the <a href="https://www.studiominus.nl/ppg-modding/gameAssets.html">official PPG modding wiki</a>.
        /// </remarks>
        /// <param name="item">The <see cref="SpawnableAsset"/> for which to change the texture.</param>
        /// <param name="childObjects">The array that should contain all child object paths (see method summary).</param>
        /// <param name="childReplaceSprites">The array of sprites to replace for the children. The length of both arrays must be equal or else the method will throw an exception.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ReplaceItemSprite(this SpawnableAsset item, string[] childObjects, Sprite[] childReplaceSprites)
        {
            if (childObjects.Length != childReplaceSprites.Length)
                IntAPI.ErrorNotify("ReplaceItemSprite(): Amount of child objects does not match replace textures");
            for (int i = 0; i < childObjects.Length; i++)
            {
                item.ReplaceItemSprite(childObjects[i], childReplaceSprites[i]);
            }
        }

        /// <summary>
        /// Replaces the sprites for the root object an an unlimited number of children of an item
        /// </summary>
        /// <remarks>
        /// All child paths for base game items can be found on the <a href="https://www.studiominus.nl/ppg-modding/gameAssets.html">official PPG modding wiki</a>.
        /// </remarks>
        /// <param name="item">The <see cref="SpawnableAsset"/> for which to change the texture.</param>
        /// <param name="replaceSprite">The sprite to replace for the root object.</param>
        /// <param name="childObjects">The array that should contain all child object paths (see method summary).</param>
        /// <param name="childReplaceSprites">The array of sprites to replace for the children. The length of both arrays must be equal or else the method will throw an exception.</param>
        /// <exception cref="ArgumentException"></exception>
        public static void ReplaceItemSprite(this SpawnableAsset item, Sprite replaceSprite, string[] childObjects, Sprite[] childReplaceSprites)
        {
            if (childObjects.Length != childReplaceSprites.Length)
                IntAPI.ErrorNotify("ReplaceItemSprite(): Amount of child objects does not match replace textures");
            ReplaceItemSprite(item, replaceSprite);
            for (int i = 0; i < childObjects.Length; i++)
            {
                item.ReplaceItemSprite(childObjects[i], childReplaceSprites[i]);
            }
        }

        /// <summary>
        /// Replaces the view (the catalog button) sprite of an item.
        /// </summary>
        public static void ReplaceViewSprite(this SpawnableAsset item, Sprite replaceSprite)
        {
            item.ViewSprite = replaceSprite;
        }

        /// <summary>
        /// An imrpoved version of the base game's<see cref="PersonBehaviour.SetBodyTextures(Texture2D, Texture2D, Texture2D, float)"/> method that takes arrays instead of individual entries.
        /// </summary>
        /// <remarks>
        /// With the help of this method, you can store all your entity sprites in one field, as shown here:
        /// <code>
        /// Texture2D[] entityTextures = new Texture2D[6]
        /// {
        ///     ModAPI.LoadTexture("sprites/my human skin.png"),
        ///     null,
        ///     null,
        ///     ModAPI.LoadTexture("sprites/my other human skin.png"),
        ///     ModAPI.LoadTexture("sprites/my other human flesh.png"),
        ///     null
        /// }
        /// </code>
        /// </remarks>
        /// <param name="person">The <see cref="PersonBehaviour"/> of the entity.</param>
        /// <param name="textures">The <see cref="Texture2D"/> array. The array must contain at least 3 textures, and must contain number of textures that is a multiple of three. You can offset empty entries withh <see cref="null"/> entries, as shown in the method summary.</param>
        /// <param name="scale">The scale of the texture. If set to 2f, this means that the tezture is <i>(should be)</i> twice the size of the default texture.</param>
        /// <param name="offset">The offset within the array. When you have entries 0-5 (6 in total), setting the offset to 1 will set the entity to <paramref name="textures"/>[3], <paramref name="textures"/>[4] and <paramref name="textures"/>[5]</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void SetBodyTextures(this PersonBehaviour person, Texture2D[] textures, float scale = 1f, int offset = 0)
        {
            if (textures.Length < 3)
                IntAPI.ErrorNotify("SetBodyTexturesArray(): Too few body textures in array!");
            if (offset < 0)
                IntAPI.ErrorNotify("SetBodyTexturesArray(): Offset cannot be less than 0!");
            if (textures.Length % 3 != 0)
                IntAPI.ErrorNotify("SetBodyTexturesArray(): Amount of textures in array must be a multiple of 3! (Offset empty slots with null elements?)");
            offset *= 3;
            person.SetBodyTextures(textures[0 + offset], textures[1 + offset], textures[2 + offset], scale);
        }

        /// <summary>
        /// Sets the health bar colors of every limb within an entity (see <see cref="SetHealthBarColor(LimbBehaviour, Color)"/> for details.
        /// </summary>
        public static void SetHealthBarColors(this PersonBehaviour person, Color color)
        {
            foreach (LimbBehaviour limbs in person.Limbs)
            {
                limbs.SetHealthBarColor(color);
            }
        }

        /// <summary>
        /// Resets the health bar color for every limb within an entity (see <see cref="ResetHealthBarColor(LimbBehaviour)"/> for details.
        /// </summary>
        public static void ResetHealthBarColors(this PersonBehaviour person)
        {
            person.SetHealthBarColors(new Color32(55, 255, 0, 255));          
        }

        /// <summary>
        /// Sets the health bar color for a limb.
        /// </summary>
        /// <param name="limb">The limb for which to change the health bar color.</param>
        /// <param name="color">The color to which the health bar should be changed</param>
        public static void SetHealthBarColor(this LimbBehaviour limb, Color color)
        {
            GameObject myStatus = (GameObject)typeof(LimbBehaviour).GetField("myStatus", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(limb);
            myStatus.transform.Find("bar").GetComponent<SpriteRenderer>().color = color;
        }

        /// <summary>
        /// Resets the color of the limb's health bar to default state, that being <b>#37FF00</b>.
        /// </summary>
        public static void ResetHealthBarColor(this LimbBehaviour limb)
        {
            limb.SetHealthBarColor(new Color32(55, 255, 0, 255));
        }

        public static void ScaleHealthBars(this PersonBehaviour person, float scale)
        {
            foreach (LimbBehaviour limb in person.Limbs)
            {
                limb.ScaleHealthBar(scale);
            }
        }

        public static void ScaleHealthBar(this LimbBehaviour limb, float scale)
        {
            GameObject myStatus = (GameObject)typeof(LimbBehaviour).GetField("myStatus", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(limb);
            myStatus.transform.localScale *= scale;
        }

        /// <summary>
        /// Do not use this method, it probably won't work unless you know what you are doing.
        /// </summary>     
        internal static void SetLimbSprite(this LimbBehaviour limb, Texture2D skin, Texture2D flesh = null, Texture2D bone = null, float scale = 1f)
        {
            SpriteRenderer renderer = limb.SkinMaterialHandler.renderer;
            LimbSpriteCache.LimbSprites sprites = LimbSpriteCache.Instance.LoadFor(renderer.sprite, skin, flesh, bone, scale);
            renderer.sprite = sprites.Skin;
            if ((bool)flesh)
                renderer.material.SetTexture(ShaderProperties.Get("_FleshTex"), flesh);
            if ((bool)bone)
                renderer.material.SetTexture(ShaderProperties.Get("_BoneTex"), bone);
            if (limb.TryGetComponent(out ShatteredObjectSpriteInitialiser component))
                component.UpdateSprites(in sprites);
        }

        /// <summary>
        /// This method allows you to set a directional body textures. Each <see cref="Texture2D"/>[] array must store either 1 or 2 elements.
        /// <param name="person"></param>
        /// <param name="directionType">
        /// Defines how back limbs are defined.<br/>
        /// <see cref="Space.Self"/>: When spawned to the right, the back arm and leg will use 2nd texture parameter, everything else will use the 1st texture parameter. Applies in reverse if spawned to the left.
        /// <see cref="Space.World"/>: The back arm and leg will use 2nd texture parameter, everything else will use the 1st texture parameter. Will apply in either direction
        /// </param>
        /// <param name="skins">Cannot equal null, nor can any values equal null.</param>
        /// <param name="fleshs">Values can equal null. setting the entire parameter to null is the same as adding a parameter with 2 null values.</param>
        /// <param name="bones">Values can equal null. setting the entire parameter to null is the same as adding a parameter with 2 null values.</param>
        public static void SetDirectionalBodyTextures(this PersonBehaviour person, DirectionTypes directionType, Texture2D[] skins, Texture2D[] fleshs = null, Texture2D[] bones = null, float scale = 1f)
        {
            if (fleshs == null)
                fleshs = new Texture2D[2] { null, null };
            if (bones == null)
                bones = new Texture2D[2] { null, null };

            if (skins.Length == 0 || fleshs.Length == 0 || bones.Length == 0 || skins.Length > 2 || fleshs.Length > 2 || bones.Length > 2 || skins == null || skins.Any(item => item == null))
                IntAPI.ErrorNotify("TexturePlus SetDirectionalBodyTextures(): Improper inputs detected!");   
            if (person.Limbs[0].SpeciesIdentity == Species.Gorse)
                IntAPI.ErrorNotify("TexturePlus SetDirectionalBodyTextures(): This method is incompatible with a gorse!");
        
            string[] backLimbNames = { "BackLeg", "BackArm" };
            string[] frontLimbNames = { "FrontLeg", "FrontArm" };

            foreach (LimbBehaviour limb in person.Limbs)
            {            
                switch(directionType) 
                {
                    case DirectionTypes.TwoHalves:
                        if (limb.transform.parent != null && backLimbNames.Contains(limb.transform.parent.name))
                        {
                            bool flipped = limb.transform.lossyScale.x > 0f;
                            limb.SetLimbSprite(skins[flipped && skins.Length > 1 ? 1 : 0], fleshs[flipped && fleshs.Length > 1 ? 1 : 0], bones[flipped && bones.Length > 1 ? 1 : 0], scale);
                        }
                        else
                        {
                            bool flipped = limb.transform.lossyScale.x < 0f;
                            limb.SetLimbSprite(skins[flipped && skins.Length > 1 ? 1 : 0], fleshs[flipped && fleshs.Length > 1 ? 1 : 0], bones[flipped && bones.Length > 1 ? 1 : 0], scale);
                        }
                        break;

                    case DirectionTypes.BackLimbs:
                        if (limb.transform.parent != null && backLimbNames.Contains(limb.transform.parent.name))
                        {
                            limb.SetLimbSprite(skins[skins.Length > 1 ? 1 : 0], fleshs[fleshs.Length > 1 ? 1 : 0], bones[bones.Length > 1 ? 1 : 0], scale);
                        }
                        else
                        {
                            limb.SetLimbSprite(skins[0], fleshs[0], bones[0], scale);
                        }
                        break;

                    case DirectionTypes.FrontLimbs:
                        if (limb.transform.parent != null && frontLimbNames.Contains(limb.transform.parent.name))
                        {
                            limb.SetLimbSprite(skins[skins.Length > 1 ? 1 : 0], fleshs[fleshs.Length > 1 ? 1 : 0], bones[bones.Length > 1 ? 1 : 0], scale);
                        }
                        else
                        {
                            limb.SetLimbSprite(skins[0], fleshs[0], bones[0], scale);
                        }
                        break;
                }
            }
        }
    }

    public enum DirectionTypes
    {
        TwoHalves,
        BackLimbs,
        FrontLimbs
    }
}


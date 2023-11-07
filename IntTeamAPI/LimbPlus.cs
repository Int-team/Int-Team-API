using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChemistryPlusMod;

//This API is released under the zlib license, by using it for your mod and/or downloading it you confirm that you read and agreed to the terms of said license.
//Link to the original repository: https://github.com/Int-team/Int-Team-API
//API DEPENDENCIES: None

namespace IntTeamAPI
{
    public static class LimbPlus
    {
        public static void ReviveLimb(this LimbBehaviour myLimb)
        {
            if (myLimb.HasBrain)
            {
                myLimb.Person.SeizureTime = 0.0f;
                myLimb.Person.ShockLevel = 0.0f;
                myLimb.Person.PainLevel = 0.0f;
                myLimb.Person.OxygenLevel = 1f;
                myLimb.Person.AdrenalineLevel = 1f;
                myLimb.Person.Consciousness = 1f;
            }
            myLimb.ReanimateLimb();
        }

        public static void ReanimateLimb(this LimbBehaviour myLimb)
        {
            if (myLimb.HasBrain)
            {
                myLimb.Person.Braindead = false;
                myLimb.Person.BrainDamaged = false;
                myLimb.Person.BrainDamagedTime = 0.0f;
            }
            myLimb.LungsPunctured = false;
            myLimb.HealBone();
            myLimb.Health = myLimb.InitialHealth;
            myLimb.Numbness = 0.0f;
            myLimb.CirculationBehaviour.HealBleeding();
            myLimb.CirculationBehaviour.IsPump = myLimb.CirculationBehaviour.WasInitiallyPumping;
            myLimb.CirculationBehaviour.BloodFlow = 1f;
            myLimb.CirculationBehaviour.AddLiquid(myLimb.GetOriginalBloodType(), Mathf.Max(0.0f, 1f - myLimb.CirculationBehaviour.GetAmount(myLimb.GetOriginalBloodType())));
            myLimb.CirculationBehaviour.IsPump = myLimb.CirculationBehaviour.WasInitiallyPumping;
        }

        public static void RegenerateLimb(this LimbBehaviour myLimb, float regenSpeed, float acidSpeed, float burnSpeed, float rottenSpeed, float woundsEfficiency, bool regenWhenDead = false)
        {
            if (myLimb.PhysicalBehaviour.isDisintegrated)
                return;
            if (myLimb.IsConsideredAlive || regenWhenDead)
            {
                if  (regenSpeed > 0f)
                    myLimb.Health += myLimb.InitialHealth * regenSpeed * Time.deltaTime;
                if (acidSpeed > 0f)
                    myLimb.HealAcid(acidSpeed);
                if (burnSpeed > 0f)
                    myLimb.HealBurn(burnSpeed);               
                if (rottenSpeed > 0f)
                    myLimb.HealRotten(rottenSpeed);
                if (woundsEfficiency > 0f)
                    myLimb.HealWounds(woundsEfficiency);
            }

        }

        public static void HealAcid(this LimbBehaviour myLimb, float speed = 1f)
        {
            if (myLimb.SkinMaterialHandler.AcidProgress > 1f)
                myLimb.SkinMaterialHandler.AcidProgress = 1f;
            else if (myLimb.SkinMaterialHandler.AcidProgress > 0f)
                myLimb.SkinMaterialHandler.AcidProgress -= Time.deltaTime * speed;
        }

        public static void HealBurn(this LimbBehaviour myLimb, float speed = 1f)
        {
            if (myLimb.PhysicalBehaviour.BurnProgress > 1f)
                myLimb.PhysicalBehaviour.BurnProgress = 1f;
            else if (myLimb.PhysicalBehaviour.BurnProgress > 0f)
                myLimb.PhysicalBehaviour.BurnProgress -= Time.deltaTime * speed;
        }

        public static void HealRotten(this LimbBehaviour myLimb, float speed = 1f)
        {
            if (myLimb.SkinMaterialHandler.RottenProgress > 1f)
                myLimb.SkinMaterialHandler.RottenProgress = 1f;
            else if (myLimb.SkinMaterialHandler.RottenProgress > 0f)
                myLimb.SkinMaterialHandler.RottenProgress -= Time.deltaTime * speed;
        }

        public static void HealWounds(this LimbBehaviour myLimb, float efficiency = 1f)
        {
            float trueEfficiency = Mathf.Clamp01(efficiency);
            if (UnityEngine.Random.value < trueEfficiency)
            {
                myLimb.CirculationBehaviour.HealBleeding();
            }
            myLimb.BruiseCount = 0;
            myLimb.CirculationBehaviour.GunshotWoundCount = 0;
            myLimb.CirculationBehaviour.StabWoundCount = 0;
            float factor = Mathf.Pow(1f - trueEfficiency, Time.fixedDeltaTime);
            for (int index = 0; index < myLimb.SkinMaterialHandler.damagePoints.Length; ++index)
                myLimb.SkinMaterialHandler.damagePoints[index].z *= factor;
            myLimb.SkinMaterialHandler.Sync();
        }

        static readonly Dictionary<LimbTypes, string> limbs = new Dictionary<LimbTypes, string>()
        {
            //Human & Android limbs
            { LimbTypes.Head, "Head" },
            { LimbTypes.UpperBody, "Body/UpperBody" },
            { LimbTypes.MiddleBody, "Body/MiddleBody" },
            { LimbTypes.LowerBody, "Body/LowerBody" },
            { LimbTypes.UpperArmFront, "FrontArm/UpperArmFront" },
            { LimbTypes.LowerArmFront, "FrontArm/LowerArmFront" },
            { LimbTypes.UpperArmBack, "BackArm/UpperArm" },
            { LimbTypes.LowerArmBack, "BackArm/LowerArm" },
            { LimbTypes.UpperLegFront, "FrontLeg/UpperLegFront" },
            { LimbTypes.LowerLegFront, "FrontLeg/LowerLegFront" },
            { LimbTypes.FootFront, "FrontLeg/FootFront" },
            { LimbTypes.UpperLegBack, "BackLeg/UpperLeg" },
            { LimbTypes.LowerLegBack, "BackLeg/LowerLeg" },
            { LimbTypes.FootBack, "BackLeg/Foot" },
            //Gorse limbs
            { LimbTypes.Body, "body" },
            { LimbTypes.RightUpperLegFront, "right upper leg" },
            { LimbTypes.RightBottomLegFront, "right bottom leg" },
            { LimbTypes.LeftUpperLegFront, "left upper leg" },
            { LimbTypes.LeftBottomLegFront, "left bottom leg" },
            { LimbTypes.RightUpperLegBack, "right upper leg background" },
            { LimbTypes.RightBottomLegBack, "right bottom leg background" },
            { LimbTypes.LeftUpperLegBack, "left upper leg background" },
            { LimbTypes.LeftBottomLegBack, "left bottom leg background" }
        };

        static bool IsGorse(LimbTypes limbType)
        {
            return (int)limbType > 13;
        }

        public static LimbBehaviour FindLimb(this CirculationBehaviour circ, LimbTypes limbType)
        {
            return circ.Limb.Person.FindLimbComp<LimbBehaviour>(limbType);
        }

        public static LimbBehaviour FindLimb(this LimbBehaviour limb, LimbTypes limbType)
        {
            return limb.Person.FindLimbComp<LimbBehaviour>(limbType);
        }

        public static LimbBehaviour FindLimb(this PersonBehaviour person, LimbTypes limbType)
        {
            return person.FindLimbComp<LimbBehaviour>(limbType);
        }


        public static T FindLimbComp<T>(this CirculationBehaviour circ, LimbTypes limbType) where T : Component
        {
            return circ.Limb.Person.FindLimbComp<T>(limbType);
        }

        public static T FindLimbComp<T>(this LimbBehaviour limb, LimbTypes limbType) where T : Component
        {
            return limb.Person.FindLimbComp<T>(limbType);
        }

        public static T FindLimbComp<T>(this PersonBehaviour person, LimbTypes limbType) where T : Component
        {
            if (IsGorse(limbType) != (person.Limbs.First().SpeciesIdentity == Species.Gorse))
                return null;
            return person.transform.Find(limbs[limbType])?.GetComponent<T>();
        }
    }

    public enum LimbTypes
    {
        //Human & Android limbs
        Head,
        UpperBody,
        MiddleBody,
        LowerBody,
        UpperArmFront,
        LowerArmFront,
        UpperArmBack,
        LowerArmBack,
        UpperLegFront,
        LowerLegFront,
        FootFront,
        UpperLegBack,
        LowerLegBack,
        FootBack,
        //Gorse limbs
        Body,
        RightUpperLegFront,
        RightBottomLegFront,
        LeftUpperLegFront,
        LeftBottomLegFront,
        RightUpperLegBack,
        RightBottomLegBack,
        LeftUpperLegBack,
        LeftBottomLegBack
    }
}


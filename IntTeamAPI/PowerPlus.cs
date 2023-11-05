using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//This API is released under the zlib license, by using it for your mod and/or downloading it you confirm that you read and agreed to the terms of said license.
//Link to the original repository: https://github.com/Int-team/Int-Team-API
//API DEPENDENCIES: LimbPlus.cs

namespace IntTeamAPI
{
    public abstract class PowerPlus : MonoBehaviour
    {
        [SkipSerialisation]
        public LimbBehaviour Limb { get; protected set; }
        [SkipSerialisation]
        public PersonBehaviour Person { get; protected set; }
        [SkipSerialisation]
        public List<Ability> Abilities { get; protected set; } = new List<Ability>();
        public bool PowerEnabled { get; protected set; } = false;

        public bool PowerActive { get; protected set; } = false;
        public bool IsCreated { get; protected set; } = false;


        protected virtual void Awake()
        {
            Limb = GetComponent<LimbBehaviour>();
            Person = Limb.Person;
        }

        protected virtual void Start()
        {
            CreatePowerInt();
        }

        protected virtual void FixedUpdate()
        {
            if (PowerActive)
            {
                if (!Person.FindLimb(LimbTypes.Head).IsConsideredAlive && PowerEnabled)
                    TogglePowerInt(false);
                else if (Person.FindLimb(LimbTypes.Head).IsConsideredAlive && !PowerEnabled)
                    TogglePowerInt(true);
            }
        }

        protected void OnDestroy()
        {
            foreach (Ability ability in Abilities)
            {
                Destroy(ability);
            }
            DeletePower();
        }

        //This method adds everything to the entity, like light sprites, strength, ability classes, etc.
        protected void CreatePowerInt()
        {
            if (!IsCreated)
            {
                PowerActive = true;
                Debug.Log("Power created!");
            }
            CreatePower();
            IsCreated = true;
        }

        //This class turns *the power* on or off, as if the power was never there. Main use for when the one with power is killed, but so he can still be revived.
        protected void TogglePowerInt(bool toggled)
        {
            PowerEnabled = toggled;
            string toggledString = toggled ? "Enabled" : "Disabled";
            Debug.Log($"Power {toggledString}!");
            TogglePower(toggled);
        }


        protected abstract void CreatePower();

        protected abstract void TogglePower(bool toggled);

        protected abstract void DeletePower();

        public void ForceTogglePower(bool toggled)
        {
            PowerActive = toggled;
            if (toggled || !PowerEnabled)
                return;
            TogglePowerInt(toggled);
            foreach (Ability ability in Abilities)
            {
                ability.ForceTogglePower(toggled);
            }
        }
    }

    public abstract class Ability : MonoBehaviour
    {
        [SkipSerialisation]
        public LimbBehaviour Limb { get; protected set; }
        [SkipSerialisation]
        public PersonBehaviour Person { get; protected set; }

        public bool IsCreated { get; protected set; } = false;

        public bool PowerActive { get; protected set; } = false;
        public bool PowerEnabled { get; protected set; } = false;

        public bool AbilityActive { get; protected set; } = false;
        public bool AbilityEnabled { get; protected set; } = false;

        protected virtual void Awake()
        {
            Limb = GetComponent<LimbBehaviour>();
            Person = Limb.Person;
        }

        protected virtual void Start()
        {
            if (!IsCreated)
            {
                PowerActive = true;

                AbilityActive = true;

                IsCreated = true;
            }
        }

        public virtual void FixedUpdate()
        {
            if (PowerActive)
            {
                if (PowerEnabled && !Person.FindLimb(LimbTypes.Head).IsConsideredAlive) 
                    TogglePowerInt(false);
                else if (!PowerEnabled && Person.FindLimb(LimbTypes.Head).IsConsideredAlive)
                    TogglePowerInt(true);
            }
            if (AbilityActive)
            {
                if (AbilityEnabled && (!Limb.NodeBehaviour.IsConnectedToRoot || !Limb.IsConsideredAlive || !Person.FindLimb(LimbTypes.Head).IsCapable))
                    ToggleAbilityInt(false);
                else if (!AbilityEnabled && Limb.NodeBehaviour.IsConnectedToRoot && Limb.IsConsideredAlive && Person.FindLimb(LimbTypes.Head).IsCapable)
                    ToggleAbilityInt(true);
            }
        }

        public abstract void TogglePower(bool toggled);

        protected virtual void TogglePowerInt(bool toggled)
        {
            PowerEnabled = toggled;
            TogglePower(toggled);
        }

        public void ForceTogglePower(bool toggled)
        {
            PowerActive = toggled;
            if (toggled || !PowerEnabled)
                return;
            TogglePowerInt(toggled);
        }

        public abstract void ToggleAbility(bool toggled);

        protected virtual void ToggleAbilityInt(bool toggled)
        {
            AbilityEnabled = toggled;
            ToggleAbility(toggled);
        }
    }
}
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

//This API is released under the zlib license, by using it for your mod and/or downloading it you confirm that you read and agreed to the terms of said license.
//Link to the original repository: https://github.com/Int-team/Int-Team-API
//API DEPENDENCIES: IntAPI.cs, CreationPlus.cs, SpawnableAssetPlus.cs

namespace IntTeamAPI
{
    public class ArmorBehaviour : MonoBehaviour
    {
        [SkipSerialisation]
        protected PhysicalBehaviour phys;
        [SkipSerialisation]
        protected string limbType;
        [SkipSerialisation]
        protected float stabResistance;
        [SkipSerialisation]
        protected int armorSortingOrder;
        [SkipSerialisation]
        protected bool properLimbContact;

        protected Collider2D[] limbColliders;
        protected List<Collider2D> otherArmorColliders = new List<Collider2D>();
        protected GameObject attachedLimb;
        protected Type armorWearerType;
        public bool Equipped { get; protected set; } = false; 
        public bool IsAttached { get; protected set; } = false;

        protected bool detachTimer = false;
        protected float timer = 0f;

        public void CreateBodyArmor(string newLimbType, float newStabResistance, bool requireProperLimbContact = false)
        {
            CreateCustom<BodyArmorWearer>(newLimbType, newStabResistance, 3, requireProperLimbContact);
        }

        public void CreateClothing(string newLimbType, bool requireProperLimbContact = false)
        {
            CreateCustom<ClothingWearer>(newLimbType, 0f, 2, requireProperLimbContact);
        }

        public void CreateCustom<T>(string newLimbType, float newStabResistance, int newArmorSorOrd, bool requireProperLimbContact = false) where T : ArmorWearer
        {
            limbType = newLimbType; //The limb that the armor will attach to (List below)
            stabResistance = Mathf.Clamp01(newStabResistance); //How likely is it for the armor to be penetrated by sharp objects? 1f = full protection, 0f = no protection
            armorSortingOrder = newArmorSorOrd; //In what order will the armor be rendered? For Studio Plus Mods, the minimum number is 2 (clothing), but 1 would also be okay
            armorWearerType = typeof(T); //What type of armor is it? (Mod adds Armor and Clothing by default which have their own shorthand methods above)
            properLimbContact = requireProperLimbContact; //If true, this will require proper limb contact for the armor piece to connect, i.e. a helmet will have to collide with the head to attach
        }
        //You can use LimbList for limbType, as it contains every single limb type for humans/androids (fuck gorses).

        //What follows this are 157 lines of delicate code that if altered could break everything.
        //Change anything at your own risk
        //(There is still stuff you can mess with down below though)

        public bool IsSameType(Type other, Type main)
        {
            if (other.IsSubclassOf(main) || other == main || other.IsAssignableFrom(main))
                return true;
            else return false;
        }

        protected void Awake()
        {
            if (attachedLimb != null)
                attachedLimb = null;
        }

        protected void Start()
        {
            phys = GetComponent<PhysicalBehaviour>();
            phys.ContextMenuOptions.Buttons.Add(
                new ContextMenuButton(
                    () => Equipped,
                    "detachArmor",
                    "Detach armor",
                    "Detach multiple armor pieces",
                    () =>
                    {
                        Detach();
                    }
                )
            );
        }

        protected void FixedUpdate()
        {
            foreach (Collider2D collider in otherArmorColliders)
            {
                if (collider == null)
                {
                    otherArmorColliders.Remove(collider);
                    break; 
                }
                var armorPiece = collider.gameObject.GetComponent<ArmorBehaviour>();
                if (Equipped || IsSameType(armorPiece.armorWearerType, armorWearerType) || limbType == armorPiece.limbType)
                    GetComponent<Collider2D>().IgnoreCollision(collider, true);
                else
                {
                    GetComponent<Collider2D>().IgnoreCollision(collider, false);
                }
            }

            if (detachTimer)
            {
                if (timer < 5f)
                    timer += Time.fixedDeltaTime;
                else
                {
                    if (attachedLimb != null)
                    {
                        GetComponent<Collider2D>().IgnoreEntityCollision(limbColliders, false);
                        foreach (GripBehaviour grip in attachedLimb.transform.root.GetComponentsInChildren<GripBehaviour>())
                        {
                            grip.RefreshNoCollide(false);
                            grip.RefreshNoCollide(true);
                        }
                        attachedLimb = null;
                    }
                    Equipped = false;
                    timer = 0f;
                    detachTimer = false;
                }
            }
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.TryGetComponent(out LimbBehaviour limb) && !Equipped)
            {
                foreach (var armorWearer in limb.transform.root.Find(limbType).gameObject.GetComponents<ArmorWearer>())
                {
                    if (IsSameType(armorWearer.GetType(), armorWearerType))
                        return;
                }
                if (properLimbContact && limb.transform.name != limbType)
                    return;
                Attach(limb.transform.root.Find(limbType).gameObject);

            }
            else if (collision.transform.TryGetComponent(out ArmorBehaviour _))
            {
                otherArmorColliders.Add(collision.collider);
            }
            else
            {
                if (UnityEngine.Random.value > stabResistance && Equipped && collision.transform.GetComponent<PhysicalBehaviour>().Properties.Sharp)
                {
                    GetComponent<Collider2D>().isTrigger = true;
                }
            }
        }

        protected void OnTriggerExit2D(Collider2D _)
        {
            GetComponent<Collider2D>().isTrigger = false;
        }

        public void Attach(GameObject limbObject)
        {
            phys = GetComponent<PhysicalBehaviour>();
            IsAttached = true;
            Equipped = true;
            attachedLimb = limbObject;
            GetComponent<SpriteRenderer>().sortingLayerName = attachedLimb.GetComponent<SpriteRenderer>().sortingLayerName;
            GetComponent<SpriteRenderer>().sortingOrder = attachedLimb.GetComponent<SpriteRenderer>().sortingOrder + armorSortingOrder;
            GetComponent<Rigidbody2D>().isKinematic = true;
            limbColliders = attachedLimb.transform.root.GetComponentsInChildren<Collider2D>();
            GetComponent<Collider2D>().IgnoreEntityCollision(limbColliders, true);

            ArmorWearer armorWearer = attachedLimb.AddComponent(armorWearerType) as ArmorWearer;
            armorWearer.isSpawnablePlus = (bool)armorWearer.gameObject.GetComponent<SpawnableAssetPlus.CustomSerializationInstructions>();
            armorWearer.armorName = transform.root.gameObject.name;
            armorWearer.ArmorObject = this;

            transform.SetParent(attachedLimb.transform);
            GetComponent<Rigidbody2D>().isKinematic = true;
            transform.rotation = attachedLimb.transform.rotation;
            transform.localPosition = Vector2.zero;
            transform.localScale = Vector2.one;

            CreationPlus.CreateFixedJoint(gameObject, attachedLimb);
            GetComponent<Rigidbody2D>().isKinematic = false;

            foreach (GripBehaviour grip in attachedLimb.transform.root.GetComponentsInChildren<GripBehaviour>())
            {
                grip.RefreshNoCollide(false);
                grip.CollidersToIgnore.Add(GetComponent<Collider2D>());
                grip.RefreshNoCollide(true);
            }
        }

        public void Detach()
        {
            IsAttached = false;
            GetComponent<SpriteRenderer>().sortingLayerName = "Default";
            GetComponent<SpriteRenderer>().sortingOrder = 0;
            Destroy(GetComponent<FixedJoint2D>());
            Destroy(attachedLimb.GetComponent(armorWearerType));
            transform.SetParent(null);
            foreach (GripBehaviour grip in attachedLimb.transform.root.GetComponentsInChildren<GripBehaviour>())
            {
                grip.CollidersToIgnore.Remove(GetComponent<Collider2D>());
            }
            detachTimer = true;
        }

        protected void OnDestroy()
        {
            if (attachedLimb != null)
                Destroy(attachedLimb.GetComponent(armorWearerType));
        }
    }

    public abstract class ArmorWearer : MonoBehaviour, Messages.IOnAfterDeserialise
    {
        [SkipSerialisation]
        public ArmorBehaviour ArmorObject { get; protected internal set; }
        protected internal bool isSpawnablePlus;
        protected internal string armorName;

        public virtual void Start()
        {
        }

        public void OnAfterDeserialise(List<GameObject> gameObjects)
        {
            GameObject newArmorObject = isSpawnablePlus ? SpawnableAssetPlus.FindSpawnablePlus(armorName).SpawnThroughItem(transform) : ModAPI.FindSpawnable(armorName).SpawnThroughItem(transform);
            ArmorObject = newArmorObject.GetComponent<ArmorBehaviour>();
            Destroy(this);
        }
    }

    public class BodyArmorWearer : ArmorWearer
    {
    }

    public class ClothingWearer : ArmorWearer
    {
    }
}
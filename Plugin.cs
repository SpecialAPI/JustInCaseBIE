using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JustInCaseBIE
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "spapi.etg.justincase";
        public const string NAME = "Just In Case";
        public const string VERSION = "1.0.0";

        public void Awake()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(x =>
            {
                ammonomiconCollection = AmmonomiconController.ForceInstance.EncounterIconCollection;

                //get guns
                var uppercaseR = PickupObjectDatabase.GetById(649) as Gun;
                var lowercaseR = PickupObjectDatabase.GetById(340) as Gun;

                //change quality
                uppercaseR.quality = PickupObject.ItemQuality.D;

                //ammonomicon-related stuff
                uppercaseR.ForcedPositionInAmmonomicon = lowercaseR.ForcedPositionInAmmonomicon;
                if (uppercaseR.encounterTrackable == null)
                {
                    uppercaseR.gameObject.AddComponent<EncounterTrackable>();
                }
                var track = uppercaseR.encounterTrackable;
                if (track.journalData == null)
                {
                    track.journalData = new JournalEntry();
                }
                track.journalData.SuppressKnownState = false;
                track.journalData.SuppressInAmmonomicon = false;
                var def = uppercaseR.sprite.CurrentSprite;
                track.journalData.AmmonomiconSprite = def.name;
                AddToAmmonomicon(def);
                
                //add to pool
                var weightedGameObject = new WeightedGameObject
                {
                    weight = 1f,
                    additionalPrerequisites = new DungeonPrerequisite[0]
                };
                weightedGameObject.SetGameObject(uppercaseR.gameObject);
                GameManager.Instance.RewardManager.GunsLootTable.defaultItemDrops.Add(weightedGameObject);

                //change synergies
                var processor = lowercaseR.GetComponents<TransformGunSynergyProcessor>().ToList().Find((TransformGunSynergyProcessor transform) => transform.SynergyToCheck == CustomSynergyType.JUST_IN_CASE);
                if (processor != null)
                {
                    Destroy(processor);
                }
                var dualWieldUppercase = uppercaseR.gameObject.AddComponent<DualWieldSynergyProcessor>();
                dualWieldUppercase.SynergyToCheck = CustomSynergyType.JUST_IN_CASE;
                dualWieldUppercase.PartnerGunID = 340;
                var dualWieldLowercase = lowercaseR.gameObject.AddComponent<DualWieldSynergyProcessor>();
                dualWieldLowercase.SynergyToCheck = CustomSynergyType.JUST_IN_CASE;
                dualWieldLowercase.PartnerGunID = 649;
                var entry = GameManager.Instance.SynergyManager.synergies.ToList().Find((AdvancedSynergyEntry synergy) => synergy.bonusSynergies.Contains(CustomSynergyType.JUST_IN_CASE) && synergy.NameKey == "#JUSTINCASE");
                entry.OptionalGunIDs = new List<int>();
                entry.MandatoryGunIDs = new List<int> { 340, 649 };
                entry.OptionalItemIDs = new List<int>();
                entry.MandatoryItemIDs = new List<int>();
            });
        }

        public static int AddToAmmonomicon(tk2dSpriteDefinition spriteDefinition)
        {
            var copyDef = CopyDefinitionFrom(spriteDefinition);
            var ammonomiconShader = ShaderCache.Acquire("tk2d/CutoutVertexColorTilted");
            if (copyDef.material != null)
            {
                copyDef.material.shader = ammonomiconShader;
            }
            if (copyDef.materialInst != null)
            {
                copyDef.materialInst.shader = ammonomiconShader;
            }
            return AddSpriteToCollection(spriteDefinition, ammonomiconCollection);
        }

        public static int AddSpriteToCollection(tk2dSpriteDefinition spriteDefinition, tk2dSpriteCollectionData collection)
        {
            var defs = collection.spriteDefinitions;
            var newDefs = defs.Concat(new tk2dSpriteDefinition[] { spriteDefinition }).ToArray();
            collection.spriteDefinitions = newDefs;
            collection.spriteNameLookupDict = null;
            collection.InitDictionary();
            return newDefs.Length - 1;
        }

        public static tk2dSpriteDefinition CopyDefinitionFrom(tk2dSpriteDefinition other)
        {
            var result = new tk2dSpriteDefinition
            {
                boundsDataCenter = new Vector3
                {
                    x = other.boundsDataCenter.x,
                    y = other.boundsDataCenter.y,
                    z = other.boundsDataCenter.z
                },
                boundsDataExtents = new Vector3
                {
                    x = other.boundsDataExtents.x,
                    y = other.boundsDataExtents.y,
                    z = other.boundsDataExtents.z
                },
                colliderConvex = other.colliderConvex,
                colliderSmoothSphereCollisions = other.colliderSmoothSphereCollisions,
                colliderType = other.colliderType,
                colliderVertices = other.colliderVertices,
                collisionLayer = other.collisionLayer,
                complexGeometry = other.complexGeometry,
                extractRegion = other.extractRegion,
                flipped = other.flipped,
                indices = other.indices,
                materialId = other.materialId,
                metadata = other.metadata,
                name = other.name,
                normals = other.normals,
                physicsEngine = other.physicsEngine,
                position0 = new Vector3
                {
                    x = other.position0.x,
                    y = other.position0.y,
                    z = other.position0.z
                },
                position1 = new Vector3
                {
                    x = other.position1.x,
                    y = other.position1.y,
                    z = other.position1.z
                },
                position2 = new Vector3
                {
                    x = other.position2.x,
                    y = other.position2.y,
                    z = other.position2.z
                },
                position3 = new Vector3
                {
                    x = other.position3.x,
                    y = other.position3.y,
                    z = other.position3.z
                },
                regionH = other.regionH,
                regionW = other.regionW,
                regionX = other.regionX,
                regionY = other.regionY,
                tangents = other.tangents,
                texelSize = new Vector2
                {
                    x = other.texelSize.x,
                    y = other.texelSize.y
                },
                untrimmedBoundsDataCenter = new Vector3
                {
                    x = other.untrimmedBoundsDataCenter.x,
                    y = other.untrimmedBoundsDataCenter.y,
                    z = other.untrimmedBoundsDataCenter.z
                },
                untrimmedBoundsDataExtents = new Vector3
                {
                    x = other.untrimmedBoundsDataExtents.x,
                    y = other.untrimmedBoundsDataExtents.y,
                    z = other.untrimmedBoundsDataExtents.z
                }
            };
            if (other.material != null)
            {
                result.material = new Material(other.material);
            }
            else
            {
                result.material = null;
            }
            if (other.materialInst != null)
            {
                result.materialInst = new Material(other.materialInst);
            }
            else
            {
                result.materialInst = null;
            }
            if (other.uvs != null)
            {
                var uvs = new List<Vector2>();
                foreach (Vector2 vector in other.uvs)
                {
                    uvs.Add(new Vector2
                    {
                        x = vector.x,
                        y = vector.y
                    });
                }
                result.uvs = uvs.ToArray();
            }
            else
            {
                result.uvs = null;
            }
            if (other.colliderVertices != null)
            {
                var colliderVertices = new List<Vector3>();
                foreach (Vector3 vector in other.colliderVertices)
                {
                    colliderVertices.Add(new Vector3
                    {
                        x = vector.x,
                        y = vector.y,
                        z = vector.z
                    });
                }
                result.colliderVertices = colliderVertices.ToArray();
            }
            else
            {
                result.colliderVertices = null;
            }
            return result;
        }

        public static tk2dSpriteCollectionData ammonomiconCollection;
    }
}

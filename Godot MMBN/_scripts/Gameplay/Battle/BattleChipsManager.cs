using Godot;
using MMBN.Gameplay.Chips;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMBN.Gameplay.Battle
{
    /// <summary>
    /// Takes care of the chips inventory while in battle.
    /// </summary>
    public partial class BattleChipsManager
    {
	    public struct BattleChipStruct
        {
            public char Code;
            public ChipBase ChipBase;
        }

        public IReadOnlyList<BattleChipStruct> EquippedBattleChips;
        public List<BattleChipStruct> AvailableBattleChips;
        public Dictionary<string, ChipBase> ChipDictionary;

        #region CHIP_IDS
        public static readonly string CANNON_CHIP_ID = "CANNON_CHIP_ID";
        public static readonly string SWORD_CHIP_ID = "SWORD_CHIP_ID";
        #endregion


        public BattleChipsManager()
        {
            // set up chips dictionary here;
            GenerateChipDictionary();

            // temporary
            List<BattleChipStruct> chipStructs = new List<BattleChipStruct>();
            
            for (int i = 0; i < 5; ++i)
            {
                BattleChipStruct cannon = new BattleChipStruct();
                cannon.Code = (char)('A' + i);
                cannon.ChipBase = ChipDictionary[CANNON_CHIP_ID];
                chipStructs.Add(cannon);

                BattleChipStruct sword = new BattleChipStruct();
                sword.Code = (char)('A' + i);
                sword.ChipBase = ChipDictionary[SWORD_CHIP_ID];
                chipStructs.Add(sword);
            }

            BattleChipStruct cannon2 = new BattleChipStruct();
            cannon2.Code = '*';
            cannon2.ChipBase = ChipDictionary[CANNON_CHIP_ID];
            chipStructs.Add(cannon2);

            BattleChipStruct sword2 = new BattleChipStruct();
            sword2.Code = '*';
            sword2.ChipBase = ChipDictionary[SWORD_CHIP_ID];
            chipStructs.Add(sword2);

            SetEquippedChips(chipStructs);
        }

        private void GenerateChipDictionary()
        {
            ChipDictionary = new Dictionary<string, ChipBase>();

            CannonChipBase cannonChip = new CannonChipBase();
            cannonChip.Init(GD.Load<ChipDataResource>("res://_resources/ChipData/chipdata_cannon.tres"));
            ChipDictionary.Add(CANNON_CHIP_ID, cannonChip);

            SwordChipBase swordChip = new SwordChipBase();
            swordChip.Init(GD.Load<ChipDataResource>("res://_resources/ChipData/chipdata_sword.tres"));
            ChipDictionary.Add(SWORD_CHIP_ID, swordChip);
        }

        public void SetEquippedChips(List<BattleChipStruct> chips)
        {
            EquippedBattleChips = chips;
        }

        public void ResetAvailableBattleChips()
        {
            AvailableBattleChips = EquippedBattleChips.ToList();
        }

        /// <summary>
        /// Gets a random assortment of available battle chips. NOTE: THIS DELETES THE CHIPS FROM THE LIST
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<BattleChipStruct> GetRandomAvailableBattleChips(int count)
        {
            List<BattleChipStruct> randomAvailableBattleChips = new List<BattleChipStruct>();
            RandomNumberGenerator generator = new RandomNumberGenerator();

            for (int i = 0; i < count; ++i)
            {
                int randomIndex = generator.RandiRange(0, AvailableBattleChips.Count - 1);
                randomAvailableBattleChips.Add(AvailableBattleChips[randomIndex]);
                AvailableBattleChips.RemoveAt(randomIndex);
            }

            return randomAvailableBattleChips;
        }
    }
}

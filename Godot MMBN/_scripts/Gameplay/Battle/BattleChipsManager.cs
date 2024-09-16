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
        public static readonly string SPREADER_CHIP_ID = "SPREADER_CHIP_ID";
        public static readonly string SWORD_CHIP_ID = "SWORD_CHIP_ID";

        public static readonly string BLASTMAN_CHIP_ID = "BLASTMAN_CHIP_ID";
        #endregion


        public BattleChipsManager()
        {
            // set up chips dictionary here;
            GenerateChipDictionary();

            // temporary
            List<BattleChipStruct> chipStructs = new List<BattleChipStruct>();

            // for chip testing
            /*
            for (int i = 0; i < 10; ++i)
            {
                BattleChipStruct testChip = new BattleChipStruct();
                testChip.Code = '*';
                testChip.ChipBase = ChipDictionary[BLASTMAN_CHIP_ID];
                chipStructs.Add(testChip);
            }
            */

            
            for (int i = 0; i < 3; ++i)
            {
                BattleChipStruct spreader = new BattleChipStruct();
                spreader.Code = (char)('A' + i);
                spreader.ChipBase = ChipDictionary[SPREADER_CHIP_ID];
                chipStructs.Add(spreader);
                
                BattleChipStruct cannon = new BattleChipStruct();
                cannon.Code = (char)('A' + i);
                cannon.ChipBase = ChipDictionary[CANNON_CHIP_ID];
                chipStructs.Add(cannon);

                BattleChipStruct sword = new BattleChipStruct();
                sword.Code = (char)('A' + i);
                sword.ChipBase = ChipDictionary[SWORD_CHIP_ID];
                chipStructs.Add(sword);


                BattleChipStruct blastman = new BattleChipStruct();
                blastman.Code = 'B';
                blastman.ChipBase = ChipDictionary[BLASTMAN_CHIP_ID];
                chipStructs.Add(blastman);
            }

            for (int i = 0; i < 2; ++i)
            {
                BattleChipStruct spreader = new BattleChipStruct();
                spreader.Code = '*';
                spreader.ChipBase = ChipDictionary[SPREADER_CHIP_ID];
                chipStructs.Add(spreader);

                BattleChipStruct cannon = new BattleChipStruct();
                cannon.Code = '*';
                cannon.ChipBase = ChipDictionary[CANNON_CHIP_ID];
                chipStructs.Add(cannon);

                BattleChipStruct sword = new BattleChipStruct();
                sword.Code = '*';
                sword.ChipBase = ChipDictionary[SWORD_CHIP_ID];
                chipStructs.Add(sword);


                BattleChipStruct blastman = new BattleChipStruct();
                blastman.Code = '*';
                blastman.ChipBase = ChipDictionary[BLASTMAN_CHIP_ID];
                chipStructs.Add(blastman);
            }
            

            SetEquippedChips(chipStructs);
        }

        private void GenerateChipDictionary()
        {
            ChipDictionary = new Dictionary<string, ChipBase>();

            CannonChipBase cannonChip = new CannonChipBase();
            cannonChip.Init(GD.Load<ChipDataResource>("res://_resources/ChipData/chipdata_001_cannon.tres"));
            ChipDictionary.Add(CANNON_CHIP_ID, cannonChip);

            SpreaderChipBase spreaderChip = new SpreaderChipBase();
            spreaderChip.Init(GD.Load<ChipDataResource>("res://_resources/ChipData/chipdata_009_spreadr1.tres"));
            ChipDictionary.Add(SPREADER_CHIP_ID, spreaderChip);

            SwordChipBase swordChip = new SwordChipBase();
            swordChip.Init(GD.Load<ChipDataResource>("res://_resources/ChipData/chipdata_070_sword.tres"));
            ChipDictionary.Add(SWORD_CHIP_ID, swordChip);

            BlastmanChipBase blastmanChip = new BlastmanChipBase();
            blastmanChip.Init(GD.Load<ChipDataResource>("res://_resources/ChipData/chipdata_322_blastman.tres"));
            ChipDictionary.Add(BLASTMAN_CHIP_ID, blastmanChip);
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

            for (int i = 0; i < count && i < AvailableBattleChips.Count; ++i)
            {
                int randomIndex = generator.RandiRange(0, AvailableBattleChips.Count - 1);
                randomAvailableBattleChips.Add(AvailableBattleChips[randomIndex]);
                AvailableBattleChips.RemoveAt(randomIndex);
            }

            return randomAvailableBattleChips;
        }
    }
}

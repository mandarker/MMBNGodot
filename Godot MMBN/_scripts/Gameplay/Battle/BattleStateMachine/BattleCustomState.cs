using Godot;
using Godot.NativeInterop;
using MMBN.UI;
using MMBN.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MMBN.Gameplay.Battle.BattleStateMachine
{
    public partial class BattleCustomState : GeneralState
    {
        [Export]
        private BattleCustomScreenUIController _battleCustomScreenUIController;

        [Export]
        private GeneralState _nextState;

        private List<BattleChipsManager.BattleChipStruct> _customChips;
        private List<BattleChipsManager.BattleChipStruct> _loadedChips;
        private bool[] _isChipLoaded;
        private Stack<int> _chipsLoadedIndexStack;

        private const int MAXIMUM_CUSTOM_CHIPS = 5;

        public override void EndState()
        {
            _battleCustomScreenUIController.CleanUpUI();
        }

        public override void StartState()
        {
            // if the custom chips are empty, reset
            if (_customChips == null)
                _customChips = new List<BattleChipsManager.BattleChipStruct>();

            // clear all loaded chips
            _loadedChips = new List<BattleChipsManager.BattleChipStruct>();

            // set the loadable flags 
            _isChipLoaded = new bool[MAXIMUM_CUSTOM_CHIPS];

            _chipsLoadedIndexStack = new Stack<int>();

            if (_customChips.Count < MAXIMUM_CUSTOM_CHIPS)
            {
                List<BattleChipsManager.BattleChipStruct> newBattleChips = Game.Instance.BattleChipsManager.GetRandomAvailableBattleChips(MAXIMUM_CUSTOM_CHIPS - _customChips.Count);
                _customChips.AddRange(newBattleChips);

                _battleCustomScreenUIController.ResetUI(_customChips);
            }

            _battleCustomScreenUIController.SetupUIEvents(TryLoadChip, SetChipDesc, SendChips);

            Game.Instance.PlayerController.OnBButtonPressed += UnloadChip;

            _battleCustomScreenUIController.ShowUI();
        }

        public override void UpdateState(float deltaTime)
        {

        }

        public void TryLoadChip(int chipIndex)
        {
            if (IsChipLoadable(_customChips[chipIndex]) && !_isChipLoaded[chipIndex])
            {
                _loadedChips.Add(_customChips[chipIndex]);
                _isChipLoaded[chipIndex] = true;
                _chipsLoadedIndexStack.Push(chipIndex);

                _battleCustomScreenUIController.AddLoadedChipSprite(_loadedChips.Count - 1, _customChips[chipIndex].ChipBase.ChipDataResource.ChipBattleTexture);
            }

            UpdateCustomChipsUI();
        }

        public void SetChipDesc(int chipIndex)
        {
            _battleCustomScreenUIController.ShowChipDesc(_customChips[chipIndex]);
        }

        public void UnloadChip()
        {
            if (_loadedChips.Count == 0)
            {
                return;
            }

            int lastChipIndex = _chipsLoadedIndexStack.Pop();
            _isChipLoaded[lastChipIndex] = false;
            _loadedChips.RemoveAt(_loadedChips.Count - 1);

            _battleCustomScreenUIController.RemoveLoadedChipSprite(_loadedChips.Count);

            UpdateCustomChipsUI();
        }

        public void SendChips()
        {
            for (int i = 0; i < _loadedChips.Count; ++i)
            {
                Game.Instance.BattleSession.ChipsController.EnqueueChip(_loadedChips[i].ChipBase);
            }
            
            Game.Instance.BattleSession.ChipsController.DisplayChipsOnPlayer();

            _battleCustomScreenUIController.HideUI();

            _parentStateMachine.SetState(_nextState);
        }

        private void UpdateCustomChipsUI()
        {
            for (int i = 0; i < _customChips.Count; ++i)
            {
                if (_isChipLoaded[i])
                {
                    _battleCustomScreenUIController.SetVisible(i, false);
                }
                else
                {
                    _battleCustomScreenUIController.SetVisible(i, true);

                    if (IsChipLoadable(_customChips[i]))
                    {
                        _battleCustomScreenUIController.SetGrey(i, false);
                    }
                    else
                    {
                        _battleCustomScreenUIController.SetGrey(i, true);
                    }
                }
            }
        }

        public bool IsChipLoadable(BattleChipsManager.BattleChipStruct loadingBattleChipStruct)
        {
            // if the loaded chips is empty, don't bother with the rest
            if (_loadedChips.Count == 0)
                return true;

            HashSet<char> codeHashset = new HashSet<char>();
            HashSet<uint> idHashset = new HashSet<uint>();

            foreach (BattleChipsManager.BattleChipStruct battlechipStruct in _loadedChips)
            {
                codeHashset.Add(battlechipStruct.Code);
                idHashset.Add(battlechipStruct.ChipBase.ChipDataResource.ID);
            }

            // special case where there's one type of chip and one of them is an asterisk code
            if (codeHashset.Contains('*'))
            {
                codeHashset.Remove('*');
            }

            // if there are multiple codes of one type of chip
            if (idHashset.Count == 1)
            {
                // if it's also the same type of chip irregardless of the code
                if (idHashset.Contains(loadingBattleChipStruct.ChipBase.ChipDataResource.ID))
                {
                    // yup you can load it
                    return true;
                }
                else
                {
                    // special case where there's an asterisk involved
                    if (codeHashset.Count > 1)
                    {
                        return false;
                    }
                }
            }

            // asterisks are always free with the exception above
            if (loadingBattleChipStruct.Code == '*')
            {
                return true;
            }

            // check if the code is within the hashset
            if (codeHashset.Contains(loadingBattleChipStruct.Code) || codeHashset.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

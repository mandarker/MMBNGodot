using Godot;
using MMBN.Gameplay.Chips;
using MMBN.Gameplay.Entities.Animation;
using System;

namespace MMBN.Gameplay.Entities.AttackBehaviours
{
    public partial class ChipAttackBehaviour : BaseAttackBehaviour
    {
        private ChipBase _chip;

        public override void BeginAttack()
        {
            _chip = Game.Instance.BattleSession.ChipsController.DequeueChip();

            _chip.StartChip(_animatorController, _currentEntity);

            _chip.OnChipFinished += OnChipFinished;
        }

        public override void EndAttack()
        {
            _chip.EndChip();
        }

        public override void DoAttack(float deltaTime)
		{
			base.DoAttack(deltaTime);
            _chip.RunChip(deltaTime);
		}

        private void OnChipFinished()
        {
            _chip.OnChipFinished -= OnChipFinished;
            OnAttackFinished?.Invoke();
        }
    }
}

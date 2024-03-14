using Godot;
using MMBN.Gameplay.Entities.Animation;
using MMBN.Utility;
using MMBN.VFX;
using System;
using System.Collections.Generic;

namespace MMBN.Gameplay.Entities.EntityStateMachine
{
	public partial class DeathState : EntityState
	{
		public override string GetStateID()
		{
			return DEATH_STATE_ID;
		}

		private List<DelayedEventHandler> _delayedExplosionHandlers;
		private List<AnimatedVFXController> _explosionVFX;

		[Export]
		private EntityAnimationController _animationController;

		[Export]
		private BattleEntity _currentEntity;

        [Export]
        private AudioStreamPlayer _deathAudioStreamPlayer;

		private bool _isPaused;

		private float[] _explosionTimes = new float[]{0f, 0.3f, 0.6f};

		private int _explosionCount;

		public override void BeginState()
		{
			_isPaused = false;
			_animationController.SetAnimationPaused(true);

			_delayedExplosionHandlers = new List<DelayedEventHandler>();
			_explosionVFX = new List<AnimatedVFXController>();

			int explosions = GD.RandRange(2, 3);

			for (int i = 0; i < explosions; ++i)
			{
				int index = i;
				DelayedEventHandler delayedExplosion = new DelayedEventHandler(
					_explosionTimes[index],
					() => {
						if (index == explosions - 1)
						{
							_animationController.SetSpriteClear(true);
						}

						DelayedSpawnExplosion();
					},
					false
					);
				_delayedExplosionHandlers.Add(delayedExplosion);
			}

			_explosionCount = explosions;

            _deathAudioStreamPlayer.Play();
		}

		private void DelayedSpawnExplosion()
		{
			Vector2 spawnPosition = _currentEntity.Position + new Vector2(GD.RandRange(-10, 10), GD.RandRange(-20, 0));
			AnimatedVFXController vfxController = VFXGeneratorHelper.GenerateBattleVFX(VFXGeneratorHelper.ExplosionID, spawnPosition);

			vfxController.ZIndex = _currentEntity.ZIndex;

			_explosionVFX.Add(vfxController);
			vfxController.OnVFXFinished += () => {
				vfxController.Free();
				--_explosionCount;
			};
		}

		public override void PauseState()
		{
			_isPaused = true;
			
			foreach (AnimatedVFXController vfxController in _explosionVFX)
			{
				vfxController.Pause();
			}
		}

		public override void ContinueState()
		{
			_isPaused = false;
			
			foreach (AnimatedVFXController vfxController in _explosionVFX)
			{
				vfxController.Unpause();
			}
		}

		public override void UpdateState(float deltaTime)
		{
			if (_isPaused)
			{
				return;
			}

			foreach (DelayedEventHandler delayedExplosion in _delayedExplosionHandlers)
			{
				delayedExplosion.Update(deltaTime);
			}

			if (_explosionCount == 0)
			{
				//_entityStateController.PauseStateMachine(this);
				Game.Instance.BattleSession.RemoveEntity(_currentEntity);
				_currentEntity.Free();
			}
		}
		
		public override void EndState()
		{

		}
	}
}

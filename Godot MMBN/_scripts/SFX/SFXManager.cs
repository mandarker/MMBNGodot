using Godot;
using System;
using System.Collections.Generic;

namespace MMBN.SFX
{
    public sealed class SFXManager
    {
        private Node _parentNode;

        private List<AudioStreamPlayer> _usedPlayers;
        private Stack<AudioStreamPlayer> _freePlayers;

        private const int DEFAULT_PLAYERS = 5;

	    public SFXManager(Node parentNode)
        {
            _parentNode = parentNode;

            _usedPlayers = new List<AudioStreamPlayer>();
            _freePlayers = new Stack<AudioStreamPlayer>();

            for (int i = 0; i < DEFAULT_PLAYERS; ++i)
            {
                AudioStreamPlayer player = GenerateAudioStreamPlayer();
                _freePlayers.Push(player);
            }
        }

        public void Update()
        {
            for (int i = 0; i < _usedPlayers.Count; ++i)
            {
                if (_usedPlayers[i].Stream.GetLength() <= _usedPlayers[i].GetPlaybackPosition())
                {
                    _freePlayers.Push(_usedPlayers[i]);
                    _usedPlayers.RemoveAt(i);
                    --i;
                }
            }
        }

        public void PlaySFX(AudioStream sfxAudioStream)
        {
            if (!_freePlayers.TryPop(out AudioStreamPlayer player))
            {
                player = GenerateAudioStreamPlayer();
            }

            _usedPlayers.Add(player);

            player.Stream = sfxAudioStream;

            player.Seek(0);
            player.Play();
        }

        private AudioStreamPlayer GenerateAudioStreamPlayer()
        {
            AudioStreamPlayer player = new AudioStreamPlayer();
            _parentNode.AddChild(player);
            player.VolumeDb = 1;

            return player;
        }
    }
}

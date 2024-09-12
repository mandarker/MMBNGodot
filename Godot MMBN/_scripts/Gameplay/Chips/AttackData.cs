using Godot;
using System;

namespace MMBN.Gameplay.Chips
{
    public class AttackData
    {
        private uint _damage;
        private string _damageSourceID;
        public enum DamageType {
            FIRE,
            AQUA,
            ELEC,
            WOOD,
            SWORD,
            WIND,
            CURSOR,
            SUMMON,
            BONUS,
            BREAK,
            NONE
        };
        private DamageType _damageType;

        public uint Damage { get { return _damage; } }
        public string DamageSource { get { return _damageSourceID; } }
        public DamageType Type { get { return _damageType; } }

	    public AttackData(uint damage, string damageSourceID, DamageType damageType)
        {
            _damage = damage;
            _damageSourceID = damageSourceID;
            _damageType = damageType;
        }

        public AttackData(ChipDataResource resource, string damageSourceID)
        {
            _damage = resource.Attack;
            _damageSourceID = damageSourceID;
            _damageType = resource.Type;
        }
    }
}

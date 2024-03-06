using Godot;
using System;
using System.Collections.Generic;

namespace MMBN{
    public sealed class GlobalVariables
    {
        private Dictionary<string, int> _intDictionary;
        private Dictionary<string, float> _floatDictionary;

        public GlobalVariables()
        {
            _intDictionary = new Dictionary<string, int>();
            _floatDictionary = new Dictionary<string, float>();
        }

        public void SubmitInt(string key, int value)
        {
            _intDictionary[key] = value;
        }

        public bool GetInt(string key, out int value)
        {
            if (!_intDictionary.ContainsKey(key))
            {
                value = 0;
                return false;
            }

            value = _intDictionary[key];
            return true;
        }   

        public void SubmitFloat(string key, float value)
        {
            _floatDictionary[key] = value;
        }

        public bool GetFloat(string key, out float value)
        {
            if (!_floatDictionary.ContainsKey(key))
            {
                value = 0;
                return false;
            }

            value = _floatDictionary[key];
            return true;
        }   
    }
}

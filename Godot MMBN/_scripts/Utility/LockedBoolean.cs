using Godot;
using System;
using System.Collections.Generic;

namespace MMBN.Utility
{
	public partial class LockedBoolean
	{
		private List<Object> _lockingObjects;
		public bool IsLocked => _lockingObjects.Count > 0;

		public LockedBoolean()
		{
			_lockingObjects = new List<object>();
		}

		public void SubmitLocker(Object obj)
		{
			if (!_lockingObjects.Contains(obj))
			{
				_lockingObjects.Add(obj);
			}
		}

		public void RemoveLocker(Object obj)
		{
			if (_lockingObjects.Contains(obj))
			{
				_lockingObjects.Remove(obj);
			}
		}
	}
}

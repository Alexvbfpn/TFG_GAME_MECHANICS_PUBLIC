using System;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
	public class ClassTypeName : PropertyAttribute
	{
		public Type type;

		public ClassTypeName(Type type)
		{
			this.type = type;
		}
	}
}

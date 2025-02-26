using UnityEngine;

namespace GameMechanics
{
	public class GameTags
	{
		public static string Player = "Player";
		public static string Enemy = "Enemy";
		public static string Hazard = "Hazard";
		public static string Platform = "Platform";
		public static bool IsEntity(Collider collider) =>
			collider.CompareTag(Player) || collider.CompareTag(Enemy);

		public static bool IsHazard(Collider collider) => collider.CompareTag(Hazard);
	}
}

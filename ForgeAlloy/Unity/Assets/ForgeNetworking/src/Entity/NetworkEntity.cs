﻿using UnityEngine;

namespace Forge.Networking.Unity
{
	public class NetworkEntity : MonoBehaviour, IUnityEntity
	{
		public int Id { get; set; }
		public GameObject OwnerGameObject => gameObject;
	}
}

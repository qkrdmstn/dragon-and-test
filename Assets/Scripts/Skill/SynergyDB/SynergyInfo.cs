using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class SynergyInfo : ScriptableObject
{
	public List<SynergyEntity> SynergyEntity; // Replace 'EntityType' to an actual type that is serializable.
}

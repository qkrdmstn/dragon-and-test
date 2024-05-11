using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class SkillBalanceTable : ScriptableObject
{
	public List<SkillBalanceEntity> SkillTableEntity; // Replace 'EntityType' to an actual type that is serializable.
}

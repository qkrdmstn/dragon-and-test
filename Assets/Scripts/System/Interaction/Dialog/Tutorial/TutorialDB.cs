using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class TutorialDB : ScriptableObject
{
	public List<TutorialDBEntity> TutorialEntity; // Replace 'EntityType' to an actual type that is serializable.
}

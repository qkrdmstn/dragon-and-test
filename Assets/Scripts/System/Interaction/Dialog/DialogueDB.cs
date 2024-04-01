using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class DialogueDB : ScriptableObject
{
	public List<DialogueDBEntity> DialogueEntity; // Replace 'EntityType' to an actual type that is serializable
}
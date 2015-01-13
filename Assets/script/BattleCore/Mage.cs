//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
public class Mage: UnitBase
{
	public Mage ()
	{
		unitType = UnitTypes.MAGE;
		this.attackPower = 90;
		this.defensePower = 15;
		this.range = 3;
		this.maxHP = 500;
		this.HP = maxHP;
		criticalRate = 30;
		criticalFactor = 5.5f;
		coolDown = 2.5f;
		coolDownFactor = 0f;
		movingSpeed = 1.5f;
	}
	public override float getAttackPower(UnitBase targetUnit)
	{
		int targetUnitType = targetUnit.unitType;
		float newAttackPower = attackPower;
		if (targetUnitType == UnitTypes.SHAMAN)
			newAttackPower *= 1f;
		else if (targetUnitType == UnitTypes.TANK)
			newAttackPower *= 1f;
		return newAttackPower;
	}
	public override float getDefensePower(UnitBase AttackerUnit)
	{
		int AttackerUnitType = AttackerUnit.unitType;
		float newDefensePower = defensePower;
		if (AttackerUnitType == UnitTypes.TANK)
			newDefensePower *= 0.7f;
		if (AttackerUnitType == UnitTypes.SHAMAN)
			newDefensePower *= 1.2f;
		return newDefensePower;
	}
}


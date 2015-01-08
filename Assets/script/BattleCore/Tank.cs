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
public class Tank:UnitBase
{
	public Tank ()
	{
		unitType = UnitTypes.TANK;
		this.attackPower = 80;
		this.defensePower = 60;
		this.range = 1;
		this.maxHP = 1600;
		this.HP = maxHP;
		criticalRate = 45;
		criticalFactor = 1.5f;
		coolDown = 3f;
		coolDownFactor = 0f;
	}
	public override float getAttackPower(UnitBase targetUnit)
	{
		int targetUnitType = targetUnit.unitType;
		float newAttackPower = attackPower;
		if (targetUnitType == UnitTypes.MAGE)
			newAttackPower *= 1f;
		if (targetUnitType == UnitTypes.ROGUE)
			newAttackPower *= 1f;
		return newAttackPower;
	}
	public override float getDefensePower(UnitBase AttackerUnit)
	{
		int AttackerUnitType = AttackerUnit.unitType;
		float newDefensePower = defensePower;
		if (AttackerUnitType == UnitTypes.ROGUE)
			newDefensePower *= 0.7f;
		if (AttackerUnitType == UnitTypes.MAGE)
			newDefensePower *= 1.2f;
		return newDefensePower;
	}
}

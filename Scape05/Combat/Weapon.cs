﻿namespace Scape05.Engine.Combat;

public record Weapon(int ItemId, int LevelReq, int Speed, CombatAnimations animation, WeaponType Type);
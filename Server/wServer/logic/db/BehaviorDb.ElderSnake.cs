using common.resources;
using wServer.logic.behaviors;
using wServer.logic.loot;
using wServer.logic.transitions;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private _ ElderSnake = () => Behav()
                .Init("Elder Stheno the Snake Queen",
                new State(
                    //9000, 150000
                    new ScaleHP(9 * 10 ^ 3, 15 * 10 ^ 4),
                    new RealmPortalDrop(),
                    new State("Idle",
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable),
                        new PlayerWithinTransition(8, "Phase1")
                          ),
                    new State("Phase1",
                       new HpLessTransition(.73, "Phase3"),
                       new Shoot(50, projectileIndex: 0, count: 3, fixedAngle: 0, coolDown: 200, rotateAngle: 10),
                       new Shoot(50, projectileIndex: 0, count: 3, fixedAngle: 10, coolDown: 200, rotateAngle: 10),
                       new TimedTransition(10900, "Phase2")
                        ),
                                new State("Phase2",
                       new HpLessTransition(.73, "Phase3"),
                       new Shoot(50, projectileIndex: 0, count: 3, fixedAngle: 0, coolDown: 200, rotateAngle: 10),
                       new Shoot(50, projectileIndex: 0, count: 3, fixedAngle: 10, coolDown: 200, rotateAngle: 10),
                       new TimedTransition(10700, "Phase1")
                                    ),
                    new State("Phase3",
                       new HpLessTransition(.21, "Death"),
                       new Grenade(2.5, 200, 10, coolDown: 3000),
                       new Shoot(50, projectileIndex: 0, count: 3, fixedAngle: 0, coolDown: 200, rotateAngle: 10),
                       new Shoot(50, projectileIndex: 0, count: 3, fixedAngle: 10, coolDown: 200, rotateAngle: 10),
                       new TimedTransition(10900, "Phase4")
                        ),
                                new State("Phase4",
                       new HpLessTransition(.21, "Death"),
                       new Grenade(2.5, 200, 10, coolDown: 3000),
                       new Shoot(50, projectileIndex: 0, count: 3, fixedAngle: 0, coolDown: 200, rotateAngle: 10),
                       new Shoot(50, projectileIndex: 0, count: 3, fixedAngle: 10, coolDown: 200, rotateAngle: 10),
                       new TimedTransition(10700, "Phase3")
                          ),
                    new State("Death",
                       new Taunt("SSSSsSSSssS......"),
                       new TimedTransition(2000, "Death2")
                ),
                    new State("Death2",
                        new Suicide()
                        )
                    ),
                new MostDamagers(3,
                    LootTemplates.Sor2Perc()
                    ),
                /*new Threshold(0.015,
                    new ItemLoot("Rare Quest Chest Item", 0.05),
                              ),*/
                new MostDamagers(3,
                    new ItemLoot("1000 Gold", 0.1),
                    new OnlyOne(
                        new ItemLoot("Stheno's Poison Crossbow", 0.008),
                        new ItemLoot("Poisonous Crafted-Star", 0.02),
                        new ItemLoot("Dagger of Stheno's Poisonous Remains", 0.006)
                        )
                    ),
                new Threshold(0.076,
                    new ItemLoot("Greater Potion of Speed", 1),
                    new ItemLoot("Greater Potion of Speed", 0.2),
                    new TierLoot(10, ItemType.Weapon, 0.15),
                    new TierLoot(5, ItemType.Ability, 0.15),
                    new TierLoot(11, ItemType.Armor, 0.15),
                    new TierLoot(4, ItemType.Ring, 0.15),
                    new TierLoot(12, ItemType.Armor, 0.05),
                    new TierLoot(6, ItemType.Ability, 0.05),
                    new TierLoot(11, ItemType.Weapon, 0.05),
                    new TierLoot(5, ItemType.Ring, 0.05)
                )
            )
            
        ;
    }
}
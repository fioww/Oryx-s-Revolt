using common.resources;
using wServer.logic.behaviors;
using wServer.logic.loot;
using wServer.logic.transitions;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private _ ElderAbyss = () => Behav()
            .Init("Elder Archdemon Malphas",
                new State(
                    //10000, 2000000
                    new ScaleHP(1 * 10^4, 2 * 10^5),
                    new OnDeathBehavior(new ApplySetpiece("AbyssDeath")),
                    new RealmPortalDrop(),
                    new State("FindPlayer",
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable),
                        new PlayerWithinTransition(16, "Basic")
                                                ),
                    new State("Basic",
                        new Taunt("Fools! You underestimate the power of the mighty god Malphas!"),
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable),
                        new TimedTransition(1500, "Phase1")
                                             ),
                new State("Phase1",
                       new HpLessTransition(.73, "Phase4"),
                       new Shoot(50, projectileIndex: 0, count: 4, fixedAngle: 45, coolDown: 120, rotateAngle: -8),
                       new TimedTransition(10900, "Phase2")
                        ),
                                new State("Phase2",
                       new HpLessTransition(.73, "Phase4"),
                       new Shoot(50, projectileIndex: 1, count: 4, shootAngle: 6, predictive: 0.7, coolDown: 750),
                       new Shoot(50, projectileIndex: 0, count: 4, fixedAngle: 45, coolDown: 120, rotateAngle: -8),
                       new TimedTransition(10700, "Phase1")
                          ),
                new State("Phase4",
                        new Taunt("Little mortals...You think your blows hurt me! Pathetic. I shall end your life at this moment."),
                    new Follow(2),
                            new Shoot(50, projectileIndex: 0, count: 1, rotateAngle: 8, fixedAngle: 102, coolDown: 120),
                           new TimedTransition(10700, "Phase6")
                        ),
                    new State("Phase6",
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable),
                        new TossObject("shtrs Fire Portal", range: 6, angle: 180, coolDown: 10000000),
                        new TossObject("shtrs Fire Portal", range: 6, angle: 360, coolDown: 10000000),
                        new TimedTransition(1500, "BasicCheck")
                                                ),
                    new State("BasicCheck",
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable),
                        new EntityNotExistsTransition("shtrs Fire Portal", 500, "Phase7")
                                             ),               
                    new State("Phase7",
                        new Wander(0.9),
                        new Shoot(10, 6, projectileIndex: 2, predictive: 1, coolDown: 500),
                        new HpLessTransition(.21, "Death")
                ),
                    new State("Death",
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable),
                        new Taunt("What... The power of the gods... Dominated my HUMANS?! I will have my revenge. Be weary adventurer..."),
                        new ReturnToSpawn(2),
                       new TimedTransition(2000, "Death2")
                ),
                    new State("Death2",
                        new Suicide()
                        )
                    ),
                new MostDamagers(3,
                    LootTemplates.Sor2Perc()
                    ),
                new MostDamagers(3,
                    new ItemLoot("Potion of Protection", 0.25),
                    new ItemLoot("Potion of Restoration", 0.66)
                    ),
                    /*),
                new Threshold(0.015,
                    new ItemLoot("Mark of Malphas", 0.15),
                    new ItemLoot("Rare Quest Chest Item", 0.05),
                              ),*/
                new MostDamagers(4,
                    new ItemLoot("1000 Gold", 0.1),
                    new ItemLoot("Demon Blade", 0.04),
                    new OnlyOne(
                        new ItemLoot("Staff of the Fiery", 0.007),
                        new ItemLoot("Wand of the Fiery", 0.009),
                        new ItemLoot("Bow of the Fiery", 0.0105)
                        )
                    ),
                new Threshold(0.05,
                    new ItemLoot("Greater Potion of Defense", 1),
                    new ItemLoot("Greater Potion of Vitality", 1),
                    new TierLoot(11, ItemType.Weapon, 0.03),
                    new TierLoot(5, ItemType.Ability, 0.03),
                    new TierLoot(12, ItemType.Armor, 0.03),
                    new TierLoot(5, ItemType.Ring, 0.03)                 
                )
            )
            .Init("Lava Minion Summoner",
                new State(
                        new ConditionalEffect(ConditionEffectIndex.Invincible),
                        new Reproduce("shtrs Lava Souls", densityMax: 1, coolDown: 5000)
                          )
                    )
        /*.Init("shtrs Lava Souls",
                new State(
                    new ScaleHP(400, 0),
                    new State("active",
                        new Follow(0.6, acquireRange: 20, range: 0),
                        new PlayerWithinTransition(2, "blink")
                    ),
                    new State("blink",
                        new Flash(0xfFF0000, flashRepeats: 10000, flashPeriod: 0.1),
                        new TimedTransition(2000, "explode")
                    ),
                    new State("explode",
                        new Flash(0xfFF0000, flashRepeats: 5, flashPeriod: 0.1),
                        new Shoot(5, 9),
                        new Suicide()
                    )
            )
            )*/
            ;
    }
}
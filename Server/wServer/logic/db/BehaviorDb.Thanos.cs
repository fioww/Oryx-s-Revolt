using common.resources;

using wServer.logic.behaviors;
using wServer.logic.transitions;
using wServer.logic.loot;

namespace wServer.logic
{
    partial class BehaviorDb
    {
        private _ Thanos = () => Behav()
            .Init("Thanos",
            new State(
                new ScaleHP(5000, 0),
                new ChangeSize(100, 250),
                new Wander(0.5),
                new State("start",
          new PlayerWithinTransition(10, "snap")
                    ),
                new State("snap",
                new Shoot(50, projectileIndex: 2, count: 3, fixedAngle: 0, rotateAngle: -10 - (1 / 3), coolDown: 440),
                new Shoot(50, projectileIndex: 2, count: 3, fixedAngle: 8, rotateAngle: 10 + (1 / 3), coolDown: 440)
                    )
                ),
                new Threshold(0.05,
                    new ItemLoot("Potion of Dexterity", 1),
                    new ItemLoot("Potion of Wisdom", 1),
                    new ItemLoot("Infinity Gauntlet", 0.006),
                    new ItemLoot("Fake Infinity Gauntlet", 0.04)
                    )
            )
            .Init("BB God Egg",
            new State(
                new ScaleHP(5000, 0),
                    new State("FindPlayer",
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable),
                        new PlayerWithinTransition(8, "Say")
                        ),
                    new State("Say",
                     new ConditionalEffect(ConditionEffectIndex.Invulnerable),
                     new Taunt("guess the magic word... and ill give you the best loot"),
                     new PlayerTextTransition("Basic", "haha", 8, false, true),
                     new TimedTransition(100000000, "Basic")
                        ),
                    new State("Basic",
                     new HpLessTransition(.93, "Ooh"),
                     new Taunt("happy easter!")
                                                ),
                    new State("Ooh",
                     new HpLessTransition(.84, "Ooh2"),
                     new Taunt("ouch that hurts")
                                                ),
                    new State("Ooh2",
                     new HpLessTransition(.75, "Ooh3"),
                     new Taunt("oof")
                                                ),
                    new State("Ooh3",
                     new Taunt("argh")
                        )
                    ),
                new Threshold(0.05,
                    new ItemLoot("Potion of Dexterity", 0.2),
                    new ItemLoot("Potion of Wisdom", 0.2),
                    new ItemLoot("Admin Crown", 0.05),
                    new ItemLoot("Infinity Gauntlet", 0.001),
                    new ItemLoot("Fake Infinity Gauntlet", 0.01)
                    )
            );
    }
}
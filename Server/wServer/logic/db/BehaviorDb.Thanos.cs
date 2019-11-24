using common.resources;
using System;
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
                    new ConditionalEffect(ConditionEffectIndex.ArmorBreakImmune),
                    new ScaleHP(10000, 0),
                    new State("start",
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable),
                        new PlayerWithinTransition(10, "snap")
                    ),
                new State("snap",
                    new Wander(0.5),
                    new ChangeSize(100, 250),
                    new Shoot(50, projectileIndex: 2, count: 3, fixedAngle: 0, rotateAngle: -10 + RandomNumbers(-10, 5), coolDown: 460),
                    new Shoot(50, projectileIndex: 2, count: 3, fixedAngle: 8, rotateAngle: 10 + RandomNumbers(0, 15), coolDown: 460)
                    )
                ),
                new Threshold(0.03,
                    new ItemLoot("Potion of Dexterity", 1),
                    new ItemLoot("Potion of Wisdom", 1),
                    new ItemLoot("Infinity Gauntlet", 0.006)
                    )
            )
            .Init("BB God Egg",
            new State(
                new ScaleHP(5000, 0),
                    new State("FindPlayer",
                        new ConditionalEffect(ConditionEffectIndex.Invulnerable, true),
                        new PlayerWithinTransition(8, "Say")
                        ),
                    new State("Say",
                        new Taunt("Guess the easter word... good loot guaranteed... ?"),
                        new PlayerTextTransition("Basic", "yuh is gay", 8, false, true)
                        ),
                    new State("Basic",
                        new RemoveConditionalEffect(ConditionEffectIndex.Invulnerable)
                        )
                    ),
                    new MostDamagers(3,
                        LootTemplates.Sor2Perc()
                        ),
                    new MostDamagers(3,
                        new ItemLoot("Infinity Gauntlet", 0.002)
                        ),
                    new MostDamagers(10,
                        new ItemLoot("Potion of Dexterity", 0.2),
                        new ItemLoot("Potion of Wisdom", 0.2),
                        new ItemLoot("Admin Crown", 0.05)
                    )
            );
    }
}
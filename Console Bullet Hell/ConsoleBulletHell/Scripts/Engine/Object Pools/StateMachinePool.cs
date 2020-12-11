using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class StateMachinePool : ObjectPool
    {
        private StateMachine _ref;

        public StateMachinePool(StateMachine reference, int initialCount) : base(initialCount)
        {
            _ref = reference;
            GenerateInitial(initialCount);
        }

        public static StateMachine GetEntityStateMachine(EntityID entity, Difficulty difficulty, int id = 0)
        {
            StateMachine stateMachine;
            State[] states = null;
            switch (entity)
            {
                default:
                    states = new State[] { new State("Default", new StateAction[] { new WaitAFrameAction() }) };
                    stateMachine = new StateMachine(states);

                    stateMachine.LoadStateMachine(typeof(Entity));
                    break;
                case EntityID.ENEMY:

                    switch (id)
                    {
                        case 0:
                            states = new State[]
                            {
                                new State("Default",
                                new StateAction[]
                                {
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-25, 20.0f), true), new Waypoint(new Vector2(-80.0f, 0), true), 2.0f, true, true),
                                    new WaitAFrameAction(),
                                    new WaitForSecondsAction(1.0f),

                                            new SpawnPatternAction(0, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.5f, 3.5f, "Enemy_Attack_0", "Bullet_Medium", 500, 1f, 0.25f, new Waypoint(new Vector2(-0, -0), true), Vector2.one, true, 0),

                            }, false, false),

                                    new WaitForSecondsAction(1.0f),
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-5, 15.0f), true), new Waypoint(new Vector2(-220.0f, -55), true), 3.0f, true, true),
                                    new CallMethodAction<Enemy>("Die", false),
                                })
                            };
                            break;
                        case 1:
                            states = new State[]
                            {
                                new State("Default",
                                new StateAction[]
                                {
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-25, -20.0f), true), new Waypoint(new Vector2(-80, 0), true), 2.0f, true, true),
                                    new WaitAFrameAction(),
                                    new WaitForSecondsAction(1.0f),

                                            new SpawnPatternAction(0, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.5f, 3.5f, "Enemy_Attack_0", "Bullet_Medium", 500, 1f, 0.25f, new Waypoint(new Vector2(-0, -0), true), Vector2.one, true, 0),
                            }, false, false),

                                    new WaitForSecondsAction(1.0f),
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-5, -15.0f), true), new Waypoint(new Vector2(-220.0f, 55), true), 3.0f, true, true),
                                    new CallMethodAction<Enemy>("Die", false),
                                })
                            };
                            break;

                        case 2:
                            states = new State[]
                            {
                                new State("Default",
                                new StateAction[]
                                {
                                    new MoveToPositionAction(2, true, new WaveModifier(FadingType.LINEAR, WaveType.LINEAR, 1, -0, -220, 0, 0), new WaveModifier(FadingType.LINEAR, WaveType.SINE, 2.5f, -45, 45, 0 , 0), 1.0f, true),
                                    new WaitAFrameAction(),
                                    new WaitForSecondsAction(1.0f),

                                            new SpawnPatternAction(0, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.5f, 3.5f, "Enemy_Attack_2", "Bullet_Small", 500, 1f, 0.25f, new Waypoint(new Vector2(-0, -0), true), Vector2.one, true, 0),

                            }, false, false),

                                    new WaitForSecondsAction(1.0f),
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-5, 15.0f), true), new Waypoint(new Vector2(-220.0f, -55), true), 3.5f, true, true),
                                    new CallMethodAction<Enemy>("Die", false),
                                })
                            };
                            break;

                        case 3:
                            states = new State[]
                            {
                                new State("Default",
                                new StateAction[]
                                {
                                    new MoveToPositionAction(2, true, new WaveModifier(FadingType.LINEAR, WaveType.LINEAR, 1, -0, -220, 0, 0), new WaveModifier(FadingType.LINEAR, WaveType.SINE, 2.5f, 45, -45, 0 , 0), 1.0f, true),
                                    new WaitAFrameAction(),
                                    new WaitForSecondsAction(1.0f),

                                            new SpawnPatternAction(0, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.5f, 3.5f, "Enemy_Attack_2_Rev", "Bullet_Small", 500, 1f, 0.25f, new Waypoint(new Vector2(-0, -0), true), Vector2.one, true, 0),

                            }, false, false),

                                    new WaitForSecondsAction(1.0f),
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-5, -15.0f), true), new Waypoint(new Vector2(-220.0f, 55), true), 3.5f, true, true),
                                    new CallMethodAction<Enemy>("Die", false),
                                })
                            };
                            break;


                        case 998:
                            states = new State[]
                            {
                                new State("Default",
                                new StateAction[]
                                {
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-75, -15.0f), true), new Waypoint(new Vector2(-70.0f, 0), true), 0.5f, true, true),
                                    new WaitAFrameAction(),
                                    new WaitForSecondsAction(0.5f),

                                            new SpawnPatternAction(0, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.15f, 3.5f, "Enemy_Attack_1", "Bullet_Small", 500, 1f, 0.25f, new Waypoint(new Vector2(-0, -0), true), Vector2.one, true, 0),

                            }, false, false),

                                    new WaitForSecondsAction(1.0f),
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-85, 0), true), new Waypoint(new Vector2(-220.0f, 50), true), 1.5f, true, true),
                                    new CallMethodAction<Enemy>("Die", false),
                                })
                            };
                            break;
                        case 999:
                            states = new State[]
                            {
                                new State("Default",
                                new StateAction[]
                                {
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-75, 15.0f), true), new Waypoint(new Vector2(-70.0f, 0), true), 0.5f, true, true),
                                    new WaitAFrameAction(),
                                    new WaitForSecondsAction(0.5f),

                                            new SpawnPatternAction(0, new PatternInstance[] {
                             new PatternInstance(true, 1, 0.0f, 0.15f, 3.5f, "Enemy_Attack_1", "Bullet_Small", 500, 1f, 0.25f, new Waypoint(new Vector2(-0, -0), true), Vector2.one, true, -0),

                            }, false, false),

                                    new WaitForSecondsAction(1.0f),
                                    new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true), new Waypoint(new Vector2(-85, 0), true), new Waypoint(new Vector2(-220.0f, -50), true), 1.5f, true, true),
                                    new CallMethodAction<Enemy>("Die", false),
                                })
                            };
                            break;
                    }
                    stateMachine = new StateMachine(states);
                    stateMachine.LoadStateMachine(typeof(Enemy));
                    break;
                case EntityID.BOSS_1:

                    switch (difficulty)
                    {
                        case Difficulty.EASY:
                            states = new State[]
                        {
                        new State("Boss_Intro", new StateAction[]
                        {
                            new SetPropertyAction<bool, MotherShip>("CanCollide", false, false),        //MARK BOSS AS "IMMUNE"
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),      //DISALLOW PLAYER FOLLOWING
                            new MoveToPositionAction(new Waypoint(new Vector2(100.0f, 0f), false)),     //SET BOSS'S POSITION OFF-SCREEN
                            new CallMethodAction<MotherShip>("StartIntroVS", false),                    //PLAY THE ITNRO VS ANIMATION
                            new WaitForSecondsAction(5.0f),                                             //WAIT A FEW SECONDS

                            new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),            //INTERPOLATE BOSS'S POSITION FROM IT'S CURRENT 
                                                     new Waypoint(new Vector2(55.0f, 0), false),        //POSITION TO THE INITIAL POSITION 
                                                     3.0f, true, true),

                            new SetPropertyAction<bool, MotherShip>("CanCollide", true, false),         //MAKE THE BOSS BE ABLE TO TAKE DAMAGE AGAIN
                            new GotoStateAction<bool, MotherShip>("Boss_Phase_1_Idle_First")            //GO TO THE NEXT STATE
                        }),

#region PHASE 1
                            //PHASE 1
                            new State("Boss_Phase_1_Idle_First", new StateAction[]
                        {
                            new SetPropertyAction<bool, MotherShip>("followPlayer", true, false),
                            new GotoStateAction<float, MotherShip>("Boss_Phase_2_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.75f)),
                            new WaitForSecondsAction(4.0f),

                            new GotoStateAction<float, MotherShip>("Boss_Phase_1_Attack_2"),
                        }),

                        new State("Boss_Phase_1_Idle", new StateAction[]
                        {
                            new SetPropertyAction<bool, MotherShip>("followPlayer", true, false),
                            new GotoStateAction<float, MotherShip>("Boss_Phase_2_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.75f)),
                            new WaitForSecondsAction(4.0f),

                            new GotoStateAction<float, MotherShip>(new WeightedObject<string>[] {
                                                                   new WeightedObject<string>("Boss_Phase_1_Attack_1", 10),
                                                                   new WeightedObject<string>("Boss_Phase_1_Attack_2", 5),
                            }),
                        }),

                        new State("Boss_Phase_1_Attack_1", new StateAction[]
                        {
                            new GotoStateAction<float, MotherShip>("Boss_Phase_2_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.75f)),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),
                            new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(65.0f, 0), false),
                                                     0.75f, true, true),

                            new SpawnPatternAction(5, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.35f, 3.5f, "Boss_Attack_Circle_1_Easy", "Bullet_NMY", 500, 3.5f, 0.025f, new Waypoint(new Vector2(-3, -13), true), Vector2.up, false, 33.0f),
                                new PatternInstance(true, 1, 0.0f, 0.35f, 3.5f, "Boss_Attack_Circle_1_Easy", "Bullet_NMY", 500, 3.5f, 0.025f, new Waypoint(new Vector2(-3, 13), true), Vector2.up, false, -33.0f),
                            }, true, true),

                            new WaitForSecondsAction(1.0f),
                              new SpawnPatternAction(12, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.4f, 3.5f, "Boss_Attack_Circle_1_Easy_Wave", "Bullet_NMY", 500, 7.0f, 0.025f, new Waypoint(new Vector2(-3, -13), true), Vector2.one, true, new WaveModifier(FadingType.SMOOTH_STEP, WaveType.SINE, 0.85f, 40, 0, 0)),
                                new PatternInstance(true, 1, 0.0f, 0.4f, 3.5f, "Boss_Attack_Circle_1_Easy_Wave", "Bullet_NMY", 500, 7.0f, 0.025f, new Waypoint(new Vector2(-3, 13), true), Vector2.one, true, new WaveModifier(FadingType.SMOOTH_STEP, WaveType.SINE, 0.85f, -40, 0, 0)),
                            }, true, true),

                            new WaitForSecondsAction(4.0f),
                            new GotoStateAction<bool, MotherShip>("Boss_Phase_1_Idle"),

                        }),

                        new State("Boss_Phase_1_Attack_1", new StateAction[]
                        {
                            new GotoStateAction<float, MotherShip>("Boss_Phase_2_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.75f)),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),
                            new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(65.0f, 0), false),
                                                     0.75f, true, true),

                            new SpawnPatternAction(3, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.35f, 3.5f, "Boss_Attack_Circle_1_Easy", "Bullet_NMY", 500, 3.5f, 0.025f, new Waypoint(new Vector2(-3, -13), true), Vector2.up, false, 33.0f),
                                new PatternInstance(true, 1, 0.0f, 0.35f, 3.5f, "Boss_Attack_Circle_1_Easy", "Bullet_NMY", 500, 3.5f, 0.025f, new Waypoint(new Vector2(-3, 13), true), Vector2.up, false, -33.0f),
                            }, false, true),

                            new WaitForSecondsAction(0.5f),
                              new SpawnPatternAction(5, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.4f, 3.5f, "Boss_Attack_Circle_1_Easy_Wave", "Bullet_NMY", 500, 7.0f, 0.025f, new Waypoint(new Vector2(-3, -13), true), Vector2.one, true, new WaveModifier(FadingType.SMOOTH_STEP, WaveType.SINE, 0.85f, 40, 0, 0)),
                                new PatternInstance(true, 1, 0.0f, 0.4f, 3.5f, "Boss_Attack_Circle_1_Easy_Wave", "Bullet_NMY", 500, 7.0f, 0.025f, new Waypoint(new Vector2(-3, 13), true), Vector2.one, true, new WaveModifier(FadingType.SMOOTH_STEP, WaveType.SINE, 0.85f, -40, 0, 0)),
                            }, true, true),

                            new WaitForSecondsAction(2.0f),
                            new GotoStateAction<bool, MotherShip>("Boss_Phase_1_Idle"),

                        }),

                        new State("Boss_Phase_1_Attack_2", new StateAction[]
                        {
                            new GotoStateAction<float, MotherShip>("Boss_Phase_2_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.75f)),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),
                            new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(65.0f, 0), false),
                                                     0.75f, true, true),

                            new SpawnPatternAction(16, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.85f, 3.5f, "Boss_Attack_2_Easy", "Bullet_NMY", 500, 3.5f, 0.025f, new Waypoint(new Vector2(-3, -13), true), Vector2.one, true, new WaveModifier(FadingType.LINEAR, WaveType.SINE, 1, 0, 0, 0, 0)),
                                new PatternInstance(true, 1, 0.0f, 0.85f, 3.5f, "Boss_Attack_2_Easy", "Bullet_NMY", 500, 3.5f, 0.025f, new Waypoint(new Vector2(-3, 13), true), Vector2.one, true, new WaveModifier(FadingType.LINEAR, WaveType.SINE, 1, 0, 0, 0, 0)),
                            }, true, false),

                            new WaitForSecondsAction(4.0f),
                            new GotoStateAction<bool, MotherShip>("Boss_Phase_1_Idle"),

                        }),
                            #endregion

#region PHASE 2
                            //PHASE 2
                            new State("Boss_Phase_2_Intro", new StateAction[]
                        {
                            new GotoStateAction<float, MotherShip>("Boss_Phase_3_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.333f)),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                            new WaitForSecondsAction(2.0f),
                            new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(55.0f, 0), false),
                                                     0.75f, true, true),
                            new SetPropertyAction<int, MotherShip>("CurrentAttack", 0, false),
                            new CallMethodAction<MotherShip>("StartLaser", false),

                            new WaitForSecondsAction(0.85f),
                                   new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(75.0f, 0), false),
                                                     0.25f, true, true),
                            new WaitForSecondsAction(6.0f),
                            new CallMethodAction<MotherShip>("StopLaser", false),
                            new WaitForSecondsAction(3.0f),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                             new GotoStateAction<float, MotherShip>("Boss_Phase_2_Idle"),
                        }),

                         new State("Boss_Phase_2_Idle", new StateAction[]
                        {
                             new GotoStateAction<float, MotherShip>("Boss_Phase_3_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.333f)),
                             new GotoStateAction<float, MotherShip>(new WeightedObject<string>[] {
                                                                   new WeightedObject<string>("Boss_Phase_2_Idle_1", 5),
                                                                   new WeightedObject<string>("Boss_Phase_2_Idle_2", 5),
                            }),
                        }),

                            new State("Boss_Phase_2_Attack_Select", new StateAction[]
                        {
                             new GotoStateAction<float, MotherShip>("Boss_Phase_3_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.333f)),
                             new GotoStateAction<float, MotherShip>(new WeightedObject<string>[] {
                                                                   new WeightedObject<string>("Boss_Phase_2_Laser_One", 5),
                                                                   new WeightedObject<string>("Boss_Phase_2_Laser_Two", 15),
                            }),
                        }),


                           new State("Boss_Phase_2_Idle_1", new StateAction[]
                        {
                             new GotoStateAction<float, MotherShip>("Boss_Phase_3_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.333f)),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", true, false),

                            new WaitForSecondsAction(1.0f),

                                       new SpawnPatternAction(8, new PatternInstance[] {
                                new PatternInstance(true, 4, 0.0f, 0.15f, 3.5f, "Enemy_Attack_0", "Bullet_Big", 500, 1.5f, 0.85f, new Waypoint(new Vector2(-3, -13), true), Vector2.one, true, 0),
                                new PatternInstance(true, 4, 0.0f, 0.15f, 3.5f, "Enemy_Attack_0", "Bullet_Big", 500, 1.5f, 0.85f, new Waypoint(new Vector2(-3, 13), true), Vector2.one, true, 0),
                            }, true, true),

                               new WaitForSecondsAction(1.0f),

                              new GotoStateAction<float, MotherShip>("Boss_Phase_2_Attack_Select"),
                        }),


                         new State("Boss_Phase_2_Idle_2", new StateAction[]
                        {
                             new GotoStateAction<float, MotherShip>("Boss_Phase_3_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.333f)),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", true, false),

                            new WaitForSecondsAction(1.0f),

                                new SpawnPatternAction(-1, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.065f, 3.5f, "Boss_Spiral_1", "Bullet_NMY", 500, 1.0f, 0.015f, new Waypoint(new Vector2(-3, -13), true), Vector2.up, false, 82.5f),
                                new PatternInstance(true, 1, 0.0f, 0.065f, 3.5f, "Boss_Spiral_1_Rev", "Bullet_NMY", 500, 1.0f, 0.015f, new Waypoint(new Vector2(-3, 13), true), Vector2.up, false, -82.5f),
                            }, true, false),


                            new SpawnPatternAction(8, new PatternInstance[] {
                                new PatternInstance(true, 2, 0.0f, 0.85f, 3.5f, "Enemy_Attack_0", "Bullet_Medium", 600, 2.5f, 0.05f, new Waypoint(new Vector2(-3, -13), true), Vector2.one, true, 0),
                                new PatternInstance(true, 2, 0.0f, 0.85f, 3.5f, "Enemy_Attack_0", "Bullet_Medium", 600, 2.5f, 0.05f, new Waypoint(new Vector2(-3, 13), true), Vector2.one, true, -0),
                            }, true, true),

                            new WaitForSecondsAction(1.0f),

                            new GotoStateAction<float, MotherShip>("Boss_Phase_2_Attack_Select"),

                        }),

                        new State("Boss_Phase_2_Laser_One", new StateAction[]
                        {
                             new GotoStateAction<float, MotherShip>("Boss_Phase_3_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.333f)),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                            new WaitForSecondsAction(2.0f),
                            new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(55.0f, 0), false),
                                                     0.75f, true, true),
                            new SetPropertyAction<int, MotherShip>("CurrentAttack", 0, false),
                            new CallMethodAction<MotherShip>("StartLaser", false),
                                  new WaitForSecondsAction(0.85f),
                                   new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(75.0f, 0), false),
                                                     0.25f, true, true),

                            new WaitForSecondsAction(6.0f),
                            new CallMethodAction<MotherShip>("StopLaser", false),
                            new WaitForSecondsAction(3.0f),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                            new GotoStateAction<float, MotherShip>("Boss_Phase_2_Idle"),
                        }),

                        new State("Boss_Phase_2_Laser_Two", new StateAction[]
                        {
                            new GotoStateAction<float, MotherShip>("Boss_Phase_3_Intro", true, true, new FloatCondition<MotherShip>(ComparisonType.LESS_OR_EQUAL, "NormalizedHealth", 0.333f)),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                            new WaitForSecondsAction(2.0f),
                            new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(55.0f, 0), false),
                                                     0.75f, true, true),
                            new SetPropertyAction<int, MotherShip>("CurrentAttack", 1, false),
                            new CallMethodAction<MotherShip>("StartLaser", false),
                                  new WaitForSecondsAction(0.85f),
                                   new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(75.0f, 0), false),
                                                     0.25f, true, true),

                            new WaitForSecondsAction(5.0f),
                            new CallMethodAction<MotherShip>("StopLaser", false),
                            new WaitForSecondsAction(3.0f),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                            new GotoStateAction<float, MotherShip>("Boss_Phase_2_Idle"),
                        }),
                            #endregion

#region PHASE 3
                        new State("Boss_Phase_3_Intro", new StateAction[]
                        {
                            new CallMethodAction<MotherShip>("StopLaser", false),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                            new WaitForSecondsAction(0.5f),
                            new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(55.0f, 0), false),
                                                     0.75f, true, true),
                            new SetPropertyAction<int, MotherShip>("CurrentAttack", 2, false),
                            new CallMethodAction<MotherShip>("StartLaser", false),

                            new WaitForSecondsAction(0.25f),
                                   new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(75.0f, 0), false),
                                                     0.25f, true, true),
                            new WaitForSecondsAction(3.0f),
                            new CallMethodAction<MotherShip>("StopLaser", false),
                            new WaitForSecondsAction(0.5f),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                             new GotoStateAction<float, MotherShip>("Boss_Phase_3_Idle"),
                        }),

                        new State("Boss_Phase_3_Idle", new StateAction[]
                        {
                             new GotoStateAction<float, MotherShip>(new WeightedObject<string>[] {
                                                                   new WeightedObject<string>("Boss_Phase_3_Idle_1", 5),
                                                                   new WeightedObject<string>("Boss_Phase_3_Idle_2", 5),
                            }),
                        }),

                            new State("Boss_Phase_3_Attack_Select", new StateAction[]
                        {
                             new GotoStateAction<float, MotherShip>(new WeightedObject<string>[] {
                                                                   new WeightedObject<string>("Boss_Phase_3_Laser_Three", 5),
                            }),
                        }),

                        new State("Boss_Phase_3_Laser_Three", new StateAction[]
                        {
                            new CallMethodAction<MotherShip>("StopLaser", false),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                            new WaitForSecondsAction(0.5f),
                            new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(55.0f, 0), false),
                                                     0.75f, true, true),
                            new SetPropertyAction<int, MotherShip>("CurrentAttack", 2, false),
                            new CallMethodAction<MotherShip>("StartLaser", false),

                            new WaitForSecondsAction(0.25f),
                                   new MoveToPositionAction(new Waypoint(new Vector2(0, 0f), true),
                                                     new Waypoint(new Vector2(75.0f, 0), false),
                                                     0.25f, true, true),

                                new WaitForSecondsAction(1.0f),

                                new SpawnPatternAction(-1, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.1f, 3.5f, "Boss_Spiral_3", "Bullet_NMY", 500, 1.0f, 0.015f, new Waypoint(new Vector2(-3, -13), true), Vector2.up, false, 62.5f, 0),
                                new PatternInstance(true, 1, 0.0f, 0.1f, 3.5f, "Boss_Spiral_3_Rev", "Bullet_NMY", 500, 1.0f, 0.015f, new Waypoint(new Vector2(-3, 13), true), Vector2.up, false, -62.5f, 0),
                            }, true, false),
                            new WaitForSecondsAction(3.0f),
                            new CallMethodAction<MotherShip>("StopLaser", false),
                            new WaitForSecondsAction(0.5f),
                            new SetPropertyAction<bool, MotherShip>("followPlayer", false, false),

                             new GotoStateAction<float, MotherShip>("Boss_Phase_3_Idle"),
                        }),

                        new State("Boss_Phase_3_Idle_1", new StateAction[]
                        {
                             new SetPropertyAction<bool, MotherShip>("followPlayer", true, false),

                            new WaitForSecondsAction(1.0f),

                                new SpawnPatternAction(-1, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.055f, 3.5f, "Boss_Spiral_2", "Bullet_NMY", 500, 1.0f, 0.015f, new Waypoint(new Vector2(-3, -13), true), Vector2.up, false, 62.5f, 0),
                                new PatternInstance(true, 1, 0.0f, 0.055f, 3.5f, "Boss_Spiral_2_Rev", "Bullet_NMY", 500, 1.0f, 0.015f, new Waypoint(new Vector2(-3, 13), true), Vector2.up, false, -62.5f, 0),
                            }, true, false),


                            new SpawnPatternAction(8, new PatternInstance[] {
                                new PatternInstance(true, 2, 0.0f, 2.25f, 3.5f, "Boss_Attack_Laser_2_Rotated", "Bullet_Small", 600, 1.5f, 0.025f, new Waypoint(new Vector2(-3, -13), true), new Vector2(-1, 1), true, 0, 0.85f),
                                new PatternInstance(true, 2, 0.0f, 2.25f, 3.5f, "Boss_Attack_Laser_2_Rotated", "Bullet_Small", 600, 1.5f, 0.025f, new Waypoint(new Vector2(-3, 13), true), new Vector2(-1, 1), true, -0, 0.85f),
                            }, true, true),

                            new WaitForSecondsAction(1.0f),

                            new GotoStateAction<float, MotherShip>("Boss_Phase_3_Attack_Select"),
                        }),


                        new State("Boss_Phase_3_Idle_2", new StateAction[]
                        {
                             new SetPropertyAction<bool, MotherShip>("followPlayer", true, false),

                            new WaitForSecondsAction(1.0f),

                                new SpawnPatternAction(-1, new PatternInstance[] {
                                new PatternInstance(true, 1, 0.0f, 0.3f, 3.5f, "Boss_Spiral_3", "Bullet_NMY", 500, 1.0f, 0.0015f, new Waypoint(new Vector2(-3, -13), true), Vector2.up, false, 62.5f, 0),
                                new PatternInstance(true, 1, 0.0f, 0.3f, 3.5f, "Boss_Spiral_3_Rev", "Bullet_NMY", 500, 1.0f, 0.015f, new Waypoint(new Vector2(-3, 13), true), Vector2.up, false, -62.5f, 0),
                            }, true, false),


                            new SpawnPatternAction(8, new PatternInstance[] {
                                new PatternInstance(true, 2, 0.0f, 0.75f, 3.5f, "Boss_Attack_Laser_3_Rotated", "Bullet_Small", 600, 1.5f, 0.0025f, new Waypoint(new Vector2(-3, -13), true), new Vector2(0, 1), false, 40, 0),
                                new PatternInstance(true, 2, 0.0f, 0.75f, 3.5f, "Boss_Attack_Laser_3_Rotated", "Bullet_Small", 600, 1.5f, 0.0025f, new Waypoint(new Vector2(-3, 13), true), new Vector2(0, 1), false, -40, 0),
                            }, true, true),

                            new WaitForSecondsAction(1.0f),

                            new GotoStateAction<float, MotherShip>("Boss_Phase_3_Attack_Select"),
                        }),

                            #endregion

                        };
                            break;
                    }
                    stateMachine = new StateMachine(states);
                    stateMachine.LoadStateMachine(typeof(MotherShip));
                    break;
            }

            return stateMachine;
        }

        public override IPoolable GetNew()
        {
            return _ref.Clone();
        }
    }
}
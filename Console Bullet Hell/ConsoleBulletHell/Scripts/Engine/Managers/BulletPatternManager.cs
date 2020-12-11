using System;
using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class BulletPatternManager
    {
        private Dictionary<string, BulletPattern> _patterns;

        public BulletPatternManager()
        {
            _patterns = new Dictionary<string, BulletPattern>();

            _patterns.Add("Player_Main", new BulletPattern(
                1,                                          //LAYERS
                new int[] { 5 },                            //BULLET COUNTS
                new float[] { 65.0f },                      //ARCS
                new float[][] { new float[] { 0 } },        //RADII
                new bool[][] { new bool[] { true } },       //ACTIVE
                new float[][] { new float[] { 0.0f } },     //ANGLE PER BULLET
                new float[][] { new float[] { 80.0f } },    //INIT SPEED
                new float[][] { new float[] { 90.0f } },    //TARGET SPEED
                new float[][] { new float[] { 0.0f } }      //LAYER OFFSET
                ));

            _patterns.Add("Player_Special", new BulletPattern(
              1,                                          //LAYERS
              new int[] { 3 },                            //BULLET COUNTS
              new float[] { 15.0f },                      //ARCS
              new float[][] { new float[] { 0 } },        //RADII
              new bool[][] { new bool[] { true } },       //ACTIVE
              new float[][] { new float[] { 0.0f } },     //ANGLE PER BULLET
              new float[][] { new float[] { 85.0f } },    //INIT SPEED
              new float[][] { new float[] { 95.0f } },    //TARGET SPEED
              new float[][] { new float[] { 0.5f } }      //LAYER OFFSET
              ));

            _patterns.Add("Player_Special_Alt", new BulletPattern(
             1,                                            //LAYERS
            new int[] { 3 },                        //POINTS
            new int[] { 10 },                    //BULLET COUNTS
            new float[] { 360 },                       //ARCS
            new float[][] { new float[] { 3.5f } },       //RADII
            new bool[][] { new bool[] { true } },         //ACTIVE
            new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
            new float[][] { new float[] { 35.0f } },      //INIT SPEED
            new float[][] { new float[] { 35f } },      //TARGET SPEED
            new float[][] { new float[] { 0.0f } },        //LAYER OFFSET
            true,
            new float[] { 0 }
            ));

            _patterns.Add("Player_Alt", new BulletPattern(
              1,                                            //LAYERS
              new int[] { 5 },                              //BULLET COUNTS
              new float[] { 5.0f },                         //ARCS
              new float[][] { new float[] { 0 } },          //RADII
              new bool[][] { new bool[] { true } },         //ACTIVE
              new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
              new float[][] { new float[] { 90.0f } },      //INIT SPEED
              new float[][] { new float[] { 120.0f } },     //TARGET SPEED
              new float[][] { new float[] { 0.0f } }        //LAYER OFFSET
              ));

            _patterns.Add("Enemy_Attack_0", new BulletPattern(
               3,                                            //LAYERS
              new int[] { 1 },                              //BULLET COUNTS
              new float[] { 0.0f },                         //ARCS
              new float[][] { new float[] { 0 } },          //RADII
              new bool[][] { new bool[] { true } },         //ACTIVE
              new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
              new float[][] { new float[] { 25.0f } },      //INIT SPEED
              new float[][] { new float[] { 40.0f } },     //TARGET SPEED
              new float[][] { new float[] { 0.0f } }        //LAYER OFFSET
              ));

            _patterns.Add("Enemy_Attack_1", new BulletPattern(
            1,                                            //LAYERS
            new int[] { 5 },                                //POINTS
            new int[] { 5 },                    //BULLET COUNTS
            new float[] { 360 },                       //ARCS
            new float[][] { new float[] { 1.0f/*, 0.75f, 1.5f, 2.25f, 3.0f, 3.5f*/ } },       //RADII
            new bool[][] { new bool[] { true } },         //ACTIVE
            new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
            new float[][] { new float[] { 15.0f } },      //INIT SPEED
            new float[][] { new float[] { 15f } },      //TARGET SPEED
            new float[][] { new float[] { 0.0f } },        //LAYER OFFSET
            true
            ));

            _patterns.Add("Enemy_Attack_2", new BulletPattern(
         10,                                            //LAYERS
        new int[] { 5 },                              //BULLET COUNTS
        new float[] { 360.0f },                         //ARCS
        new float[][] { new float[] { 0 } },          //RADII
        new bool[][] { new bool[] { true } },         //ACTIVE
        new float[][] { new float[] { 5 } },       //ANGLE PER BULLET
        new float[][] { new float[] { 60.0f } },      //INIT SPEED
        new float[][] { new float[] { 65.0f } },     //TARGET SPEED
        new float[][] { new float[] { 0.1f } }        //LAYER OFFSET
        ));


            _patterns.Add("Enemy_Attack_2_Rev", new BulletPattern(
 10,                                            //LAYERS
new int[] { 5 },                              //BULLET COUNTS
new float[] { 360.0f },                         //ARCS
new float[][] { new float[] { 0 } },          //RADII
new bool[][] { new bool[] { true } },         //ACTIVE
new float[][] { new float[] { -5 } },       //ANGLE PER BULLET
new float[][] { new float[] { 60.0f } },      //INIT SPEED
new float[][] { new float[] { 65.0f } },     //TARGET SPEED
new float[][] { new float[] { 0.1f } }        //LAYER OFFSET
));

            #region BOSS_EASY

            _patterns.Add("Boss_Spiral_1", new BulletPattern(
            1,                                          //LAYERS
           new int[] { 5 },                              //BULLET COUNTS
           new float[] { 360.0f },                         //ARCS
           new float[][] { new float[] { 0 } },          //RADII
           new bool[][] { new bool[] { true } },         //ACTIVE
           new float[][] { new float[] { 0 } },       //ANGLE PER BULLET
           new float[][] { new float[] { 40.0f } },      //INIT SPEED
           new float[][] { new float[] { 40.0f } },     //TARGET SPEED
           new float[][] { new float[] { 0 } }        //LAYER OFFSET
           ));

            _patterns.Add("Boss_Spiral_1_Rev", new BulletPattern(
          1,                                          //LAYERS
         new int[] { 5 },                              //BULLET COUNTS
         new float[] { 360.0f },                         //ARCS
         new float[][] { new float[] { 0 } },          //RADII
         new bool[][] { new bool[] { true } },         //ACTIVE
         new float[][] { new float[] { -0.0f } },       //ANGLE PER BULLET
         new float[][] { new float[] { 40.0f } },      //INIT SPEED
         new float[][] { new float[] { 40.0f } },     //TARGET SPEED
         new float[][] { new float[] { 0 } }        //LAYER OFFSET
         ));


            _patterns.Add("Boss_Spiral_2", new BulletPattern(
       1,                                          //LAYERS
      new int[] { 3 },                              //POINT COUNTS
      new int[] { 2 },                              //BULLET COUNTS
      new float[] { 360.0f },                         //ARCS
      new float[][] { new float[] { 0 } },          //RADII
      new bool[][] { new bool[] { true } },         //ACTIVE
      new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
      new float[][] { new float[] { 40 } },      //INIT SPEED
      new float[][] { new float[] { 40 } },     //TARGET SPEED
      new float[][] { new float[] { 0 } }        //LAYER OFFSET
      ));

            _patterns.Add("Boss_Spiral_2_Rev", new BulletPattern(
          1,                                          //LAYERS
         new int[] { 3 },                              //POINT COUNTS
         new int[] { 2 },                             //BULLET COUNTS
         new float[] { 360.0f },                         //ARCS
         new float[][] { new float[] { 0 } },          //RADII
         new bool[][] { new bool[] { true } },         //ACTIVE
         new float[][] { new float[] { -0.0f } },       //ANGLE PER BULLET
         new float[][] { new float[] { 40 } },      //INIT SPEED
         new float[][] { new float[] { 40 } },     //TARGET SPEED
         new float[][] { new float[] { 0 } }        //LAYER OFFSET
         ));

            _patterns.Add("Boss_Spiral_3", new BulletPattern(
1,                                          //LAYERS
new int[] { 8 },                              //POINT COUNTS
new int[] { 4 },                              //BULLET COUNTS
new float[] { 360.0f },                         //ARCS
new float[][] { new float[] { 0 } },          //RADII
new bool[][] { new bool[] { true } },         //ACTIVE
new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
new float[][] { new float[] { 50 } },      //INIT SPEED
new float[][] { new float[] { 50 } },     //TARGET SPEED
new float[][] { new float[] { 0 } }        //LAYER OFFSET
));

            _patterns.Add("Boss_Spiral_3_Rev", new BulletPattern(
          1,                                          //LAYERS
         new int[] { 8 },                              //POINT COUNTS
         new int[] { 4 },                             //BULLET COUNTS
         new float[] { 360.0f },                         //ARCS
         new float[][] { new float[] { 0 } },          //RADII
         new bool[][] { new bool[] { true } },         //ACTIVE
         new float[][] { new float[] { -0.0f } },       //ANGLE PER BULLET
         new float[][] { new float[] { 50 } },      //INIT SPEED
         new float[][] { new float[] { 50 } },     //TARGET SPEED
         new float[][] { new float[] { 0 } }        //LAYER OFFSET
         ));


            _patterns.Add("Boss_Laser_Up_Easy", new BulletPattern(
               4,                                          //LAYERS
              new int[] { 4 },                             //BULLET COUNTS
              new float[] { 85.0f },                         //ARCS
              new float[][] { new float[] { 0 } },          //RADII
              new bool[][] { new bool[] { true } },         //ACTIVE
              new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
              new float[][] { new float[] { 40.0f } },      //INIT SPEED
              new float[][] { new float[] { 50.0f } },     //TARGET SPEED
              new float[][] { new float[] { 0.05f } }        //LAYER OFFSET
              ));

            _patterns.Add("Boss_Laser_Up_Easy_Rev", new BulletPattern(
               4,                                            //LAYERS
              new int[] { 4 },                              //BULLET COUNTS
              new float[] { 85.0f },                         //ARCS
              new float[][] { new float[] { 0 } },          //RADII
              new bool[][] { new bool[] { true } },         //ACTIVE
              new float[][] { new float[] { -0.0f } },       //ANGLE PER BULLET
              new float[][] { new float[] { 40.0f } },      //INIT SPEED
              new float[][] { new float[] { 50.0f } },     //TARGET SPEED
              new float[][] { new float[] { 0.05f } }        //LAYER OFFSET
              ));

            _patterns.Add("Boss_Attack_Laser_2", new BulletPattern(
            1,                                            //LAYERS
            new int[] { 3 },                        //POINTS
            new int[] { 12 },                    //BULLET COUNTS
            new float[] { 360 },                       //ARCS
            new float[][] { new float[] { 4.0f } },       //RADII
            new bool[][] { new bool[] { true } },         //ACTIVE
            new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
            new float[][] { new float[] { 25.0f } },      //INIT SPEED
            new float[][] { new float[] { 25f } },      //TARGET SPEED
            new float[][] { new float[] { 1.0f } },        //LAYER OFFSET
            true,
            new float[] { 0 }
            ));

            _patterns.Add("Boss_Attack_Laser_2_Rotated", new BulletPattern(
          3,                                            //LAYERS
          new int[] { 3 },                        //POINTS
          new int[] { 8 },                    //BULLET COUNTS
          new float[] { 360 },                       //ARCS
          new float[][] { new float[] { 0.05f } },       //RADII
          new bool[][] { new bool[] { true } },         //ACTIVE
          new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
          new float[][] { new float[] { 15.0f } },      //INIT SPEED
          new float[][] { new float[] { 30f } },      //TARGET SPEED
          new float[][] { new float[] { 0.015f } },        //LAYER OFFSET
          true,
          new float[] { 0, 11.25f, 22.5f, 45f, 56.25f, 67.5f, 78.75f, 90f }
          ));

            _patterns.Add("Boss_Attack_Laser_3_Rotated", new BulletPattern(
      2,                                            //LAYERS
      new int[] { 4 },                        //POINTS
      new int[] { 12 },                    //BULLET COUNTS
      new float[] { 360 },                       //ARCS
      new float[][] { new float[] { 0.05f } },       //RADII
      new bool[][] { new bool[] { true } },         //ACTIVE
      new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
      new float[][] { new float[] { 15.0f } },      //INIT SPEED
      new float[][] { new float[] { 50f } },      //TARGET SPEED
      new float[][] { new float[] { 0.05f } },        //LAYER OFFSET
      true,
      new float[] { 0, 45, 22.5f, 45f, 56.25f, 67.5f, 78.75f, 90f }
      ));

            #region BOSS_CIRCLE_1_EASY
            _patterns.Add("Boss_Attack_Circle_1_Easy", new BulletPattern(
             13,                                            //LAYERS
             new int[] { 32, 4, 4, 4, 4, 4, 32, 4, 4, 4, 4, 4, 32 },                              //BULLET COUNTS
             new float[] { 360.0f },                       //ARCS
             new float[][] { new float[] { 2.0f } },       //RADII
             new bool[][] { new bool[] { true } },         //ACTIVE
             new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
             new float[][] { new float[] { 10.0f } },      //INIT SPEED
             new float[][] { new float[] { 20.0f } },      //TARGET SPEED
             new float[][] { new float[] { 0.0f } }        //LAYER OFFSET
             ));

            _patterns.Add("Boss_Attack_Circle_1_Easy_Wave", new BulletPattern(
             8,                                            //LAYERS
             new int[] { 16, 3, 3, 3, 16, 3, 3, 16 },                              //BULLET COUNTS
             new float[] { 70 },                        //ARCS
             new float[][] { new float[] { 0.50f } },       //RADII
             new bool[][] { new bool[] { true } },         //ACTIVE
             new float[][] { new float[] { -0 } },       //ANGLE PER BULLET
             new float[][] { new float[] { 5f } },      //INIT SPEED
             new float[][] { new float[] { 7.25f } },      //TARGET SPEED
             new float[][] { new float[] { 0f } }        //LAYER OFFSET
             ));

            #endregion

            #region BOSS_ATTACK_2_EASY

            _patterns.Add("Boss_Attack_2_Easy", new BulletPattern(
             8,                                            //LAYERS
             new int[] { 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8 },                        //POINTS
             new int[] { 6, 6, 6, 6, 6, 6 },                    //BULLET COUNTS
             new float[] { 360 },                       //ARCS
             new float[][] { new float[] { 0.0f } },       //RADII
             new bool[][] { new bool[] { true } },         //ACTIVE
             new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
             new float[][] { new float[] { 5.0f } },      //INIT SPEED
             new float[][] { new float[] { 15f } },      //TARGET SPEED
             new float[][] { new float[] { 0.0f } },        //LAYER OFFSET
             true
             ));

            #endregion

            #endregion

            #region BOSS_CIRCLE_1_MEDIUM
            _patterns.Add("Boss_Attack_Circle_1_Medium", new BulletPattern(
            13,                                            //LAYERS
            new int[] { 64, 8, 8, 8, 8, 8, 64, 8, 8, 8, 8, 8, 64 },                              //BULLET COUNTS
            new float[] { 360.0f },                       //ARCS
            new float[][] { new float[] { 2.0f } },       //RADII
            new bool[][] { new bool[] { true } },         //ACTIVE
            new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
            new float[][] { new float[] { 10.0f } },      //INIT SPEED
            new float[][] { new float[] { 20.0f } },      //TARGET SPEED
            new float[][] { new float[] { 0.0f } }        //LAYER OFFSET
            ));

            _patterns.Add("Boss_Attack_Circle_1_Medium_Wave", new BulletPattern(
             8,                                            //LAYERS
             new int[] { 26, 7, 7, 7, 26, 7, 7, 26 },                              //BULLET COUNTS
             new float[] { 65 },                        //ARCS
             new float[][] { new float[] { 0.50f } },       //RADII
             new bool[][] { new bool[] { true } },         //ACTIVE
             new float[][] { new float[] { -0 } },       //ANGLE PER BULLET
             new float[][] { new float[] { 5f } },      //INIT SPEED
             new float[][] { new float[] { 7.25f } },      //TARGET SPEED
             new float[][] { new float[] { 0f } }        //LAYER OFFSET
             ));
            #endregion

            _patterns.Add("Boss_Attack_Circle_1_Hard", new BulletPattern(
             8,                                            //LAYERS
             new int[] { 24 },                             //BULLET COUNTS
             new float[] { 360.0f },                       //ARCS
             new float[][] { new float[] { 2.0f } },       //RADII
             new bool[][] { new bool[] { true } },         //ACTIVE
             new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
             new float[][] { new float[] { 20.0f } },      //INIT SPEED
             new float[][] { new float[] { 25.0f } },      //TARGET SPEED
             new float[][] { new float[] { 0.0f } }        //LAYER OFFSET
             ));



            _patterns.Add("Default_Circle_X16_4", new BulletPattern(
             4,                                            //LAYERS
             new int[] { 16 },                             //BULLET COUNTS
             new float[] { 360.0f },                       //ARCS
             new float[][] { new float[] { 5.0f } },       //RADII
             new bool[][] { new bool[] { true,             //ACTIVE
                                         false,
                                         true,
                                         false} },
             new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
             new float[][] { new float[] { 20.0f } },      //INIT SPEED
             new float[][] { new float[] { 40.0f } },      //TARGET SPEED
             new float[][] { new float[] { 0.0f } }        //LAYER OFFSET
             ));

            _patterns.Add("Default_Circle_X6_8_1_TGT", new BulletPattern(
             8,                                             //LAYERS
             new int[] {       32,     32,    24,    24,    //BULLET COUNTS
                               16,       16,     8,    8 },
             new float[] { 45, 45, 45, 45,   //ARCS
                           45 , 45,  45,  45},

             new float[][] { new float[] { 4.0f } },       //RADII
             new bool[][] { new bool[] { true,             //ACTIVE
                                         false,
                                         false } },
             new float[][] { new float[] { 0.0f } },       //ANGLE PER BULLET
             new float[][] { new float[] { 20.0f } },      //INIT SPEED
             new float[][] { new float[] { 80.0f } },      //TARGET SPEED
             new float[][] { new float[] { 0.05f } }       //LAYER OFFSET
             ));
        }

        public bool TryGetPattern(string patternName, out BulletPattern patten)
        {
            return _patterns.TryGetValue(patternName, out patten);
        }
    }
}
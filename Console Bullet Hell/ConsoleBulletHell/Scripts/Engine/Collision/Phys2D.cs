namespace Joonaxii.ConsoleBulletHell
{
    public static class Phys2D
    {
        private static int[] _matrix = new int[14];

        static Phys2D()
        {
            _matrix[0] = 0;
            _matrix[1] = 1 << (int)EntityType.BULLET_NMY | 1 << (int)EntityType.BULLET_WRLD_NMY;
            _matrix[2] = 1 << (int)EntityType.BULLET_PLR | 1 << (int)EntityType.BULLET_WRLD_PLR;
            _matrix[3] = 1 << (int)EntityType.PLAYER | 1 << (int)EntityType.WORLD | 1 << (int)EntityType.BULLET_PLR;
            _matrix[4] = 1 << (int)EntityType.WORLD | 1 << (int)EntityType.BULLET_WRLD_PLR | 1 << (int)EntityType.BULLET_WRLD_NMY;

            _matrix[5] = 1 << (int)EntityType.ENEMY;
            _matrix[6] = 1 << (int)EntityType.PLAYER;

            _matrix[7] = 1 << (int)EntityType.ENEMY | 1 << (int)EntityType.WORLD;
            _matrix[8] = 1 << (int)EntityType.PLAYER | 1 << (int)EntityType.WORLD;
        }

        public static bool MaskHasLayer(EntityType mask, EntityType layer)
        {
            int num = 1 << (int)layer;
            return (_matrix[(int)mask] & num) == num;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class BulletPattern
    {
        private Vector2[][] _positions;
        private Vector2[][] _directions;

        private float[][] _speeds;
        private bool[][] _bulletEnabled;

        private int _layers;
        private int[] _bulletsPerLayer;
        private int _bulletCount;

        public BulletPattern(int layers, int[] bulletCounts, float[] arcs, float[][] radii, bool[][] activeBullets, float[][] anglePerBullet, float[][] baseSpds, float[][] tgtSpeeds, float[][] layerOffset, float[] angleOffset = null)
        {
            if(angleOffset == null|| angleOffset.Length < 1)
            {
                angleOffset = new float[] { 0 };
            }

            _layers = Math.Max(layers, 1);
            _bulletsPerLayer = bulletCounts;

            int l = _bulletsPerLayer.Length;
            if (l < layers)
            {
                Array.Resize(ref _bulletsPerLayer, layers);
                for (int i = l; i < layers; i++)
                {
                    _bulletsPerLayer[i] = _bulletsPerLayer[l - 1];
                }
            }

            float layerL = _layers < 3 ? 1.0f : _layers - 1.0f;
            _bulletEnabled = activeBullets;

            _positions = new Vector2[_layers][];
            _directions = new Vector2[_layers][];
            _speeds = new float[_layers][];
            _bulletCount = 0;

            bool[] enabled;
            for (int i = 0; i < _layers; i++)
            {
                enabled = _bulletEnabled[i % _bulletEnabled.Length];
                float ln = i / layerL;

                int bulletCount = _bulletsPerLayer[i];

                float arc = arcs[i % arcs.Length];
                float halfArc = arc == 360 ? 0 : arc * 0.5f;
                float anglePerB = arc / bulletCount;

                Vector2 dir = new Vector2(0, 1).Rotate(halfArc - (arc == 360 ? 0 : anglePerB * 0.5f) + angleOffset[i % angleOffset.Length]);

                int bulletCountOne = bulletCount;

                _positions[i] = new Vector2[bulletCountOne];
                _directions[i] = new Vector2[bulletCountOne];
                _speeds[i] = new float[bulletCountOne];
                float[] _radii = radii[i % radii.Length];
                float[] _spds = baseSpds[i % baseSpds.Length];
                float[] _tgtSpds = tgtSpeeds[i % tgtSpeeds.Length];
                float[] _lrOffsets = layerOffset[i % layerOffset.Length];
                float[] _angles = anglePerBullet[i % anglePerBullet.Length];

                for (int j = 0; j < bulletCountOne; j++)
                {
                    if (enabled[j % enabled.Length])
                    {
                        _bulletCount++;
                    }

                    float offset = _lrOffsets[j % _lrOffsets.Length];
                    float spd = _spds[j % _spds.Length];
                    float tgtSpd = _tgtSpds[j % _tgtSpds.Length];

                    _directions[i][j] = dir.Rotate(-anglePerB * j + (_angles[j % _angles.Length] * i));
                    _positions[i][j] = _directions[i][j] * (_radii[j % _radii.Length] + Maths.Lerp(0.0f, offset, ln));
                    _speeds[i][j] = Maths.Lerp(spd, tgtSpd, ln);
                }
            }
        }

        public BulletPattern(int layers, int[] points, int[] bulletCounts, float[] arcs, float[][] radii, bool[][] activeBullets, float[][] anglePerBullet, float[][] baseSpds, float[][] tgtSpeeds, float[][] layerOffset, bool close = true, float[] angleOffset = null)
        {
            if (angleOffset == null || angleOffset.Length < 1)
            {
                angleOffset = new float[] { 0 };
            }

            _layers = Math.Max(layers, 1);
            _bulletCount = 0;
            if (points == null | points.Length < 1)
            {
                points = new int[1] { 2 };
            }
            else
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = Math.Max(points[i], 2);
                }
            }
            _bulletEnabled = activeBullets;

            _positions = new Vector2[_layers][];
            _directions = new Vector2[_layers][];
            _speeds = new float[_layers][];

            _bulletsPerLayer = new int[_layers];
            float layerL = _layers < 3 ? 1.0f : _layers - 1.0f;

            _layers = Math.Max(layers, 1);
            for (int i = 0; i < _layers; i++)
            {
                float ln = i / layerL;

                int pointCount = points[i % points.Length];

                float arc = arcs[i % arcs.Length];
                float halfArc = arc == 360 ? 0 : arc * 0.5f;
                float anglePerB = arc / pointCount;

                Vector2 dir = new Vector2(0, 1).Rotate(halfArc - (arc == 360 ? 0 : anglePerB * 0.5f) + angleOffset[i % angleOffset.Length]);

                Vector2[] verts = new Vector2[pointCount];
                Vector2[] vertDirs = new Vector2[pointCount];
                float[] vertSpeeds = new float[pointCount];

                float[] _radii = radii[i % radii.Length];
                float[] _spds = baseSpds[i % baseSpds.Length];
                float[] _tgtSpds = tgtSpeeds[i % tgtSpeeds.Length];
                float[] _lrOffsets = layerOffset[i % layerOffset.Length];
                float[] _angles = anglePerBullet[i % anglePerBullet.Length];

                for (int j = 0; j < pointCount; j++)
                {
                    float offset = _lrOffsets[j % _lrOffsets.Length];
                    float spd = _spds[j % _spds.Length];
                    float tgtSpd = _tgtSpds[j % _tgtSpds.Length];

                    Vector2 point = dir.Rotate(-anglePerB * j + (_angles[j % _angles.Length] * i));

                    vertDirs[j] = point;
                    verts[j] = point * (_radii[j % _radii.Length] + Maths.Lerp(0.0f, offset, ln));
                    vertSpeeds[j] = Maths.Lerp(spd, tgtSpd, ln);
                }

                int bulletCountsLayer = bulletCounts[i % bulletCounts.Length];
                int ii = 0;
                for (int j = 0; j < pointCount; j++)
                {
                    ii++;
                    if (j == 1 & (pointCount == 2 | !close)) { break; }

                    for (int k = 1; k < bulletCountsLayer; k++)
                    {
                        ii++;
                    }
                }
                _bulletsPerLayer[i] = ii;

                _positions[i] = new Vector2[ii];
                _directions[i] = new Vector2[ii];
                _speeds[i] = new float[ii];

                ii = 0;
                for (int j = 0; j < pointCount; j++)
                {
                    _directions[i][ii] = vertDirs[j];
                    _positions[i][ii] = verts[j];
                    _speeds[i][ii] = vertSpeeds[j];

                    ii++;

                    if(j == 1 & (pointCount == 2 | !close)) { break; }
                    int nextI = j == pointCount - 1 ? 0 : j + 1;
                    for (int k = 1; k < bulletCountsLayer; k++)
                    {
                        float n = k / (float)bulletCountsLayer;

                        _directions[i][ii] = Vector2.Lerp(vertDirs[j], vertDirs[nextI], n);
                        _positions[i][ii] = Vector2.Lerp(verts[j], verts[nextI], n);
                        _speeds[i][ii] = Maths.Lerp(vertSpeeds[j], vertSpeeds[nextI], n);

                        ii++;
                    }
                }
                _bulletCount += ii;
            }
        }

        public void GetControllableBullets(Entity owner, int damage, int order, float bulletSize, float lifeTime, BulletPool pool, out Bullet[] bullets, out Vector2[] positions, out Vector2[] directions)
        {
            positions = new Vector2[_bulletCount];
            directions = new Vector2[_bulletCount];

            bullets = new Bullet[_bulletCount];

            bool[] enabled;
            int ii = 0;
            for (int i = 0; i < _layers; i++)
            {
                enabled = _bulletEnabled[i % _bulletEnabled.Length];
                ref float[] spds = ref _speeds[i];
                ref Vector2[] poss = ref _positions[i];
                ref Vector2[] dirss = ref _directions[i];

                for (int j = 0; j < _bulletsPerLayer[i]; j++)
                {
                    if (!enabled[j % enabled.Length]) { continue; }

                    Bullet bullet = bullets[ii] = pool.GetNew() as Bullet;

                    Vector2 pos = poss[j];
                    Vector2 dir = dirss[j];

                    positions[ii] = pos;
                    directions[ii] = dir;

                    pos.y *= 0.6725f;
                    dir.y *= 0.6725f;

                    bullet.SetupBullet(owner, damage, bulletSize, spds[j], lifeTime, order - i, true);
                    bullet.Spawn(pos, dir);
                    ii++;
                }
            }
        }

        public void SpawnBullets<T>(Entity source, bool correctY, Vector2 position, Vector2 direction, BulletPool pool, int damage, float bulletSize, float speedMultiplier, float lifeTime, int order, float alingToDirection = 0) where T : Bullet
        {
            float dirAngle = Vector2.up.SignedAngle(direction);
            bool[] enabled;
            for (int i = 0; i < _layers; i++)
            {
                enabled = _bulletEnabled[i % _bulletEnabled.Length];
                ref float[] spds = ref _speeds[i];
                ref Vector2[] poss = ref _positions[i];
                ref Vector2[] dirss = ref _directions[i];

                float angle = -dirAngle;
                for (int j = 0; j < _bulletsPerLayer[i]; j++)
                {
                    if (!enabled[j % enabled.Length]) { continue; }

                    Bullet bullet = pool.Get() as Bullet;

                    Vector2 pos = poss[j].Rotate(angle);
                    Vector2 dir = dirss[j].Rotate(angle);

                    if (correctY)
                    {
                        pos.y *= 0.6725f;
                        dir.y *= 0.6725f;
                    }

                    bullet.SetupBullet(source, damage, bulletSize, spds[j] * speedMultiplier, lifeTime, order - i);
                    bullet.Spawn(position + pos, alingToDirection == 0 ? dir : alingToDirection == 1.0f ? direction : Vector2.Lerp(dir, direction, alingToDirection));
                }
            }
        }
    }
}
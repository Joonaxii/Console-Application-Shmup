using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class EntityManager
    {
        private List<Entity> _entities;
        private List<Entity> _updateableEntities;

        public EntityManager()
        {
            _entities = new List<Entity>(4096);
            _updateableEntities = new List<Entity>(4096);
        }

        public void Update(float delta)
        {
            for (int i = 0; i < _updateableEntities.Count; i++)
            {
                if (!_updateableEntities[i].IsAlive) { continue; }
                _updateableEntities[i].Update(delta);
            }
        }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
            if (entity.AutoUpdate)
            {
                _updateableEntities.Add(entity);
            }
        }

        public Entity GetEntity(int id)
        {
            return _entities[id];
        }

        public void Unload()
        {
            for (int i = 0; i < _entities.Count; i++)
            {
                _entities[i].Despawn(true);
            }
        }
    }
}
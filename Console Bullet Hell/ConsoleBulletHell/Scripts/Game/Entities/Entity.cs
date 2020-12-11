using System;
using System.Collections;
using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public class Entity : ICollideable, IPoolable
    {
        public static int CURRENT_ID { get; set; }
        public int ID { get; private set; }

        public virtual int Damage { get; protected set; }

        public virtual int CurrentHealth { get; set; }
        public int MaxHealth { get; protected set; }
        public float NormalizedHealth => CurrentHealth / Math.Max(MaxHealth, 1.0f);

        public EntityType Type;
        public EntityID EntityID;

        public Vector2 Position;
        public Vector2 Direction;

        public Sprite Sprite;
        public Animation Animation;

        public Collider Collider;

        public bool IsAlive { get; set; }
        public bool Render { get { return _isVisible & IsAlive; } }

        public bool CanCollide { get { return Collider != null & IsAlive & !_isOutOfBounds & _canCollide; } set { _canCollide = value; } }

        public bool AutoUpdate { get; private set; }
        public int renderingOffset = 0;

        protected float _time;
        protected float _timeAnim;
        protected int _frameAnim;
        protected int _currentAnim;

        protected bool _isImmune;
        protected bool _isVisible;
        protected bool _isOutOfBounds;
        protected bool _canCollide;

        protected ObjectPool _pool;

        public List<int> Nodes = new List<int>(8);
        private List<int> _collisions = new List<int>(48);
        protected List<IEnumerator> _coroutines = new List<IEnumerator>(16);

        public Entity(EntityID id, EntityType type, int damage, int maxHP, bool autoUpdate, int order, Sprite sprt, Animation anim, Vector2 colliderOffset, params ColliderBase[] colliders)
        {
            MaxHealth = maxHP;
            EntityID = id;

            Damage = damage;

            renderingOffset = order;

            _isVisible = true;
            _canCollide = false;

            AutoUpdate = autoUpdate;
            Reset();

            ID = CURRENT_ID++;

            Type = type;

            Sprite = sprt;
            IsAlive = false;

            if (colliders == null || colliders.Length < 1)
            {
                colliders = null;
                Collider = null;
            }
            else
            {
                Collider = new Collider(this, colliderOffset, colliders);
            }

            Program.EntityManager.AddEntity(this);
            if (EntityID != EntityID.NONE)
            {
                Renderer.AddRenderer(this);
            }

            if (Collider != null & type != EntityType.NONE)
            {
                Program.CollisionSystem.AllCollideables.Add(Collider);
            }
            Animation = anim;
        }

        public Entity GetEntity()
        {
            return this;
        }

        public virtual void OnCollisionEnter(ICollideable other) { }
        public virtual void OnCollisionStay(ICollideable other) { }
        public virtual void OnCollisionExit(ICollideable other) { }

        public bool IsImmune()
        {
            return _isImmune;
        }

        public virtual void Reset()
        {
            _time = 0;
            _timeAnim = 0;
            _frameAnim = 0;
        }

        public virtual int TakeDamage(int damage)
        {
            if (_isImmune | damage <= 0) { return 0; }

            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                return 2;
            }
            return 1;
        }

        public void UpdateAnimation(float deltaTime)
        {
            Animation?.Animate(this, ref _timeAnim, ref _frameAnim, deltaTime);
        }

        public virtual void Update(float deltaTime)
        {
            UpdateAnimation(deltaTime);
            _time += deltaTime;

            UpdateCoroutines();
        }

        public void SetPosition(Vector2 pos)
        {
            Position = pos;
        }

        public virtual void Spawn(Vector2 pos, Vector2 dir)
        {
            Position = pos;
            Direction = dir;

            Reset();

            CurrentHealth = MaxHealth;

            if (Collider != null)
            {
                Collider.Update(Position);
            }

            _isOutOfBounds = false;
            _canCollide = true;
            _isVisible = true;
            if (Animation != null)
            {
                Sprite = Animation.Frames[0];
            }

            IsAlive = true;
        }

        private void ClearCollisions(bool slient)
        {
            for (int i = 0; i < _collisions.Count; i++)
            {
                Entity e = Program.EntityManager.GetEntity(_collisions[i]);

                if (!slient && e.HasCollision(ID))
                {
                    OnCollisionExit(e);
                    e.OnCollisionExit(this);
                }
                e.RemoveCollision(ID);
            }
            _collisions.Clear();
        }

        public IEnumerator StartCoroutine(IEnumerator enumerator)
        {
            if (enumerator == null) { return enumerator; }
            _coroutines.Add(enumerator);
            return enumerator;
        }

        public void StopCoroutine(IEnumerator enumerator)
        {
            if (enumerator == null || !_coroutines.Contains(enumerator)) { return; }
            _coroutines.Remove(enumerator);
        }

        public void StopAllCoroutines()
        {
            _coroutines.Clear();
        }

        public bool UpdateCoroutines()
        {
            for (int i = 0; i < _coroutines.Count; i++)
            {
                RecursiveRoutine(i, _coroutines[i].Current);
            }

            for (int i = _coroutines.Count - 1; i >= 0; i--)
            {
                if (_coroutines[i] == null) { _coroutines.RemoveAt(i); }
            }

            return _coroutines.Count > 0;
        }

        private void RecursiveRoutine(int i, object obj)
        {
            if (obj == null)
            {
                if (!_coroutines[i].MoveNext()) { if (_coroutines.Count <= i) { return; } _coroutines[i] = null; }
                return;
            }

            if (obj is IEnumerator routine)
            {
                if (!routine.MoveNext())
                {
                    if (_coroutines.Count <= i) { return; }
                    RecursiveRoutine(i, routine.Current);
                }
            }
        }

        public virtual void Despawn(bool silent)
        {
            IsAlive = false;
            _canCollide = false;
            ClearCollisions(silent);
            StopAllCoroutines();

            _pool?.Return(this);
        }

        public void AddCollision(int id)
        {
            _collisions.Add(id);
        }

        public void RemoveCollision(int id)
        {
            _collisions.Remove(id);
        }

        public bool HasCollision(int id)
        {
            return _collisions.Contains(id);
        }

        public void AddNode(int id)
        {
            Nodes.Add(id);
        }

        public void RemoveNode(int id)
        {
            Nodes.Remove(id);
        }

        public bool HasNode(int id)
        {
            return Nodes.Contains(id);
        }

        public bool IsOOB()
        {
            return _isOutOfBounds;
        }

        public void SetOOB(bool isOOB)
        {
            _isOutOfBounds = isOOB;
        }

        public void Create(ObjectPool pool)
        {
            _pool = pool;
        }
    }
}
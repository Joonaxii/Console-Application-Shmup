namespace Joonaxii.ConsoleBulletHell
{
    public class Laser : Entity
    {
        public LaserState State;

        public override int Damage { get => _owner != null ? base.Damage + _owner.Damage : base.Damage; protected set => base.Damage = value; }

        private LaserSegment[] _segments;
        private Vector2[] _offsets;
        private Entity _laserContact;

        private float _contactX = -65f;

        public bool stopping;
        private Entity _owner;

        public Laser(Entity owner, int order) : base(EntityID.NONE, EntityType.NONE, 5, 0, false, order, null, null, Vector2.zero, null)
        {
            _owner = owner;
            State = LaserState.InActive;
            _laserContact = new Entity(EntityID.NONE, EntityType.NONE, 0, 0, false, order + 1, null, SpriteBank.GetAnimation("Laser_Contact"), Vector2.zero, null);
            _segments = new LaserSegment[6];
            _offsets = new Vector2[6];
            for (int i = 0; i < _segments.Length; i++)
            {
                _segments[i] = new LaserSegment(this, i == 0, EntityType.ENEMY_MELEE, false, order, null, null);
                _offsets[i] = new Vector2(-31.0f * (i + 1.0f), 0);
            }
        }

        public override void OnCollisionEnter(ICollideable other)
        {
            Entity ent = other.GetEntity();
            switch (ent.Type)
            {
                case EntityType.BULLET_PLR:
                    ent.Despawn(false);
                    break;
            }
        }
        public override void OnCollisionExit(ICollideable other) {}
        public override void OnCollisionStay(ICollideable other)
        {
            Entity ent = other.GetEntity();
            switch (ent.Type)
            {
                case EntityType.PLAYER:
                    ent.TakeDamage(Damage);
                    break;
            }
        }

        public override void Despawn(bool silent)
        {
            base.Despawn(silent);
          
            stopping = false;
            _laserContact.Despawn(false);
            for (int i = 0; i < _segments.Length; i++)
            {
                _segments[i].Despawn(silent);
            }
            UpdateState();
        }

        public override void Spawn(Vector2 pos, Vector2 dir)
        {
            base.Spawn(pos, dir);
            stopping = false;
            for (int i = 0; i < _segments.Length; i++)
            {
                _segments[i].Spawn(Position + _offsets[i], Vector2.zero);
            }

            _laserContact.Spawn(new Vector2(_contactX, Position.y), Vector2.zero);
            UpdateState();
        }

        public void Stop()
        {
            if (stopping | !IsAlive) { return; }

            stopping = true;
            for (int i = 0; i < _segments.Length; i++)
            {
                _segments[i].Stop();
            }
        }

        public override void Update(float deltaTime)
        {
            if (!IsAlive) { return; }

            UpdateState();
            if (State == LaserState.InActive)
            {
                Despawn(false);
                return;
            }

            _laserContact.Update(deltaTime);
            _laserContact.SetPosition(new Vector2(_contactX, Position.y));

            if (stopping)
            {
                _laserContact.Update(deltaTime);
                _laserContact.SetPosition(new Vector2(_contactX, Position.y));

                for (int i = 0; i < _segments.Length; i++)
                {
                    _segments[i].Update(deltaTime);
                    _segments[i].SetPosition(Position + _offsets[i]);
                }
                return;
            }

            for (int i = 0; i < _segments.Length; i++)
            {
                _segments[i].SetPosition(Position + _offsets[i]);
                _segments[i].Update(deltaTime);
            }
        }

        private void UpdateState()
        {
            LaserState state = LaserState.InActive;
            for (int i = 0; i < _segments.Length; i++)
            {
                LaserState curState = _segments[i].State;
                if (curState != LaserState.InActive) { state = LaserState.Active; break; }
            }
            State = state;
        }

        public class LaserSegment : Entity
        {
            public LaserState State;
            private Animation[] _animations;
            private Laser _root;

            public LaserSegment(Laser root, bool isHead, EntityType type, bool autoUpdate, int order, Sprite sprt, Animation anim = null) : base(EntityID.PROP, type, 5, 0, autoUpdate, order, sprt, anim, Vector2.zero, new BoxCollider(Vector2.zero, new Vector2(31, 8)))
            {
                _root = root;
                string headExt = isHead ? "_Head" : "";
                _animations = new Animation[3]
                {
                    SpriteBank.GetAnimation($"Laser{headExt}_Small"),
                    SpriteBank.GetAnimation($"Laser{headExt}_Init"),
                    SpriteBank.GetAnimation($"Laser{headExt}_Loop"),
                };
            }

            public override void Despawn(bool silent)
            {
                base.Despawn(silent);
                State = LaserState.InActive;
            }

            public override void OnCollisionEnter(ICollideable other)
            {
                _root.OnCollisionEnter(other);
            }
            public override void OnCollisionExit(ICollideable other)
            {
                _root.OnCollisionExit(other);
            }
            public override void OnCollisionStay(ICollideable other)
            {
                _root.OnCollisionStay(other);
            }

            public override void Spawn(Vector2 pos, Vector2 dir)
            {
                Animation = _animations[_currentAnim = 0];
                base.Spawn(pos, dir);
                _canCollide = false;

                State = LaserState.Activating;
                _timeAnim = 0;
                _frameAnim = 0;
            }

            public void Stop()
            {
                _canCollide = false;
                State = LaserState.DeActivationTransition;
                Animation = _animations[_currentAnim = 1];

                _timeAnim = 0;
                _frameAnim = Animation.Frames.Length - 1;

                Animation.Animate(this, ref _timeAnim, ref _frameAnim, 0, true);

                _timeAnim = 0;
                _frameAnim = Animation.Frames.Length - 1;
            }

            public override void Update(float delta)
            {
                if (Animation == null) { return; }
                switch (State)
                {
                    case LaserState.Activating:
                        if (Animation.Animate(this, ref _timeAnim, ref _frameAnim, delta))
                        {
                            State = LaserState.ActivationTransition;
                            Animation = _animations[_currentAnim = 1];
                            Animation.Animate(this, ref _timeAnim, ref _frameAnim, 0);

                            _timeAnim = 0;
                            _frameAnim = 0;
                            return;
                        }
                        break;

                    case LaserState.ActivationTransition:
                        if (Animation.Animate(this, ref _timeAnim, ref _frameAnim, delta))
                        {
                            State = LaserState.Active;
                            Animation = _animations[_currentAnim = 2];
                            Animation.Animate(this, ref _timeAnim, ref _frameAnim, 0);

                            _timeAnim = 0;
                            _frameAnim = 0;

                            _canCollide = true;
                            return;
                        }
                        break;
                    case LaserState.Active:
                        Animation.Animate(this, ref _timeAnim, ref _frameAnim, delta);
                        break;

                    case LaserState.DeActivationTransition:
                        if (Animation.Animate(this, ref _timeAnim, ref _frameAnim, delta, true))
                        {
                            State = LaserState.DeActivating;
                            Animation = _animations[_currentAnim = 0];
                            Animation.Animate(this, ref _timeAnim, ref _frameAnim, 0);

                            _timeAnim = 0;
                            _frameAnim = 0;
                            return;
                        }
                        break;

                    case LaserState.DeActivating:
                        if (Animation.Animate(this, ref _timeAnim, ref _frameAnim, delta))
                        {
                            Despawn(false);
                            return;
                        }
                        break;
                }
            }
        }

        public enum LaserState
        {
            InActive,
            Active,
            Activating,
            ActivationTransition,
            DeActivationTransition,
            DeActivating,
        }
    }
}
using System.Collections.Generic;

namespace Joonaxii.ConsoleBulletHell
{
    public interface ICollideable
    {
        bool CanCollide { get; set; }
        Entity GetEntity();

        bool IsOOB();
        void SetOOB(bool isOOB);

        void OnCollisionEnter(ICollideable other);
        void OnCollisionStay(ICollideable other);
        void OnCollisionExit(ICollideable other);

        void AddCollision(int id);
        void RemoveCollision(int id);
        bool HasCollision(int id);

        void AddNode(int id);
        void RemoveNode(int id);
        bool HasNode(int id);
    }
}
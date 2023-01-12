namespace Commodore.Engine
{
    public abstract class EngineObject
    {
        protected bool AlreadyUpdatedOnce { get; private set; }

        public virtual void LoadContent() { }
        public virtual void UpdateOnce(float deltaTime) { AlreadyUpdatedOnce = true; }
        public virtual void Update(float deltaTime) { if (!AlreadyUpdatedOnce) UpdateOnce(deltaTime); }
    }
}

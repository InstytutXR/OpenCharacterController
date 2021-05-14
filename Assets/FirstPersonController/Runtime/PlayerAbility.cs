namespace FirstPersonController
{
    public abstract class PlayerAbility
    {
        public bool isActive { get; private set; }

        public virtual bool isBlocking => false;

        public virtual bool updatesWhenNotActive => false;

        public virtual bool canActivate => true;

        public void TryActivate()
        {
            if (!isActive && canActivate)
            {
                isActive = true;
                OnActivate();
            }
        }

        public void Deactivate()
        {
            if (isActive)
            {
                isActive = false;
                OnDeactivate();
            }
        }

        public virtual void OnActivate()
        {
        }

        public virtual void OnDeactivate()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void Update()
        {
        }
    }
}
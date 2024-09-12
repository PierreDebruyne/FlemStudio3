namespace FlemStudio.LayoutManagement.Core
{
    public class Cooldown
    {
        protected float TotalTime;

        protected float CurrentTime = 0;

        public Cooldown(float totalTime)
        {
            TotalTime = totalTime;
        }

        public bool Update(float deltaTime)
        {
            CurrentTime += deltaTime;
            if (CurrentTime >= TotalTime)
            {
                return true;
            }
            return false;
        }

        public void Reset()
        {
            CurrentTime = 0;
        }
    }
}

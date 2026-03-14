namespace Game.Data
{
    public class GameflowManager
    {
        public DayData GameflowData { get; private set; }

        public GameflowManager()
        {
            InitializeGameflow();
        }

        public void LoadSavedGameflowData()
        {
            if (Repository.TryGetData("GameflowData", out DayData data))
            {
                GameflowData = data;
            }
        }

        public void SaveGameflowData()
        {
            Repository.SetData("GameflowData", GameflowData);
        }

        private void InitializeGameflow()
        {
            GameflowData = new DayData
            {
                Id = 1,
                CurrentPart = DayPart.Part1,
                CurrentQuestId = null
            };
        }
    }
}

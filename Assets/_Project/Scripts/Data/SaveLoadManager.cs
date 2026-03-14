using Zenject;

namespace Game.Data
{
    public class SaveLoadManager
    {

        private PlayerManager playerManager;
        private QuestManager questManager;

        [Inject]
        public SaveLoadManager(PlayerManager playerManager, QuestManager questManager) {
            this.playerManager = playerManager;
            this.questManager = questManager;
        }

        public void LoadGame()
        {
            Repository.LoadState();
            playerManager.LoadSavedPlayerData();
            questManager.LoadSavedQuestsStatus();
        }

        /// <summary>
        /// Полное сохранение игрового прогресса (в том числе позиции игрока).
        /// Вызывается, например, при сохранении перед выходом или при переходе в меню "Продолжить".
        /// </summary>
        public void SaveGame()
        {
            playerManager.SavePlayerData();
            questManager.SaveQuestsStatus();
            Repository.SetUserProgress(true);
            Repository.SaveState();
        }

        /// <summary>
        /// Проверяет, есть ли сохранённый игровой прогресс.
        /// </summary>
        public bool CanLoad()
        {
            Repository.LoadState();
            return Repository.HasAnyData();
        }

        /// <summary>
        /// Очищает сохранённые данные и устанавливает флаг новой игры.
        /// </summary>
        public void Clear()
        {
            AssembledPickups.Clear();
            Repository.ClearSaveData();
            PlayerSpawnData.ClearData();
        }


    }
}

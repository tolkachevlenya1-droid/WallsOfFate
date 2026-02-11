namespace Game.Data
{
    public static class SaveLoadManager
    {
        /// <summary>
        /// Полная загрузка сохранённого прогресса (в том числе позиции игрока).
        /// Вызывается, например, при выборе "Продолжить".
        /// </summary>
        public static void LoadGame()
        {
            Repository.LoadState();
            PlayerSaveLoader.LoadData();
            CollectionSaveLoader.LoadData();
            QuestSaveLoader.LoadData();
        }

        /// <summary>
        /// Полное сохранение игрового прогресса (в том числе позиции игрока).
        /// Вызывается, например, при сохранении перед выходом или при переходе в меню "Продолжить".
        /// </summary>
        public static void SaveGame()
        {
            //PlayerSaveLoader.SaveData();
            CollectionSaveLoader.SaveData();
            QuestSaveLoader.SaveData();
            Repository.SetUserProgress(true);
            Repository.SaveState();
        }

        /// <summary>
        /// Проверяет, есть ли сохранённый игровой прогресс.
        /// </summary>
        public static bool CanLoad()
        {
            Repository.LoadState();
            return Repository.HasAnyData();
        }

        /// <summary>
        /// Очищает сохранённые данные и устанавливает флаг новой игры.
        /// </summary>
        public static void Clear()
        {
            Quest.QuestCollection.ClearQuests();
            AssembledPickups.Clear();
            Repository.ClearSaveData();
            PlayerSpawnData.ClearData();
        }

    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MineList
    {
        // Длина списка мин
        public int Length { get; private set; }
        // Список мин
        public List<Mine> Minelist { get; private set; }

        public MineList(int length)
        {
            Length = length;
            Minelist = new List<Mine>();
        }

        // Инициализация списка мин хила и дамага
        public void InitializeMines(GameObject prefab, float cooldown, System.Func<uint, float, GameObject, Mine> createMine)
        {
            for (int i = 0; i < Length; i++)
            {
                uint number = (uint)i;
                GameObject mineGameObject = Object.Instantiate(prefab);
                mineGameObject.SetActive(false);
                Mine newMine = createMine(number, cooldown, mineGameObject);
                Minelist.Add(newMine);
            }
        }

        // Инициализация списка мин с бафом к скорости
        public void InitializeSpeedBuffMines(GameObject prefab, float cooldown, float speedbuff, float buffcooldown,
            int timebeforeexplosion, float radius, uint damage, bool isDebuff)
        {
            for (int i = 0; i < Length; i++)
            {
                uint number = (uint)i;
                GameObject mineGameObject = Object.Instantiate(prefab);
                mineGameObject.SetActive(false);
                Mine newMine = new BuffSpeedMine(number, cooldown, mineGameObject, speedbuff, buffcooldown, timebeforeexplosion, radius, damage, isDebuff);
                Minelist.Add(newMine);
            }
        }

        public Mine AddMine(GameObject prefab, float cooldown, System.Func<uint, float, GameObject, Mine> createMine)
        {
            // Создать уникальный номер для новой мины
            uint number = (uint)Minelist.Count;

            // Создать новый игровой объект для мины
            GameObject mineGameObject = Object.Instantiate(prefab);
            mineGameObject.SetActive(false);

            // Создать объект мины с помощью переданной функции
            Mine newMine = createMine(number, cooldown, mineGameObject);

            // Добавить новую мину в список
            Minelist.Add(newMine);

            // Увеличить длину списка
            Length++;
            return Minelist[Minelist.Count - 1];
        }

        public Mine AddMine(GameObject prefab, float cooldown, float speedbuff, float buffcooldown, int timebeforeexplosion, float radius, uint damage, System.Func<uint, float, GameObject, float, float, int, float, uint, Mine> createMine)
        {
            // Создать уникальный номер для новой мины
            uint number = (uint)Minelist.Count;

            // Создать новый игровой объект для мины
            GameObject mineGameObject = Object.Instantiate(prefab);
            mineGameObject.SetActive(false);

            // Создать объект мины с помощью переданной функции
            Mine newMine = createMine(number, cooldown, mineGameObject, speedbuff, buffcooldown, timebeforeexplosion, radius, damage);

            // Добавить новую мину в список
            Minelist.Add(newMine);

            // Увеличить длину списка
            Length++;
            return Minelist[Minelist.Count - 1];
        }
    }
}


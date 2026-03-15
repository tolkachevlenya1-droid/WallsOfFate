using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame.Agility
{
    public class PlayerHPManager : MonoBehaviour
    {
        [Header("HP Icons")]
        public List<Image> HPIcons = new List<Image>();

        [Header("References")]
        public Sprite FirstHPSprite;
        public Sprite NormalHPSprite;
        public Sprite DeadHPSprite;

        private int currentHP;

        public void InitializeHP(int startHP)
        {
            currentHP = startHP;

            for (int i = 0; i < HPIcons.Count; i++)
            {
                if (i < startHP)
                {
                    HPIcons[i].gameObject.SetActive(true);
                    if (HPIcons[i].sprite == DeadHPSprite)
                    {
                        if (i == 0) HPIcons[i].sprite = FirstHPSprite;
                        else HPIcons[i].sprite = NormalHPSprite; 
                    }
                }
                else
                {
                    HPIcons[i].gameObject.SetActive(false);
                }
            }
        }

        public void UpdateHP(int newHP)
        {
            if (newHP >= currentHP)
            {
                currentHP = newHP;
                return;
            }

            for (int i = currentHP - 1; i >= 0; i--)
            {
                if (i < HPIcons.Count && HPIcons[i].sprite != DeadHPSprite)
                {
                    HPIcons[i].sprite = DeadHPSprite;
                    break;
                }
            }

            currentHP = newHP;
        }
    }
}
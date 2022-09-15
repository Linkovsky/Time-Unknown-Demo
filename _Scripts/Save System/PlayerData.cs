using System.Collections.Generic;
using UnityEngine;
using RPG.UI.HealthSystem;

namespace RPG.SaveSystem
{
    [System.Serializable]
    
    public class PlayerData
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        public float[] position;
        public int currentIndex;
        public string gameObjectName;
        public int heartContainerNumber;
        public string musicName;
        public int currentHealth;
        public int[] numberOfItems;
        public string[] nameOfItem;

        /// <summary>
        /// Construtor da classe que visa em guardar todos os dados do jogador para depois utilizando o ficheiro de save
        /// guardar tudo num ficheiro
        /// </summary>
        public PlayerData(Vector3 playerPosition, int currentIndex, string objectName, int heartContainerNumber, string musicName, int currentHealth, Inventory.Inventory inventory)
        {
            position = new float[3];
            numberOfItems = new int[inventory._inventory.Count];
            nameOfItem = new string[inventory._inventory.Count];
            position[0] = playerPosition.x;
            position[1] = playerPosition.y;
            position[2] = playerPosition.z;

            this.currentIndex = currentIndex;
            gameObjectName = objectName;
            this.heartContainerNumber = heartContainerNumber;
            this.musicName = musicName;
            this.currentHealth = currentHealth;

            for (int i = 0; i <= inventory._inventory.Count - 1; i++)
            {
                numberOfItems[i] = inventory._inventory[i].stackSize;
                nameOfItem[i] = inventory._inventory[i].itemData.name;
            }
        }
    }
}
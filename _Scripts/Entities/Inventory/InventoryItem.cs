using System;

namespace RPG.Inventory
{
    [Serializable]
    public class InventoryItem
    {
        /// <summary>
        /// Declaração de variáveis
        /// </summary>
        public ItemData itemData;
        public int stackSize;

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="item"></param>
        /// <param name="num"></param>
        public InventoryItem(ItemData item, int num)
        {
            itemData = item;
            AddToStack(num);
        }

        /// <summary>
        /// Incrementa +1 ao número total de items
        /// </summary>
        /// <param name="num">Recebe um valor inteiro como parâmetro</param>
        public void AddToStack(int num)
        {
            stackSize += num;
        }
        /// <summary>
        /// Decrementa -1 ao número total de items
        /// </summary>
        /// <param name="num">Recebe um valor inteiro como parâmetro</param>
        public void RemoveFromStack(int num)
        {
            stackSize -= num;
        }
    }
}
using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;

namespace RPG.UI.HealthSystem
{
    [System.Serializable]
    public class HealthSystem
    {
        /// <summary>
        /// Declara��es de vari�veis
        /// </summary>
        public EventHandler OnDamaged;
        public EventHandler OnSaveFileReloaded;
        public EventHandler OnHealed;
        public EventHandler OnDead;

        public const int MAX_FRAG_AMOUNT = 4;
        public int saveFileFragments = 0;

        public List<Heart> heartList;

        /// <summary>
        /// Construtor da classe que inicia a lista e adiciona o n�mero de fragmentos a cada ciclo do for at� o i for menor que
        /// o n�mero total de cora��es
        /// </summary>
        public HealthSystem(int totalFrags)
        {
            heartList = new List<Heart>();
            for (int i = 0; i < totalFrags; i++)
            {
                heartList.Add(new Heart(MAX_FRAG_AMOUNT));
            }
        }
        /// <summary>
        /// Por precau��o verificamos se a lista � nula e se for ent�o retornamos pois n�o podemos alterar valores que n�o existem
        /// Num ciclo for pelo n�mero total de cora��es da lista adicionamos o valor m�ximo de fragmentos e decrementamos o valor do savefile pelo
        /// n�mero m�ximo de fragmentos at� obtermos um valor menor que 4 que nos d� a vida atual que o jogador tinha quando fez save game.
        /// Colocamos depois a vari�vel a 0 e continuamos o ciclo at� o valor da lista for o �ltimo adicionamos sempre 0.
        /// </summary>
        public void LoadingHealthFromSaveFile()
        {
            if (heartList == null) return;
            
            for (int i = 0; i < heartList.Count; i++)
            {
                if (saveFileFragments >= 4)
                {
                    saveFileFragments -= MAX_FRAG_AMOUNT;
                    heartList[i].Fragments = MAX_FRAG_AMOUNT;
                }
                else
                {
                    heartList[i].Fragments = saveFileFragments;
                    saveFileFragments = 0;
                }
            }
            OnSaveFileReloaded?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Retorna a lista</returns>
        public List<Heart> HeartList()
        {
            return heartList;
        }
        /// <summary>
        /// Fun��o quando � carregado o jogo a partir do sistema de save, recebe a lista com os dados como par�mentro 
        /// e adiciona � lista que no inicio estar� vazia
        /// </summary>
        /// <param name="heart"></param>
        public void ReloadFromFile(List<HealthSystem.Heart> heart)
        {
            heartList = heart;
        }
        /// <summary>
        /// Adiciona um novo cora��o � lista atual no �ltima posi��o
        /// </summary>
        internal void AddHeartToList()
        {
            heartList.Add(new Heart(MAX_FRAG_AMOUNT));
        }
        /// <summary>
        /// Depois de adicionar um cora��o novo � lista, teremos de dar vida ao jogador para carregar a vida cheia
        /// </summary>
        internal void HealAfterNewHeartContainer()
        {
            for (int i = 0; i < heartList.Count; i++)
            {
                heartList[i].Fragments = MAX_FRAG_AMOUNT;
            }
        }
        public void Damage(int damageAmount)
        {
            // Inicia o ciclo por todos os cora��es come�ando pelo ultimo � direita
            for (int i = heartList.Count - 1; i >= 0; i--)
            {
                // A classe cora��o � igual a um cora��o da lista total dos cora��es
                Heart heart = heartList[i];
                // Testa se este cora��o consegue absorver o dano
                if (damageAmount > heart.Fragments)
                {
                    // Cora��o n�o consegue absorver o dano todo, d� dano ao cora��o e vai para o pr�ximo
                    damageAmount -= heart.Fragments;
                    heart.Damage(heart.Fragments);
                }
                else
                {
                    // Dano toal n�o faz perder 1 cora��o ent�o retira os fragmentos e sai do ciclo
                    heart.Damage(damageAmount);
                    break;
                }
            }
            OnDamaged?.Invoke(this, EventArgs.Empty);
            if (IsDead()) OnDead?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Ciclo for na lista de cora��es e para index adicionar os fragmentos desse index 
        /// para uma vari�vel temporar�ria que depois ser� retornada
        /// </summary>
        /// <returns>Vida atual do jogador</returns>
        public int CurrentFragments()
        {
            int currentFragments = 0;
            for (int i = 0; i < heartList.Count; i++)
            {
                currentFragments += heartList[i].Fragments;
            }
            return currentFragments;
        }
        public bool IsDead()
        {
            return heartList[0].Fragments == 0;
        }
        public void Heal(int healamount)
        {
            // Percorre todos os cora��es da lista
            for (int i = 0; i < heartList.Count; i++)
            {
                // A classe cora��o recebe 1 cora��o da lista
                Heart heart = heartList[i];
                // Criada uma vari�vel que � o total de fragmentos que faltam para estar completamente cheio
                int missingFragments = MAX_FRAG_AMOUNT - heart.Fragments;
                if (healamount > missingFragments)
                {
                    healamount -= missingFragments;
                    heart.Heal(healamount);
                }
                else
                {
                    heart.Heal(healamount);
                    break;
                }
            }
            OnHealed?.Invoke(this, EventArgs.Empty);
        }
        [System.Serializable]
        public class Heart
        {
            private int fragments;

            public int Fragments
            {
                get { return fragments; }
                set { fragments = value; }
            }

            public Heart(int fragments)
            {
                Fragments = fragments;
            }

            public void Damage(int damageAmount)
            {
                if (damageAmount > MAX_FRAG_AMOUNT)
                    Fragments = 0;
                else
                    Fragments -= damageAmount;
            }

            public void Heal(int healAmount)
            {
                if (healAmount + Fragments > MAX_FRAG_AMOUNT)
                    Fragments = MAX_FRAG_AMOUNT;
                else
                    Fragments += healAmount;
            }
        }

    }

}
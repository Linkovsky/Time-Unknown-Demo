using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;

namespace RPG.UI.HealthSystem
{
    [System.Serializable]
    public class HealthSystem
    {
        /// <summary>
        /// Declarações de variáveis
        /// </summary>
        public EventHandler OnDamaged;
        public EventHandler OnSaveFileReloaded;
        public EventHandler OnHealed;
        public EventHandler OnDead;

        public const int MAX_FRAG_AMOUNT = 4;
        public int saveFileFragments = 0;

        public List<Heart> heartList;

        /// <summary>
        /// Construtor da classe que inicia a lista e adiciona o número de fragmentos a cada ciclo do for até o i for menor que
        /// o número total de corações
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
        /// Por precaução verificamos se a lista é nula e se for então retornamos pois não podemos alterar valores que não existem
        /// Num ciclo for pelo número total de corações da lista adicionamos o valor máximo de fragmentos e decrementamos o valor do savefile pelo
        /// número máximo de fragmentos até obtermos um valor menor que 4 que nos dá a vida atual que o jogador tinha quando fez save game.
        /// Colocamos depois a variável a 0 e continuamos o ciclo até o valor da lista for o último adicionamos sempre 0.
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
        /// Função quando é carregado o jogo a partir do sistema de save, recebe a lista com os dados como parâmentro 
        /// e adiciona à lista que no inicio estará vazia
        /// </summary>
        /// <param name="heart"></param>
        public void ReloadFromFile(List<HealthSystem.Heart> heart)
        {
            heartList = heart;
        }
        /// <summary>
        /// Adiciona um novo coração à lista atual no última posição
        /// </summary>
        internal void AddHeartToList()
        {
            heartList.Add(new Heart(MAX_FRAG_AMOUNT));
        }
        /// <summary>
        /// Depois de adicionar um coração novo à lista, teremos de dar vida ao jogador para carregar a vida cheia
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
            // Inicia o ciclo por todos os corações começando pelo ultimo à direita
            for (int i = heartList.Count - 1; i >= 0; i--)
            {
                // A classe coração é igual a um coração da lista total dos corações
                Heart heart = heartList[i];
                // Testa se este coração consegue absorver o dano
                if (damageAmount > heart.Fragments)
                {
                    // Coração não consegue absorver o dano todo, dá dano ao coração e vai para o próximo
                    damageAmount -= heart.Fragments;
                    heart.Damage(heart.Fragments);
                }
                else
                {
                    // Dano toal não faz perder 1 coração então retira os fragmentos e sai do ciclo
                    heart.Damage(damageAmount);
                    break;
                }
            }
            OnDamaged?.Invoke(this, EventArgs.Empty);
            if (IsDead()) OnDead?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Ciclo for na lista de corações e para index adicionar os fragmentos desse index 
        /// para uma variável temporarária que depois será retornada
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
            // Percorre todos os corações da lista
            for (int i = 0; i < heartList.Count; i++)
            {
                // A classe coração recebe 1 coração da lista
                Heart heart = heartList[i];
                // Criada uma variável que é o total de fragmentos que faltam para estar completamente cheio
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
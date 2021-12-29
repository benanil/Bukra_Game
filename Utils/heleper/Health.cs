
using System;

namespace AnilTools.Utils.heleper
{
    public class HealthSystem : IDisposable
    {
        public int health = 100;
        public int maxHealth = 100;
        public event Action OnDead = () => {};
        public event Action OnAddHealth = () => { };
        public event Action OnAddDamage = () => { };

        public HealthSystem(int health, int maxHealth){
            this.health = health;
            this.maxHealth = maxHealth;
        }

        public HealthSystem(){
            this.health = 100;
            this.maxHealth = 100;
        }

        public void AddDamage(int damage)
        {
            OnAddDamage();
            health -= damage;
            if (health <= 0){
                OnDead();
                Dispose();
            }
        }

        public void AddHealth(int health){
            this.health = Math.Max(this.health + health, maxHealth);
        }

        public void Dispose(){
            GC.SuppressFinalize(this);
        }
    }
}

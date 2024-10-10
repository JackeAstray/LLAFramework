
using GameLogic.Base;
using UnityEngine;

namespace GameLogic.Example
{
    // 创建一个继承自 Observed 的类
    public class Player : Observed
    {
        private int health;

        public void TakeDamage(int damage)
        {
            health -= damage;

            // 当玩家受到伤害时，通知所有的观察者
            SetState();
        }
    }

    // 创建一个继承自 Observer 的类
    public class HealthBar : Observer
    {
        private Player player;

        public HealthBar(Player player)
        {
            this.player = player;

            // 将 HealthBar 添加到 Player 的观察者列表中
            player.Attach(this);
        }

        public override void UpdateData(params object[] args)
        {
            // 当 Player 的状态改变时，更新 HealthBar 的显示
            Debug.Log("Player's health has changed!");
        }
    }
}
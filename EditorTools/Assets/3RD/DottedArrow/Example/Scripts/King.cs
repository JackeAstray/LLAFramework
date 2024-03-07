using TMPro;
using UnityEngine;

namespace Kalkatos.DottedArrow
{
	public class King : MonoBehaviour
	{
		[SerializeField] private TMP_Text healthText;
		[SerializeField] private TMP_Text damageText;
		[SerializeField] private int health;

		private Animator damageTextAnimator;

		private void Awake ()
		{
			healthText.text = health.ToString();
			damageText.text = "";
			damageTextAnimator = damageText.GetComponent<Animator>();
		}

		public void ReceiveAttack ()
		{
			CombatManager.instance.EndAttack(this);
		}

		public void TakeDamage (int amount)
		{
			health -= amount;
			healthText.text = health.ToString();
			damageText.text = $"-{amount}";
			damageTextAnimator.SetTrigger("flyUp");
			if (health <= 0)
				Debug.Log("King has died");
		}
	}
}

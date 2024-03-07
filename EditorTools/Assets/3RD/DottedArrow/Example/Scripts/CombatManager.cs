using System.Collections;
using UnityEngine;

namespace Kalkatos.DottedArrow
{
	public class CombatManager : MonoBehaviour
	{
		public static CombatManager instance;

		public Arrow Arrow { get => arrow; set => arrow = value; }

		[SerializeField] private Arrow arrow;
		[SerializeField] private AnimationCurve attackAnimCurve;

		private Card attacker;

		private void Awake ()
		{
			instance = this;
		}

		private IEnumerator AttackAnimationCoroutine (Card attacker, King receiver)
		{
			Vector3 originalUp = attacker.transform.up;
			Vector3 startPos = attacker.transform.position;
			yield return MoveTo(attacker.transform, startPos + Vector3.back, 0.2f);
			yield return new WaitForSeconds(0.1f);
			Vector3 distance = receiver.transform.position - startPos;
			distance = Vector3.MoveTowards(distance, distance * 0.001f, 1f);
			attacker.transform.up = distance;
			yield return MoveTo(attacker.transform, startPos + distance, 0.3f, attackAnimCurve);
			receiver.TakeDamage(attacker.Power);
			yield return MoveTo(attacker.transform, startPos, 0.3f);
			attacker.transform.up = originalUp;
		}

		private IEnumerator MoveTo (Transform transform, Vector3 endPos, float time, AnimationCurve curve = null)
		{
			float startTime = Time.time;
			float elapsed = 0;
			Vector3 startPos = transform.position;
			while (elapsed < time)
			{
				elapsed = Time.time - startTime;
				float t = curve != null ? curve.Evaluate(elapsed / time) : elapsed / time;
				transform.position = Vector3.Lerp(startPos, endPos, t);
				yield return null;
			}
			transform.position = endPos;
		}

		public void BeginAttack (Card card)
		{
			CancelAttack();
			arrow.SetupAndActivate(card.transform);
			attacker = card;
		}

		public void EndAttack (King king)
		{
			arrow.Deactivate();
			StartCoroutine(AttackAnimationCoroutine(attacker, king));
			attacker.EndAttack();
		}

		public void CancelAttack ()
		{
			arrow.Deactivate();
			if (attacker != null)
			{
				attacker.EndAttack();
				attacker = null;
			}
		}
	}
}

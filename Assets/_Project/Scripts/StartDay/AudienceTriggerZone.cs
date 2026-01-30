using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace Game
{
    public class AudienceTriggerZone : MonoBehaviour
    {
        [SerializeField] private Transform throne;

       /* private void OnTriggerEnter(Collider other)
        {
            // ищем скрипт диалога на вошедшем объекте
            DialogueTrigger dlg = other.GetComponent<DialogueTrigger>();
            if (dlg == null || dlg.IsDone) return;

            // останавливаем навигацию
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = true;                        // сразу тормозим
                StartCoroutine(StartWhenStopped(agent, dlg));  // ждём полного покоя
            }
            else
            {
                // если NPC передвигается без NavMeshAgent
                StartCoroutine(StartAfterDelay(0.3f, dlg));
            }
        }

        private IEnumerator StartWhenStopped(NavMeshAgent agent, DialogueTrigger dlg)
        {
            // ждём, пока агент «утрясётся» (1‑2 кадра) и точно встал
            yield return null;
            while (agent.pathPending ||
                   agent.remainingDistance > agent.stoppingDistance + 0.05f ||
                   agent.velocity.sqrMagnitude > 0.0001f)
            {
                yield return null;    // проверяем каждый кадр
            }

            // небольшая пауза, чтобы завершилась анимация шага
            yield return new WaitForSeconds(0.25f);

            // разворачиваемся к трону (опционально)
            agent.transform.LookAt(new Vector3(
            throne.position.x,
            agent.transform.position.y,   // сохраняем высоту NPC
            throne.position.z));

            dlg.Triggered();          // запускаем вашу логику Ink‑диалога
        }

        // fallback, если нет NavMeshAgent
        private IEnumerator StartAfterDelay(float time, DialogueTrigger dlg)
        {
            yield return new WaitForSeconds(time);
            dlg.Triggered();
        }*/
    }
}

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity_UI_Samples.Scripts
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField] private Animator initiallyOpen;

        private int _mOpenParameterId;
        private Animator _mOpen;
        private GameObject _mPreviouslySelected;

        private const string KeyOpenTransitionName = "Open";
        private const string KeyClosedStateName = "Closed";

        private void OnEnable()
        {
            _mOpenParameterId = Animator.StringToHash(KeyOpenTransitionName);

            if (initiallyOpen == null)
                return;

            OpenPanel(initiallyOpen);
        }

        public void OpenPanel(Animator anim)
        {
            if (_mOpen == anim)
                return;

            anim.gameObject.SetActive(true);
            var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

            anim.transform.SetAsLastSibling();

            CloseCurrent();

            _mPreviouslySelected = newPreviouslySelected;

            _mOpen = anim;
            _mOpen.SetBool(_mOpenParameterId, true);

            var go = FindFirstEnabledSelectable(anim.gameObject);

            SetSelected(go);
        }

        private static GameObject FindFirstEnabledSelectable(GameObject gameObject)
        {
            var selectables = gameObject.GetComponentsInChildren<Selectable>(true);

            return (from selectable in selectables
                where selectable.IsActive() && selectable.IsInteractable()
                select selectable.gameObject).FirstOrDefault();
        }

        public void CloseCurrent()
        {
            if (!_mOpen) return;

            _mOpen.SetBool(_mOpenParameterId, false);
            SetSelected(_mPreviouslySelected);
            StartCoroutine(DisablePanelDelayed(_mOpen));
            _mOpen = null;
        }

        private IEnumerator DisablePanelDelayed(Animator anim)
        {
            var closedStateReached = false;
            var wantToClose = true;
            while (!closedStateReached && wantToClose)
            {
                if (!anim.IsInTransition(0))
                    closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(KeyClosedStateName);

                wantToClose = !anim.GetBool(_mOpenParameterId);

                yield return new WaitForEndOfFrame();
            }

            if (wantToClose)
                anim.gameObject.SetActive(false);
        }

        private static void SetSelected(GameObject go)
        {
            EventSystem.current.SetSelectedGameObject(go);
        }
    }
}
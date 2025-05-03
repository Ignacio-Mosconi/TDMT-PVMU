using UnityEngine;

namespace Class3
{
    public class CursorLocker : MonoBehaviour
    {
        [SerializeField] private bool startLocked = true;

        
        void Awake ()
        {
            SetLockState(startLocked);
        }

        void Update ()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
                Unlock();
            else if (Input.GetKeyUp(KeyCode.LeftAlt))
                Lock();
        }

        void OnApplicationFocus ()
        {
            if (enabled)
                Lock();
        }


        private void Lock ()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Unlock ()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void SetLockState (bool isLocked)
        {
            enabled = isLocked;

            if (isLocked)
                Lock();
            else
                Unlock();
        }
    }
}
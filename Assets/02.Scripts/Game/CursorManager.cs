using UnityEngine;

namespace Game
{
    public class CursorManager : MonoBehaviour
    {
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // 게임 중이 아니면 커서를 건드리지 않는다.
            if (GameManager.Instance.State != GameState.Playing)
            {
                return;
            }

            // 왼쪽 Alt를 누르고 있는 동안만 커서를 보여 준다.
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                UnlockCursor();
            }
            else if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                LockCursor();
            }
        }
    }
}

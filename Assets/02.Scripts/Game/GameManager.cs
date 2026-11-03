using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager s_instance;
        private static readonly int s_moveMotion = Animator.StringToHash("MoveMotion");
        public static GameManager Instance => s_instance;

        [SerializeField] private TextMeshProUGUI _stateText;
        [SerializeField] private CursorManager _cursorManager;
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private GameObject _optionPopup;
        [SerializeField] private GameObject _gameOverPopup;

        private GameState _state = GameState.Ready;
        public GameState State => _state;

        private void Awake()
        {
            s_instance = this;
        }

        private void Start()
        {
            StartCoroutine(StartSequenceRoutine());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_state == GameState.Playing)
                    Pause();
                else if (_state == GameState.Pause)
                    Resume();
            }
        }

        public void Pause()
        {
            if (_state != GameState.Playing)
                return;

            ChangeState(GameState.Pause);
            Time.timeScale = 0f;
            _optionPopup.SetActive(true);
            CursorManager.UnlockCursor();
        }

        public void Resume()
        {
            if (_state != GameState.Pause)
                return;

            _state = GameState.Playing;
            Time.timeScale = 1f;
            _optionPopup.SetActive(false);
            CursorManager.LockCursor();
        }

        public void GameOver()
        {
            _state = GameState.GameOver;
            Time.timeScale = 0f;
            _gameOverPopup.SetActive(true);
            CursorManager.UnlockCursor();
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ChangeState(GameState newState)
        {
            _state = newState;
        }

        private IEnumerator StartSequenceRoutine()
        {
            yield return StartCoroutine(ShowTextRoutine("준비", 2f));
            yield return StartCoroutine(ShowTextRoutine("시작!", 0.5f));

            _stateText.gameObject.SetActive(false);
            ChangeState(GameState.Playing);
        }

        private IEnumerator ShowTextRoutine(string text, float duration)
        {
            _stateText.text = text;
            _stateText.gameObject.SetActive(true);

            yield return new WaitForSeconds(duration);

            _stateText.gameObject.SetActive(false);
        }
    }
}

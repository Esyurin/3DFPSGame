using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager s_instance;
        public static GameManager Instance => s_instance;

        [SerializeField] private TextMeshProUGUI _stateText;
        [SerializeField] private CursorManager _cursorManager;

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

        public void GameOver()
        {
            if (_state == GameState.GameOver)
                return;

            ChangeState(GameState.GameOver);
            _stateText.text = "게임 오버";
            _stateText.color = Color.red;
            _stateText.gameObject.SetActive(false);
            CursorManager.UnlockCursor();
        }

        private void ChangeState(GameState newState)
        {
            _state = newState;
        }
    }
}

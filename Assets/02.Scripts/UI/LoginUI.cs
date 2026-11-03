using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    // 로그인 화면의 입력칸과 버튼을 처리한다. A안(3D 배경)과 B안(2D 일러스트)이 함께 쓴다.
    // 아직 서버가 없으므로 ID와 비밀번호가 비어 있지 않으면 통과시키고, 마지막 ID만 기억해 둔다.
    public class LoginUI : MonoBehaviour
    {
        private const string LastIdKey = "LastLoginId";

        [SerializeField] private TMP_InputField _idInput;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private LoginIntroSequence _intro;
        [SerializeField] private UIShake _panelShake;
        [SerializeField] private string _gameSceneName = "GameScene";

        private bool _isLoading;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _idInput.text = PlayerPrefs.GetString(LastIdKey, string.Empty);
            _messageText.text = string.Empty;
        }

        private void Update()
        {
            // 등장 연출 중이거나 씬을 넘기는 중에는 키 입력을 받지 않는다.
            if (_intro.IsPlaying || _isLoading)
                return;

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                Login();

            // Tab으로 ID 칸과 비밀번호 칸을 오간다.
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_idInput.isFocused)
                    _passwordInput.ActivateInputField();
                else
                    _idInput.ActivateInputField();
            }
        }

        public void Login()
        {
            if (_isLoading)
                return;

            if (string.IsNullOrWhiteSpace(_idInput.text))
            {
                ShowError("ID를 입력하세요.");
                return;
            }

            if (string.IsNullOrEmpty(_passwordInput.text))
            {
                ShowError("비밀번호를 입력하세요.");
                return;
            }

            PlayerPrefs.SetString(LastIdKey, _idInput.text.Trim());
            StartCoroutine(LoadGameRoutine());
        }

        public void StartAsGuest()
        {
            if (_isLoading)
                return;

            StartCoroutine(LoadGameRoutine());
        }

        public void SignUp()
        {
            _messageText.text = "회원가입은 준비 중이에요.";
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void ShowError(string message)
        {
            _messageText.text = message;
            _panelShake.Play();
        }

        private IEnumerator LoadGameRoutine()
        {
            _isLoading = true;
            _messageText.text = string.Empty;

            yield return _intro.PlayOutroRoutine();

            SceneManager.LoadScene(_gameSceneName);
        }
    }
}

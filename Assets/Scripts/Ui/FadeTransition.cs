using Events;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    private Image _image;
    private Color _color;
    [SerializeField]
    private float _fadeTime;
    [SerializeField]
    private bool _fadeIn;
    [SerializeField]
    private bool _fadeOut;
    [SerializeField]
    private string _newSceneName;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _color = _image.color;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.levelEvents.LevelTimerFinished -= FadeIn;
    }

    void Start()
    {
        GameEventManager.Instance.levelEvents.LevelTimerFinished += FadeIn;
        if (_color.a == 0) return;
        FadeOut();
    }

    private void Update()
    {
        if (_fadeIn)
        {
            _color.a = Mathf.Lerp(_color.a, 1, Time.deltaTime * _fadeTime * (_color.a + _fadeTime / 10));
            _image.color = _color;
            if (_color.a > .97f) GameEventManager.Instance.sceneEvents.OnChangeScene(_newSceneName);
        }

        if (_fadeOut)
        {
            _color.a = Mathf.Lerp(_color.a, 0, Time.deltaTime * _fadeTime * (_fadeTime - _color.a / 10));
            _image.color = _color;
        }
        // GameEventManager.Instance.sceneEvents.OnChangeScene(_newSceneName);
    }

    private void FadeOut()
    {
        _fadeOut = true;
        _fadeIn = false;
    }

    private void FadeIn()
    {
        _fadeIn = true;
        _fadeOut = false;
    }
}

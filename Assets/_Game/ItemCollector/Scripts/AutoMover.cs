using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dt.Attribute;
using UnityEngine;

public class AutoMover : MonoBehaviour
{
    [Title("Time")]
    [SerializeField]
    private float delayTime;

    [SerializeField]
    private float moveDuration;

    [Line]
    [SerializeField]
    private Ease ease;

    [Line, SerializeField, ReadOnly]
    private Vector3 target;

    private Tweener moveTweener;
    public event Action OnFinished;

    public async void Move(Vector3 target)
    {
        await UniTask.WaitForSeconds(this.delayTime);
        this.moveTweener?.Kill();
        this.moveTweener = transform.DOMove(target, this.moveDuration);
        this.moveTweener.SetEase(this.ease);
        this.moveTweener.OnComplete(OnCompleteHandler);
    }

    private void OnCompleteHandler()
    {
        gameObject.SetActive(false);
        OnFinished?.Invoke();
    }
}
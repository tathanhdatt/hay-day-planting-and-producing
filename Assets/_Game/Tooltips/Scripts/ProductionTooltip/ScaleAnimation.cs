using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dt.Attribute;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    [SerializeField]
    private bool playOnEnable;

    [SerializeField]
    private float startDelay;

    [Title("Start")]
    [SerializeField]
    private float startDuration;

    [SerializeField]
    private Vector3 startTarget;

    [SerializeField]
    private Ease startEase;

    [Title("End")]
    [SerializeField]
    private float endDuration;

    [SerializeField]
    private Vector3 endTarget;

    [SerializeField]
    private Ease endEase;

    [Title("Loop")]
    [SerializeField]
    private bool loop;

    [SerializeField]
    private float intervalDelay;

    private bool isBroken;
    private Tweener tweener;

    private void OnEnable()
    {
        if (this.playOnEnable)
        {
            Scale();
        }
    }


    [Button]
    public async void Scale()
    {
        this.isBroken = false;
        await UniTask.WaitForSeconds(this.startDelay);
        if (this.loop)
        {
            while (!this.isBroken)
            {
                await DoScale();
                await UniTask.WaitForSeconds(this.intervalDelay);
            }
        }
        else
        {
            await DoScale();
        }
    }

    private async UniTask DoScale()
    {
        this.tweener?.Kill();
        this.tweener = transform.DOScale(this.startTarget, this.startDuration);
        this.tweener.SetEase(this.startEase);
        await this.tweener.AsyncWaitForCompletion();
        this.tweener = transform.DOScale(this.endTarget, this.endDuration);
        this.tweener.SetEase(this.endEase);
        await this.tweener.AsyncWaitForCompletion();
    }

    [Button]
    public void Kill()
    {
        this.isBroken = true;
        this.tweener?.Kill();
    }

    private void OnDisable()
    {
        Kill();
    }
}
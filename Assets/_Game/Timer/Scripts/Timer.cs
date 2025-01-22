using System;
using System.Globalization;
using System.Text;
using Dt.Attribute;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private double timeLeft;

    [SerializeField, ReadOnly]
    private bool isDestroyOnFinished;

    [SerializeField, ReadOnly]
    private int heartbeat;

    [SerializeField, ReadOnly]
    private int currentHeartbeat;

    [SerializeField, ReadOnly]
    private bool isFinished;

    private DateTime startTime;
    private TimeSpan timeSpan;
    private DateTime finishTime;

    public string Name { get; private set; }
    public double TimeLeft => this.timeLeft;
    public bool IsFinished => this.isFinished;

    public event Action<int> OnHeartbeat;
    public event Action OnStarted;
    public event Action OnFinished;

    public void Initialize(bool isDestroyOnFinished = true)
    {
        enabled = false;
        this.isDestroyOnFinished = isDestroyOnFinished;
        this.isFinished = true;
    }

    public void StartTimer(string name, TimeSpan timeSpan, int heartbeat = 1)
    {
        Name = name;
        this.timeSpan = timeSpan;
        this.heartbeat = heartbeat;
        UpdateStartTime();
        CalculateFinishedTime();
        ResetState();
        enabled = true;
        OnStarted?.Invoke();
        StartInvokeHeartbeat();
    }

    private void UpdateStartTime()
    {
        this.startTime = DateTime.Now;
    }

    private void CalculateFinishedTime()
    {
        this.finishTime = this.startTime + this.timeSpan;
    }

    private void ResetState()
    {
        this.timeLeft = this.timeSpan.TotalSeconds;
        this.isFinished = false;
        this.currentHeartbeat = 0;
    }

    private void StartInvokeHeartbeat()
    {
        if (this.timeSpan.TotalSeconds <= 0) return;
        float timeInterval = (float)this.timeSpan.TotalSeconds / this.heartbeat;
        try
        {
            InvokeRepeating(nameof(InvokeHeartbeat), 0, timeInterval);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void InvokeHeartbeat()
    {
        OnHeartbeat?.Invoke(this.currentHeartbeat + 1);
        this.currentHeartbeat++;
        if (this.currentHeartbeat >= this.heartbeat)
        {
            CancelInvoke(nameof(InvokeHeartbeat));
        }
    }

    private void Update()
    {
        if (this.isFinished) return;
        this.timeLeft -= Time.deltaTime;
        if (this.timeLeft <= 0)
        {
            OnFinishTimer();
        }
    }

    private void OnFinishTimer()
    {
        this.finishTime = DateTime.Now;
        this.isFinished = true;
        OnFinished?.Invoke();
        if (this.isDestroyOnFinished)
        {
            Destroy(this);
        }
        else
        {
            enabled = false;
        }
    }

    public void SkipTimer()
    {
        this.timeLeft = 0;
        CancelInvoke(nameof(InvokeHeartbeat));
        OnFinishTimer();
    }

    public float GetTimeLeftPercentage()
    {
        return (float)(this.timeLeft / this.timeSpan.TotalSeconds);
    }

    public void Subtract(TimeSpan timeSpan)
    {
        CancelInvoke(nameof(InvokeHeartbeat));
        this.timeLeft -= timeSpan.TotalSeconds;
        this.finishTime -= timeSpan;
        CalculateCurrentHeartbeatAndRestartInvoker();
    }

    private void CalculateCurrentHeartbeatAndRestartInvoker()
    {
        float timeInterval = (float)this.timeSpan.TotalSeconds / this.heartbeat;
        TimeSpan passedTime = this.timeSpan - TimeSpan.FromSeconds(this.timeLeft);
        int numberOfHeartbeatsSkipped = (int)Math.Floor(passedTime.TotalSeconds / timeInterval);
        this.currentHeartbeat = Math.Clamp(numberOfHeartbeatsSkipped, 0, this.heartbeat);
        float delay = (float)(this.timeLeft % timeInterval);
        InvokeRepeating(nameof(InvokeHeartbeat), delay, timeInterval);
        InvokeHeartbeat();
    }

    public string GetFormattedTimeLeft()
    {
        StringBuilder time = new StringBuilder();
        TimeSpan timeLeftSpan = TimeSpan.FromSeconds(this.timeLeft);
        if (timeLeftSpan.Days > 0)
        {
            time.Append(timeLeftSpan.Days);
            time.Append("d ");
        }

        if (timeLeftSpan.Hours > 0)
        {
            time.Append(timeLeftSpan.Hours);
            time.Append("h ");
        }

        if (timeLeftSpan.Minutes > 0)
        {
            time.Append(timeLeftSpan.Minutes);
            time.Append("m ");
        }

        if (timeLeftSpan.Seconds > 0)
        {
            time.Append(timeLeftSpan.Seconds);
            time.Append("s");
        }
        else
        {
            time.Append("0s");
        }

        return time.ToString();
    }

    public int GetGemToSkip()
    {
        if (this.timeLeft < 600)
        {
            return ExchangeRate.GemPer10Minutes;
        }

        return (int)(this.timeLeft / 60 * ExchangeRate.GemPer10Minutes);
    }

    public string GetFinishTimeString()
    {
        return this.finishTime.ToString(CultureInfo.InvariantCulture);
    }
}
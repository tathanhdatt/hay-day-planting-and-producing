using System;
using System.Text;
using Dt.Attribute;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private double timeLeft;

    [SerializeField, ReadOnly]
    private bool isFinished;

    private DateTime startTime;
    private TimeSpan timeSpan;
    private DateTime finishTime;

    public string Name { get; private set; }
    public bool IsFinished => this.isFinished;
    public double TimeLeft => this.timeLeft;

    public event Action OnFinished;

    public void Initialize()
    {
        this.isFinished = true;
    }

    public void StartTimer(string name, TimeSpan timeSpan)
    {
        Name = name;
        this.timeSpan = timeSpan;
        this.startTime = DateTime.Now;
        this.finishTime = this.startTime + this.timeSpan;
        this.timeLeft = this.timeSpan.TotalSeconds;
        this.isFinished = false;
    }

    private void Update()
    {
        if (this.isFinished) return;
        this.timeLeft -= Time.deltaTime;
        if (this.timeLeft <= 0)
        {
            FinishTimer();
        }
    }

    private void FinishTimer()
    {
        this.isFinished = true;
        this.finishTime = DateTime.Now;
        OnFinished?.Invoke();
        Destroy(this);
    }

    public void SkipTimer()
    {
        this.timeLeft = 0;
        FinishTimer();
    }

    public float GetTimeLeftPercentage()
    {
        return (float)(this.timeLeft / this.timeSpan.TotalSeconds);
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

        if (timeLeftSpan.Seconds >= 0)
        {
            time.Append(timeLeftSpan.Seconds);
            time.Append("s");
        }

        if (this.timeLeft < double.Epsilon)
        {
            time.Append("Finished!");
        }

        return time.ToString();
    }
}
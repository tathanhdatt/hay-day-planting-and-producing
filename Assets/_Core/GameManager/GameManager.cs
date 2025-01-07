using System;
using Core.AudioService;
using Core.Service;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

namespace Core.Game
{
    public class GameManager : MonoBehaviour, IDisposable
    {
        [SerializeField, Required]
        private GamePresenter presenter;

        [SerializeField, Required]
        private BuildingSystem buildingSystem;

        private IAudioService audioService;
        private ILevelRequirement levelRequirement;
        public ILevelXpStorage LevelXpStorage { get; private set; }
        public ICurrency Currency { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            Application.targetFrameRate = 60;
            InitAudioService();
            InitCurrency();
            InitLevelRequirement();
            InitLevelStorage();
            InitBuildingSystem();
        }


        private void InitAudioService()
        {
            this.audioService = FindAnyObjectByType<NativeAudioService>();
            ServiceLocator.Register(this.audioService);
        }

        private void InitCurrency()
        {
            Currency = new Currency();
            Currency.AddCurrency(CurrencyType.Coin);
            Currency.AddCurrency(CurrencyType.Gem);
        }

        private void InitLevelRequirement()
        {
            LevelXpDatabase database = Resources.Load<LevelXpDatabase>("Level experience Database");
            this.levelRequirement = new LevelRequirement(database);
        }

        private void InitLevelStorage()
        {
            LevelXpStorage = new LevelXpStorage(this.levelRequirement);
            LevelXpStorage.SetCurrentLevel(1);
        }

        private void InitBuildingSystem()
        {
            if (Currency == null)
            {
                InitCurrency();
            }
            this.buildingSystem.Initialize(Currency);
        }

        private async void Start()
        {
            this.presenter.Enter(this);
            await this.presenter.InitialViewPresenters();
            await OnEnter();
        }

        private async UniTask OnEnter()
        {
            await this.presenter.GetViewPresenter<GameViewPresenter>().Show();
            Currency.SetAmount(CurrencyType.Coin, 1000);
            Currency.SetAmount(CurrencyType.Gem, 1000);
        }

        public void Dispose()
        {
        }
    }
}
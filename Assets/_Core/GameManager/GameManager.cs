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

        [SerializeField, Required]
        private TimerTooltip timerTooltip;

        [SerializeField, Required]
        private GoodsDatabase barnDatabase;

        [SerializeField, Required]
        private GoodsDatabase siloDatabase;

        private IAudioService audioService;
        private ILevelRequirement levelRequirement;
        public ILevelXpStorage LevelXpStorage { get; private set; }
        public ICurrency Currency { get; private set; }
        public GoodsDatabase BarnDatabase => this.barnDatabase;
        public GoodsDatabase SiloDatabase => this.siloDatabase;

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
            InitTimerTooltip();
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
            LevelXpDatabase database = Resources.Load<LevelXpDatabase>("Level Experience Database");
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

        private void InitTimerTooltip()
        {
            this.timerTooltip.Initialize(Currency);
        }

        private async void Start()
        {
            this.presenter.Enter(this);
            await this.presenter.InitialViewPresenters();
            await OnEnter();
        }

        private async UniTask OnEnter()
        {
            Messenger.AddListener<OpenableView>(Message.OpenView, OpenViewHandler);
            await this.presenter.GetViewPresenter<GameViewPresenter>().Show();
            Currency.SetAmount(CurrencyType.Coin, 1000);
            Currency.SetAmount(CurrencyType.Gem, 1000);
        }

        private async void OpenViewHandler(OpenableView openableView)
        {
            switch (openableView)
            {
                case OpenableView.Achievement:
                    await this.presenter.GetViewPresenter<AchievementViewPresenter>().Show();
                    break;
                case OpenableView.Barn:
                    await this.presenter.GetViewPresenter<BarnStorageViewPresenter>().Show();
                    break;
                case OpenableView.Silo:
                    await this.presenter.GetViewPresenter<SiloStorageViewPresenter>().Show();
                    break;
            }
        }

        public void Dispose()
        {
        }
    }
}
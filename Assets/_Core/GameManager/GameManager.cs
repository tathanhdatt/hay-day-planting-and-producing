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
        private ItemCollector itemCollector;

        [Title("Tooltips")]
        [SerializeField, Required]
        private TimerTooltip timerTooltip;

        [SerializeField, Required]
        private CropTooltip cropTooltip;
        
        [SerializeField, Required]
        private ProductionTooltip productionTooltip;

        [SerializeField, Required]
        private HarvestTooltip harvestTooltip;

        [Title("Database")]
        [SerializeField, Required]
        private GoodsDatabase barnDatabase;

        [SerializeField, Required]
        private GoodsDatabase siloDatabase;

        [Line]
        [SerializeField, Required]
        private DialogManager dialogManager;

        private IAudioService audioService;
        private IPoolService poolService;
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
            InitPoolingService();
            InitCurrency();
            InitLevelRequirement();
            InitLevelStorage();
            InitItemCollector();
            InitBuildingSystem();
            InitTooltips();
        }


        private void InitAudioService()
        {
            this.audioService = FindAnyObjectByType<NativeAudioService>();
            ServiceLocator.Register(this.audioService);
        }

        private void InitPoolingService()
        {
            this.poolService = FindAnyObjectByType<NativePoolService>();
            ServiceLocator.Register(this.poolService);
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

        private void InitItemCollector()
        {
            this.itemCollector.Initialize(LevelXpStorage, Currency, 
                this.siloDatabase, this.barnDatabase);
        }

        private void InitBuildingSystem()
        {
            if (Currency == null)
            {
                InitCurrency();
            }

            this.buildingSystem.Initialize(Currency, this.barnDatabase);
        }

        private void InitTooltips()
        {
            this.timerTooltip.Initialize(Currency);
            this.cropTooltip.Initialize();
            this.productionTooltip.Initialize(Currency);
            this.harvestTooltip.Initialize();
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
            Messenger.AddListener<string>(Message.PopupDialog, PopupHandler);
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

        private void PopupHandler(string content)
        {
            if (!this.dialogManager.TryGetDialog(out PopupDialog dialog)) return;
            dialog.SetContent(content);
            dialog.Show();
            dialog.OnHide += OnPopupDialogHideHandler;
        }

        private void OnPopupDialogHideHandler(Dialog dialog)
        {
            dialog.OnHide -= OnPopupDialogHideHandler;
            this.dialogManager.AddDialog(dialog as PopupDialog);
        }


        public void Dispose()
        {
        }
    }
}
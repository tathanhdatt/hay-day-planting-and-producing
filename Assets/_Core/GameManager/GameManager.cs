using Core.AudioService;
using Core.Service;
using Cysharp.Threading.Tasks;
using Dt.Attribute;
using UnityEngine;

namespace Core.Game
{
    public class GameManager : MonoBehaviour
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
        public ISaveManager SaveManager { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            Application.targetFrameRate = 60;
            InitSaveManager();
            InitAudioService();
            InitPoolingService();
            InitCurrency();
            InitLevelRequirement();
            InitLevelStorage();
            InitItemCollector();
            InitBuildingSystem();
            InitTooltips();
        }

        private void InitSaveManager()
        {
            SaveManager = new SaveManager();
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
            string data = SaveManager.Load(SaveId.Level);
            LevelXpStorage = new LevelXpStorage(this.levelRequirement, data);
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

            string data = SaveManager.Load(SaveId.Building);
            this.buildingSystem.Initialize(Currency, this.barnDatabase, data);
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
            LoadCurrency();
        }

        private void LoadCurrency()
        {
            string coinAmount = SaveManager.Load(SaveId.Coin);
            if (coinAmount.IsNullOrEmpty())
            {
                Currency.SetAmount(CurrencyType.Coin, 1000);
            }
            else
            {
                Currency.SetAmount(CurrencyType.Coin, int.Parse(coinAmount));
            }

            string gemAmount = SaveManager.Load(SaveId.Gem);
            if (gemAmount.IsNullOrEmpty())
            {
                Currency.SetAmount(CurrencyType.Gem, 100);
            }
            else
            {
                Currency.SetAmount(CurrencyType.Gem, int.Parse(gemAmount));
            }
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

        private void OnApplicationQuit()
        {
            SaveManager.SaveData(SaveId.Building, this.buildingSystem.GetJsonData());
            SaveManager.SaveData(SaveId.Coin, Currency.GetAmount(CurrencyType.Coin).ToString());
            SaveManager.SaveData(SaveId.Gem, Currency.GetAmount(CurrencyType.Gem).ToString());
            SaveManager.SaveData(SaveId.Level, LevelXpStorage.GetJsonData());
        }
    }
}
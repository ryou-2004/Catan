using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Cysharp.Threading.Tasks;

public class GameController : MonoBehaviour
{
    public bool isAuto;
    [Header("ボタン自動クリック")]
    public bool isDebug;
    [Header("falseにすればSetUpをスキップできる")]
    public bool _isSetupBuild = true;//

    [Header("Game")]
    public GameObject _bord;
    public GameObject _tileParent;
    public ResourceData _resourceData;
    public DevelopmentCardData _deveCardData;
    public GameObject _fragObj;
    public GameObject _thieves;
    public Material _bordDefaultMate;
    public List<Material> _bordOutLine;
    public DiceRoll _diceRoll;
    public GameObject _selectPlayerObj;

    [Header("UI")]
    public GameObject _colorObj;
    public GameObject _colorSelectParent;
    public GameObject _colorSelectButton;
    public GameObject _colorSelect;
    public GameObject _notColorSelect;
    public GameObject _startButton;
    public GameObject _diceButton;
    public GameObject _panelMainObj;
    public GameObject _panelSubObj;
    public GameObject _logContent;
    public GameObject _logPrefab;
    public GameObject _exChangeButton;
    public GameObject _backCard;
    public GameObject _rankingContent;
    public GameObject _rankingElement;
    public GameObject _victory;
    public Text _diceText;
    public Toggle _menuToggle;
    public Toggle _resourceToggle;

    [Header("GameUI")]

    public GameObject _gameUI;
    public GameObject _playerNameParent;
    public GameObject _playerNameContent;
    public GameObject _frameObj;
    public GameObject test;

    private bool _playerListDisplay = true;
    private bool _playerListDisplayAni = false;

    private int _colorSelectPlayer = 0;
    private int _myNumber = 0;
    private int _playerCount;
    private int _order = 1;
    private int _cardCount;
    private int _diceResult;
    private int _buildSelectId = -1;
    private int _buildInfo;
    private int _soldierCount;
    private int _skillBuildCount;
    private int _deveId;
    private int _monopolyCompleteCount;
    private int _discardCount;
    private int _discardSelectCount;
    private int _discardCompoleteCount;
    private int _robPlayerId;
    private int _robCardNumber;
    private int _foreignCount;
    private int _foreignResourceId;
    private int _resultCount;
    private List<int> _setupBuildList = new List<int>();
    private List<int> _buildCount = new List<int>();
    private List<int> _playerCardCount = new List<int>();
    private List<int> _playerOrder = new List<int>();
    private List<int> _deveCardNow = new List<int>();
    private List<int> _deveCard = new List<int>();
    private List<int> _deveCardDeck = new List<int>()
    {
        0,0,0,0,0,0,0,0,0,0,0,0,0,0,//騎士
        1,1,//街道建設
        2,2,//収穫
        3,3,//独占
        4,4,4,4,4//勝利ポイント
    };
    private List<int> _harvestList = new List<int>();
    private List<int> _myWinPoint = new List<int>();
    private List<int> _winPoint = new List<int>() { 1, 2, 2, 1 };

    private bool _diceClick;
    private bool _isFirstDice;
    private bool _isOrdering;
    private bool _isOrderRevers;
    private bool _isMyTurn;
    private bool _isDisplayClick;

    private bool _isBuild;
    private bool _isBuildSelect;
    private bool _isTileSelect;
    private bool _isDeve;
    private bool _isDeveNow;
    private bool _isSkillBuild;
    private bool _isRobPlayerSelect;
    private bool _isCardSelect;
    private bool _isRobResult;
    private bool _isRobPlayer;
    private bool _isDispose;
    private bool _isForeignTrade;
    private bool _isForeignExpart;
    public static bool _isGameNow;
    private List<bool> _isSetupBuildLi = new List<bool>() { false, false };

    private float _animationSpeed = 0.5f;

    private string myName;

    private Tile desert;
    private Button exchangeButton;

    private List<City> _myCity = new List<City>();
    private List<Highway> _myHighway = new List<Highway>();
    private List<Button> _colorButtons = new List<Button>();
    private List<GameObject> _selectPlayerCity = new List<GameObject>();
    private List<Color> _playerColorDef = new List<Color>()
    {
        Color.red,
        Color.blue,
        Color.yellow,
        Color.green
    };
    private List<Tile> _tileList = new List<Tile>();
    private Dictionary<int, int> _playerColor = new Dictionary<int, int>();
    private Dictionary<int, int> _firstDiceResult = new Dictionary<int, int>();
    private Dictionary<int, int> _resourceList = new Dictionary<int, int>();
    private Dictionary<int, GameObject> _nameList = new Dictionary<int, GameObject>();
    private Dictionary<int, Text> _playerCardCountText = new Dictionary<int, Text>();
    private Dictionary<int, Toggle> _playerSelectToggle = new Dictionary<int, Toggle>();
    private Dictionary<int, List<int>> _playerWinPoints = new Dictionary<int, List<int>>();

    private List<List<int>> _resourceConsumption = new List<List<int>>()
    {
        new List<int>() { 1, 1, 1, 1, 0 },
        new List<int>() { 1, 1, 0, 0, 0 },
        new List<int>() { 0, 0, 0, 2, 3 },
        new List<int>() { 0, 0, 1, 1, 1 }
    };
    private List<List<GameObject>> _bordObjList = new List<List<GameObject>>();//町と道のリスト
    private List<List<BordObject>> _bordBaseObjList = new List<List<BordObject>>();//町と道のBordObjectのリスト
    private List<List<GameObject>> _setupObjList = new List<List<GameObject>>() { new List<GameObject>(), new List<GameObject>() };
    private List<Dictionary<int, int>> _negoResourceList = new List<Dictionary<int, int>>();
    private PhotonView _view;

    private Panel _panel;
    private SubPanel _subPanel;
    private ResourcePanel _resourcePanel;
    private PointPanel _pointPanel;

    private class Panel
    {
        public Button dice;
        public Button turnEnd;
        public Button build;
        public Button summary;
        public Button development;
        public Button negotiation;
        public List<Button> buttonLi;
        public Panel(Transform parent)
        {
            var li = new List<Button>();
            foreach (Transform child in parent)
                li.Add(child.GetComponent<Button>());
            dice = li[0];
            turnEnd = li[1];
            build = li[2];
            summary = li[3];
            development = li[4];
            negotiation = li[5];
            buttonLi = new List<Button>(li);
        }
        public void Display(bool isDisplay)
        {
            foreach (var v in buttonLi)
            {
                v.interactable = isDisplay;
            }
        }
    }
    private class SubPanel
    {
        public List<Button> buildButtons = new List<Button>();
        public GameObject build;
        public GameObject summary;

        public GameObject development;
        public GameObject deveDefault;
        public GameObject deveContent;
        public Button buyButton;
        public GameObject harvest;
        public List<Button> harvestPlus = new List<Button>();
        public List<Button> harvestMinus = new List<Button>();
        public List<Text> harvestCount = new List<Text>();
        public Button harvestConfirm;
        public GameObject soldier;
        public GameObject deveBuild;
        public GameObject monopoly;
        public List<GameObject> devePanels = new List<GameObject>();

        public GameObject negotiation;
        public List<(List<Button> plus, List<Button> minus)> negoButton = new List<(List<Button> plus, List<Button> minus)>();
        public List<List<Text>> negoText = new List<List<Text>>();
        public Button resourceConfirm;
        public GameObject foreignTradeObj;
        public Text foreignOutText;
        public Transform card;
        public GameObject selectPlayer;
        public GameObject selectPlayerContent;
        public Button exChangeButton;

        public GameObject discardCard;
        public Text discardText;
        public List<Button> discardButtonli = new List<Button>();
        public GameObject cardSelect;
        public Text cardSelectPlayerName;

        public Button closeButton;
        public List<GameObject> objLi;
        public SubPanel(Transform parent)
        {
            var li = new List<GameObject>();
            foreach (Transform child in parent)
                li.Add(child.gameObject);
            build = li[0];
            summary = li[1];
            development = li[2];
            negotiation = li[3];
            discardCard = li[4];
            cardSelect = li[5];

            Build(build.transform);
            Deve(development.transform);
            Nego(negotiation.transform);
            Discard(discardCard.transform);
            CardSelect(cardSelect.transform);

            closeButton = parent.parent.transform.Find("CloseButton").GetComponent<Button>();
            objLi = new List<GameObject>(li);
        }
        private void Build(Transform build)
        {
            foreach (Transform child in build.transform)
            {
                buildButtons.Add(child.GetComponent<Button>());
            }
        }
        private void Deve(Transform development)
        {
            deveDefault = development.transform.Find("Default").gameObject;
            soldier = development.transform.Find("Soldier").gameObject;
            deveBuild = development.transform.Find("Build").gameObject;
            deveContent = deveDefault.transform.Find("Content").gameObject;
            buyButton = deveDefault.transform.Find("BuyButton").GetComponent<Button>();
            harvest = development.transform.Find("Harvest").gameObject;
            foreach (Transform element in harvest.transform.Find("In").transform)
            {
                harvestPlus.Add(element.Find("Plus").GetComponent<Button>());
                harvestMinus.Add(element.Find("Minus").GetComponent<Button>());
                harvestCount.Add(element.Find("Count").GetComponent<Text>());
            }
            harvestConfirm = harvest.transform.Find("Confirm").GetComponent<Button>();
            monopoly = development.transform.Find("Monopoly").gameObject;
            devePanels = new List<GameObject>()
            {
                soldier,
                deveBuild,
                harvest,
                monopoly,
                deveDefault
            };
        }
        private void Nego(Transform negotiation)
        {
            int i = 0;
            card = negotiation.Find("Card");
            foreach (Transform child in card.Find("Content"))
            {
                negoButton.Add((new List<Button>(), new List<Button>()));
                var texts = new List<Text>();
                foreach (Transform element in child.transform)
                {
                    negoButton[i].plus.Add(element.Find("Plus").GetComponent<Button>());
                    negoButton[i].minus.Add(element.Find("Minus").GetComponent<Button>());
                    texts.Add(element.Find("Count").GetComponent<Text>());
                }
                negoText.Add(texts);
                i++;
            }
            resourceConfirm = card.Find("ResourceConfirm").GetComponent<Button>();
            foreignTradeObj = card.Find("ForeignTradeText").gameObject;
            foreignOutText = foreignTradeObj.transform.Find("Out").GetComponent<Text>();
            selectPlayer = negotiation.Find("Player").gameObject;
            exChangeButton = selectPlayer.transform.Find("ExChangeButton").GetComponent<Button>();
            selectPlayerContent = selectPlayer.transform.Find("Content").gameObject;
        }
        private void Discard(Transform discardCard)
        {
            discardText = discardCard.transform.Find("Count").GetComponent<Text>();
            foreach (Transform child in discardCard.transform.Find("Content"))
            {
                discardButtonli.Add(child.Find("Delete").GetComponent<Button>());
            }
        }
        private void CardSelect(Transform cardSelect)
        {
            cardSelectPlayerName = cardSelect.transform.Find("PlayerName").GetComponent<Text>();
        }
        public void Display(bool isDisplay)
        {
            foreach (var v in objLi)
            {
                v.SetActive(isDisplay);
            }
        }
        public void Display(GameObject obj)
        {
            Display(false);
            obj.SetActive(true);
        }
        public void Display(bool isDisplay, List<Button> buttons)
        {
            foreach (var v in buttons) v.interactable = isDisplay;
        }
        public void DeveDisplay(bool isDisplay)
        {
            foreach (var v in devePanels)
                v.SetActive(isDisplay);
        }
        public void DeveDisplay(GameObject obj)
        {
            DeveDisplay(false);
            obj.SetActive(true);
        }
        public void DeveDisplay(int id)
        {
            foreach (var v in devePanels)
                v.SetActive(false);
            deveDefault.SetActive(false);
            devePanels[id].SetActive(true);
        }

    }
    private class ResourcePanel
    {
        public List<Text> texts = new List<Text>();
        public ResourcePanel(GameObject parent)
        {
            foreach (Transform child in parent.transform)
            {
                texts.Add(child.Find("Count").GetComponent<Text>());
            }
        }
    }
    private class PointPanel
    {
        public List<Text> pointTexts = new List<Text>();
        public PointPanel(Transform parent)
        {
            foreach (Transform child in parent.Find("Content"))
            {
                pointTexts.Add(child.Find("Count").GetComponent<Text>());
            }
        }
    }
    private void Awake()
    {
        if (isAuto && !PhotonNetwork.InRoom)
        {
            SceneManager.LoadScene("Title");
        }
        else if (!isAuto && !PhotonNetwork.InRoom)
        {
            PhotonNetwork.OfflineMode = true;
            _playerColor.Add(0, 0);
            _playerOrder.Add(0);
        }
    }
    private void Start()
    {
        _notColorSelect.SetActive(false);
        _startButton.SetActive(false);
        _view = GetComponent<PhotonView>();
        _myNumber = PhotonNetwork.PlayerList.ToList().IndexOf(PhotonNetwork.LocalPlayer);
        if (!isAuto)
        {
            _myNumber = 0;
        }
        ColorButtonCreate();

        _panel = new Panel(_panelMainObj.transform.Find("Menu/Content"));
        _subPanel = new SubPanel(_panelSubObj.transform.Find("Content"));
        _resourcePanel = new ResourcePanel(_panelMainObj.transform.Find("Resource/Content").gameObject);
        _pointPanel = new PointPanel(_panelMainObj.transform.Find("Point"));

        SetEvent("City", 0);
        SetEvent("Highway", 1);
        SetHarborEvent();

        if (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName) || PhotonNetwork.LocalPlayer.NickName == "Player")
        {
            myName = "Player" + _myNumber;
        }
        else
        {
            myName = PhotonNetwork.LocalPlayer.NickName;
        }


        //初期化
        _buildCount = new List<int>() { 5, 15, 4 };
        _setupBuildList = new List<int>() { 0, 0 };
        if (isAuto)
        {
            _gameUI.SetActive(false);
            _bord.SetActive(false);
            _colorObj.SetActive(true);
        }
        _subPanel.Display(false);
        for (int i = 0; i < 5; i++)
        {
            _resourceList.Add(i, 0);
        }
        SetNegoEvent();//_resourceListが必要
        for (int i = 0; i < 4; i++)
            _myWinPoint.Add(0);

        _menuToggle.isOn = false;
        _menuToggle.isOn = true;

        _playerCount = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < _playerCount; i++)
        {
            _playerCardCount.Add(0);
        }
        _isGameNow = true;

        //てすと
        //for (int i = 0; i < 5; i++)
        {
            //    _resourceList[i] = 15;
        }
        //for (int i = 0; i < _deveCardData.sheet.Count; i++)
        {
            //    _deveCard.Add(i);
            //    _deveCard.Add(i);
        }
    }

    private void SetBord()
    {
        List<int> token = new List<int>()
            {
                2,3,3,4,4,5,5,6,6,8,8,9,9,10,10,11,11,12
            };
        List<int> resourceId = new List<int>()
            {
                0,0,0,
                1,1,1,1,
                2,2,2,2,
                3,3,3,3,
                4,
                5,4,4
            };
        token = token.OrderBy(a => Guid.NewGuid()).ToList();
        resourceId = resourceId.OrderBy(a => Guid.NewGuid()).ToList();
        _deveCardDeck = _deveCardDeck.OrderBy(a => Guid.NewGuid()).ToList();
        _view.RPC("P_SetBord", RpcTarget.AllBufferedViaServer, token.ToArray(), resourceId.ToArray(), _deveCardDeck.ToArray());
    }
    [PunRPC]
    private void P_SetBord(int[] token, int[] resourceId, int[] deveCardDeck)
    {
        _deveCardDeck.CopyTo(deveCardDeck);
        int i = 0;
        int tokenCount = 0;
        int skipId = 5;
        foreach (Transform child in _tileParent.transform)
        {
            child.GetComponent<Renderer>().material = _resourceData.sheet[resourceId[i]].material;
            var tile = child.GetComponent<Tile>();
            _tileList.Add(tile);
            if (resourceId[i] == skipId)
            {
                child.name = "砂漠";
                child.Find("Token").GetComponent<TextMesh>().text = "";
                skipId = -1;
                tile.resourceId = 5;
                desert = tile;
                P_SetDesert(_tileList.IndexOf(tile), true);
            }
            else
            {
                child.name = token[tokenCount].ToString();
                child.Find("Token").GetComponent<TextMesh>().text = token[tokenCount].ToString();
                tile.resourceId = resourceId[i];
                tile.token = token[tokenCount];
                child.GetComponent<Renderer>().material = _resourceData.sheet[resourceId[i]].material;
                tokenCount++;
            }
            i++;

            child.gameObject.AddComponent<EventTrigger>();
            EventTrigger trigger = child.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) =>
            {
                TileClick(tile);
            });
            trigger.triggers.Add(entry);
        }
        SetTileOutLine(false);
    }

    private void SetEvent(string objName, int buildId)
    {
        var objLi = new List<GameObject>();
        var bObjLi = new List<BordObject>();
        foreach (Transform c in _bord.transform.Find(objName))
        {
            foreach (Transform child in c)
            {
                child.gameObject.AddComponent<EventTrigger>();
                EventTrigger trigger = child.GetComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) =>
                {
                    BordClick(child.gameObject, buildId);
                });
                trigger.triggers.Add(entry);
                objLi.Add(child.gameObject);

                var su = child.GetComponent<BordObject>(); ;
                su.spriteRenderer = child.GetComponent<SpriteRenderer>();
                su.spriteRenderer.material = _bordDefaultMate;
                bObjLi.Add(su);
            }
        }
        _bordObjList.Add(objLi);
        _bordBaseObjList.Add(bObjLi);
    }
    private void SetHarborEvent()
    {
        var harborLi = GameObject.FindGameObjectsWithTag("Harbor").ToList();
        foreach (var harborObj in harborLi)
        {
            var harbor = harborObj.GetComponentInChildren<Harbor>();
            var itemSr = harborObj.transform.Find("Item");
            var sprite = itemSr.Find("Sprite").GetComponent<SpriteRenderer>();
            var token = itemSr.Find("Token");
            var foreignCount = 3;
            if (harbor.isExpert)
            {
                sprite.sprite = _resourceData.sheet[harbor.resourceId].item;
                foreignCount = 2;
            }
            sprite.gameObject.SetActive(harbor.isExpert);//専門港だったらtrue
            token.gameObject.SetActive(!harbor.isExpert);//一般港だったらtrue

            var colliderObj = harborObj.transform.Find("Collider").gameObject;
            colliderObj.AddComponent<EventTrigger>();
            EventTrigger trigger = colliderObj.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) =>
            {
                bool isAroundCity = harbor.aroundCity.Any(c => c.isSomeone && c.playerId == _myNumber);
                if (_isMyTurn && isAroundCity && !_isSetupBuild)
                {
                    _isForeignTrade = true;
                    _foreignCount = foreignCount;
                    _foreignResourceId = harbor.resourceId;
                    _isForeignExpart = harbor.isExpert;
                    _subPanel.foreignOutText.text = foreignCount + _subPanel.foreignOutText.text.Remove(0, 1);
                    Nego(false);
                }
            });
            trigger.triggers.Add(entry);
        }
    }

    private void ColorButtonCreate()
    {
        foreach (Transform child in _colorSelectParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var color in _playerColorDef)
        {
            var content = Instantiate(_colorSelectButton, _colorSelectParent.transform);
            content.GetComponent<Image>().color = color;

            var button = content.GetComponent<Button>();
            button.onClick.AddListener(() => ColorSelect(_playerColorDef.IndexOf(color)));
            _colorButtons.Add(button);
        }
    }

    //Button
    private void ColorSelect(int id)
    {
        _view.RPC("P_ColorSelect", RpcTarget.AllViaServer, id, _myNumber);
        _notColorSelect.transform.Find("ColorImage").GetComponent<Image>().color = _playerColorDef[id];
        _notColorSelect.SetActive(true);
    }
    public void ColorCancel()
    {
        _view.RPC("P_ColorCancel", RpcTarget.All, 0, _myNumber);
        _notColorSelect.SetActive(false);
    }
    [PunRPC]
    private void P_ColorSelect(int id, int from)
    {
        _colorButtons[id].interactable = false;
        _colorButtons[id].transform.Find("Text").GetComponent<Text>().text = GetOtherName(from);
        _colorSelectPlayer++;
        if (_playerColor.ContainsKey(from))
        {
            _playerColor[from] = id;
        }
        else
        {
            _playerColor.Add(from, id);
        }
        if (_colorSelectPlayer == PhotonNetwork.PlayerList.Length && PhotonNetwork.IsMasterClient)
        {
            SetBord();
            if (isDebug)
            {
                GameStart();//デバッグ用
            }
            else
            {
                _startButton.SetActive(true);
            }
        }
    }
    [PunRPC]
    private void P_ColorCancel(int id, int from)
    {
        _colorButtons[id].interactable = true;
        _colorButtons[id].transform.Find("Text").GetComponent<Text>().text = "";
        _colorSelectPlayer--;
        _playerColor[from] = -1;
        if (PhotonNetwork.IsMasterClient)
        {
            _startButton.SetActive(false);
        }
    }

    public void GameStart()
    {
        _view.RPC("P_GameStart", RpcTarget.All);
        _view.RPC("P_LogShare", RpcTarget.AllViaServer, "GameStart");
        _view.RPC("P_NextTurn", RpcTarget.AllViaServer, 0);
    }
    [PunRPC]
    private void P_GameStart()
    {
        _bord.SetActive(true);
        SetNameList();
        NegoListenerEvent();
        _gameUI.SetActive(true);
        _colorObj.SetActive(false);
        LogClear();
        ResourceCountUpdate();
    }
    private void SetNameList()
    {
        foreach (Transform child in _playerNameParent.transform.Find("Tab/List"))
        {
            Destroy(child.gameObject);
        }

        _playerColor.OrderBy(c => c.Key);
        foreach (var v in _playerColor)
        {
            var content = Instantiate(_playerNameContent, _playerNameParent.transform.Find("Tab/List"));
            content.transform.Find("Name").GetComponent<Text>().text = GetOtherName(v.Key);
            content.transform.Find("Arrow").gameObject.SetActive(false);
            content.transform.Find("Image").GetComponent<Image>().color = _playerColorDef[v.Value];

            _nameList.Add(v.Key, content);
            _playerCardCountText.Add(v.Key, content.transform.Find("CardCount").GetComponent<Text>());
        }
    }
    private List<int> Sort(int count, int target)
    {
        var serial = Enumerable.Range(0, count).ToList();
        var ret = new List<int>();
        for (int i = 0; i < serial.Count; i++)
        {
            if (i + target < serial.Count)
            {
                ret.Add(serial[i + target]);
            }
            else
            {
                ret.Add(serial[Math.Abs(count - target - i)]);
            }
        }
        return ret;
    }

    public void PlayerListDisplay()
    {
        if (_playerListDisplayAni) return;

        _playerListDisplayAni = true;
        float scale = 1f;
        if (_playerListDisplay)
        {
            scale = 0f;
        }
        var button = _playerNameParent.transform.Find("CloseButton");
        var bPos = (button.transform as RectTransform).anchoredPosition;

        button.DOLocalMove(new Vector2(-bPos.x, -bPos.y), _animationSpeed);

        (_playerNameParent.transform.Find("Tab") as RectTransform)
            .DOScale(scale, _animationSpeed)
            .OnComplete(() => _playerListDisplayAni = false);
        _playerListDisplay = !_playerListDisplay;
    }
    public void SubDisplayClose()
    {
        _subPanel.Display(false);
        ResetBuild();
    }

    private void LogClear()
    {
        foreach (Transform child in _logContent.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void LocalLog(string log)
    {
        var content = Instantiate(_logPrefab, _logContent.transform);
        content.GetComponent<Text>().text = log;
    }
    [PunRPC]
    private void P_LogShare(string log)
    {
        var content = Instantiate(_logPrefab, _logContent.transform);
        content.GetComponent<Text>().text = log;
    }

    [PunRPC]
    private async void P_NextTurn(int to)
    {
        _panel.Display(false);
        _subPanel.Display(false);
        _isMyTurn = false;
        if (isAuto)
        {
            ArrowSet(to);
        }
        else
        {
            _isOrdering = true;
        }
        if (_myNumber == to)
        {
            LocalLog("あなたのターンです");
            _menuToggle.isOn = true;
            SubCamera(false);
            WinCheck();
            _isMyTurn = true;
            _isDeve = false;
            _deveCard.AddRange(_deveCardNow);
            _deveCard.Sort();
            _deveCardNow.Clear();
            _frameObj.SetActive(false);
            if (PhotonNetwork.IsMasterClient && !_isOrdering && _isFirstDice)//マスターで順番を決めてなかったら
            {
                var maxElem = _firstDiceResult.FirstOrDefault(c => c.Value == _firstDiceResult.Values.Max());
                var li = Sort(_playerCount, maxElem.Key);
                _view.RPC("P_Order", RpcTarget.All, li.ToArray());
                _isOrdering = true;
                _view.RPC("P_NextTurn", RpcTarget.All, _playerOrder[0]);
                _view.RPC("P_LogShare", RpcTarget.All, myName + " First");
                return;
            }

            if (_isSetupBuild && _isFirstDice)
            {
                LocalLog("開拓地を建設してください");
                _panel.build.interactable = true;
                _subPanel.Display(true, _subPanel.buildButtons);
                _isSetupBuildLi = new List<bool>() { false, false };
                await UniTask.WaitUntil(() => _isSetupBuildLi[0] && _isSetupBuildLi[1]);//町と道を一つづつ作ったら
                _subPanel.build.SetActive(false);
                if (_setupBuildList[0] == 2 && _setupBuildList[1] == 2)
                {
                    var tiles = _tileList.Where(tile => tile.aroundCity.Find(city => city == _setupObjList[0][1].gameObject)).ToList();
                    var resourceList = tiles.Select(tile => tile.resourceId).ToList();
                    resourceList.RemoveAll(r => r == 5);
                    var resourceResult = new List<int>() { 0, 0, 0, 0, 0 };
                    for (int i = 0; i < resourceList.Count; i++)
                    {
                        resourceResult[resourceList[i]]++;
                    }

                    AddResource(resourceResult);
                    _isSetupBuild = false;
                    if (_playerOrder[0] == _myNumber)
                    {
                        _order = 0;
                        print("SetUpLastPlayer");
                    }
                }
                if (_setupBuildList[0] == 1 && _setupBuildList[1] == 1 && _playerOrder[_playerOrder.Count - 1] == _myNumber)
                {
                    print("最後");
                    _order = 0;
                }
                _panel.Display(false);
                _panel.turnEnd.interactable = true;
                //初めの建設だけ
                return;
            }

            _panel.dice.interactable = true;
            if (!_isFirstDice)
            {
                LocalLog("順番を決めるためにダイスを振ってください");
            }
            if (isDebug)
            {
                DiceClick();
            }
            _diceClick = false;
            _diceResult = 0;
            await UniTask.WaitUntil(() => _diceClick);
            System.Random r = new System.Random();
            _view.RPC("P_LogShare", RpcTarget.All, myName + " Dice：" + _diceResult);
            if (_isFirstDice)
            {
                if (_diceResult == 7)
                {
                    _panel.Display(false);
                    SoldierCard(false);
                }
                else
                {
                    _view.RPC("P_AddResource", RpcTarget.All, _diceResult);
                    _panel.Display(true);
                    _panel.dice.interactable = false;
                }
                //他の処理
            }
            else//セットアップ(ダイス振るだけ)
            {
                _isFirstDice = true;
                _view.RPC("P_FirstDice", RpcTarget.MasterClient, _diceResult, _myNumber);
                if (isAuto)
                    _view.RPC("P_NextTurn", RpcTarget.All, GetWithinRange(_playerCount, to + 1));
                else
                    P_NextTurn(0);
            }
        }
        else
        {
            _frameObj.SetActive(true);
            _frameObj.GetComponent<Image>().color = _playerColorDef[_playerColor[to]];
        }
    }
    [PunRPC]
    private void P_FirstDice(int result, int from)
    {
        _firstDiceResult.Add(from, result);
    }
    [PunRPC]
    private void P_Order(int[] order)
    {
        _playerOrder = order.ToList();
        foreach (var num in _playerOrder)
        {
            _nameList[num].transform.SetSiblingIndex(_playerOrder.IndexOf(num));
        }
    }

    private void ArrowSet(int id)
    {
        foreach (var content in _nameList)
        {
            content.Value.transform.Find("Arrow").gameObject.SetActive(false);
        }
        _nameList[id].transform.Find("Arrow").gameObject.SetActive(true);
    }

    public void DiceClick()
    {
        DiceRoll();
    }
    private async void DiceRoll()
    {
        _view.RPC("P_DiceRoll", RpcTarget.Others);
        SubCamera(true);
        _diceText.gameObject.SetActive(true);
        _isDisplayClick = false;
        _panel.dice.interactable = false;
        _diceText.text = "クリックでダイスを振る";
        await UniTask.WaitUntil(() => _isDisplayClick);//画面クリックするまで待機
        _diceResult = await _diceRoll.Roll();//さいころが止まるまで待機
        _view.RPC("P_DiceComplete", RpcTarget.Others);
        _isDisplayClick = false;
        SubCameraClose();
        await UniTask.WaitUntil(() => _isDisplayClick);//画面クリックするまで待機
        _diceClick = true;
        SubCamera(false);
        Dice.Clear();
    }
    private async void SubCameraClose()
    {
        _diceText.text = "クリックで閉じる(3)";
        await UniTask.Delay(1000);
        _diceText.text = "クリックで閉じる(2)";
        await UniTask.Delay(1000);
        _diceText.text = "クリックで閉じる(1)";
        await UniTask.Delay(1000);
        _isDisplayClick = true;
    }
    [PunRPC]
    private void P_DiceRoll()
    {
        SubCamera(true);
        _diceText.gameObject.SetActive(false);
    }
    [PunRPC]
    private async void P_DiceComplete()
    {
        _diceText.gameObject.SetActive(true);
        SubCameraClose();
        _isDisplayClick = false;
        await UniTask.WaitUntil(() => _isDisplayClick);//画面クリックするまで待機
        SubCamera(false);
    }
    private void SubCamera(bool isDisplay)
    {
        _diceRoll.diceCamera.gameObject.SetActive(isDisplay);
        _diceRoll.diceRoll.SetActive(isDisplay);
    }
    public void DiceDisplayClick()
    {
        _isDisplayClick = true;
    }

    public void TurnEnd()
    {
        _view.RPC("P_LogShare", RpcTarget.All, myName + "ターンエンド");
        if (isAuto)
        {
            _view.RPC("P_NextTurn", RpcTarget.All,
                _playerOrder[GetWithinRange(_playerCount, _playerOrder.IndexOf(_myNumber) + _order)]);
            if (_order == 0 && !_isOrderRevers)
            {
                _view.RPC("P_OrderChange", RpcTarget.All, -1);
                _isOrderRevers = true;
            }
            else if (_order == 0 && _isOrderRevers)
            {
                _view.RPC("P_OrderChange", RpcTarget.All, 1);
            }
        }
        else
            P_NextTurn(0);
        _view.RPC("P_ForceExchangeEnd", RpcTarget.All);
        _subPanel.Display(false);
    }
    [PunRPC]
    private void P_OrderChange(int order)
    {
        _order = order;
        _isOrderRevers = true;
    }

    #region 建設関係
    public void Build()
    {
        _isBuildSelect = true;
        _subPanel.Display(_subPanel.build);
        BuildPossibleCheck();
    }
    private void BuildPossibleCheck()//資源が足りないとき、建設場所がないとき
    {
        _subPanel.Display(true, _subPanel.buildButtons);
        if (_isSetupBuild) //セットアップ時
        {
            if (!_isSetupBuildLi[0])//開拓地をまだ配置していないとき
                _subPanel.buildButtons[1].interactable = false;
            else//開拓地を配置したとき
                _subPanel.buildButtons[0].interactable = false;

            foreach (var button in _subPanel.buildButtons)
            {
                button.transform.Find("ErrorText").gameObject.SetActive(false);
            }
            _subPanel.buildButtons[2].interactable = false; //街のボタンを押せないように
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                IsResource(_subPanel.buildButtons[i], _resourceConsumption[i]);
            }

        }
    }
    private void IsResource(Button button, List<int> resource)
    {
        bool isInteractable = true;
        for (int i = 0; i < resource.Count; i++)
        {
            if (_resourceList[i] < resource[i] && resource[i] != 0)
            {
                isInteractable = false;
            }
        }
        button.interactable = isInteractable;
        if (button.transform.Find("ErrorText"))
            button.transform.Find("ErrorText").gameObject.SetActive(!isInteractable);
    }
    public async void BuildInfo(int buildId)
    {
        if (_buildSelectId == buildId) return;
        _buildInfo++;
        int buildInfoTmp = _buildInfo;
        SetOutLine(false, 0);//アウトライン非表示
        SetOutLine(false, 1);//アウトライン非表示
        _isBuild = false;
        _buildSelectId = buildId;

        LocalLog("建設場所を選んでください");
        if (buildId == 0)
        {
            if (_isSetupBuild)//セットアップ
            {
                if (buildId == 0)
                {
                    SetOutLine(true, buildId);//アウトライン表示
                    var notOutLine = BordImpossibleObj<BordObject>(buildId);
                    foreach (var obj in notOutLine)
                    {
                        obj.spriteRenderer.material = _bordDefaultMate;
                    }
                }
            }
            else//通常
            {
                var result = new List<GameObject>();

                var aroundCity = _myHighway
                    .Select(b => b.GetComponent<Highway>().aroundCity);

                foreach (var city in aroundCity)
                {
                    result = result.Union(city).ToList();
                }

                result = result
                    .Select(c => c.GetComponent<City>())
                    .Where(c => !c.isSomeone && !c.aroundCity.Any(city => city.GetComponent<City>().isSomeone))
                    .Select(c => c.gameObject)
                    .ToList();

                SetOutLine(true, buildId, result.Select(c => c.GetComponent<BordObject>()).ToList());
            }
        }
        else if (buildId == 1)//道
        {
            if (_isSetupBuild)
            {
                if (_setupObjList[0].Count != 0)
                {
                    SetOutLine(true, buildId, _setupObjList[0][_setupObjList[0].Count - 1].GetComponent<City>().aroundHighway.Select(highway => highway.GetComponent<BordObject>()).ToList());
                }
            }
            else
            {
                var myObj = _myHighway.Select(c => c.gameObject);
                var possible = _bordBaseObjList[1]
                    .Where(b => (b as Highway).aroundHighway
                    .FindAll(myObj.Contains).Count != 0 && !b.isSomeone)
                    .ToList();
                SetOutLine(true, 1, possible);
            }
        }
        else if (buildId == 2)//都市
        {
            SetOutLine(true, buildId, _myCity.Where(c => c._resoureYieldCount == 1).Select(c => (c as BordObject)).ToList());
        }

        await UniTask.WaitUntil(() => _buildSelectId == -1);//スプライトを選択するまで待機

        SetOutLine(false, buildId);//アウトライン非表示
        if (_isBuild && _buildInfo == buildInfoTmp || _isSkillBuild)
        {
            if (_isSetupBuild)//セットアップ
            {
                _setupBuildList[buildId]++;
                _subPanel.buildButtons[buildId].interactable = false;
                _isSetupBuildLi[buildId] = true;
                if (buildId == 0)
                {
                    LocalLog("街道を建設してください");
                }
            }
            else if (_isSkillBuild)
            {
                _skillBuildCount++;
                if (_skillBuildCount == 2)
                {
                    _isSkillBuild = false;
                    DeveComplete();
                }
                else
                {
                    BuildInfo(1);
                }
            }
            else
            {
                RemoveResource(_resourceConsumption[buildId]);
            }
            _buildCount[buildId]--;
            BuildPossibleCheck();
            _buildInfo = 0;
        }
    }
    public void BordClick(GameObject clickObj, int buildId)
    {
        if (_isBuildSelect)//建設選択中にクリックされたら
        {
            bool isPossible = true;
            if (buildId == 0)
            {
                var notOutLine = BordImpossibleObj<GameObject>(buildId);
                if (notOutLine.Contains(clickObj))
                    isPossible = false;
            }

            var bordObj = clickObj.GetComponent<BordObject>();
            if (_buildSelectId == buildId && !bordObj.isSomeone && bordObj.isBuild && isPossible)
            {
                bordObj.playerId = _myNumber;
                bordObj.isSomeone = true;
                if (clickObj.TryGetComponent(out City c))
                    c._resoureYieldCount = 1;
                clickObj.GetComponent<SpriteRenderer>().color = GetMyColor();
                _view.RPC("P_BuildShare", RpcTarget.Others, buildId, _bordObjList[buildId].IndexOf(clickObj), _myNumber);
                _isBuild = true;
                _buildSelectId = -1;
                _setupObjList[buildId].Add(clickObj);
                if (buildId == 0)
                {
                    _myCity.Add(clickObj.GetComponent<City>());
                    ChangePoint(0);
                }
                else if (buildId == 1)
                {
                    _myHighway.Add(clickObj.GetComponent<Highway>());
                }
            }
            if (_buildSelectId == 2 && buildId == 0 && bordObj.isSomeone && bordObj.playerId == _myNumber)
            {
                print("都市に進化");
                clickObj.GetComponent<City>()._resoureYieldCount = 2;
                _isBuild = true;
                _buildSelectId = -1;
                var flag = Instantiate(_fragObj, clickObj.transform);
                flag.GetComponent<SpriteRenderer>().color = GetMyColor();
                _buildCount[0]++;//開拓地を一つ返還
                _view.RPC("P_CityShare", RpcTarget.Others, _bordObjList[0].IndexOf(clickObj), _myNumber);
                LocalLog("都市に進化しました");
                LocalLog("開拓地が1つ返還されました");
                ChangePoint(0, -1);
                ChangePoint(1);
            }
        }
        if (buildId == 0 || buildId == 2)
        {
            SelectPlayer(clickObj);
        }
    }
    private void ResetBuild()
    {
        _isBuildSelect = false;
        _isBuild = false;
        SetOutLine(false, 0);
        SetOutLine(false, 1);
        _buildSelectId = -1;
        _buildInfo = 0;
    }
    [PunRPC]
    private void P_BuildShare(int buildId, int buildNumber, int from)
    {
        GameObject obj = _bordObjList[buildId][buildNumber];
        obj.GetComponent<SpriteRenderer>().color = GetOtherColor(from);
        var bordObj = obj.GetComponent<BordObject>();
        bordObj.playerId = from;
        bordObj.isSomeone = true;
        if (buildId == 0)
        {

            LocalLog(GetOtherName(from) + "が開拓地を建設しました");
        }
        else if (buildId == 1)
        {
            LocalLog(GetOtherName(from) + "が街道を建設しました");
        }
    }
    [PunRPC]
    private void P_CityShare(int buildNumber, int from)
    {
        var flag = Instantiate(_fragObj, _bordObjList[0][buildNumber].transform);
        flag.GetComponent<SpriteRenderer>().color = GetOtherColor(from);
        LocalLog(GetOtherName(from) + "が都市を建設しました");
    }
    #endregion

    public void Sammary()
    {
        _subPanel.Display(_subPanel.summary);
    }

    #region 発展
    public void Deve()
    {
        if (_isDeveNow)
        {
            _subPanel.Display(_subPanel.development);
            _subPanel.DeveDisplay(_deveId);
        }
        else
        {
            DeveCardReset();
            _subPanel.Display(_subPanel.development);
            _subPanel.DeveDisplay(_subPanel.deveDefault);
        }
    }
    private void DeveCardReset()
    {
        foreach (Transform child in _subPanel.deveContent.transform)
        {
            Destroy(child.gameObject);
        }

        _deveCard.Sort();
        _deveCardNow.Sort();
        var winCard = _deveCard.Where(card => card == 4).Count();

        foreach (var item in _deveCard)
        {
            if (item != 4)//勝利ポイント以外
            {
                var content = Instantiate(_deveCardData.sheet[item].obj, _subPanel.deveContent.transform);
                var button = content.GetComponent<Button>();
                button.interactable = !_isDeve;
                int i = item;
                button.onClick.AddListener(() => CardSkill(i));
            }
        }

        foreach (var item in _deveCardNow)
        {
            var content = Instantiate(_deveCardData.sheet[item].obj, _subPanel.deveContent.transform);
            content.GetComponent<Button>().interactable = false;
        }
        for (int i = 0; i < winCard; i++)
        {
            var content = Instantiate(_deveCardData.sheet[4].obj, _subPanel.deveContent.transform);
            content.GetComponent<Button>().interactable = false;
        }
        IsResource(_subPanel.buyButton, _resourceConsumption[3]);
    }
    public void BuyDeve()
    {
        var cardId = _deveCardDeck[0];
        _deveCardNow.Add(cardId);
        _view.RPC("P_BuyDeve", RpcTarget.All);
        _view.RPC("P_LogShare", RpcTarget.All, myName + "が発展カードを引いた");
        LocalLog(_deveCardData.sheet[cardId].cardName + "を引いた");
        RemoveResource(_resourceConsumption[3]);
        DeveCardReset();
        if (cardId == 4)
        {
            ChangePoint(3);
        }
    }
    [PunRPC]
    private void P_BuyDeve()
    {
        _deveCardDeck.RemoveAt(0);
    }
    private void CardSkill(int id)
    {
        _subPanel.closeButton.interactable = false;
        _isDeveNow = true;
        _deveId = id;
        DeveCardReset();

        _panel.Display(false);
        _panel.summary.interactable = true;
        _panel.development.interactable = true;
        _subPanel.Display(_subPanel.development);
        _subPanel.DeveDisplay(_deveId);

        _view.RPC("P_LogShare", RpcTarget.Others, myName + "が" + _deveCardData.sheet[id].cardName + "カードを使用しました");
        switch (id)
        {
            case 0:
                {
                    _panel.Display(false);
                    LocalLog("騎士カードを使用しました");
                    SoldierCard(true);
                    break;
                }
            case 1:
                {
                    _skillBuildCount = 0;
                    _isSkillBuild = true;
                    LocalLog("街道を2つ建設してください");
                    BuildInfo(1);
                    break;
                }
            case 2:
                {
                    LocalLog("カードを2枚選択してください");
                    Harvest();
                    break;
                }
            case 3:
                {
                    LocalLog("独占するカードを選択してください");
                    Monopoly();
                    break;
                }

        }
    }
    private void DeveComplete()
    {
        _panel.Display(true);
        _panel.dice.interactable = false;
        _subPanel.closeButton.interactable = true;
        _isDeve = true;
        _isDeveNow = false;
        _deveCard.Remove(_deveId);
        _deveId = -1;
        Deve();
    }
    private void Harvest()
    {
        _subPanel.deveDefault.SetActive(false);
        _subPanel.harvest.SetActive(true);
        HarvestReset();
        SetHarvestEvent();
        HarvestCheck();
    }
    private void HarvestReset()
    {
        _harvestList.Clear();
        for (int i = 0; i < 5; i++)
        {
            _harvestList.Add(0);
            _subPanel.harvestCount[i].text = "0";
        }
    }
    private void SetHarvestEvent()
    {
        for (int i = 0; i < 5; i++)
        {
            int ii = i;
            _subPanel.harvestPlus[ii].onClick.AddListener(() =>
            {
                HarvestResourceValueChange(ii, +1, _subPanel.harvestCount[ii]);
            });
            _subPanel.harvestMinus[ii].onClick.AddListener(() =>
            {
                HarvestResourceValueChange(ii, -1, _subPanel.harvestCount[ii]);
            });
        }
    }
    private void HarvestResourceValueChange(int resource, int code, Text text)
    {
        _harvestList[resource] += code;
        text.text = _harvestList[resource].ToString();
        HarvestCheck();
        //ExChangeCheck();
    }
    private void HarvestCheck()
    {
        if (_harvestList.Sum() == 2)
        {
            for (int i = 0; i < _harvestList.Count; i++)
            {
                _subPanel.harvestPlus[i].interactable = false;
                _subPanel.harvestMinus[i].interactable = false;
                if (_harvestList[i] != 0)
                {
                    _subPanel.harvestMinus[i].interactable = true;
                }
            }
            _subPanel.harvestConfirm.interactable = true;
        }
        else
        {
            for (int i = 0; i < _harvestList.Count; i++)
            {
                var resource = _harvestList[i];
                _subPanel.harvestPlus[i].interactable = true;
                _subPanel.harvestMinus[i].interactable = false;
                if (resource == 1)
                {
                    _subPanel.harvestPlus[i].interactable = true;
                    _subPanel.harvestMinus[i].interactable = true;
                }
                else if (resource == 2)
                {
                    _subPanel.harvestPlus[i].interactable = false;
                    _subPanel.harvestMinus[i].interactable = true;
                }
            }
            _subPanel.harvestConfirm.interactable = false;
        }
    }
    public void HarvestConfirm()
    {
        AddResource(_harvestList);
        LocalLog("収穫カードを使用しました");
        var msg = "";
        for (int i = 0; i < _harvestList.Count; i++)
        {
            var resource = _harvestList[i];
            if (resource != 0)
            {
                msg += _resourceData.sheet[i].resourceName + " : " + resource + " ";
            }
        }
        LocalLog(msg.Trim() + "入手した");
        DeveComplete();
    }

    private void Monopoly()
    {
        _subPanel.DeveDisplay(_subPanel.monopoly);
        _panel.Display(false);
    }
    public void MonopolySelect(int resourceId)
    {
        _view.RPC("P_LogShare", RpcTarget.Others, myName + "が" + _resourceData.sheet[resourceId].resourceName + "を独占しました");
        _view.RPC("P_Monopoly", RpcTarget.Others, resourceId, _myNumber);
    }
    [PunRPC]
    private void P_Monopoly(int resourceId, int from)
    {
        int count = _resourceList[resourceId];
        _view.RPC("P_MonopolyAcceptance", RpcTarget.All, count, resourceId, from, myName);
        RemoveResource(resourceId, count);
        LocalLog(GetOtherName(from) + "に" + _resourceData.sheet[resourceId].resourceName + "を" + count + "個奪われました");
    }
    [PunRPC]
    private void P_MonopolyAcceptance(int count, int resourceId, int to, string name)
    {
        if (to == _myNumber)
        {
            AddResource(resourceId, count);
            LocalLog(name + "から" + count + "枚奪いました");
            _monopolyCompleteCount++;
            if (_playerCount - 1 == _monopolyCompleteCount)//全員から奪ったら
            {
                DeveComplete();
                LocalLog("全員から奪いました");
                _view.RPC("P_LogShare", RpcTarget.Others, name + "が全員からうばいました");
            }
        }
    }
    #endregion

    #region 交渉
    public void Nego(bool isNego)
    {
        _isForeignTrade = !isNego;
        _subPanel.foreignTradeObj.SetActive(_isForeignTrade);
        NegoReset(-1);
        _subPanel.Display(_subPanel.negotiation);
        _resourceToggle.isOn = true;
        _subPanel.selectPlayer.SetActive(false);
        _subPanel.card.gameObject.SetActive(true);
    }
    private void NegoResourceReset(int id)
    {
        var dic = new Dictionary<int, int>();
        switch (id)
        {
            case -1:
                _negoResourceList.Clear();
                for (int i = 0; i < 2; i++)
                {
                    dic = new Dictionary<int, int>();
                    for (int j = 0; j < 5; j++)
                    {
                        dic.Add(j, 0);
                        _subPanel.negoText[i][j].text = "0";
                    }
                    _negoResourceList.Add(dic);
                }
                break;
            case 0:
            case 1:
                for (int i = 0; i < 5; i++)
                {
                    dic.Add(i, 0);
                    _subPanel.negoText[id][i].text = "0";

                }
                _negoResourceList[id] = dic;

                break;
        }
    }
    private void ExChangeResourceCheck()
    {
        if (_isForeignTrade)//海外貿易
        {
            var outPlus = _subPanel.negoButton[0].plus;
            var outMinus = _subPanel.negoButton[0].minus;
            for (int i = 0; i < _negoResourceList[0].Count; i++)
            {
                outPlus[i].interactable = false;
                outMinus[i].interactable = false;
            }
            if (_negoResourceList[0].Any(c => c.Value != 0))
            {
                var outSelect = _negoResourceList[0].FirstOrDefault(c => c.Value != 0);
                if (outSelect.Value == 0)//選択されている資材が0だからマイナスにはできない
                {
                    outPlus[outSelect.Key].interactable = true;
                    outMinus[outSelect.Key].interactable = false;
                }
                else if (outSelect.Value == _foreignCount)//選択されている資材が最大値だからプラスにできない
                {
                    outPlus[outSelect.Key].interactable = false;
                    outMinus[outSelect.Key].interactable = true;
                }
                else
                {
                    outPlus[outSelect.Key].interactable = true;
                    outMinus[outSelect.Key].interactable = true;
                }
            }
            else
            {
                if (_isForeignExpart)
                {
                    for (int i = 0; i < _negoResourceList[0].Count; i++)
                    {
                        outPlus[i].interactable = false;
                        outMinus[i].interactable = false;
                    }
                    outPlus[_foreignResourceId].interactable = true;
                }
                else
                {
                    for (int i = 0; i < _negoResourceList[0].Count; i++)
                    {
                        outPlus[i].interactable = true;
                        outMinus[i].interactable = false;
                    }
                }
            }


            var inPlus = _subPanel.negoButton[1].plus;
            var inMinus = _subPanel.negoButton[1].minus;
            if (_negoResourceList[1].Any(c => c.Value != 0))//一つでも0以外があれば
            {
                var select = _negoResourceList[1].Select(c => c.Value).ToList().Find(c => c == 1);
                for (int i = 0; i < _negoResourceList[1].Count; i++)
                {
                    if (i == select)
                    {
                        inMinus[i].interactable = true;
                    }
                    else
                    {
                        inMinus[i].interactable = false;
                    }
                    inPlus[i].interactable = false;
                }
            }
            else
            {
                for (int i = 0; i < _negoResourceList[1].Count; i++)
                {
                    inPlus[i].interactable = true;
                    inMinus[i].interactable = true;
                }
            }
        }
        else//国内貿易
        {
            for (int i = 0; i < 2; i++)
            {
                var plus = _subPanel.negoButton[i].plus;
                var minus = _subPanel.negoButton[i].minus;

                foreach (var resource in _negoResourceList[i])
                {
                    if (resource.Value == 0)
                    {
                        minus[resource.Key].interactable = false;
                    }
                    if (resource.Value != 0)
                    {
                        minus[resource.Key].interactable = true;
                    }
                    if (i == 0)
                    {
                        if (resource.Value == _resourceList[resource.Key] || _resourceList[resource.Key] == 0)//選択している資材が所持している資材の数と同じだったら
                        {
                            plus[resource.Key].interactable = false;
                        }
                        else
                        {
                            plus[resource.Key].interactable = true;
                        }
                    }
                }


            }

        }
    }
    private void NegoResourceValueChange(int negoId, int resource, int code, Text text)
    {
        _negoResourceList[negoId][resource] += code;
        text.text = _negoResourceList[negoId][resource].ToString();
        ExChangeResourceCheck();
        ExChangeCheck();
    }
    private void SetNegoEvent()
    {
        for (int i = 0; i < 2; i++)
        {
            foreach (var resource in _resourceList)
            {
                int ii = i;
                _subPanel.negoButton[i].plus[resource.Key].onClick.AddListener(() =>
                {
                    NegoResourceValueChange(ii, resource.Key, +1, _subPanel.negoText[ii][resource.Key]);
                });
                _subPanel.negoButton[i].minus[resource.Key].onClick.AddListener(() =>
                {
                    NegoResourceValueChange(ii, resource.Key, -1, _subPanel.negoText[ii][resource.Key]);
                });
            }
        }
    }
    private void ExChangeCheck()
    {
        if (_isForeignTrade)
        {
            if (_negoResourceList[0].Sum(d => d.Value) == _foreignCount && _negoResourceList[1].Sum(d => d.Value) == 1)//出が2以上&&入が1以上
            {
                _subPanel.resourceConfirm.interactable = true;
            }
            else
            {
                _subPanel.resourceConfirm.interactable = false;
            }
        }
        else
        {
            if (_negoResourceList[0].Sum(d => d.Value) != 0 && _negoResourceList[1].Sum(d => d.Value) != 0)//出が1以上&&入が1以上
            {
                _subPanel.resourceConfirm.interactable = true;
            }
            else
            {
                _subPanel.resourceConfirm.interactable = false;
            }
        }
    }
    public void ExChange()
    {
        if (_isForeignTrade)
        {
            RemoveResource(_negoResourceList[0].Select(re => re.Value).ToList());
            AddResource(_negoResourceList[1].Select(re => re.Value).ToList());
            var remove = _negoResourceList[0].Where(re => re.Value != 0).FirstOrDefault();
            var add = _negoResourceList[1].Where(re => re.Value != 0).FirstOrDefault();
            _view.RPC("P_LogShare", RpcTarget.All, myName + "が" + ResourceToString(remove.Key, remove.Value) + "を" + ResourceToString(add.Key, add.Value) + "に交換した");
            LocalLog(myName + "が" + ResourceToString(remove.Key, remove.Value) + "を" + ResourceToString(add.Key, add.Value) + "に交換した");

        }
        else
        {
            int[] outResource = new int[_negoResourceList[0].Keys.Count];
            _negoResourceList[0].Values.CopyTo(outResource, 0);
            int[] inResource = new int[_negoResourceList[1].Keys.Count];
            _negoResourceList[1].Values.CopyTo(inResource, 0);
            _view.RPC("P_ForceExchangeEnd", RpcTarget.All);
            _view.RPC("P_ExchangeLog", RpcTarget.Others, outResource, inResource, _myNumber, _selectPlayer.ToArray());
            P_ExchangeLog(outResource, inResource, _myNumber, _selectPlayer.ToArray());
        }
        NegoReset(-1);
    }
    public void NegoReset(int id)
    {
        NegoResourceReset(id);
        ExChangeResourceCheck();
        ExChangeCheck();
    }
    [PunRPC]
    private void P_ExchangeLog(int[] outResource, int[] inResource, int from, int[] to)
    {
        var text = "出 ";
        for (int i = 0; i < outResource.Length; i++)
        {
            text += _resourceData.sheet[i].resourceName + ":" + outResource[i].ToString() + " ";
        }
        text = text.Trim();
        var textObj = Instantiate(_logPrefab, _logContent.transform);

        textObj.GetComponent<Text>().text = text;
        text = "入 ";
        for (int i = 0; i < inResource.Length; i++)
        {
            text += _resourceData.sheet[i].resourceName + ":" + inResource[i].ToString() + " ";
        }
        text = text.Trim();
        textObj = Instantiate(_logPrefab, _logContent.transform);

        textObj.GetComponent<Text>().text = text;
        var buttonObj = Instantiate(_exChangeButton, _logContent.transform);
        var button = buttonObj.GetComponent<Button>();
        button.onClick.AddListener(() => ExchangeEstablished(outResource.ToList(), inResource.ToList(), from));
        exchangeButton = button;
        if (0 <= Array.IndexOf(to, _myNumber))
            exchangeButton.interactable = true;
        else
            exchangeButton.interactable = false;

        if (from == _myNumber)
            exchangeButton.interactable = false;

        IsResource(button, inResource.ToList());
    }
    private void ExchangeEstablished(List<int> outResource, List<int> inResource, int from)
    {
        exchangeButton.interactable = false;
        AddResource(outResource);
        RemoveResource(inResource);
        print("交換成立");
        _view.RPC("P_LogShare", RpcTarget.All, myName + "が交換に応じました");
        _view.RPC("P_ExchangeComplete", RpcTarget.All, outResource.ToArray(), inResource.ToArray(), from);
    }
    [PunRPC]
    private void P_ExchangeComplete(int[] outResource, int[] inResource, int from)
    {
        if (from == _myNumber)
        {
            AddResource(inResource.ToList());
            RemoveResource(outResource.ToList());
            print("交渉完了");
        }
        ForceExchangeEnd();
    }
    private void ForceExchangeEnd()
    {
        if (exchangeButton != null)
        {
            exchangeButton.interactable = false;
            exchangeButton = null;
        }
    }
    [PunRPC]
    private void P_ForceExchangeEnd()
    {
        ForceExchangeEnd();
    }
    private void NegoListenerEvent()
    {
        foreach (Transform child in _subPanel.selectPlayerContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var v in _playerColor)
        {
            if (v.Key != _myNumber)
            {
                var content = Instantiate(_selectPlayerObj, _subPanel.selectPlayerContent.transform);
                content.transform.Find("Name").GetComponent<Text>().text = GetOtherName(v.Key);
                content.transform.Find("Image").GetComponent<Image>().color = _playerColorDef[v.Value];
                var toggle = content.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(isOn => NegoPlayerSelect(v.Key, isOn));
                _playerSelectToggle.Add(v.Key, toggle);
            }
        }
    }
    private List<int> _selectPlayer = new List<int>();
    private void NegoPlayerSelect(int id, bool isOn)
    {
        if (isOn)
        {
            _selectPlayer.Add(id);
        }
        else
        {
            _selectPlayer.Remove(id);
        }
        IsExChange();
    }
    public void NegoPlayerSelect(bool isAll)
    {
        ResetSelectPlayer();
        if (isAll)
        {
            foreach (var v in _playerSelectToggle)
            {
                _selectPlayer.Add(v.Key);
                v.Value.isOn = true;
            }
        }
        else
        {
            foreach (var v in _playerSelectToggle)
            {
                v.Value.isOn = false;
            }
        }
    }
    public void IsExChange()
    {
        if (1 <= _selectPlayer.Count)
            _subPanel.exChangeButton.interactable = true;
        else
            _subPanel.exChangeButton.interactable = false;
    }
    public void ResetSelectPlayer()
    {
        _selectPlayer.Clear();
        foreach (var v in _playerSelectToggle)
        {
            v.Value.isOn = false;
        }
    }
    #endregion

    #region 資源関係
    private void AddResource(List<int> resourceLi)
    {
        for (int i = 0; i < resourceLi.Count; i++)
        {
            _resourceList[i] += resourceLi[i];
        }
        ResourceCountUpdate();
    }
    private void AddResource(int resourceId)
    {
        _resourceList[resourceId]++;
        ResourceCountUpdate();
    }
    private void AddResource(int resourceId, int count)
    {
        _resourceList[resourceId] += count;
        ResourceCountUpdate();
    }
    private void RemoveResource(int ResourceId)
    {
        _resourceList[ResourceId]--;
        ResourceCountUpdate();
    }
    private void RemoveResource(int resourceId, int count)
    {
        _resourceList[resourceId] -= count;
        ResourceCountUpdate();
    }
    private void RemoveResource(List<int> resourceLi)
    {
        for (int i = 0; i < resourceLi.Count; i++)
        {
            _resourceList[i] -= resourceLi[i];
        }
        ResourceCountUpdate();
    }
    private void ResourceCountUpdate()
    {
        _cardCount = _resourceList.Sum(r => r.Value);
        _view.RPC("P_ResourceCountUpdate", RpcTarget.All, _myNumber, _cardCount);
        foreach (var count in _resourceList)
        {
            _resourcePanel.texts[count.Key].text = "×" + count.Value.ToString();
        }
        _resourcePanel.texts[_resourcePanel.texts.Count - 1].text = "×" + _resourceList.Sum(r => r.Value);
    }
    [PunRPC]
    private void P_ResourceCountUpdate(int from, int count)
    {
        _playerCardCount[from] = count;
        _playerCardCountText[from].text = count.ToString();
    }
    [PunRPC]
    private void P_AddResource(int token)
    {
        var tileList = _tileList.Where(tile => tile.token == token);
        foreach (var tile in tileList)
        {
            var resourceCount = tile.aroundCity.Select(c => c.GetComponent<City>()).Where(c => c.isSomeone && c.playerId == _myNumber).Sum(c => c._resoureYieldCount);
            var resourceId = tile.GetComponent<Tile>().resourceId;
            if (tile.isDesert)
            {
                print("盗賊がいるため入手できない");
                if (resourceCount != 0)
                {
                    LocalLog("盗賊がいるため" + _resourceData.sheet[resourceId].resourceName + " : " + resourceCount + "が入手できませんでした");
                }
            }
            else
            {
                AddResource(resourceId, resourceCount);
                if (resourceCount != 0)
                {
                    LocalLog(_resourceData.sheet[resourceId].resourceName + " : " + resourceCount + "入手しました");
                }
            }
        }
        ResourceCountUpdate();
    }
    #endregion

    #region ダイス7or騎士カード
    private async void SoldierCard(bool isSoldier)
    {
        _panel.Display(false);
        //全員のカード数をログに流す　オーバーしている人は赤色で
        if (isSoldier)
        {
            _view.RPC("P_LogShare", RpcTarget.All, myName + "が騎士カードを使用しました");
        }
        else
        {
            _view.RPC("P_LogShare", RpcTarget.All, myName + "が7の目を出しました");
        }
        _discardCompoleteCount = 0;
        _robPlayerId = -1;

        _isRobResult = false;
        _isRobPlayer = false;
        _isCardSelect = false;
        _isRobPlayerSelect = false;

        if (!isSoldier)
        {
            _view.RPC("DiscardCard", RpcTarget.Others);
            DiscardCard();
            await UniTask.WaitUntil(() => _discardCompoleteCount == _playerCount);//捨てるまで待機
            print("全員捨てた");
            _view.RPC("P_LogShare", RpcTarget.All, "全員捨てました");
        }
        _isTileSelect = true;
        SetTileOutLine(true);

        _view.RPC("P_LogShare", RpcTarget.Others, myName + "がタイルを選んでいます");
        LocalLog("盗賊の移動先を選んでください");
        await UniTask.WaitUntil(() => !_isTileSelect);//タイルを選ぶまで待機
        print("タイル選んだ");
        SetTileOutLine(false);
        _view.RPC("P_LogShare", RpcTarget.All, "盗賊が移動しました");


        if (_isRobPlayer)//タイルの移動先に街が隣接している時
        {
            _isRobPlayerSelect = true;
            _view.RPC("P_LogShare", RpcTarget.Others, myName + "が資源を奪う相手を選んでいます");
            LocalLog("資源を奪う相手を選んでください");
            await UniTask.WaitUntil(() => !_isRobPlayerSelect);//プレイヤーを選ぶまで待機

            _view.RPC("P_LogShare", RpcTarget.All, myName + "は" + GetOtherName(_robPlayerId) + "を選びました");
            LocalLog("カードを選択してください");
            SetOutLine(false, 0);
            _subPanel.Display(_subPanel.cardSelect);
            DisplayRobCard(_robPlayerId);

            await UniTask.WaitUntil(() => _isCardSelect);//カードを選ぶまで待機
            print("カードを選んだ");

            _view.RPC("P_LogShare", RpcTarget.All, myName + "が" + GetOtherName(_robPlayerId) + "から資源を盗みました");
            _view.RPC("P_DeprivedResource", RpcTarget.Others, _robPlayerId, _myNumber, _robCardNumber);

            await UniTask.WaitUntil(() => _isRobResult);
        }

        _isRobResult = false;
        if (isSoldier)//騎士カード
        {
            _soldierCount++;
            DeveComplete();
        }
        _panel.Display(true);
        _subPanel.Display(false);
        _panel.dice.interactable = false;
    }//ダイス7or騎士カード
    [PunRPC]
    private void DiscardCard()
    {
        _isDispose = false;
        print("ダイス7");
        if (7 < _cardCount)//カードの枚数が7枚以上だったら
        {
            _discardCount = Mathf.FloorToInt(_cardCount / 2);
            _discardSelectCount = 0;
            print("7枚以上だった");
            _subPanel.Display(_subPanel.discardCard);
            _resourceToggle.isOn = true;
            _subPanel.discardText.text = "残り" + (_discardCount - _discardSelectCount) + "枚";
            DiscardPossible();
            LocalLog("7枚以上のため半分の" + (_discardCount - _discardSelectCount) + "枚捨ててください");
        }
        else
        {
            print("7枚以下だった");
            _isDispose = true;
            _view.RPC("P_DiscardComplete", RpcTarget.All);
        }
    }//捨てる通知
    public void DiscardSelect(int resourceId)
    {
        _discardSelectCount++;
        _subPanel.discardText.text = "残り" + (_discardCount - _discardSelectCount) + "枚";
        RemoveResource(resourceId);
        if (_discardCount == _discardSelectCount)//必要な枚数捨てたら
        {
            _menuToggle.isOn = true;
            _subPanel.discardText.text = "完了";
            _subPanel.Display(false);
            _view.RPC("P_DiscardComplete", RpcTarget.All);
            _isDispose = true;
            LocalLog("規定枚数捨てました");
        }
        DiscardPossible();
    }//捨てるカードを選択
    [PunRPC]
    private void P_DiscardComplete()
    {
        _discardCompoleteCount++;
    }//捨てた事の通知

    private void SetTileOutLine(bool isDisplay)
    {
        foreach (var tile in _tileList)
        {
            var outline = tile.transform.Find("OutLine");
            outline.gameObject.SetActive(isDisplay);
            if (tile.isDesert)
            {
                outline.gameObject.SetActive(false);
            }

        }
    }//タイルのアウトライン
    private void TileClick(Tile tile)//タイルをクリックした時の処理
    {
        if (_isTileSelect)
        {
            if (tile.isDesert)
            {
                print("同じ場所には移動できない");
                return;
            }
            _view.RPC("P_SetDesert", RpcTarget.All, _tileList.IndexOf(desert), false);
            _view.RPC("P_SetDesert", RpcTarget.All, _tileList.IndexOf(tile), true);
            var citys = tile.aroundCity.Select(c => c.GetComponent<City>()).Where(c => c.isSomeone && c.playerId != _myNumber);
            if (citys.Count() != 0)
            {
                SetOutLine(true, 0, citys.Select(c => c.GetComponent<BordObject>()).ToList());
                _selectPlayerCity = new List<GameObject>(citys.Select(x => x.gameObject).ToArray());
                foreach (var city in citys)
                {
                    city._isRob = true;
                }
                _isRobPlayer = true;
            }
            else
            {
                _isRobPlayerSelect = false;
                _isCardSelect = true;
                _isRobPlayer = false;
            }
            _isTileSelect = false;
        }
    }
    [PunRPC]
    private void P_SetDesert(int tileId, bool isDesert)
    {
        var tile = _tileList[tileId];
        tile.isDesert = isDesert;
        if (isDesert)
        {
            var thieves = Instantiate(_thieves, tile.transform);
            thieves.name = "Thieves";
            desert = tile;
        }
        else
        {
            Destroy(desert.transform.Find("Thieves").gameObject);
        }
    }
    private void SelectPlayer(GameObject clickObj)
    {
        if (_isRobPlayerSelect && _selectPlayerCity.Contains(clickObj))
        {
            var city = clickObj.GetComponent<City>();
            if (city._isRob)
            {
                _robPlayerId = clickObj.GetComponent<BordObject>().playerId;
            }
            _isRobPlayerSelect = false;
        }
    }
    public void RobCardSelect(int number)
    {
        _robCardNumber = number;
        _isCardSelect = true;
    }//ボタンからカードを選択
    private void DisplayRobCard(int playerId)
    {
        var content = _subPanel.cardSelect.transform.Find("Content");
        foreach (Transform card in content)
        {
            Destroy(card.gameObject);
        }
        for (int i = 0; i < _playerCardCount[playerId]; i++)
        {
            int ii = i;
            var backCard = Instantiate(_backCard, content);
            backCard.GetComponent<Button>().onClick.AddListener(() => RobCardSelect(ii));
        }
        _subPanel.cardSelectPlayerName.text = GetOtherName(playerId);

    }
    [PunRPC]
    private void P_DeprivedResource(int to, int from, int number)
    {
        if (to == _myNumber)
        {
            var resourceList = new List<int>();
            foreach (var resource in _resourceList)
            {
                for (int i = 0; i < resource.Value; i++)
                {
                    resourceList.Add(resource.Key);
                }
            }
            resourceList = resourceList.OrderBy(a => new Guid()).ToList();
            RemoveResource(resourceList[number]);
            _view.RPC("P_RobResource", RpcTarget.Others, from, resourceList[number]);
        }
    }//カードを奪われる
    [PunRPC]
    private void P_RobResource(int to, int resource)
    {
        if (to == _myNumber)
        {
            AddResource(resource);
            _isRobResult = true;
        }
    }//カードを奪う
    private void DiscardPossible()
    {
        foreach (var resource in _resourceList)
        {
            if (resource.Value == 0 || _isDispose)
            {
                _subPanel.discardButtonli[resource.Key].interactable = false;
            }
            else
            {
                _subPanel.discardButtonli[resource.Key].interactable = true;
            }
        }
    }//捨てられるカードのボタンを表示捨てられないカードを非表示
    #endregion

    public void PointDisplay()
    {
        for (int i = 0; i < _winPoint.Count; i++)
        {
            _pointPanel.pointTexts[i].text = (_myWinPoint[i] * _winPoint[i]).ToString();
        }
    }
    private void ChangePoint(int id, int count = 1)
    {
        _myWinPoint[id] += count;
        WinCheck();
    }
    private void WinCheck()
    {
        int point = 0;
        for (int i = 0; i < 4; i++)
        {
            point += _myWinPoint[i] * _winPoint[i];
        }
        if (10 <= point)
        {
            print("Win");
            _victory.SetActive(true);
        }
        PointDisplay();
    }
    public void Victory()
    {
        LocalLog("おめでとうございます");
        LocalLog("あなたの勝ちです");
        LocalLog("お疲れさまでした");
        _view.RPC("P_Victory", RpcTarget.Others, _myNumber, _myWinPoint.ToArray());
        _playerWinPoints.Add(_myNumber, _myWinPoint);
        _resultCount++;
        _isGameNow = false;
    }
    [PunRPC]
    private void P_Victory(int from, int[] pointInfo)
    {
        _isGameNow = false;
        _playerWinPoints.Add(from, pointInfo.ToList());
        _resultCount++;
        LocalLog(GetOtherName(from) + "の勝ちです");
        LocalLog("お疲れさまでした");
        _view.RPC("P_PointShare", RpcTarget.AllViaServer, _myNumber, _myWinPoint.ToArray());
    }
    [PunRPC]
    private void P_PointShare(int from, int[] pointInfo)
    {
        foreach (Transform child in _rankingContent.transform)
        {
            Destroy(child.gameObject);
        }
        _playerWinPoints.Add(from, pointInfo.ToList());
        _resultCount++;
        if (_resultCount == _playerCount)
        {
            var pairs = new List<KeyValuePair<int, List<int>>>(_playerWinPoints);
            pairs.Sort(CompareKeyValuePair);
            pairs.Reverse();
            int i = 0;
            int prevPoint = 0;
            int prevRanking = 0;
            foreach (var kvp in pairs)
            {
                var element = Instantiate(_rankingElement, _rankingContent.transform).transform;
                element.Find("Name").GetComponent<Text>().text = myName;
                var count = element.Find("Count");
                int j = 0;
                int point = 0;
                foreach (Transform child in count.transform)
                {
                    child.GetComponent<Text>().text = (kvp.Value[j] * _winPoint[j]).ToString();
                    point += kvp.Value[j] * _winPoint[j];
                    j++;
                }
                if (prevPoint == point)
                {
                    element.Find("Ranking").GetComponent<Text>().text = prevRanking.ToString();
                }
                else
                {
                    element.Find("Ranking").GetComponent<Text>().text = (i + 1).ToString();
                    prevRanking = i + 1;
                }
                prevPoint = point;
                i++;
            }

            int CompareKeyValuePair(KeyValuePair<int, List<int>> x, KeyValuePair<int, List<int>> y)
            {
                // Valueで比較した結果を返す
                return x.Value.Sum() - y.Value.Sum();
            }
            PhotonNetwork.LeaveRoom();//接続解除
        }
        _rankingContent.transform.parent.gameObject.SetActive(true);
    }

    public void TitleLoad()
    {
        Dice.Clear();
        SceneManager.LoadScene("Title");
    }

    public void ReturnTitle()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        TitleLoad();
    }

    private void SetOutLine(bool isDisplay, int buildId, List<BordObject> bordObjects = null)
    {
        Material mate;
        if (buildId == 2)
            buildId = 0;
        var list = new List<BordObject>();
        if (isDisplay)
        {
            mate = _bordOutLine[buildId];
        }
        else
            mate = _bordDefaultMate;
        if (bordObjects == null)
        {
            list = new List<BordObject>(_bordBaseObjList[buildId]);
        }
        else
        {
            list = new List<BordObject>(bordObjects);
        }
        foreach (var v in list)
        {
            v.spriteRenderer.material = mate;
        }

        foreach (var v in list)
        {
            v.isBuild = isDisplay;
        }

    }
    private List<T> BordImpossibleObj<T>(int buildId)
    {
        var someoneObj = _bordBaseObjList[buildId].Where(b => b.isSomeone).Select(b => b.gameObject).ToList();
        var impossibleObj = new List<BordObject>();
        if (buildId == 0)
        {
            impossibleObj = _bordBaseObjList[buildId].Where(b => (b as City).aroundCity.FindAll(someoneObj.Contains).Count != 0).ToList();
        }
        impossibleObj.AddRange(someoneObj.Select(s => s.GetComponent<BordObject>()).ToList());
        if (typeof(T) == typeof(GameObject))
            return (List<T>)(object)impossibleObj.Select(b => b.gameObject).ToList();
        else
            return (List<T>)(object)impossibleObj.ToList();
    }
    private Color GetMyColor()
    {
        return _playerColorDef[_playerColor[_myNumber]];
    }
    private Color GetOtherColor(int number)
    {
        return _playerColorDef[_playerColor[number]];
    }
    private int GetWithinRange(int max, int value)
    {
        if (value < 0)
        {
            print("Test");
            return max - 1;
        }
        else if (max - 1 < value)
        {
            return 0;
        }
        return value;
    }
    private string ResourceToString(int resource, int count)
    {
        return _resourceData.sheet[resource].resourceName + " : " + count;
    }
    private string ResourceToString(List<int> resource)
    {
        var msg = "";
        for (int i = 0; i < resource.Count; i++)
        {
            msg += _resourceData.sheet[i].resourceName + " : " + resource[i] + " ";
        }
        return msg.Trim();
    }
    private string GetOtherName(int playerID)
    {
        var name = "";
        if (string.IsNullOrEmpty(PhotonNetwork.PlayerList[playerID].NickName) || PhotonNetwork.PlayerList[playerID].NickName == "Player")
            name = "Player" + playerID;
        else
            name = PhotonNetwork.PlayerList[playerID].NickName;
        return name;
    }
}
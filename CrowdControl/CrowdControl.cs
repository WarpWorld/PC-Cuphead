using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
#if ML_Il2Cpp
using UnityEngine.Networking;
using UnhollowerBaseLib.Attributes;
using MelonLoader.Assertions;
#else
using UnityEngine.Assertions;
#endif
#pragma warning disable 0414
#pragma warning disable 0649

namespace WarpWorld.CrowdControl {
    /// <summary>
    /// The Crowd Control client instance. Handles communications with the server and triggering effects.
    /// </summary>
#if !ML_Il2Cpp
    [DisallowMultipleComponent]
    [AddComponentMenu("Crowd Control/Manager")]
#endif
    public sealed class CrowdControl : MonoBehaviour 
    {
#if ML_Il2Cpp
        public CrowdControl(IntPtr value) : base(value) { }
#endif

        #region Configuration

#if !ML_Il2Cpp
        [Tooltip("Unique game identifier provided by Warp World.")]
        [SerializeField]
#endif
        uint _gameKey;

#if !ML_Il2Cpp
        [Tooltip("Whether to use the Staging Server or production server.")]
        [SerializeField]
#endif
        bool _staging = false;

#if !ML_Il2Cpp
        [Tooltip("Don't destroy this game object when changing scenes.")]
        [SerializeField]
#endif
        private bool _dontDestroyOnLoad = true;

#if !ML_Il2Cpp
        [SerializeField]
#endif
        private BroadcasterType _broadcasterType;

#if !ML_Il2Cpp
        [SerializeField]
#endif
        private Dictionary<uint, CCBidWarLibrary> bidWarLibraries = new Dictionary<uint, CCBidWarLibrary>();

#if !ML_Il2Cpp
        [Space]
        [Tooltip("How many times to attempt reconnecting? (-1 for unlimited)")]
        [SerializeField]
#endif
        private short _reconnectRetryCount = -1;

#if !ML_Il2Cpp
        [Tooltip("How many seconds to wait until trying to automatically reconnect again")]
        [SerializeField]
#endif
        private float _reconnectRetryDelay = 5;

#if !ML_Il2Cpp
        [Tooltip("Time to wait after triggering an effect before attempting to trigger another.")]
        [Range(0, 10)]
#endif
        public float delayBetweenEffects = .5f;

#if !ML_Il2Cpp
        [Header("Debug Outputs")]
        [SerializeField]
#endif
        bool _debugLog = true;

#if !ML_Il2Cpp
        [SerializeField]
#endif
        bool _debugWarning = true;

#if !ML_Il2Cpp
        [SerializeField]
#endif
        bool _debugError = true;

#if !ML_Il2Cpp
        [SerializeField]
#endif
        bool _debugExceptions = true;

        private SocketProvider _socketProvider;
        private ushort _port = 27442;
        private string _sslAddress = "gamesocket.crowdcontrol.live";
        private string _sslStagingAddress = "staging-gamesocket.crowdcontrol.live";
        private uint _currentEffectID = 0;
        private ulong _deviceFingerprint;
        private uint _blockID = 1;
        private string _token = "";
        private bool _disconnectedFromDisable = false;

#pragma warning restore 0649, 1591

        #endregion

        #region State

        /// <summary>Singleton instance. Will be <see langword="null"/> if the behaviour isn't in the scene.</summary>
        public static CrowdControl instance { get; private set; }
        /// <summary>Reference to the test user object. Used to dispatch local effects.</summary>
        public static TwitchUser testUser { get; private set; }
        /// <summary>Reference to the crowd user object. Used to dispatch pooled effects.</summary>
        public static TwitchUser crowdUser { get; private set; }
        /// <summary>Reference to the crowd user object. Used to dispatch effects with an unknown contributor.</summary>
        public static TwitchUser anonymousUser { get; private set; }
        /// <summary>Reference to the streamer user object.</summary>
        public static TwitchUser streamerUser { get; private set; }

        private Queue<CCEffectInstance> pendingQueue;
        private Queue<PendingMessage> _pendingMessages = new Queue<PendingMessage>();
        private Dictionary<uint, Queue<uint>> haltedTimers;
        private Dictionary<uint, CCEffectInstanceTimed> runningEffects = new Dictionary<uint, CCEffectInstanceTimed>(); // Timed effects currently running.
        private readonly Dictionary<string, TwitchUser> twitchUsers = new Dictionary<string, TwitchUser>();
        private Dictionary<uint, CCEffectBase> effectsByID = new Dictionary<uint, CCEffectBase>();

        private float timeUntilNextEffect;
        private float timeToNextPing = float.MaxValue; // When to send the next ping message.
        private float timeToTimeout = float.MaxValue; // When to consider the server connection as timed out.

#pragma warning disable 0649
        private byte[] _sendBuffer;
        private byte _recvSize;
        private byte _sendSize;
        private short _currentRetryCount;
        private bool _duplicatedInstance = false;
        private bool _paused = false;
        private bool _adjustPauseTime = false;

        /// <summary>Did we start a session?</summary>
        public bool isAuthenticated { get; private set; }
        /// <summary>Whether the connection to the server is currently initializing.</summary>
        public bool isConnecting { get; private set; }
        /// <summary>Are you connected or not</summary>
        public bool isConnected => _socketProvider != null && _socketProvider.Connected;
        /// <summary>The latest disconnect occured due to an error.</summary>
        public bool disconnectedFromError { get; private set; }
        #endregion

        #region Events

        /// <summary>Invoked when attempting a connection to the Crowd Control server.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action OnConnecting;
        /// <summary>Invoked when the connection to the Crowd Control server has failed.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<SocketError> OnConnectionError;
        /// <summary>Invoked when the Authentication Token has been Authorized and Confirmed by the Crowd Control server.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action OnAuthenticated;
        /// <summary>Invoked when successfully connected to the Crowd Control server.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action OnConnected;
        /// <summary>Invoked when disconnected from the Crowd Control server.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action OnDisconnected;

        /// <summary>Invoked when an effect is scheduled for execution.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<CCEffectInstance> OnEffectQueue;
        /// <summary>Invoked when an effect leaves the scheduling queue.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<CCEffectInstance, EffectResult> OnEffectDequeue;

        /// <summary>Invoked when an important message needs to be displayed.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<string> OnDisplayMessage;

#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<bool> OnToggleTokenView;

#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action OnNoToken;
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action OnSubmitTempToken;
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action OnTempTokenFailure;

        /// <summary>Invoked when an <see cref="CCEffectBase"/> is successfully triggered.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<CCEffectInstance> OnEffectTrigger;
        /// <summary>Invoked when a <see cref="CCEffectTimed"/> is successfully started.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<CCEffectInstanceTimed> OnEffectStart;
        /// <summary>Invoked when a <see cref="CCEffectTimed"/> is successfully stopped.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<CCEffectInstanceTimed> OnEffectStop;

        /// <summary>Invoked on effect instances when the associated <see cref="CCEffectTimed"/> is disabled.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<CCEffectInstanceTimed> OnEffectPause;
        /// <summary>Invoked on effect instances when the associated <see cref="CCEffectTimed"/> is enabled.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<CCEffectInstanceTimed> OnEffectResume;
        /// <summary>Invoked on effect instances when the associated <see cref="CCEffectTimed"/> is reset.</summary>
#if ML_Il2Cpp
        [method: HideFromIl2Cpp]
#endif
        public event Action<CCEffectInstanceTimed> OnEffectReset;
        #endregion

        #region Unity Component Life Cycle

        void Awake() {
            if (instance != null)
            {
                _duplicatedInstance = true;
                Destroy(gameObject);
                return;
            }

            if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

            crowdUser = new TwitchUser {
                id = 1,
                name = "the_crowd",
                displayName = "The Crowd",
            };

            anonymousUser = new TwitchUser
            {
                id = 2,
                name = "anonymous",
                displayName = "User",
            };

            twitchUsers.Add(crowdUser.name, crowdUser);
            twitchUsers.Add(anonymousUser.name, anonymousUser);

#if ML_Il2Cpp
            MelonLoader.Assertions.LemonAssert.
#else
            Assert.
#endif
            IsNull(instance);
            instance = this;

            pendingQueue = new Queue<CCEffectInstance>();
            haltedTimers = new Dictionary<uint, Queue<uint>>();
        }

        void OnEnable() {
            if (!_disconnectedFromDisable || isConnecting)
            {
                return;
            }

            Connect();
            _disconnectedFromDisable = false;
        }

        void OnDisable() {
            if (_duplicatedInstance)
            {
                return;
            }

            StopAllCoroutines();
            StopAllEffects();
            Disconnect();
            _disconnectedFromDisable = true;
        }

        void OnDestroy()
        {
            if (_duplicatedInstance)
            {
                return;
            }

            //Disconnect();

            if (_socketProvider != null)
            {
                CCMessageDisconnect cCMessageDisconnect = new CCMessageDisconnect(_blockID++);
                _socketProvider.QuickSend(cCMessageDisconnect.ByteStream);
                _socketProvider.Dispose();
            }

#if ML_Il2Cpp
            MelonLoader.Assertions.LemonAssert.
#else
            Assert.
#endif
            IsNotNull(instance);
            instance = null;

            testUser = null;
            crowdUser = null;

            pendingQueue = null;
            runningEffects = null;
            haltedTimers = null;

            twitchUsers.Clear();
            effectsByID.Clear();
        }

        void OnApplicationPause(bool paused)
        {
            _paused = paused;

            if (paused)
            {
                _adjustPauseTime = true;
            }
        }

        void Update() {
            // Handle connection timeout and reconnects.
            float now = Time.unscaledTime;

            if (_adjustPauseTime)
            {
                UpdateTimerEffectsFromIdle();

                if (!_paused)
                {
                    timeToNextPing = now + Protocol.PING_INTERVAL;
                    timeToTimeout = now + Protocol.PING_INTERVAL * 2;
                    _adjustPauseTime = false;
                }
            }

            if (now >= timeToTimeout && isConnected) {
                Disconnect(true);
            }
            else if (!isConnected && !isConnecting && now >= timeToNextPing && disconnectedFromError) { 
                timeToNextPing = float.MaxValue;
                ConnectSocket();
            }

            // Process effects.
            timeUntilNextEffect -= Time.unscaledDeltaTime;

            RunQueue(TryStop);


            HandlePending();

            // Send messages to the server.
            if (isConnected) {
                if (_pendingMessages.Count > 0)
                {
                    ProcessMsg(_pendingMessages.Dequeue());
                }

                if (timeToNextPing <= now) {
#if !ML_Il2Cpp
                    Assert.IsFalse
#else
                    LemonAssert.IsTrue
#endif
                    (isConnecting);

                    Send(new CCMessagePing(_blockID++));
                    timeToNextPing = now + Protocol.PING_INTERVAL;
                    timeToTimeout = Time.unscaledTime + Protocol.PING_INTERVAL * 2;
                }
            }
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        void ReceivedBytes(byte[] recv)
        {
            var offset = 0;
            var available = _recvSize + recv.Length;

            while (available >= Protocol.FRAME_SIZE)
            {
                var size = (recv[offset] << 8) | recv[offset + 1];

                if (available < size)
                {
                    // TODO receive more than recv.Length

                    if (_recvSize != 0)
                        Buffer.BlockCopy(recv, offset, recv, 0, available);

                    _recvSize = Convert.ToByte(available);

                    break;
                }

                _pendingMessages.Enqueue(new PendingMessage(Protocol.SplitByteArray(recv, offset, size), recv[offset + 2], size - Protocol.FRAME_SIZE));

                offset += size;
                available -= size;
            }
        }

        #endregion

        #region Client

        private void HandlePending()
        {
            if (pendingQueue.Count == 0)
                return;

            CCEffectInstance currentPending = pendingQueue.Dequeue();

            if (IsRunning(currentPending))
            {
                uint id = currentPending.effectID;
                if (!haltedTimers.ContainsKey(id))
                    haltedTimers.Add(id, new Queue<uint>());

                haltedTimers[id].Enqueue(currentPending.id);
                return;
            }

            if (TryStart(currentPending))
                return;

            pendingQueue.Enqueue(currentPending);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private bool TryStart(CCEffectInstance effectInstance)
        {
            if (timeUntilNextEffect > 0) return false;

            var now = Time.unscaledTime;
            if (effectInstance.effect.delayUntilUnscaledTime > now) return false;

            return effectInstance.unscaledStartTime <= now && StartEffect(effectInstance);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private bool IsRunning(CCEffectInstance effectInstance)
        {
            if (!(effectInstance is CCEffectInstanceTimed))
                return false;

            return runningEffects.ContainsKey(effectInstance.effectID);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private bool TryStop(CCEffectInstanceTimed effectInstance)
        {

            if (effectInstance.isPaused)
                return false;

            effectInstance.unscaledTimeLeft -= Time.unscaledDeltaTime;
           
            return effectInstance.unscaledTimeLeft <= 0 && StopEffect(effectInstance, true);
        }

        private void ConnectError()
        {
            MelonLogger.Msg("Connect Error");

            if (_reconnectRetryCount == 0)
                return;
            if (_reconnectRetryCount == -1 || ++_currentRetryCount <= _reconnectRetryCount)
            {
                timeToTimeout = float.MaxValue;
                timeToNextPing = float.MaxValue;

                _token = PlayerPrefs.GetString($"CCToken{_gameKey}{_staging}", string.Empty);
                ConnectSocket();
            }
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void ConnectError(Socket socket, SocketError error, Exception e)
        {
            socket.Close();
            OnConnectionError?.Invoke(error);
            LogException(e); // TODO
            ConnectError();
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void Send(CCRequest message)
        {
            Task.Factory.StartNew(() => _socketProvider.Send(message.ByteStream));
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private async void DisconnectAndDisposeSocket(CCRequest message)
        {
            await _socketProvider.Send(message.ByteStream);
            _socketProvider.Dispose();
            _socketProvider = null;
        }

        /// <summary>
        /// Connects to the Crowd Control server.
        /// </summary>
        public void Connect() {
            if ((_socketProvider != null && _socketProvider._socket != null) || isConnecting) throw new InvalidOperationException();
             
            _currentRetryCount = 0;
            timeToNextPing = float.MaxValue;
            _token = PlayerPrefs.GetString($"CCToken{_gameKey}{_staging}", string.Empty);

            ConnectSocket();
        }

        private async void ConnectSocket()
        {
            isConnecting = true;
            OnConnecting?.Invoke();

            _socketProvider = new SocketProvider();
            _socketProvider.OnMessageReceived += ReceivedBytes;
            await _socketProvider.Connect(_staging ? _sslStagingAddress : _sslAddress, _port);

            timeToNextPing = Time.unscaledTime + Protocol.PING_INTERVAL;
            timeToTimeout = timeToNextPing + Protocol.PING_INTERVAL;

            _deviceFingerprint = Utils.Randomulong();
            CCMessageVersion cCMessageVersion = new CCMessageVersion(_blockID++, Protocol.VERSION, _gameKey, _deviceFingerprint);
            Send(cCMessageVersion);
        }

        /// <summary>
        /// Disconnects from the Crowd Control server.
        /// </summary>
        public void Disconnect() => Disconnect(false);

        private void Disconnect(bool fromError) {
            Log("Disconnect");

            if (_socketProvider != null && _socketProvider._stream != null) {
                if (!fromError)
                {
                    CCMessageDisconnect cCMessageDisconnect = new CCMessageDisconnect(_blockID++);
                    DisconnectAndDisposeSocket(cCMessageDisconnect);
                }
                else
                {
                    _socketProvider.Dispose();
                    _socketProvider = null;
                }

            }

            if (fromError) { 
                ConnectError();
                timeToNextPing = Time.unscaledTime + Protocol.PING_INTERVAL;
            }
            else { 
                timeToNextPing = float.MaxValue;
            }

            
            timeToTimeout = float.MaxValue;
            isConnecting = false;
            OnDisconnected?.Invoke();
            isAuthenticated = false;
            disconnectedFromError = fromError;
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        public void UpdateEffect(uint effectID, Protocol.EffectState effectState, uint callbackID = 0, ushort payload = 0)
        {
            if (effectState == Protocol.EffectState.AvailableForOrder || effectState == Protocol.EffectState.UnavailableForOrder ||
                effectState == Protocol.EffectState.VisibleOnMenu || effectState == Protocol.EffectState.HiddenOnMenu)
            {
                CCMessageEffectUpdate effectUpdate = new CCMessageEffectUpdate(_blockID++, effectID, effectState, payload);
                Send(effectUpdate);
                return;
            }

            CCMessageEffectRequest messageEffectRequest = new CCMessageEffectRequest(_blockID++, callbackID, effectState, payload);
            Send(messageEffectRequest);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void UpdateEffect(CCEffectInstance instance, Protocol.EffectState effectState, ushort payload = 0)
        {
            if (instance.isTest)
                return;

            UpdateEffect(instance.effectID, effectState, instance.id, payload);
        }

        private bool EffectIsBidWar(uint effectID)
        {
            return effectsByID.ContainsKey(effectID) && (effectsByID[effectID] is CCEffectBidWar);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void EffectSuccess(CCEffectInstance instance, byte delay = 0)
        {
            if (EffectIsBidWar(instance.effectID))
                UpdateEffect(instance, Protocol.EffectState.BidWarSuccess);
            else
                UpdateEffect(instance, Protocol.EffectState.Success, delay);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void EffectFailure(CCEffectInstance instance)
        {
            UpdateEffect(instance, EffectIsBidWar(instance.effectID) ? Protocol.EffectState.BidWarFailure : Protocol.EffectState.TemporaryFailure);
        }

        private void EffectDelay(uint effectID, byte delay = 5)
        {
            if (EffectIsBidWar(effectID))
                UpdateEffect(effectID, Protocol.EffectState.BidWarDelay);
            else
                UpdateEffect(effectID, Protocol.EffectState.ExactDelay);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void SetTimedEffectState(CCEffectInstanceTimed instance, bool begin)
        {
            UpdateEffect(instance, begin ? Protocol.EffectState.TimedEffectBegin : Protocol.EffectState.TimedEffectEnd, Convert.ToUInt16(instance.effect.duration));
        }

        /// <summary> Check if the effect is registered already or not. </summary>
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        public bool EffectIsRegistered(CCEffectBase effectBase)
        {
            return effectsByID.ContainsKey(effectBase.identifier);
        }

        /// <summary> Registers this effect during runtime. </summary>
        public void RegisterEffect(CCEffectBase effectBase)
        {
            if (EffectIsRegistered(effectBase))
            {
                LogError("Effect ID " + effectBase.identifier + " is already registered!");
                return;
            }

            effectsByID.Add(effectBase.identifier, effectBase);
            Log("Registered Effect ID " + effectBase.identifier);
        }

        /// <summary> Toggles whether an effect can currently be sold during this session. </summary>
        public void ToggleEffectSellable(uint effectID, bool sellable)
        {
            UpdateEffect(effectID, sellable ? Protocol.EffectState.AvailableForOrder : Protocol.EffectState.UnavailableForOrder);
        }

        /// <summary> Toggles whether an effect is visible in the menu during this session. </summary>
        public void ToggleEffectVisible(uint effectID, bool visible)
        {
            UpdateEffect(effectID, visible ? Protocol.EffectState.VisibleOnMenu : Protocol.EffectState.HiddenOnMenu);
        }

        private void Send(byte   value) => Protocol.Write(_sendBuffer, ref _sendSize, value);
        private void Send(ushort value) => Protocol.Write(_sendBuffer, ref _sendSize, value);
        private void Send(uint   value) => Protocol.Write(_sendBuffer, ref _sendSize, value);
        private void Send(ulong  value) => Protocol.Write(_sendBuffer, ref _sendSize, value);

        private void Send(UUID value) => value.Write(_sendBuffer, ref _sendSize);
        private void Send(string value) => Protocol.Write(_sendBuffer, ref _sendSize, value, value.Length);

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void HelloMessage(byte [] bytes)
        {
            CCMessageVersion messageVersion = new CCMessageVersion(bytes);

            if (messageVersion.response == CCMessageVersion.ResponseType.Success)
            {
                OnConnected?.Invoke();
                _currentRetryCount = 0;

                if (string.IsNullOrEmpty(_token))
                {
                    OnToggleTokenView?.Invoke(true); // We don't have a token.
                    OnNoToken?.Invoke();
                    return;
                }

                UUID uuid = new UUID();
                uuid.FromString(_token);

                CCMessageTokenHandshake tokenHandshake = new CCMessageTokenHandshake(_blockID++, uuid);
                Send(tokenHandshake);
            }

            Log(messageVersion.response.ToString());
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void TokenAquisition(byte [] bytes)
        {
            CCMessageTokenAquisition getTokenMessage = new CCMessageTokenAquisition(bytes);

            if (getTokenMessage.greeting != Greeting.Success)
            {
                LogError(getTokenMessage.greeting.ToString());
                OnToggleTokenView?.Invoke(true);
                OnTempTokenFailure?.Invoke();
                return;
            }

            CCMessageTokenHandshake tokenHandshake = new CCMessageTokenHandshake(_blockID++, getTokenMessage.uniqueToken);
            Send(tokenHandshake);

            PlayerPrefs.SetString($"CCToken{_gameKey}{_staging}", getTokenMessage.uniqueToken.ToString());
        }

        /// <summary> Submits Temporary Token to the server. </summary>
        public void SubmitTempToken(string token)
        {
            OnToggleTokenView?.Invoke(false);
            OnSubmitTempToken?.Invoke();
            CCMessageTokenAquisition getTokenMessage = new CCMessageTokenAquisition(_blockID++, token);
            Send(getTokenMessage);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void TokenHandshake(byte[] bytes)
        {
            CCMessageTokenHandshake tokenHandShake = new CCMessageTokenHandshake(bytes);

            if (tokenHandShake.greeting != Greeting.Success)
            {
                LogError(tokenHandShake.greeting.ToString());
                OnToggleTokenView?.Invoke(true);
                OnTempTokenFailure?.Invoke();
                return;
            }

            streamerUser = new TwitchUser
            {
                name = tokenHandShake.streamerName,
                displayName = tokenHandShake.streamerName,
                profileIconUrl = tokenHandShake.streamerIconURL
            };

            if (!twitchUsers.ContainsKey(streamerUser.name))
                twitchUsers.Add(streamerUser.name, streamerUser);

            _broadcasterType = tokenHandShake.broadcasterType;

            DisplayMessage(tokenHandShake.streamerName + " started the Crowd Control Session!");
            isAuthenticated = true;
            OnAuthenticated?.Invoke();
        }
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void DisplayMessage(string message)
            => OnDisplayMessage?.Invoke(message);

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void BlockError(byte[] bytes)
        {
            CCMessageBlockError blockError = new CCMessageBlockError(bytes);
            LogError("Block: " + blockError.blockID);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void BroadcastedMessage(byte[] bytes)
        {
            CCMessageUserMessage message = new CCMessageUserMessage(bytes);
            DisplayMessage(message.receivedMessage);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void TriggerEffect(byte[] bytes)
        {
            CCMessageEffectRequest effect = new CCMessageEffectRequest(bytes);

            if (!effectsByID.ContainsKey(effect.effectID))
            {
                LogError("Invalid effect identifier '{0}'.", effect.effectID);
                UpdateEffect(effect.effectID, Protocol.EffectState.PermanentFailure);
                return;
            }

            QueueEffect(effectsByID[effect.effectID], effect.viewers, effect.blockID, effect.parameters);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void ProcessMsg(PendingMessage message) {
            byte[] bytes = message._bytes;
            MessageType messageType = (MessageType)message._msgType;

            Log("Received message type {0} of size {1}", messageType, message._size);

            switch (messageType)
            {
                case MessageType.Version: HelloMessage(bytes);
                    break;
                case MessageType.TokenAquisition: TokenAquisition(bytes);
                     break;
                case MessageType.TokenHandshake: TokenHandshake(bytes);
                    break;
                case MessageType.BlockError: BlockError(bytes);
                    break;
                case MessageType.UserMessage: BroadcastedMessage(bytes);
                    break;
                case MessageType.EffectRequest: TriggerEffect(bytes);
                    break;
            }
        }

        #endregion

        #region Effect Handlings

        /// <summary>Test an effect locally. Its events won't be sent to the server.</summary>
        public void TestEffect(CCEffectBase effect) {
            if (!isActiveAndEnabled) return;

            if (testUser == null) {
                testUser = new TwitchUser {
                    id = Convert.ToUInt64(UnityEngine.Random.Range(1, 100000)),
                    name = "test_user",
                    displayName = "Test_User",
                };

                twitchUsers.Add(testUser.displayName, testUser);
            }

#if ML_Il2Cpp
            MelonLoader.MelonCoroutines.Start(
#else
            StartCoroutine(
#endif
            WaitForEffectListToLoad(effect));
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private IEnumerator WaitForEffectListToLoad(CCEffectBase effect)
        {
            while (effectsByID.Count == 0)
                yield return Extensions.CreateWaitForSeconds(1.0f);

            if (effectsByID.ContainsKey(effect.identifier))
            {
                SendCCEffectLocally(effect, testUser);
                yield break;
            }

            LogError("Invalid effect identifier '{0}'.", effect.identifier);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void SendCCEffectLocally(CCEffectBase effect, TwitchUser twitchUser)
        {
            QueueEffect(effect, new CCMessageEffectRequest.Viewer [] { new CCMessageEffectRequest.Viewer(twitchUser.name) }, _blockID, effect.Params(), true);
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        // Allocates an effect instance and add it to the pending list.
        private void QueueEffect(CCEffectBase effect, CCMessageEffectRequest.Viewer [] viewers, uint blockID, string parameters = null, bool test = false) 
        {
#if ML_Il2Cpp
            MelonLoader.Assertions.LemonAssert.IsTrue(isActiveAndEnabled);
            MelonLoader.MelonCoroutines.Start(DownloadUserInfo(effect, viewers, blockID, parameters, test));
#else
            Assert.IsTrue(isActiveAndEnabled);
            StartCoroutine(DownloadUserInfo(effect, viewers, blockID, parameters, test));
#endif
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private IEnumerator DownloadUserInfo(CCEffectBase effect, CCMessageEffectRequest.Viewer [] viewers, uint receivedBlockID, string parameters = null, bool test = false)
        {
            TwitchUser displayUser;

            if (viewers != null && viewers.Length > 0)
            {
                foreach (CCMessageEffectRequest.Viewer viewer in viewers)
                {
                    if (twitchUsers.ContainsKey(viewer.displayName))
                    {
                        continue;
                    }

                    TwitchUser user = new TwitchUser();
                    twitchUsers.Add(viewer.displayName, user);
                    user.displayName = viewer.displayName;
                    user.name = viewer.displayName;
                    user.profileIconUrl = viewer.iconURL;
                    twitchUsers[user.name] = user;
                }

                if (viewers.Length >= 2)
                {
                    displayUser = crowdUser;
                }
                else
                {
                    displayUser = twitchUsers[viewers[0].displayName];
                }
            }
            else
            {
                displayUser = anonymousUser;
            }

            if (effect is CCEffectTimed)
                CreateEffectInstance<CCEffectInstanceTimed>(displayUser, effect as CCEffectTimed, receivedBlockID, parameters, test);
            else
                CreateEffectInstance<CCEffectInstance>(displayUser, effect, receivedBlockID, parameters, test);
            yield return null;
        }

        private void CreateEffectInstance<T>(TwitchUser user, CCEffectBase effect, uint blockID, string parameters, bool test) where T : CCEffectInstance, new()
        {
            T effectInstance = new T();

            effectInstance.id = blockID++;
            effectInstance.user = user; 
            effectInstance.effect = effect;
            effectInstance.retryCount = 0;
            effectInstance.unscaledStartTime = Time.unscaledTime; // TODO add some delay?
            effectInstance.isTest = test;

            if (!string.IsNullOrEmpty(parameters))
            {
                if (parameters.Contains(","))
                    effectInstance.parameters = parameters.Split(',');
                else
                    effectInstance.parameters = new string[] { parameters };
            }

            uint effectID = effect.identifier;
            if (effectsByID[effectID] is CCEffectParameters)
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    LogError("Received effect " + effect.displayName + " has no parameters!");
                    CancelEffect(effectInstance);
                    return;
                }
                CCEffectParameters paramEffect = effect as CCEffectParameters;
                paramEffect.AssignParameters(effectInstance.parameters);
            }

            else if (effectsByID[effectID] is CCEffectBidWar)
            {
                if (string.IsNullOrEmpty(parameters) || effectInstance.parameters.Length != 2)
                {
                    LogError("Received effect " + effect.displayName + " has an incorrect amount of parameters! A bid war must have two (Bid Name, Bid Amount)");
                    CancelEffect(effectInstance);
                    return;
                }

                if (!bidWarLibraries.ContainsKey(effectID))
                    bidWarLibraries.Add(effectID, new CCBidWarLibrary());

                string bidName = effectInstance.parameters[0];

                bool newWinner = bidWarLibraries[effectID].PlaceBid(bidName, Convert.ToUInt32(effectInstance.parameters[1]));

                if (!newWinner)
                    return;
            }

            pendingQueue.Enqueue(effectInstance);
            OnEffectQueue?.Invoke(effectInstance);
        }
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private IEnumerator StartTimedEffect(CCEffectInstance effectInstance)
        {
            EffectSuccess(effectInstance);
            yield return new WaitForSeconds(0.1f);
            SetTimedEffectState(effectInstance as CCEffectInstanceTimed, true);
        }

        // Process an effect instance in the pending list.
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private bool StartEffect(CCEffectInstance effectInstance) {
            EffectResult result;
            bool dequeue = true;
            bool isTest = effectInstance.isTest;
            CCEffectBase effect = effectInstance.effect;
            uint id = effectInstance.id;

            CCEffectInstanceTimed timedEffectInstance = effectInstance as CCEffectInstanceTimed;

            if (timedEffectInstance != null)
            {
                timedEffectInstance.effect = effect as CCEffectTimed;

                if (timedEffectInstance != null && !timedEffectInstance.isActive)
                {
                    result = EffectResult.Retry;
                    goto Retry;
                }
            }

            if (effect.CanBeRan())
            {
                result = effect.OnTriggerEffect(effectInstance);
            }
            else
            {
                result = EffectResult.Retry;
            }

#if ML_Il2Cpp
            MelonLoader.Assertions.LemonAssert.IsEqual
#else
            Assert.AreEqual
#endif
            (effectInstance.effect, effect);

            switch (result) {
                default:
                    LogError("Unhandled EffectResult.{0}", result);
                    break;
                // Effect instance is cancelled.
                case EffectResult.Failure:
                case EffectResult.Unavailable:
                    OnEffectDequeue?.Invoke(effectInstance, result);
                    break;

                 // Effect instance triggered successfully.
                case EffectResult.Success:
                    timeUntilNextEffect = delayBetweenEffects;
                    OnEffectDequeue?.Invoke(effectInstance, EffectResult.Success);
                    OnEffectTrigger?.Invoke(effectInstance);
                    EffectSuccess(effectInstance);
                    break;

                // Move from the pending list to the running list.
                case EffectResult.Running:
#if ML_Il2Cpp
                    MelonLoader.Assertions.LemonAssert.
#else
                    Assert.
#endif
                    IsNotNull(timedEffectInstance);
                    timeUntilNextEffect = delayBetweenEffects;

                    runningEffects.Add(effectInstance.effectID, timedEffectInstance);
                    OnEffectDequeue?.Invoke(effectInstance, EffectResult.Success);
                    OnEffectStart?.Invoke(timedEffectInstance);

#if ML_Il2Cpp
                    MelonLoader.MelonCoroutines.Start(
#else
                    StartCoroutine(
#endif
                    StartTimedEffect(effectInstance));
                    break;

                // Moved from the pending list to the behaviour's internal queue.
                case EffectResult.Queue:
#if ML_Il2Cpp
                    MelonLoader.Assertions.LemonAssert.
#else
                    Assert.
#endif
                    IsNotNull(timedEffectInstance);
                    break;

                // Leave in the pending list unless the instance reached max retries.
                case EffectResult.Retry:
                    goto Retry;
            }
            goto Done;

        Retry:
            effectInstance.retryCount++;
            if (effectInstance.retryCount > effect.maxRetries) {
                result = EffectResult.Failure;
                CancelEffect(effectInstance);
            }
            else {
                effectInstance.unscaledStartTime = effect.retryDelay + Time.unscaledTime;
                dequeue = false;
            }

        Done:
            if (dequeue) 
                effect.delayUntilUnscaledTime = effect.pendingDelay + Time.unscaledTime;
            else 
                effect.delayUntilUnscaledTime = effectInstance.unscaledStartTime;

            return dequeue;
        }

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private IEnumerator ResetTimedEffect(CCEffectInstanceTimed effectInstance, uint newID)
        {
            ResetEffect(effectInstance.effect);

            SetTimedEffectState(effectInstance, false);
            yield return new WaitForSeconds(0.1f);
            effectInstance.id = newID;
            UpdateEffect(effectInstance, Protocol.EffectState.Success);
            yield return new WaitForSeconds(0.1f);
            SetTimedEffectState(effectInstance, true);
        }

        // Process an effect instance in the running list.
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private bool StopEffect(CCEffectInstanceTimed effectInstance, bool removeFromList, bool force = false)
        {
#if ML_Il2Cpp
            MelonLoader.Assertions.LemonAssert.
#else
            Assert.
#endif
            IsNotNull(effectInstance);

            if (effectInstance.unscaledTimeLeft > 0.0f && !force)
            {
                // TODO force stop after maxRetries?
                effectInstance.unscaledEndTime = effectInstance.effect.retryDelay + Time.unscaledTime;
                effectInstance.unscaledTimeLeft = effectInstance.effect.retryDelay;

                if (removeFromList)
                {
                    SetTimedEffectState(effectInstance, false);
                    effectInstance.effect.OnStopEffect(effectInstance, false);
                }

                return false;
            }

            uint id = effectInstance.effectID;

            if (removeFromList)
            {
                if (haltedTimers.ContainsKey(id) && haltedTimers[id].Count > 0)
                {
                    OnEffectDequeue?.Invoke(effectInstance, EffectResult.Success);
#if ML_Il2Cpp
                    MelonLoader.MelonCoroutines.Start(
#else
                    StartCoroutine(
#endif
                    ResetTimedEffect(effectInstance, haltedTimers[id].Dequeue()));
                    
                    return false;
                }
            }

            effectInstance.effect.OnStopEffect(effectInstance, force);
            OnEffectStop?.Invoke(effectInstance);
            SetTimedEffectState(effectInstance, false);

            if (removeFromList)
            {
                runningEffects.Remove(id);
                effectInstance = null;
            }

            return true;
        }

        /// <summary>Cancels a received effect</summary>
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        public void CancelEffect(CCEffectInstance effectInstance) {
            EffectFailure(effectInstance);
            OnEffectDequeue?.Invoke(effectInstance, EffectResult.Failure);
        }

        /// <summary>Forcefully terminates all pending and running effects.</summary>
        public void StopAllEffects()
        {
            haltedTimers.Clear();

            foreach (CCEffectInstance instance in pendingQueue) // Cancel the rest of the pending effects
                CancelEffect(instance);

            pendingQueue.Clear();

            foreach (CCEffectInstanceTimed instance in runningEffects.Values) // Stop all running timers
                StopEffect(instance, false, true);

            runningEffects.Clear();
        }

#endregion

#region Linked List Utilities

        // Runs the action on every effect in the list and removes entries where the action returns true.
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        private void RunQueue(Func<CCEffectInstanceTimed, bool> action)
        {
            foreach (CCEffectInstanceTimed instance in runningEffects.Values)
            {
                if (action(instance))
                    return;
            }
        }

#endregion

#region Pause/Resume Timed Effects

        /// <summary>Resume a timed effect.</summary>
        public static void EnableEffect (CCEffectTimed effect) => RunEffect(effect, Resume);
        /// <summary>Disable a timed effect.</summary>
        public static void DisableEffect(CCEffectTimed effect) => RunEffect(effect, Pause);
        /// <summary>Reset a timed effect.</summary>
        public static void ResetEffect(CCEffectTimed effect) => RunEffect(effect, Reset);

        /// <summary>Pauses a timer effect.</summary>
        public static void Pause(CCEffectInstanceTimed effectInstance) {
            if (effectInstance.isPaused)
            {
                return;
            }
            effectInstance.effect.Pause(effectInstance);
            effectInstance.effect.OnPauseEffect();
            instance.OnEffectPause?.Invoke(effectInstance);
            instance.UpdateEffect(effectInstance, Protocol.EffectState.TimedPause, 0);
        }

        /// <summary>Resumes a timer command</summary>
        public static void Resume(CCEffectInstanceTimed effectInstance) {
            if (!effectInstance.isPaused)
            {
                return;
            }
            effectInstance.effect.Resume(effectInstance);
            effectInstance.effect.OnResumeEffect();
            instance.OnEffectResume?.Invoke(effectInstance);
            instance.UpdateEffect(effectInstance, Protocol.EffectState.TimedResume, Convert.ToUInt16(effectInstance.unscaledTimeLeft));
        }

        /// <summary>Resets a timer command</summary>
        public static void Reset(CCEffectInstanceTimed effectInstance)
        {
            effectInstance.effect.Reset(effectInstance);
            effectInstance.effect.OnResetEffect();
            instance.OnEffectReset?.Invoke(effectInstance);
        }

        private static void UpdateTimerEffectsFromIdle()
        {

            if (instance == null || instance.runningEffects == null)
            {
                return;
            }

            foreach (CCEffectInstanceTimed timedEffect in instance.runningEffects.Values)
            {
                if (!timedEffect.effect.ShouldBeRunning())
                {
                    continue;
                }

                if (!instance._paused)
                {
                    EnableEffect(timedEffect.effect);
                }
                else
                {
                    DisableEffect(timedEffect.effect);
                }
            }
        }

        /// <summary>Checks all running timer effects to see if they should be running or not. </summary>
        public static void UpdateTimerEffectStatuses()
        {
            if (instance == null || instance.runningEffects == null || instance._paused)
            {
                return;
            }

            foreach (CCEffectInstanceTimed timedEffect in instance.runningEffects.Values)
            {
                if (timedEffect.effect.ShouldBeRunning())
                {
                    EnableEffect(timedEffect.effect);
                }
                else
                {
                    DisableEffect(timedEffect.effect);
                }
            }
        }

        // Runs the action on every running effect instance matching the given effect behaviour.
        static void RunEffect(CCEffectTimed effect, Action<CCEffectInstanceTimed> action) {
            var self = instance;
            if (self == null || self.runningEffects == null) return;

            if (self.runningEffects.ContainsKey(effect.identifier))
                action(self.runningEffects[effect.identifier]);
        }
    
#endregion

#region Querying & Actions

        /// <summary>Returns true if at least one timed effect is currently running.</summary>
        public bool HasRunningEffects() => runningEffects != null;

        /// <summary>Returns true if timed effect is running.</summary>
        public bool IsRunning(CCEffectTimed type) => RunFirst(type.identifier, IsRunning);
        /// <summary>Stops a timer effect.</summary>
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        public bool StopOne  (CCEffectTimed type) => RunFirst(type.identifier, StopOne);
        /// <summary>Returns true if timed effect is paused.</summary>
        public bool IsPaused(CCEffectTimed type) => RunFirst(type.identifier, IsPaused);

        static bool IsRunning(CCEffectInstanceTimed effectInstance) => effectInstance.isActive;
        static bool IsPaused(CCEffectInstanceTimed effectInstance) => effectInstance.isPaused;

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        bool StopOne(CCEffectInstanceTimed effectInstance) => StopEffect(effectInstance, true, true);

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        bool RunFirst(uint identifier, Func<CCEffectInstanceTimed, bool> action) {
            if (!runningEffects.ContainsKey(identifier))
                return false;

            return action(runningEffects[identifier]);
        }

#endregion

#region Debug

#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        public void Log(string fmt, params object[] args) => Log(string.Format(fmt, args));
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        public void LogWarning(string fmt, params object[] args) => LogWarning(string.Format(fmt, args));
#if ML_Il2Cpp
        [HideFromIl2Cpp]
#endif
        public void LogError(string fmt, params object[] args) => LogError(string.Format(fmt, args));

        private const string _ccPrefix = "[CC] ";

        public static void Log(string content)
        {
#if !ML_Il2Cpp
            if (instance != null && instance._debugLog)
            {
                Debug.Log(_ccPrefix + content);
            }
#endif
        }

        public static void LogWarning(string content)
        {
#if !ML_Il2Cpp
            if (instance != null && instance._debugWarning)
            {
                Debug.LogWarning(_ccPrefix + content);
            }
#endif
        }

        public static void LogError(string content)
        {
#if !ML_Il2Cpp
            if (instance != null && instance._debugError)
            {
                Debug.LogError(_ccPrefix + content);
            }
#endif
        }

        public static void LogException(Exception e)
        {
#if !ML_Il2Cpp
            if (instance != null && instance._debugExceptions)
            {
                Debug.LogException(e);
            }
#endif
        }

#endregion

        const byte PROTOCOL_ENGINE = 0x11; // Unity3D

#if false
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX

#elif UNITY_STANDALONE_LINUX

#elif UNITY_STANDALONE_ANDROID

#elif UNITY_STANDALONE_IOS

#elif UNITY_STANDALONE_TVOS

#elif UNITY_PS4

#elif UNITY_XBOXONE

#elif UNITY_SWITCH

#elif UNITY_WEBGL

#elif UNITY_PS5

#else
#error "Unknown CrowdControl Platform"
#endif
#endif
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using RakDotNet.Protocols.Packets;
using RakDotNet.Protocols.Packets.ConnectionPackets;
using RakDotNet.Protocols.Packets.PingPackets;

namespace RakDotNet.Protocols
{
    public class PacketIdentifier : IDisposable
    {
        public const int CONNECTED_PING = 0x00;
        public const int UNCONNECTED_PING = 0x01;
        public const int UNCONNECTED_PING_OPEN_CONNECTIONS = 0x02;
        public const int CONNECTED_PONG = 0x03;
        public const int DETECT_LOST_CONNECTIONS = 0x04;
        public const int OPEN_CONNECTION_REQUEST_1 = 0x05;
        public const int OPEN_CONNECTION_REPLY_1 = 0x06;
        public const int OPEN_CONNECTION_REQUEST_2 = 0x07;
        public const int OPEN_CONNECTION_REPLY_2 = 0x08;
        public const int CONNECTION_REQUEST = 0x09;
        public const int REMOTE_SYSTEM_REQUIRES_PUBLIC_KEY = 0x0A;
        public const int OUR_SYSTEM_REQUIRES_SECURITY = 0x0B;
        public const int PUBLIC_KEY_MISMATCH = 0x0C;
        public const int OUT_OF_BAND_INTERNAL = 0x0D;
        public const int SND_RECEIPT_ACKED = 0x0E;
        public const int SND_RECEIPT_LOSS = 0x0F;
        public const int CONNECTION_REQUEST_ACCEPTED = 0x10;
        public const int CONNECTION_ATTEMPT_FAILED = 0x11;
        public const int ALREADY_CONNECTED = 0x12;
        public const int NEW_INCOMING_CONNECTION = 0x13;
        public const int NO_FREE_INCOMING_CONNECTIONS = 0x14;
        public const int DISCONNECTION_NOTIFICATION = 0x15;
        public const int CONNECTION_LOST = 0x16;
        public const int CONNECTION_BANNED = 0x17;
        public const int INVALID_PASSWORD = 0x18;
        public const int INCOMPATIBLE_PROTOCOL_VERSION = 0x19;
        public const int IP_RECENTLY_CONNECTED = 0x1A;
        public const int TIMESTAMP = 0x1B;
        public const int UNCONNECTED_PONG = 0x1C;
        public const int ADVERTISE_SYSTEM = 0x1D;
        public const int DOWNLOAD_PROGRESS = 0x1E;
        public const int REMOTE_DISCONNECTION_NOTIFICATION = 0x1F;
        public const int REMOTE_CONNECTION_LOST = 0x20;
        public const int REMOTE_NEW_INCOMING_CONNECTION = 0x21;
        public const int FILE_LIST_TRANSFER_HEADER = 0x22;
        public const int FILE_LIST_TRANSFER_FILE = 0x23;
        public const int FILE_LIST_REFERENCE_PUSH_ACK = 0x24;
        public const int DDT_DOWNLOAD_REQUEST = 0x25;
        public const int TRANSPORT_STRING = 0x26;
        public const int REPLICA_MANAGER_CONSTRUCTION = 0x27;
        public const int REPLICA_MANAGER_SCOPE_CHANGE = 0x28;
        public const int REPLICA_MANAGER_SERIALIZE = 0x29;
        public const int REPLICA_MANAGER_DOWNLOAD_STARTED = 0x2A;
        public const int REPLICA_MANAGER_DOWNLOAD_COMPLETE = 0x2B;
        public const int RAKVOICE_OPEN_CHANNEL_REQUEST = 0x2C;
        public const int RAKVOICE_OPEN_CHANNEL_REPLY = 0x2D;
        public const int RAKVOICE_CLOSE_CHANNEL = 0x2E;
        public const int RAKVOICE_DATA = 0x2F;
        public const int AUTOPATCHER_GET_CHANGELIST_SINCE_DATE = 0x30;
        public const int AUTOPATCHER_CREATION_LIST = 0x31;
        public const int AUTOPATCHER_DELETION_LIST = 0x32;
        public const int AUTOPATCHER_GET_PATCH = 0x33;
        public const int AUTOPATCHER_PATCH_LIST = 0x34;
        public const int AUTOPATCHER_REPOSITORY_FATAL_ERROR = 0x35;
        public const int AUTOPATCHER_CANNOT_DOWNLOAD_ORIGINAL_UNMODIFIED_FILES = 0x36;
        public const int AUTOPATCHER_FINISHED_INTERNAL = 0x37;
        public const int AUTOPATCHER_FINISHED = 0x38;
        public const int AUTOPATCHER_RESTART_APPLICATION = 0x39;
        public const int NAT_PUNCHTHROUGH_REQUEST = 0x3A;
        public const int NAT_CONNECT_AT_TIME = 0x3B;
        public const int NAT_GET_MOST_RECENT_PORT = 0x3C;
        public const int NAT_CLIENT_READY = 0x3D;
        public const int NAT_TARGET_NOT_CONNECTED = 0x3E;
        public const int NAT_TARGET_UNRESPONSIVE = 0x3F;
        public const int NAT_CONNECTION_TO_TARGET_LOST = 0x40;
        public const int NAT_ALREADY_IN_PROGRESS = 0x41;
        public const int NAT_PUNCHTHROUGH_FAILED = 0x42;
        public const int NAT_PUNCHTHROUGH_SUCCEEDED = 0x43;
        public const int READY_EVENT_SET = 0x44;
        public const int READY_EVENT_UNSET = 0x45;
        public const int READY_EVENT_ALL_SET = 0x46;
        public const int READY_EVENT_QUERY = 0x47;
        public const int LOBBY_GENERAL = 0x48;
        public const int RPC_REMOTE_ERROR = 0x49;
        public const int RPC_PLUGIN = 0x4A;
        public const int FILE_LIST_REFERENCE_PUSH = 0x4B;
        public const int READY_EVENT_FORCE_ALL_SET = 0x4C;
        public const int ROOMS_EXECUTE_FUNC = 0x4D;
        public const int ROOMS_LOGON_STATUS = 0x4E;
        public const int ROOMS_HANDLE_CHANGE = 0x4F;
        public const int LOBBY2_SEND_MESSAGE = 0x50;
        public const int LOBBY2_SERVER_ERROR = 0x51;
        public const int FCM2_NEW_HOST = 0x52;
        public const int FCM2_REQUEST_FCMGUID = 0x53;
        public const int FCM2_RESPOND_CONNECTION_COUNT = 0x54;
        public const int FCM2_INFORM_FCMGUID = 0x55;
        public const int FCM2_UPDATE_MIN_TOTAL_CONNECTION_COUNT = 0x56;
        public const int FCM2_VERIFIED_JOIN_START = 0x57;
        public const int FCM2_VERIFIED_JOIN_CAPABLE = 0x58;
        public const int FCM2_VERIFIED_JOIN_FAILED = 0x59;
        public const int FCM2_VERIFIED_JOIN_ACCEPTED = 0x5A;
        public const int FCM2_VERIFIED_JOIN_REJECTED = 0x5B;
        public const int UDP_PROXY_GENERAL = 0x5C;
        public const int SQLITE3_EXEC = 0x5D;
        public const int SQLITE3_UNKNOWN_DB = 0x5E;
        public const int SQLLITE_LOGGER = 0x5F;
        public const int NAT_TYPE_DETECTION_REQUEST = 0x60;
        public const int NAT_TYPE_DETECTION_RESULT = 0x61;
        public const int ROUTER_2_INTERNAL = 0x62;
        public const int ROUTER_2_FORWARDING_NO_PATH = 0x63;
        public const int ROUTER_2_FORWARDING_ESTABLISHED = 0x64;
        public const int ROUTER_2_REROUTED = 0x65;
        public const int TEAM_BALANCER_INTERNAL = 0x66;
        public const int TEAM_BALANCER_REQUESTED_TEAM_FULL = 0x67;
        public const int TEAM_BALANCER_REQUESTED_TEAM_LOCKED = 0x68;
        public const int TEAM_BALANCER_TEAM_REQUESTED_CANCELLED = 0x69;
        public const int TEAM_BALANCER_TEAM_ASSIGNED = 0x6A;
        public const int LIGHTSPEED_INTEGRATION = 0x6B;
        public const int XBOX_LOBBY = 0x6C;
        public const int TWO_WAY_AUTHENTICATION_INCOMING_CHALLENGE_SUCCESS = 0x6D;
        public const int TWO_WAY_AUTHENTICATION_OUTGOING_CHALLENGE_SUCCESS = 0x6E;
        public const int TWO_WAY_AUTHENTICATION_INCOMING_CHALLENGE_FAILURE = 0x6F;
        public const int TWO_WAY_AUTHENTICATION_OUTGOING_CHALLENGE_FAILURE = 0x70;
        public const int TWO_WAY_AUTHENTICATION_OUTGOING_CHALLENGE_TIMEOUT = 0x71;
        public const int TWO_WAY_AUTHENTICATION_NEGOTIATION = 0x72;
        public const int CLOUD_POST_REQUEST = 0x73;
        public const int CLOUD_RELEASE_REQUEST = 0x74;
        public const int CLOUD_GET_REQUEST = 0x75;
        public const int CLOUD_GET_RESPONSE = 0x76;
        public const int CLOUD_UNSUBSCRIBE_REQUEST = 0x77;
        public const int CLOUD_SERVER_TO_SERVER_COMMAND = 0x78;
        public const int CLOUD_SUBSCRIPTION_NOTIFICATION = 0x79;
        public const int LIB_VOICE = 0x7A;
        public const int RELAY_PLUGIN = 0x7B;
        public const int NAT_REQUEST_BOUND_ADDRESSES = 0x7C;
        public const int NAT_RESPOND_BOUND_ADDRESSES = 0x7D;
        public const int FCM2_UPDATE_USER_CONTEXT = 0x7E;
        public const int RESERVED_3 = 0x7F;
        public const int RESERVED_4 = 0x80;
        public const int RESERVED_5 = 0x81;
        public const int RESERVED_6 = 0x82;
        public const int RESERVED_7 = 0x83;
        public const int RESERVED_8 = 0x84;
        public const int RESERVED_9 = 0x85;
        public const int USER_PACKET_ENUM = 0x86;

        private ConcurrentDictionary<int, Type> _packetIdentifier = new ConcurrentDictionary<int, Type>();

        private ConcurrentDictionary<int, Func<RakNetPacket>> _identifierCreateFunc =
            new ConcurrentDictionary<int, Func<RakNetPacket>>();

        public PacketIdentifier()
        {
            RegisterDefaults();
        }

        public void RegisterDefaults()
        {
            Reset();

            Register(UNCONNECTED_PING, typeof(UnconnectedPing));
            Register(UNCONNECTED_PING_OPEN_CONNECTIONS, typeof(UnconnectedPingOpenConnections));
            Register(UNCONNECTED_PONG, typeof(UnconnectedPong));

            Register(OPEN_CONNECTION_REQUEST_1, typeof(OpenConnectionRequestOne));
            Register(OPEN_CONNECTION_REPLY_1, typeof(OpenConnectionReplyOne));
            Register(OPEN_CONNECTION_REQUEST_2, typeof(OpenConnectionRequestTwo));
            Register(OPEN_CONNECTION_REPLY_2, typeof(OpenConnectionReplyTwo));

            CompileAll();
        }

        public void Register(int id, Type type, bool compile = false)
        {
            _packetIdentifier[id] = type;
            if (compile)
                SingleCompile(id, type).GetAwaiter().GetResult();
        }

        public void CompileAll()
        {
            Parallel.ForEach(_packetIdentifier, async pair => { await SingleCompile(pair.Key, pair.Value); });
        }

        public async Task SingleCompile(int id, Type type)
        {
            await Task.Factory.StartNew(() =>
            {
                ConstructorInfo ctor = type.GetConstructor(
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    Type.DefaultBinder, new Type[0], null);
                _identifierCreateFunc[id] = Expression.Lambda<Func<RakNetPacket>>(
                    Expression.New(ctor)).Compile();
            });
        }

        public RakNetPacket GetPacketFormId(int id)
        {
            if (_identifierCreateFunc.ContainsKey(id))
                return _identifierCreateFunc[id].Invoke();

            throw new KeyNotFoundException($"{id} is key not found.");
        }

        public void Dispose()
        {
            Reset();
        }

        private void Reset()
        {
            _packetIdentifier.Clear();
            _identifierCreateFunc.Clear();
        }
    }
}
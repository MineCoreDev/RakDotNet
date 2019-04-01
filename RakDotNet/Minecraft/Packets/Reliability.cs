using System;

namespace RakDotNet.Minecraft.Packets
{
    public enum Reliability
    {
        Unreliable,
        UnreliableSequenced,
        Reliable,
        ReliableOrdered,
        ReliableSequenced,
        UnreliableWithAckReceipt,
        ReliableWithAckReceipt,
        ReliableOrderedWithAckReceipt
    }

    public static class ReliabilityExtension
    {
        public static bool IsReliable(this Reliability reliability)
        {
            return reliability == Reliability.Reliable ||
                   reliability == Reliability.ReliableOrdered ||
                   reliability == Reliability.ReliableSequenced ||
                   reliability == Reliability.ReliableWithAckReceipt ||
                   reliability == Reliability.ReliableOrderedWithAckReceipt;
        }

        public static bool IsSequenced(this Reliability reliability)
        {
            return reliability == Reliability.ReliableSequenced ||
                   reliability == Reliability.UnreliableSequenced;
        }

        public static bool IsOrdered(this Reliability reliability)
        {
            return reliability == Reliability.ReliableOrdered ||
                   reliability == Reliability.ReliableOrderedWithAckReceipt;
        }

        public static bool IsUnreliable(this Reliability reliability)
        {
            return reliability == Reliability.Unreliable ||
                   reliability == Reliability.UnreliableSequenced ||
                   reliability == Reliability.UnreliableWithAckReceipt;
        }

        public static bool IsAckReceipt(this Reliability reliability)
        {
            return reliability == Reliability.ReliableWithAckReceipt ||
                   reliability == Reliability.UnreliableWithAckReceipt ||
                   reliability == Reliability.ReliableOrderedWithAckReceipt;
        }
    }
}
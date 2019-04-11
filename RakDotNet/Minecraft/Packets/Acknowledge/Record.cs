using System;
using System.Collections.Generic;

namespace RakDotNet.Minecraft.Packets.Acknowledge
{
    public class Record
    {
        public uint StartIndex { get; set; }
        public uint EndIndex { get; set; }

        public static List<Record> SortRecords(Record[] records)
        {
            List<uint> rebuildList = new List<uint>();
            for (int i = 0; i < records.Length; i++)
            {
                Record record = records[i];
                if (!record.IsRanged())
                    rebuildList.Add(record.StartIndex);
                else
                    throw new InvalidOperationException("records sorted.");
            }

            rebuildList.Sort();

            List<Record> result = new List<Record>();
            int pointer = 1;
            uint start = rebuildList[0];
            uint last = rebuildList[0];

            while (pointer < records.Length)
            {
                uint current = rebuildList[pointer++];
                uint diff = current - last;
                if (diff == 1)
                    last = current;
                else if (diff > 1)
                {
                    if (start == last)
                    {
                        result.Add(new Record(start));
                        start = last = current;
                    }
                    else
                    {
                        result.Add(new Record(start, last));
                        start = last = current;
                    }
                }
            }

            if (start == last)
                result.Add(new Record(start));
            else
                result.Add(new Record(start, last));

            return result;
        }

        public Record(uint index)
        {
            StartIndex = index;
            EndIndex = index;
        }

        public Record(uint startIndex, uint endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public bool IsRanged()
        {
            return StartIndex != EndIndex;
        }
    }
}